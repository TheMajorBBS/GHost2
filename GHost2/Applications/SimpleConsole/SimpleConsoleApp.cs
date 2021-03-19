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
using System.Diagnostics;

namespace MajorBBS.GHost
{
    static class SimpleConsoleApp
    {
        private static GHost _GHost = null;

        public static void Start()
        {
            // Initialize the screen
            Crt.ClrScr();
            StatusText(Helpers.Copyright, false);

            // Check if running as root
            if (Helpers.StartedAsRoot)
            {
                StatusText("", false);
                StatusText("*** WARNING: Running GHost as root is NOT recommended ***", false);
                StatusText("", false);
                StatusText("A safer alternative to running GHost as root is to run it via 'privbind'", false);
                StatusText("This will ensure GHost is able to bind to ports in the < 1024 range, but", false);
                StatusText("it will run as a regular unprivileged program in every other way", false);
                StatusText("", false);
                StatusText("See start.sh for an example of the recommended method to start GHost", false);
                StatusText("", false);
            }

            // Setup log handler
            RMLog.Handler += RMLog_Handler;

            // Init GHost 
            _GHost = new GHost();
            _GHost.Start();

            // Main program loop
            bool Quit = false;
            while (!Quit)
            {
                char Ch = Crt.ReadKey();
                switch (Ch.ToString().ToUpper())
                {
                    case "\0":
                        char Ch2 = Crt.ReadKey();
                        if (Ch2 == ';') // F1
                        {
                            StatusText("", false);
                            StatusText("GHost/2 WFC Screen Help", false);
                            StatusText("-=-=-=-=-=-=-=-=-=-=-=-", false);
                            StatusText("F1 = Help  (this screen)", false);
                            StatusText("C  = Clear (clear the status window)", false);
                            StatusText("P  = Pause (reject new connections, leave existing connections alone)", false);
                            StatusText("S  = Setup (launch the config program)", false);
                            StatusText("Q  = Quit  (shut down and terminate existing connections)", false);
                            StatusText("", false);
                        }
                        break;
                    case "C":
                        Crt.ClrScr();
                        break;
                    case "P":
                        _GHost.Pause();
                        break;
                    case "S":
                        Process.Start(StringUtils.PathCombine(ProcessUtils.StartupPath, "GHostConfig.exe"));
                        break;
                    case "Q":
                        // Check if we're already stopped (or are stopping)
                        if ((_GHost.Status != GHostStatus.Stopped) && (_GHost.Status != GHostStatus.Stopping))
                        {
                            int ConnectionCount = NodeManager.ConnectionCount;
                            if (ConnectionCount > 0)
                            {
                                StatusText("", false);
                                StatusText("There are " + ConnectionCount.ToString() + " active connections.", false);
                                StatusText("Are you sure you want to quit [y/N]: ", false);
                                Ch = Crt.ReadKey();
                                if (Ch.ToString().ToUpper() != "Y")
                                {
                                    StatusText("", false);
                                    StatusText("Cancelling quit request.", false);
                                    StatusText("", false);
                                    continue;
                                }
                            }
                        }

                        _GHost.Stop();
                        _GHost.Dispose();
                        Quit = true;
                        break;
                }
            }

            Environment.Exit(0);
        }

        // TODOX Have entries in the INI file that define which colour to use for each type of message
        private static void RMLog_Handler(object sender, RMLogEventArgs e)
        {
            StatusText($"{e.Level.ToString().ToUpper()}: {e.Message}");
        }

        private static void StatusText(string text, bool prefixWithTime = true)
        {
            if (prefixWithTime && (!string.IsNullOrEmpty(text)))
            {
                text = $"{DateTime.Now.ToString(Config.Instance.TimeFormatUI)}  {text}";
            }
            Crt.Write($"{text}\r\n");
        }
    }
}
