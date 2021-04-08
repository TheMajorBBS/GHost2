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
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;

namespace MajorBBS.GHost
{
    public class RunDoor
    {
        private ClientThread _ClientThread;

        public RunDoor(ClientThread clientThread)
        {
            _ClientThread = clientThread;
        }

        private void WriteDropFile(string fileName, List<string> lines)
        {
            FileUtils.FileWriteAllText(
                StringUtils.PathCombine(
                    ProcessUtils.StartupPath,
                    "node" + _ClientThread.NodeInfo.Node.ToString(),
                    fileName
                ),
                String.Join("\r\n", lines.ToArray())
            );
        }

        private void CreateDropChain()
        {
            List<string> Sl = new List<string>();

            // Create CHAIN.TXT
            Sl.Clear();
            Sl.Add((_ClientThread.NodeInfo.User.UserId - 1).ToString());                    // 1 - User Number
            Sl.Add(_ClientThread.NodeInfo.User.Alias);                                      // 2 - User Alias
            Sl.Add(Config.Instance.SysopFirstName + " " + Config.Instance.SysopLastName);   // 3 - User Real Name
            Sl.Add("");                                                                     // 4 - User Callsign (HAM radio)
            Sl.Add("21");                                                                   // 5 - User Age
            Sl.Add("M");                                                                    // 6 - User Sex
            Sl.Add("999.00");                                                               // 7 - User Gold (credits)
            Sl.Add("00/00/00");                                                             // 8 - User Last Logon Date
            Sl.Add("80");                                                                   // 9 - Columns
            Sl.Add("24");                                                                   // 10 - Rows
            Sl.Add(_ClientThread.NodeInfo.User.AccessLevel.ToString());                     // 11 - Security Level (0-255)
            Sl.Add("0");                                                                    // 12 - Is CoSysop (1 or 0)
            Sl.Add("0");                                                                    // 13 - Is SysOp (1 or 0)
            Sl.Add(_ClientThread.NodeInfo.TerminalType == TerminalType.ASCII ? "0" : "1");  // 14 - Is ANSI (1 or 0)
            Sl.Add("1");                                                                    // 15 - Is Remote (1 or 0)
            Sl.Add(_ClientThread.NodeInfo.SecondsLeft.ToString());                          // 16 - User number of seconds left
            Sl.Add(StringUtils.ExtractShortPathName(ProcessUtils.StartupPath));             // 17 - GFiles or general text path
            Sl.Add(StringUtils.ExtractShortPathName(ProcessUtils.StartupPath));             // 18 - BBS Data directory
            Sl.Add("fake.log");                                                             // 19 - Current Logfile
            Sl.Add("38400");                                                                // 20 - Connection Baud Rate
            Sl.Add("1");                                                                    // 21 - Com Port
            Sl.Add(Config.Instance.BBSName);                                                // 22 - BBS Name
            Sl.Add(Config.Instance.SysopFirstName + " " + Config.Instance.SysopLastName);   // 23 - Sysop Name
            Sl.Add("0");                                                                    // 24 - Time of day user logged in (Seconds from midnight)
            Sl.Add("0");                                                                    // 25 - Number of seconds connected.
            Sl.Add("0");                                                                    // 26 - User upload in KB
            Sl.Add("0");                                                                    // 27 - Number of files uploaded
            Sl.Add("0");                                                                    // 28 - User download in KB
            Sl.Add("0");                                                                    // 29 - Number of files downloaded
            Sl.Add("8N1");                                                                  // 30 - parity
            Sl.Add("38400");                                                                // 31 - Port baud rate
            Sl.Add("7400");                                                                 // 32 - WWIVnet node number
            WriteDropFile("chain.txt", Sl);
        }

