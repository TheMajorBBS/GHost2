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
using System.Collections.Generic;
using System.Diagnostics;

namespace MajorBBS.GHost
{
    public class Config : ConfigHelper
    {
        public string BBSName { get; set; }
        public int FirstNode { get; set; }
        public int LastNode { get; set; }
        public int NextUserId { get; set; }
        public string PasswordPepper { get; set; }
        public string RLoginServerIP { get; set; }
        public int RLoginServerPort { get; set; }
        public string SysopEmail { get; set; }
        public string SysopFirstName { get; set; }
        public string SysopLastName { get; set; }
        public TerminalType TerminalType { get; set; }
        public string TimeFormatLog { get; set; }
        public string TimeFormatUI { get; set; }
        public int TimePerCall { get; set; }
        public string UnixUser { get; set; }

        public Config()
            : base(ConfigSaveLocation.Relative, StringUtils.PathCombine("config", "GHost.ini"))
        {
            BBSName = "New GHost/2 BBS";
            FirstNode = 1;
            LastNode = 5;
            NextUserId = 1;
            PasswordPepper = Debugger.IsAttached ? "ABCDEFGHIJKLMNOPQRSTUVWXYZ" : StringUtils.RandomString(100);
            RLoginServerIP = "0.0.0.0";
            RLoginServerPort = 513;
            SysopEmail = "root@localhost";
            SysopFirstName = "New";
            SysopLastName = "Sysop";
            TerminalType = TerminalType.ANSI;
            TimeFormatLog = "G";
            TimeFormatUI = "T";
            TimePerCall = 60;
            UnixUser = "GHost";

            Load();

            if (base.Loaded)
            {
                // Check for blank pepper (means it was the first time the config was loaded, and there's no value yet)
                if (string.IsNullOrEmpty(PasswordPepper))
                {
                    PasswordPepper = Debugger.IsAttached ? "ABCDEFGHIJKLMNOPQRSTUVWXYZ" : StringUtils.RandomString(100);
                    Save();
                }
            }
        }

        public void Init()
        {
            // Settings are actually loaded already, just checking that the node numbers are sane here
            RMLog.Info("Loading Global Settings");

            // Flip nodes, if necessary
            if (FirstNode > LastNode)
            {
                int Temp = FirstNode;
                FirstNode = LastNode;
                LastNode = Temp;
            }

            // Save default config file, if necessary
            if (!Loaded)
            {
                RMLog.Info("Unable To Load Global Settings...Will Use Defaults");
                Save();
            }
        }

        public string ServerPorts
        {
            get
            {
                List<string> Result = new List<string>();
                if (RLoginServerPort > 0) Result.Add(RLoginServerPort.ToString());
                return string.Join(",", Result.ToArray());
            }
        }

        public new void Save()
        {
            base.Save();
        }

        #region Singleton https://msdn.microsoft.com/en-us/library/ff650316.aspx
        private static volatile Config _Instance;
        private static object _Lock = new object();

        public static Config Instance
        {
            get
            {
                if (_Instance == null)
                {
                    lock (_Lock)
                    {
                        if (_Instance == null) _Instance = new Config();
                    }
                }

                return _Instance;
            }
        }
        #endregion
    }
}
