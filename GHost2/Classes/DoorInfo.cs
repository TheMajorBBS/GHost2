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
using System.Diagnostics;

namespace MajorBBS.GHost
{
    public class DoorInfo : ConfigHelper
    {
        public string Command { get; set; }
        public int ForceQuitDelay { get; set; }
        public string Name { get; set; }
        public string Parameters { get; set; }
        public string Platform { get; set; }
        public bool WatchDTR { get; set; }
        public ProcessWindowStyle WindowStyle { get; set; }
        public bool DoorHasSpecifiedPlatform { get; set; }
        public string Authorization { get; set; }
        public bool ClearScreenBefore { get; set; }
        public bool ClearScreenAfter { get; set; }
        public int MaxDoorNodes { get; set; }

        public DoorInfo(string door)
            : base(ConfigSaveLocation.Relative, StringUtils.PathCombine("doors", door.ToLower() + ".ini"))
        {
            Command = "";
            ForceQuitDelay = 5;
            Name = "";
            Parameters = "";
            Platform = "NotSpecified";
            WatchDTR = true;
            WindowStyle = ProcessWindowStyle.Minimized;
            DoorHasSpecifiedPlatform = false;
            Authorization = "+[default]";
            ClearScreenAfter = false;
            ClearScreenBefore = false;
            MaxDoorNodes = 5;

            if (Load("DOOR"))
            {
                // Check if sysop supplies a Platform value
                if (Platform == "NotSpecified")
                {
                    // Nope, this must be an old door .ini, so guess a platform based on the Native property
                    using (IniFile Ini = new IniFile(base.FileName))
                    {
                        bool Native = Ini.ReadBoolean(base.SectionName, "Native", false);
                        Platform = Native ? "Windows" : "DOS";
                    }
                }
                else
                {
                    DoorHasSpecifiedPlatform = true;
                }

                if (Authorization == "")
                {
                    Authorization = "+[default]";
                }
            }
        }

        public new void Save()
        {
            base.Save("DOOR");
        }
    }
}