        private void CreateDropDoor()
        {
            List<string> Sl = new List<string>();
            // Create DOOR.SYS
            Sl.Clear();
            Sl.Add("COM1:");                                                                // 1 - Comm Port
            Sl.Add("38400");                                                                // 2 - Connection Baud Rate
            Sl.Add("8");                                                                    // 3 - Parity
            Sl.Add(_ClientThread.NodeInfo.Node.ToString());                                 // 4 - Current Node Number
            Sl.Add("38400");                                                                // 5 - Locked Baud Rate
            Sl.Add("Y");                                                                    // 6 - Screen Display
            Sl.Add("Y");                                                                    // 7 - Printer Toggle
            Sl.Add("Y");                                                                    // 8 - Page Bell
            Sl.Add("Y");                                                                    // 9 - Caller Alarm
            Sl.Add(_ClientThread.NodeInfo.User.Alias);                                      // 10 - User's Real Name
            Sl.Add("City, State");                                                          // 11 - User's Location
            Sl.Add("555-555-5555");                                                         // 12 - User's Home Phone #
            Sl.Add("555-555-5555");                                                         // 13 - User's Work Phone #
            Sl.Add("PASSWORD");                                                             // 14 - User's Password
            Sl.Add(_ClientThread.NodeInfo.User.AccessLevel.ToString());                     // 15 - User's Access Level
            Sl.Add("1");                                                                    // 16 - User's Total Calls
            Sl.Add("00/00/00");                                                             // 17 - User's Last Call Date
            Sl.Add(_ClientThread.NodeInfo.SecondsLeft.ToString());                          // 18 - Users's Seconds Left This Call
            Sl.Add(_ClientThread.NodeInfo.MinutesLeft.ToString());                          // 19 - User's Minutes Left This Call (I love redundancy!)
            Sl.Add("GR");                                                                   // 20 - Graphics Mode GR=Graphics, NG=No Graphics, 7E=7-bit
            Sl.Add("24");                                                                   // 21 - Screen Length
            Sl.Add("N");                                                                    // 22 - Expert Mode
            Sl.Add("");                                                                     // 23 - Conferences Registered In
            Sl.Add("");                                                                     // 24 - Conference Exited To Door From
            Sl.Add("00/00/00");                                                             // 25 - User's Expiration Date
            Sl.Add((_ClientThread.NodeInfo.User.UserId - 1).ToString());                    // 26 - User's Record Position (0 based)
            Sl.Add("Z");                                                                    // 27 - User's Default XFer Protocol
            Sl.Add("0");                                                                    // 28 - Total Uploads
            Sl.Add("0");                                                                    // 29 - Total Downloads
            Sl.Add("0");                                                                    // 30 - Total Downloaded Today (kB)
            Sl.Add("0");                                                                    // 31 - Daily Download Limit (kB)
            Sl.Add("00/00/00");                                                             // 32 - User's Birthday
            Sl.Add(StringUtils.ExtractShortPathName(ProcessUtils.StartupPath));             // 33 - Path To User File
            Sl.Add(StringUtils.ExtractShortPathName(ProcessUtils.StartupPath));             // 34 - Path To GEN Directory
            Sl.Add(Config.Instance.SysopFirstName + " " + Config.Instance.SysopLastName);   // 35 - SysOp's Name
            Sl.Add(_ClientThread.NodeInfo.User.Alias);                                      // 36 - User's Alias
            Sl.Add("00:00");                                                                // 37 - Next Event Time
            Sl.Add("Y");                                                                    // 38 - Error Correcting Connection
            Sl.Add(_ClientThread.NodeInfo.TerminalType == TerminalType.ASCII ? "N" : "Y");  // 39 - ANSI Supported
            Sl.Add("Y");                                                                    // 40 - Use Record Locking
            Sl.Add("7");                                                                    // 41 - Default BBS Colour
            Sl.Add("0");                                                                    // 42 - Time Credits (In Minutes)
            Sl.Add("00/00/00");                                                             // 43 - Last New File Scan
            Sl.Add("00:00");                                                                // 44 - Time Of This Call
            Sl.Add("00:00");                                                                // 45 - Time Of Last Call
            Sl.Add("0");                                                                    // 46 - Daily File Limit
            Sl.Add("0");                                                                    // 47 - Files Downloaded Today
            Sl.Add("0");                                                                    // 48 - Total Uploaded (kB)
            Sl.Add("0");                                                                    // 49 - Total Downloaded (kB)
            Sl.Add("No Comment");                                                           // 50 - User's Comment
            Sl.Add("0");                                                                    // 51 - Total Doors Opened
            Sl.Add("0");                                                                    // 52 - Total Messages Left
            WriteDropFile("door.sys", Sl);
        }

