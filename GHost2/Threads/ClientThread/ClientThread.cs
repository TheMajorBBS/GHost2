/*
  GHost/2: Door Server
  Copyleft 2021 Major BBS (GPL3)
    original: Rick Parrish, R&M Software

  This file is part of GHost/2.

  GHost/2 is free software: you can redistribute it and/or modify
  it under the terms of the GNU General Public License as published by
  the Free Software Foundation, either version 3 of the License, or
  any later version.

  GHost/2 is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU General Public License for more details.

  You should have received a copy of the GNU General Public License
  along with GHost/2.  If not, see <http://www.gnu.org/licenses/>.
*/
using RandM.RMLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;

namespace MajorBBS.GHost
{
    public class ClientThread : RMThread
    {
        private string _LastDisplayFile = "";
        private NodeInfo _NodeInfo = new NodeInfo();
        private Random _R = new Random();
        private string _Status = "";

        // TODOZ Add a Disconnect event of some sort to allow a sysop to disconnect another node
        public event EventHandler<NodeEventArgs> NodeEvent = null;
        public event EventHandler<WhoIsOnlineEventArgs> WhoIsOnlineEvent = null; // TODOX Gotta be a better way to get

        public ClientThread(TcpConnection connection, ConnectionType connectionType)
        {
            _NodeInfo.Connection = connection;
            _NodeInfo.ConnectionType = connectionType;
            _NodeInfo.TerminalType = Config.Instance.TerminalType;
        }

        protected override void Dispose(bool disposing)
        {
            if (!_Disposed)
            {
                if (disposing)
                {
                    // dispose managed state (managed objects).
                    if (_NodeInfo.Connection != null) _NodeInfo.Connection.Dispose();
                }

                GC.Collect();
                GC.WaitForPendingFinalizers();

                // Remove node directory
                var currentNode = Environment.CurrentDirectory + "\\node" + _NodeInfo.Node.ToString();

                if (Directory.Exists(currentNode))
                {
                    try
                    {
                        Directory.Delete(currentNode, true);
                    }
                    catch (Exception)
                    {
                        RMLog.Warning("Unable to remove node directory (non-fatal)");
                    }
                }

                // free unmanaged resources (unmanaged objects)
                // set large fields to null.

                // Call the base dispose
                base.Dispose(disposing);
            }
        }

        public string Alias
        {
            get { return _NodeInfo.User.Alias ?? ""; }
        }

        private bool HandleInvalidCreditionals()
        {
            DisplayAnsi("RLOGIN_INVALID");
            return false;
        }

        private bool DoDoorSrvRLogin(string UserName, string Password, string TerminalType)
        {
            if (string.IsNullOrEmpty(UserName))
            {
                return HandleInvalidCreditionals();
            }

            _NodeInfo.Door = new DoorInfo(TerminalType.Substring(5)); // 5 = strip off leading xtrn=
            if (!_NodeInfo.Door.Loaded)
            {
                // Requested door was not found
                DisplayAnsi("RLOGIN_INVALID_XTRN");
                return false;
            }

            // Check if the username already exists
            _NodeInfo.User = new UserInfo(UserName);
            if (!_NodeInfo.User.Loaded)
            {
                lock (Helpers.RegistrationLock)
                {
                    if (_NodeInfo.User.StartRegistration(Alias))
                    {
                        _NodeInfo.User.SetPassword(Password);
                        _NodeInfo.User.UserId = Config.Instance.NextUserId++;
                        _NodeInfo.User.SaveRegistration();
                        Config.Instance.Save();
                    }
                    else
                    {
                        // TODOZ This user lost the race (_NodeInfo.User.Loaded returned false above, so the user should have been free to register, but between then and .StartRegistration the alias was taken)
                        //       Not sure what to do in this case, aside from log that it happened
                        RMLog.Warning("RLogin user lost a race condition and couldn't register as '" + UserName + "'");
                    }
                }
            }

            return true;
        }

        private bool AuthenticateRLogin()
        {
            string UserName = ((RLoginConnection)_NodeInfo.Connection).ServerUserName;
            string Password = ((RLoginConnection)_NodeInfo.Connection).ClientUserName;
            string TerminalType = ((RLoginConnection)_NodeInfo.Connection).TerminalType;

            if (Helpers.IsBannedUser(UserName))
            {
                RMLog.Warning("RLogin user not allowed due to banned alias: '" + UserName + "'");
                return false;
            }

            return DoDoorSrvRLogin(UserName, Password, TerminalType);
        }

