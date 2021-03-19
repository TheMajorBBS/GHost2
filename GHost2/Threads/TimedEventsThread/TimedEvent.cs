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
using System.Linq;

namespace MajorBBS.GHost
{
    public class TimedEvent : ConfigHelper
    {
        public string Name { get; set; }
        public string Command { get; set; }
        public string Days { get; set; }
        public string Time { get; set; }
        public bool GoOffline { get; set; }
        public string Platform { get; set; }
        public ProcessWindowStyle WindowStyle { get; set; }

        private TimedEvent()
        {
            // Don't let the user instantiate this without a constructor
        }

        public TimedEvent(string eventName = null)
            : base(ConfigSaveLocation.Relative, StringUtils.PathCombine("config", "timed-events.ini"))
        {
            Name = "";
            Command = "";
            Days = "";
            Time = "";
            GoOffline = false;
            Platform = "";
            WindowStyle = ProcessWindowStyle.Normal;

            if (eventName != null)
                Load(eventName);
        }

        public static string[] GetEventNames()
        {
            using (IniFile Ini = new IniFile(StringUtils.PathCombine(ProcessUtils.StartupPath, StringUtils.PathCombine("config", "timed-events.ini"))))
            {
                // Return all the sections in timed-events.ini
                return Ini.ReadSections().ToArray();
            }
        }

        public static void RemoveSection(string eventName)
        {
            using (IniFile Ini = new IniFile(StringUtils.PathCombine(ProcessUtils.StartupPath, StringUtils.PathCombine("config", "timed-events.ini"))))
            {
                Ini.EraseSection(eventName);
            }
        }

       public void SaveEvent(string eventName)
        {
            Save(eventName);
        }
    }
}