        private void CreateDropDoor32()
        {
            List<string> Sl = new List<string>();

            // Create DOOR32.SYS
            Sl.Clear();
            Sl.Add("2");                                                    // 1 - Comm Type (0=local, 1=serial, 2=telnet)
            Sl.Add(_ClientThread.NodeInfo.Connection.Handle.ToString());    // 2 - Comm Or Socket Handle
            Sl.Add("38400");                                                // 3 - Baud Rate
            Sl.Add(ProcessUtils.ProductName + " v" + GHost.Version);      // 4 - BBSID (Software Name & Version
            Sl.Add(_ClientThread.NodeInfo.User.UserId.ToString());          // 5 - User's Record Position (1 based)
            Sl.Add(_ClientThread.NodeInfo.User.Alias);                      // 6 - User's Real Name
            Sl.Add(_ClientThread.NodeInfo.User.Alias);                      // 7 - User's Handle/Alias
            Sl.Add(_ClientThread.NodeInfo.User.AccessLevel.ToString());     // 8 - User's Access Level
            Sl.Add(_ClientThread.NodeInfo.MinutesLeft.ToString());          // 9 - User's Time Left (In Minutes)
            switch (_ClientThread.NodeInfo.TerminalType)                    // 10 - Emulation (0=Ascii, 1=Ansi, 2=Avatar, 3=RIP, 4=MaxGfx)
            {
                case TerminalType.ANSI: Sl.Add("1"); break;
                case TerminalType.ASCII: Sl.Add("0"); break;
                case TerminalType.RIP: Sl.Add("3"); break;
            }
            Sl.Add(_ClientThread.NodeInfo.Node.ToString());                 // 11 - Current Node Number
            WriteDropFile("door32.sys", Sl);
        }

        private void CreateDropDoorfile()
        {
            List<string> Sl = new List<string>();

            // Create DOORFILE.SR
            Sl.Clear();
            Sl.Add(_ClientThread.NodeInfo.User.Alias);                                      // Complete name or handle of user
            Sl.Add(_ClientThread.NodeInfo.TerminalType == TerminalType.ASCII ? "0" : "1");  // ANSI status:  1 = yes, 0 = no, -1 = don't know
            Sl.Add("1");                                                                    // IBM Graphic characters:  1 = yes, 0 = no, -1 = unknown
            Sl.Add("24");                                                                   // Page length of screen, in lines.  Assume 25 if unknown
            Sl.Add("38400");                                                                // Baud Rate:  300, 1200, 2400, 9600, 19200, etc.
            Sl.Add("1");                                                                    // Com Port:  1, 2, 3, or 4; 0 if local.
            Sl.Add(_ClientThread.NodeInfo.MinutesLeft.ToString());                          // Time Limit:  (in minutes); -1 if unknown.
            Sl.Add(_ClientThread.NodeInfo.User.Alias);                                      // Real name (the same as line 1 if not known)
            WriteDropFile("doorfile.sr", Sl);
        }

        private void CreateDropDorinfo()
        {
            List<string> Sl = new List<string>();

            // Create DORINFO.DEF
            Sl.Clear();
            Sl.Add(Config.Instance.BBSName);                                                // 1 - BBS Name
            Sl.Add(Config.Instance.SysopFirstName);                                         // 2 - Sysop's First Name
            Sl.Add(Config.Instance.SysopLastName);                                          // 3 - Sysop's Last Name
            Sl.Add("COM1");                                                                 // 4 - Comm Number in COMxxx Form
            Sl.Add("38400 BAUD,N,8,1");                                                     // 5 - Baud Rate in 38400 BAUD,N,8,1 Form
            Sl.Add("0");                                                                    // 6 - Networked?
            Sl.Add(_ClientThread.NodeInfo.User.Alias);                                      // 7 - User's First Name / Alias
            Sl.Add("");                                                                     // 8 - User's Last Name
            Sl.Add("City, State");                                                          // 9 - User's Location (City, State, etc.)
            Sl.Add(_ClientThread.NodeInfo.TerminalType == TerminalType.ASCII ? "0" : "1");  // 10 - User's Emulation (0=Ascii, 1=Ansi)
            Sl.Add(_ClientThread.NodeInfo.User.AccessLevel.ToString());                     // 11 - User's Access Level
            Sl.Add(_ClientThread.NodeInfo.MinutesLeft.ToString());                          // 12 - User's Time Left (In Minutes)
            Sl.Add("1");                                                                    // 13 - Fossil?
            WriteDropFile("dorinfo.def", Sl);
            WriteDropFile("dorinfo1.dat", Sl);
            WriteDropFile("dorinfo" + _ClientThread.NodeInfo.Node.ToString() + ".def", Sl);
        }