        public void ClrScr()
        {
            switch (_NodeInfo.TerminalType)
            {
                case TerminalType.ANSI:
                    _NodeInfo.Connection.Write(Ansi.TextAttr(7) + Ansi.ClrScr() + Ansi.GotoXY(1, 1));
                    break;
                case TerminalType.ASCII:
                    _NodeInfo.Connection.Write("\r\n\x0C");
                    break;
                case TerminalType.RIP:
                    _NodeInfo.Connection.Write("\r\n!|*" + Ansi.TextAttr(7) + Ansi.ClrScr() + Ansi.GotoXY(1, 1));
                    break;
            }
        }

        public bool DisplayAnsi(string fileName)
        {
            return DisplayAnsi(fileName, false);
        }

        public bool DisplayAnsi(string fileName, bool pauseAtEnd)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return false;
            }
            else
            {
                List<string> FilesToCheck = new List<string>();
                if (_NodeInfo.TerminalType == TerminalType.RIP) FilesToCheck.Add(StringUtils.PathCombine(ProcessUtils.StartupPath, "ansi", fileName.ToLower() + ".rip"));
                if ((_NodeInfo.TerminalType == TerminalType.RIP) || (_NodeInfo.TerminalType == TerminalType.ANSI)) FilesToCheck.Add(StringUtils.PathCombine(ProcessUtils.StartupPath, "ansi", fileName.ToLower() + ".ans"));
                FilesToCheck.Add(StringUtils.PathCombine(ProcessUtils.StartupPath, "ansi", fileName.ToLower() + ".asc"));

                for (int i = 0; i < FilesToCheck.Count; i++)
                {
                    if (File.Exists(FilesToCheck[i])) return DisplayFile(FilesToCheck[i], false, pauseAtEnd, false);
                }
            }

