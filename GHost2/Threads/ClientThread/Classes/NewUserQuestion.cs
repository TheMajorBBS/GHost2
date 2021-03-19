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
using System.Linq;

namespace MajorBBS.GHost
{
    public class NewUserQuestion : ConfigHelper
    {
        public bool Confirm { get; set; }
        public bool Required { get; set; }
        public ValidationType Validate { get; set; }

        private NewUserQuestion()
        {
            // Don't let the user instantiate this without a constructor
        }

        public NewUserQuestion(string question)
            : base(ConfigSaveLocation.Relative, StringUtils.PathCombine("config", "newuser.ini"))
        {
            Confirm = false;
            Required = false;
            Validate = ValidationType.None;

            Load(question);
        }

        public static string[] GetQuestions()
        {
            using (IniFile Ini = new IniFile(StringUtils.PathCombine(ProcessUtils.StartupPath, StringUtils.PathCombine("config", "newuser.ini"))))
            {
                // Return all the sections in newuser.ini, except for [alias] and [password] since they're reserved
                return Ini.ReadSections().Where(x => (x.ToLower() != "alias") && (x.ToLower() != "password")).ToArray();
            }
        }
    }
}