        private void CreateNodeDirectory()
        {
            Directory.CreateDirectory(StringUtils.PathCombine(ProcessUtils.StartupPath, "node" + _ClientThread.NodeInfo.Node.ToString()));

            CreateDropChain();
            CreateDropDoor();
            CreateDropDoor32();
            CreateDropDoorfile();
            CreateDropDorinfo();
        }

        public void Run(string door)
        {
            _ClientThread.NodeInfo.Door = new DoorInfo(door);
            if (_ClientThread.NodeInfo.Door.Loaded)
            {
                Run();
            }
            else
            {
                RMLog.Error("Unable to find door: '" + door + "'");
            }
        }

        private bool IsAuthorized(string authStr, string userName)
        {
            bool authed = true;
            string[] conditionList = authStr.Split(' ');

            foreach (string condition in conditionList)
            {
                char op = condition.ToCharArray()[0];
                bool isGrant = op != '-' ? true : false;  // + or nothing equals true. - equals false.
                string condName = "";

                if (op != '+' && op != '-')
                {
                    // use the entire string if no operand specified.
                    condName = condition;
                }
                else
                {
                    // pull string after operand.
                    condName = condition.Substring(1);
                }

                if (condName.ToLower() == userName)
                {
                    // quit immediately if we find a user specified.
                    return isGrant;
                }

                if (condName == "[default]")
                {
                    // set the default if user isn't specifically specified.
                    authed = isGrant;
                }

            }

            return authed;
        }

        /* replaces t
         */
        private string ReplaceTokens(string platformCommand)
        {
            return platformCommand != "" ?
                    platformCommand
                        .Replace("%args%", TranslateCLS(_ClientThread.NodeInfo.Door.Parameters))
                        .Replace("%cmd%", TranslateCLS(_ClientThread.NodeInfo.Door.Command) + " " + TranslateCLS(_ClientThread.NodeInfo.Door.Parameters))
                        .Replace("%exe%", TranslateCLS(_ClientThread.NodeInfo.Door.Command))
                        .Replace("%node%", _ClientThread.NodeInfo.Node.ToString())
                        .Replace("%handle%", _ClientThread.NodeInfo.Connection.Handle.ToString())
                    : TranslateCLS(_ClientThread.NodeInfo.Door.Command);
        }