            return false;
        }

        private bool DisplayFile(string fileName, bool clearAtBeginning, bool pauseAtEnd, bool pauseAfter24)
        {
            try
            {
                _LastDisplayFile = fileName;

                if (clearAtBeginning)
                {
                    // Clear the screen
                    ClrScr();
                }

                // Translate the slashes accordingly
                if (OSUtils.IsWindows)
                {
                    fileName = fileName.Replace("/", "\\");
                }
                else if (OSUtils.IsUnix)
                {
                    fileName = fileName.Replace("\\", "/");
                }

                // If file starts with @, then it's an index file and we want to choose a random row from it
                if (fileName.StartsWith("@"))
                {
                    // Strip @
                    fileName = fileName.Substring(1);

                    if (File.Exists(fileName))
                    {
                        // Read index file
                        string[] FileNames = FileUtils.FileReadAllLines(fileName, RMEncoding.Ansi);

                        // Pick random filename
                        fileName = FileNames[_R.Next(0, FileNames.Length)];
                        _LastDisplayFile = fileName;

                        // Translate the slashes accordingly
                        if (OSUtils.IsWindows)
                        {
                            fileName = fileName.Replace("/", "\\");
                        }
                        else if (OSUtils.IsUnix)
                        {
                            fileName = fileName.Replace("\\", "/");
                        }
                    }
                    else
                    {
                        return false;
                    }
                }

                // Check if we need to pick an extension based on the user's terminal type (do this if the file doesn't exist, and doesn't have an extension)
                if (!File.Exists(fileName) && !Path.HasExtension(fileName))
                {
                    List<string> FileNamesWithExtension = new List<string>();
                    if (_NodeInfo.TerminalType == TerminalType.RIP)
                    {
                        FileNamesWithExtension.Add(fileName + ".rip");
                    }
                    if ((_NodeInfo.TerminalType == TerminalType.RIP) || (_NodeInfo.TerminalType == TerminalType.ANSI))
                    {
                        FileNamesWithExtension.Add(fileName + ".ans");
                    }
                    FileNamesWithExtension.Add(fileName + ".asc");

                    foreach (string FileNameWithExtension in FileNamesWithExtension)
                    {
                        if (File.Exists(FileNameWithExtension))
                        {
                            fileName = FileNameWithExtension;
                            break;
                        }
                    }
                }

                if (File.Exists(fileName))
                {
                    string TranslatedText = TranslateMCI(FileUtils.FileReadAllText(fileName, RMEncoding.Ansi), fileName);

                    // Check if we need to filter out a SAUCE record
                    if (TranslatedText.Contains("\x1A")) TranslatedText = TranslatedText.Substring(0, TranslatedText.IndexOf("\x1A"));

                    // Check if the file contains manual pauses
                    if (TranslatedText.Contains("{PAUSE}"))
                    {
                        // When the file contains {PAUSE} statements then pauseAfter24 is ignored
                        string[] Pages = TranslatedText.Split(new string[] { "{PAUSE}" }, StringSplitOptions.None);
                        for (int i = 0; i < Pages.Length; i++)
                        {
                            _NodeInfo.Connection.Write(Pages[i]);
                            if (i < (Pages.Length - 1)) ReadChar();
                        }
                    }
                    else
                    {
                        // Check if we want to pause every 24 lines
                        if (pauseAfter24)
                        {
                            // Yep, so split the file on CRLF
                            string[] TranslatedLines = TranslatedText.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                            if (TranslatedLines.Length <= 24)
                            {
                                // But if the file is less than 24 lines, then just send it all at once
                                _NodeInfo.Connection.Write(TranslatedText);
                            }
                            else
                            {
                                // More than 24 lines, do it the hard way
                                for (int i = 0; i < TranslatedLines.Length; i++)
                                {
                                    _NodeInfo.Connection.Write(TranslatedLines[i]);
                                    if (i < TranslatedLines.Length - 1) _NodeInfo.Connection.WriteLn();

                                    // TODOZ This doesn't work when a single line of output is spread across multiple lines of input
                                    //      Should somehow count the number of lines scrolled, and pause after 24
                                    if ((i % 24 == 23) && (i < TranslatedLines.Length - 1))
                                    {
                                        _NodeInfo.Connection.Write("<more>");
                                        var Ch = ReadChar();
                                        _NodeInfo.Connection.Write("\b\b\b\b\b\b      \b\b\b\b\b\b");
                                        if (Ch.ToString().ToUpper() == "Q") return true;
                                    }
                                }
                            }
                        }
                        else
                        {
                            // Nope, so just send it as is.
                            _NodeInfo.Connection.Write(TranslatedText);
                        }
                    }

                    if (pauseAtEnd)
                    {
                        ReadChar();
                        ClrScr();
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (IOException ioex)
            {
                RMLog.Exception(ioex, "Unable to display '" + fileName + "'");
                return false;
            }
        }

        protected override void Execute()
        {
            bool ShouldRaiseLogOffEvent = false;

            try
            {
                // Make telnet connections convert CRLF to CR
                _NodeInfo.Connection.LineEnding = "\r";
                _NodeInfo.Connection.StripLF = true;

                // Check for an ignored IP
                if (Helpers.IsIgnoredIP(_NodeInfo.Connection.GetRemoteIP()))
                {
                    // Do nothing for ignored IPs
                    RMLog.Debug("Ignored " + _NodeInfo.ConnectionType.ToString() + " connection from " + _NodeInfo.Connection.GetRemoteIP() + ":" + _NodeInfo.Connection.GetRemotePort());
                    return;
                }

                // Log the incoming connction
                RMLog.Info("Incoming " + _NodeInfo.ConnectionType.ToString() + " connection from " + _NodeInfo.Connection.GetRemoteIP() + ":" + _NodeInfo.Connection.GetRemotePort());

                // Get our terminal type, if necessary
                if (_NodeInfo.TerminalType == TerminalType.AUTODETECT) GetTerminalType();

                // Check for allowlist/blocklist type rejections
                if ((_NodeInfo.ConnectionType == ConnectionType.RLogin) && !Helpers.IsRLoginIP(_NodeInfo.Connection.GetRemoteIP()))
                {
                    // Do nothing for non-allowlisted RLogin IPs
                    RMLog.Warning("IP " + _NodeInfo.Connection.GetRemoteIP() + " doesn't match RLogin IP allowlist");
                    return;
                }
                else if (Helpers.IsBannedIP(_NodeInfo.Connection.GetRemoteIP()))
                {
                    RMLog.Warning("IP " + _NodeInfo.Connection.GetRemoteIP() + " matches banned IP filter");
                    DisplayAnsi("IP_BANNED");
                    Thread.Sleep(2500);
                    return;
                }
                else if (_Paused)
                {
                    DisplayAnsi("SERVER_PAUSED");
                    Thread.Sleep(2500);
                    return;
                }
                else if (!_NodeInfo.Connection.Connected)
                {
                    RMLog.Info("No carrier detected (probably a portscanner)");
                    return;
                }

                // Get our node number and bail if there are none available
                _NodeInfo.Node = NodeManager.GetFreeNode(this);
                if (_NodeInfo.Node == 0)
                {
                    DisplayAnsi("SERVER_BUSY");
                    Thread.Sleep(2500);
                    return;
                }

                // If we get here we can raise a logoff event at the end of the method
                ShouldRaiseLogOffEvent = true;

                // Handle authentication based on the connection type
                bool Authed = (_NodeInfo.ConnectionType == ConnectionType.RLogin) ? AuthenticateRLogin() : false;
                if (!Authed || QuitThread()) return;

                // Update the user's time remaining
                _NodeInfo.UserLoggedOn = true;
                _NodeInfo.SecondsThisSession = Config.Instance.TimePerCall * 60;
                NodeEvent?.Invoke(this, new NodeEventArgs(_NodeInfo, "Logging on", NodeEventType.LogOn));

                // Check if RLogin is requesting to launch a door immediately via the xtrn= command
                if ((_NodeInfo.Door != null) && _NodeInfo.Door.Loaded)
                {
                    (new RunDoor(this)).Run();
                    Thread.Sleep(2500);
                    return;
                }
            }
            catch (Exception ex)
            {
                RMLog.Exception(ex, "Exception in ClientThread::Execute()");
            }
            finally
            {
                // Try to close the connection
                try { _NodeInfo.Connection.Close(); } catch (Exception ex) { RMLog.Debug($"Exception closing connection in client thread: {ex.ToString()}"); }

                // Try to free the node
                if (ShouldRaiseLogOffEvent)
                {
                    try { NodeEvent?.Invoke(this, new NodeEventArgs(_NodeInfo, "Logging off", NodeEventType.LogOff)); } catch { /* Ignore */ }
                }
            }
        }

        // Logic for this terminal type detection taken from Synchronet's ANSWER.CPP
        private void GetTerminalType()
        {
            try
            {
                /* Detect terminal type */
                Thread.Sleep(200);
                _NodeInfo.Connection.ReadString();		/* flush input buffer */
                _NodeInfo.Connection.Write("\r\n" +		/* locate cursor at column 1 */
                    "\x1b[s" +	                /* save cursor position (necessary for HyperTerm auto-ANSI) */
                    "\x1b[255B" +	            /* locate cursor as far down as possible */
                    "\x1b[255C" +	            /* locate cursor as far right as possible */
                    "\b_" +		                /* need a printable at this location to actually move cursor */
                    "\x1b[6n" +	                /* Get cursor position */
                    "\x1b[u" +	                /* restore cursor position */
                    "\x1b[!_" +	                /* RIP? */
                    "\x1b[0m_" +	            /* "Normal" colors */
                    "\x1b[2J" +	                /* clear screen */
                    "\x1b[H" +	                /* home cursor */
                    "\xC" +		                /* clear screen (in case not ANSI) */
                    "\r"		                /* Move cursor left (in case previous char printed) */
                );

                char? c = '\0';
                int i = 0;
                string str = "";
                while (i++ < 50)
                { 	/* wait up to 5 seconds for response */
                    c = _NodeInfo.Connection.ReadChar(100);
                    if (_NodeInfo.Connection.ReadTimedOut)
                        continue;
                    if (c == null)
                        continue;
                    c = (char)(c & 0x7f);
                    if (c == 0)
                        continue;
                    i = 0;
                    if (string.IsNullOrEmpty(str) && c != '\x1b')	// response must begin with escape char
                        continue;
                    str += c;
                    if (c == 'R')
                    {   /* break immediately if ANSI response */
                        Thread.Sleep(500);
                        break;
                    }
                }

                while (_NodeInfo.Connection.CanRead(100))
                {
                    str += _NodeInfo.Connection.ReadString();
                }

                if (str.ToUpper().Contains("RIPSCRIP"))
                {
                    _NodeInfo.TerminalType = TerminalType.RIP;
                }
                else if (Regex.IsMatch(str, "\\x1b[[]\\d{1,3};\\d{1,3}R"))
                {
                    _NodeInfo.TerminalType = TerminalType.ANSI;
                }
            }
            catch (Exception)
            {
                // Ignore, we'll just assume ASCII if something bad happens
            }

            _NodeInfo.TerminalType = TerminalType.ASCII;
        }

        public string IPAddress
        {
            get { return _NodeInfo.Connection.GetRemoteIP(); }
        }

        public NodeInfo NodeInfo { get { return _NodeInfo; } }

        public void OnDoorWait(object sender, RMProcessStartAndWaitEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }

            e.Stop = QuitThread();
        }

        public bool QuitThread()
        {
            if (_Stop) return true;
            if (!_NodeInfo.Connection.Connected) return true;
            if (_NodeInfo.Connection.ReadTimedOut) return true;
            if (_NodeInfo.SecondsLeft <= 0) return true;
            return false;
        }

        private char? ReadChar()
        {
            char? Result = null;

            Result = _NodeInfo.Connection.ReadChar(_NodeInfo.ReadTimeout);
            if (Result == null)
            {
                if ((!_Stop) && (_NodeInfo.Connection.Connected))
                {
                    if (_NodeInfo.SecondsLeft > 0)
                    {
                        // User has time left so they timed out
                        DisplayAnsi("EXCEEDED_IDLE_LIMIT");
                        Thread.Sleep(2500);
                        _NodeInfo.Connection.Close();
                    }
                    else
                    {
                        // User has no time left, so they exceeded the call limit
                        DisplayAnsi("EXCEEDED_CALL_LIMIT");
                        Thread.Sleep(2500);
                        _NodeInfo.Connection.Close();
                    }
                }
            }

            return Result;
        }

        private string ReadLn()
        {
            // Call the main SocketReadLn() indicating no password character
            return ReadLn('\0');
        }

        private string ReadLn(char passwordChar)
        {
            string Result = _NodeInfo.Connection.ReadLn(passwordChar, _NodeInfo.ReadTimeout);
            if ((_NodeInfo.Connection.ReadTimedOut) && (!_Stop) && (_NodeInfo.Connection.Connected))
            {
                if (_NodeInfo.SecondsLeft > 0)
                {
                    // User has time left so they timed out
                    DisplayAnsi("EXCEEDED_IDLE_LIMIT");
                    Thread.Sleep(2500);
                    _NodeInfo.Connection.Close();
                }
                else
                {
                    // User has no time left, so they exceeded the call limit
                    DisplayAnsi("EXCEEDED_CALL_LIMIT");
                    Thread.Sleep(2500);
                    _NodeInfo.Connection.Close();
                }
            }

            return Result;
        }


        public string Status
        {
            get { return _Status; }
        }

        public override void Stop()
        {
            // Close the socket so that any waits on ReadLn(), ReadChar(), etc, will not block
            if (_NodeInfo.Connection != null)
            {
                _NodeInfo.Connection.Close();
            }

            base.Stop();
        }

        private void RemoteConnectOut(string hostname)
        {
            bool _RLogin = false;
            string _RLoginClientUserName = "TODO";
            string _RLoginServerUserName = "TODO";
            string _RLoginTerminalType = "TODO";

            ClrScr();
            _NodeInfo.Connection.WriteLn();
            _NodeInfo.Connection.Write(" Connecting to remote server...");

            TcpConnection RemoteServer = null;
            if (_RLogin)
            {
                RemoteServer = new RLoginConnection();
            }
            else
            {
                RemoteServer = new TelnetConnection();
            }

            // Sanity check on the port
            int Port = 23;
            if ((Port < 1) || (Port > 65535))
            {
                Port = (_RLogin) ? 513 : 23;
            }

            if (RemoteServer.Connect(hostname, Port))
            {
                bool CanContinue = true;
                if (_RLogin)
                {
                    // Send rlogin header
                    RemoteServer.Write("\0" + _RLoginClientUserName + "\0" + _RLoginServerUserName + "\0" + _RLoginTerminalType + "\0");

                    // Wait up to 5 seconds for a response
                    char? Ch = RemoteServer.ReadChar(5000);
                    if ((Ch == null) || (Ch != '\0'))
                    {
                        CanContinue = false;
                        _NodeInfo.Connection.WriteLn("failed!");
                        _NodeInfo.Connection.WriteLn();
                        _NodeInfo.Connection.WriteLn(" Looks like the remote server doesn't accept RLogin connections.");
                    }
                }

                if (CanContinue)
                {
                    _NodeInfo.Connection.WriteLn("connected!");

                    bool UserAborted = false;
                    while (!UserAborted && RemoteServer.Connected && !QuitThread())
                    {
                        bool Yield = true;

                        // See if the server sent anything to the client
                        if (RemoteServer.CanRead())
                        {
                            _NodeInfo.Connection.Write(RemoteServer.ReadString());
                            Yield = false;
                        }

                        // See if the client sent anything to the server
                        if (_NodeInfo.Connection.CanRead())
                        {
                            //string ToSend = "";
                            //while (_NodeInfo.Connection.KeyPressed())
                            //{
                            //    byte B = (byte)_NodeInfo.Connection.ReadByte();
                            //    if (B == 29)
                            //    {
                            //        // Ctrl-]
                            //        RemoteServer.Close();
                            //        UserAborted = true;
                            //        break;
                            //    }
                            //    else
                            //    {
                            //        ToSend += (char)B;
                            //    }
                            //}
                            //RemoteServer.Write(ToSend);

                            RemoteServer.Write(_NodeInfo.Connection.ReadString());
                            Yield = false;
                        }

                        // See if we need to yield
                        if (Yield) Crt.Delay(1);
                    }

                    if (UserAborted)
                    {
                        _NodeInfo.Connection.WriteLn();
                        _NodeInfo.Connection.WriteLn();
                        _NodeInfo.Connection.WriteLn(" User hit CTRL-] to disconnect from server.");
                        ReadChar();
                    }
                    else if ((_NodeInfo.Connection.Connected) && (!RemoteServer.Connected))
                    {
                        _NodeInfo.Connection.WriteLn();
                        _NodeInfo.Connection.WriteLn();
                        _NodeInfo.Connection.WriteLn(" Remote server closed the connection.");
                        ReadChar();
                    }
                }
            }
            else
            {
                _NodeInfo.Connection.WriteLn("failed!");
                _NodeInfo.Connection.WriteLn();
                _NodeInfo.Connection.WriteLn(" Looks like the remote server isn't online, please try back later.");
            }
        }

        private string TranslateMCI(string text, string fileName)
        {
            StringDictionary MCI = new StringDictionary() {
                { "ACCESSLEVEL", _NodeInfo.User.AccessLevel.ToString() },
                { "ALIAS", _NodeInfo.User.Alias },
                { "BBSNAME", Config.Instance.BBSName },
                { "DATE", DateTime.Now.ToShortDateString() },
                { "FILENAME", fileName.Replace(StringUtils.PathCombine(ProcessUtils.StartupPath, ""), "") },
                { "GSDIR", StringUtils.PathCombine(ProcessUtils.StartupPath, "") },
                { "NODE", _NodeInfo.Node.ToString() },
                { "OPERATINGSYSTEM", OSUtils.GetNameAndVersion() },
                { "SYSOPEMAIL", Config.Instance.SysopEmail },
                { "SYSOPNAME", Config.Instance.SysopFirstName + " " + Config.Instance.SysopLastName },
                { "TIME", DateTime.Now.ToShortTimeString() },
                { "TIMELEFT", StringUtils.SecToHMS(_NodeInfo.SecondsLeft) },
            };
            foreach (DictionaryEntry DE in _NodeInfo.User.AdditionalInfo)
            {
                MCI.Add(DE.Key.ToString(), DE.Value.ToString());
            }
            if (text.Contains("WHOSONLINE_"))
            {
                EventHandler<WhoIsOnlineEventArgs> Handler = WhoIsOnlineEvent;
                if (Handler != null)
                {
                    WhoIsOnlineEventArgs WOEA = new WhoIsOnlineEventArgs();
                    Handler(this, WOEA);
                    foreach (DictionaryEntry DE in WOEA.WhoIsOnline)
                    {
                        MCI.Add(DE.Key.ToString(), DE.Value.ToString());
                    }
                }
            }

            // Perform MCI Translations
            foreach (DictionaryEntry DE in MCI)
            {
                if (DE.Value != null)
                {
                    text = text.Replace("{" + DE.Key.ToString().ToUpper() + "}", DE.Value.ToString());
                    for (int PadWidth = 1; PadWidth <= 80; PadWidth++)
                    {
                        // Now translate anything that needs right padding
                        text = text.Replace("{" + DE.Key.ToString().ToUpper() + PadWidth.ToString() + "}", StringUtils.PadRight(DE.Value.ToString(), ' ', PadWidth));

                        // And now translate anything that needs left padding
                        text = text.Replace("{" + PadWidth.ToString() + DE.Key.ToString().ToUpper() + '}', StringUtils.PadLeft(DE.Value.ToString(), ' ', PadWidth));
                    }
                }
            }

            return text;
        }

        public void UpdateStatus(string newStatus)
        {
            _Status = newStatus;
            NodeEvent?.Invoke(this, new NodeEventArgs(_NodeInfo, newStatus, NodeEventType.StatusChange));
        }
    }
}