        private void RunPlatform(string platformName)
        {
            //if (_ClientThread.NodeInfo.Door)
            PlatformInfo platform = new PlatformInfo(platformName);
            if (IsAuthorized(_ClientThread.NodeInfo.Door.Authorization, _ClientThread.NodeInfo.User.Alias) == false)
            {
                string log = String.Format("Access-Denied: {0} to running {1}.", _ClientThread.NodeInfo.User.Alias, _ClientThread.NodeInfo.Door.Name);
                RMLog.Info(log);
                return;
            }

            string command = TranslateCLS(_ClientThread.NodeInfo.Door.Command);
            string parameters = TranslateCLS(_ClientThread.NodeInfo.Door.Parameters);
            string platformParams = ReplaceTokens(platform.Arguments);

            if (Helpers.Debug) _ClientThread.UpdateStatus("DEBUG: " + platform.Name + " launching " + command + " " + parameters);

            if (platform.BootstrapName != "")
            {
                /* If there is a bootstrap name specified than we need to 
                 * create the bootstrap in the node direectory.
                 * Than executes the bootstrap.
                 */

                string scriptPath = StringUtils.PathCombine(
                    ProcessUtils.StartupPath,
                    "node" + _ClientThread.NodeInfo.Node.ToString(),
                    platform.BootstrapName
                );

                //make bootstrap
                string bootstrap = ReplaceTokens(platform.Bootstrap).Replace("\\r", "\r\n").Replace("%r%", "\r\n");
                FileUtils.FileWriteAllText(
                    scriptPath,
                    bootstrap
                );

                platformParams += " " + scriptPath;
            }

            /* Create process with environment settings and execute door command.
             */
            string platformCmd = Path.Combine(ProcessUtils.StartupPath, ReplaceTokens(platform.Shell));

            Process proc = new Process();
            ProcessStartInfo PSI = new ProcessStartInfo(platformCmd, platformParams)
            {
                WindowStyle = _ClientThread.NodeInfo.Door.WindowStyle,
                WorkingDirectory = ProcessUtils.StartupPath,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
            };

            TcpConnection conn = new TcpConnection();
            conn.Open((int)_ClientThread.NodeInfo.Connection.Handle);
            proc.StartInfo = PSI;
            proc.Start();

            Socket sock = conn.GetSocket();
            byte[] tmp = new byte[1];

            while (conn.Connected && !proc.HasExited)
            {
                try
                {

                    if (platform.RedirectLocal)
                    {
                        if (conn.CanRead(100))
                        {
                            proc.StandardInput.Write(conn.ReadString());
                        }

                        string inData = proc.StandardOutput.ReadToEnd();
                        if (inData != "")
                        {
                            //sock.Send(Encoding.ASCII.GetBytes(inData));
                            conn.Write(inData);
                        }
                    }

                    string errorStr = proc.StandardError.ReadLine();
                    if (errorStr != "" && errorStr != null)
                    {
                        if (!platform.SupressErrors)
                        {
                            string errFormat = String.Format(
                                "DOOR ERROR [{0}]: {1}",
                                _ClientThread.NodeInfo.Door.Name,
                                errorStr
                            );
                            RMLog.Error(errFormat);
                        }

                        // search error log for DOSBox disconnect.
                        if (errorStr == "Serial1: Disconnected.")
                        {
                            RMLog.Error("DOSBox serial disconnect discovered.");
                            proc.WaitForExit(60000);
                            proc.Kill();
                        }
                    }

                    // probe socket to see if it's still connected.
                    sock.Send(tmp, 0, 0);
                }
                catch (SocketException sockExcept)
                {
                    if (!sockExcept.NativeErrorCode.Equals(10035))
                    {
                        string errMsg = String.Format("User {0} has disconnected from '{1}'.", _ClientThread.Alias, _ClientThread.NodeInfo.Door.Name);
                        RMLog.Error(errMsg);
                        break;
                    }
                }
            }

            while (!proc.HasExited)
            {
                proc.WaitForExit(60000);
                proc.Kill();
            }

        }

        private void RunOldIniLogic()
        {
            // Determine how to run the door
            if (((_ClientThread.NodeInfo.Door.Platform == "Linux") && OSUtils.IsUnix) || ((_ClientThread.NodeInfo.Door.Platform == "Windows") && OSUtils.IsWindows))
            {
                //RunDoorNative(TranslateCLS(_ClientThread.NodeInfo.Door.Command), TranslateCLS(_ClientThread.NodeInfo.Door.Parameters));
                RunPlatform("native");
            }
            else if ((_ClientThread.NodeInfo.Door.Platform == "DOS") && OSUtils.IsWindows)
            {
                if (ProcessUtils.Is64BitOperatingSystem)
                {
                    if (Helpers.IsDOSBoxInstalled())
                    {
                        RunPlatform("dosbox");
                    }
                    else
                    {
                        RMLog.Error("DOS doors are not supported on 64bit Windows (unless you install DOSBox 0.73)");
                    }
                }
                else
                {
                    RunPlatform("ntvdm");
                }
            }
            else if ((_ClientThread.NodeInfo.Door.Platform == "DOS") && OSUtils.IsUnix)
            {
                if (Helpers.IsDOSEMUInstalled())
                {
                    RunPlatform("dosemu");
                }
                else
                {
                    RMLog.Error("DOS doors are not supported on Linux (unless you install DOSEMU)");
                }
            }
            else
            {
                RMLog.Error("Unsure how to run door on current platform");
            }
        }

        public void Run()
        {
            try
            {
                // Clear the buffers and reset the screen
                if (_ClientThread.NodeInfo.Door.ClearScreenBefore) _ClientThread.ClrScr();
                //_ClientThread.NodeInfo.Connection.ReadString();

                // Create the node directory and drop files
                CreateNodeDirectory();
                if (_ClientThread.NodeInfo.Door.DoorHasSpecifiedPlatform)
                {
                    RunPlatform(_ClientThread.NodeInfo.Door.Platform);
                }
                else
                {
                    RunOldIniLogic();
                }
            }
            catch (Exception ex)
            {
                RMLog.Exception(ex, "Error while running door '" + _ClientThread.NodeInfo.Door.Name + "'");
            }
            finally
            {
                // Clean up
                try
                {
                    if (_ClientThread.NodeInfo.Door.ClearScreenAfter) _ClientThread.ClrScr();
                    _ClientThread.NodeInfo.Connection.SetBlocking(true); // In case native door disabled blocking sockets
                    _ClientThread.NodeInfo.Connection.ReadString(); // flush buffer so it doesn't spill out from game
                }
                catch { /* Ignore */ }
            }
        }

        private string TranslateCLS(string command)
        {
            List<KeyValuePair<string, string>> CLS = new List<KeyValuePair<string, string>>() {
                new KeyValuePair<string, string>("**ALIAS", _ClientThread.NodeInfo.User.Alias),
                new KeyValuePair<string, string>("DOOR32", StringUtils.ExtractShortPathName(StringUtils.PathCombine(ProcessUtils.StartupPath, "node" + _ClientThread.NodeInfo.Node.ToString(), "door32.sys"))),
                new KeyValuePair<string, string>("DOORSYS", StringUtils.ExtractShortPathName(StringUtils.PathCombine(ProcessUtils.StartupPath, "node" + _ClientThread.NodeInfo.Node.ToString(), "door.sys"))),
                new KeyValuePair<string, string>("DOORFILE", StringUtils.ExtractShortPathName(StringUtils.PathCombine(ProcessUtils.StartupPath, "node" + _ClientThread.NodeInfo.Node.ToString(), "doorfile.sr"))),
                new KeyValuePair<string, string>("DORINFOx", StringUtils.ExtractShortPathName(StringUtils.PathCombine(ProcessUtils.StartupPath, "node" + _ClientThread.NodeInfo.Node.ToString(), "dorinfo" + _ClientThread.NodeInfo.Node.ToString() + ".def"))),
                new KeyValuePair<string, string>("DORINFO1", StringUtils.ExtractShortPathName(StringUtils.PathCombine(ProcessUtils.StartupPath, "node" + _ClientThread.NodeInfo.Node.ToString(), "dorinfo1.def"))),
                new KeyValuePair<string, string>("DORINFO", StringUtils.ExtractShortPathName(StringUtils.PathCombine(ProcessUtils.StartupPath, "node" + _ClientThread.NodeInfo.Node.ToString(), "dorinfo.def"))),
                new KeyValuePair<string, string>("HANDLE", _ClientThread.NodeInfo.Connection.Handle.ToString()),
                new KeyValuePair<string, string>("IPADDRESS", _ClientThread.NodeInfo.Connection.GetRemoteIP()),
                new KeyValuePair<string, string>("MINUTESLEFT", _ClientThread.NodeInfo.MinutesLeft.ToString()),
                new KeyValuePair<string, string>("NODE", _ClientThread.NodeInfo.Node.ToString()),
                new KeyValuePair<string, string>("**PASSWORD", _ClientThread.NodeInfo.User.PasswordHash),
                new KeyValuePair<string, string>("SECONDSLEFT", _ClientThread.NodeInfo.SecondsLeft.ToString()),
                new KeyValuePair<string, string>("SOCKETHANDLE", _ClientThread.NodeInfo.Connection.Handle.ToString()),
                new KeyValuePair<string, string>("**USERNAME", _ClientThread.NodeInfo.User.Alias),
            };
            foreach (DictionaryEntry DE in _ClientThread.NodeInfo.User.AdditionalInfo)
            {
                CLS.Add(new KeyValuePair<string, string>("**" + DE.Key.ToString(), DE.Value.ToString()));
            }

            // Perform translation
            for (int i = 0; i < CLS.Count; i++)
            {
                if (CLS[i].Value != null)
                {
                    command = command.Replace("*" + CLS[i].Key.ToString().ToUpper(), CLS[i].Value.ToString());
                }
            }
            return command;
        }
    }
}
