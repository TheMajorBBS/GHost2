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
using System.Linq;

namespace MajorBBS.GHost
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            // Check for service mode or console mode
            if (Environment.UserInteractive || OSUtils.IsUnix)
            {
                // Interactive mode (in other words, not service mode)
                if (args.Contains("console", StringComparer.OrdinalIgnoreCase))
                {
                    ConsoleApp.Start(args);
                }
                else if (args.Contains("gui", StringComparer.OrdinalIgnoreCase))
                {
                    GuiApp.Start();
                }
                else if (args.Contains("service", StringComparer.OrdinalIgnoreCase) && args.Contains("install", StringComparer.OrdinalIgnoreCase))
                {
                    ServiceApp.Install();
                }
                else if (args.Contains("service", StringComparer.OrdinalIgnoreCase) && args.Contains("uninstall", StringComparer.OrdinalIgnoreCase))
                {
                    ServiceApp.Uninstall();
                }
                else if (args.Contains("simpleconsole", StringComparer.OrdinalIgnoreCase))
                {
                    SimpleConsoleApp.Start();
                }
                else
                {
                    DisplayUsage();
                }
            }
            else
            {
                // Non-interactive mode (in other words, service mode)
                ServiceApp.Start();
            }
        }

        private static void DisplayUsage()
        {
            Console.WriteLine();
            Console.WriteLine("GHost.exe Usage:");
            Console.WriteLine();
            Console.WriteLine("  CONSOLE MODE");
            Console.WriteLine("  ============");
            Console.WriteLine("      Fancy:       GHost.exe console");
            Console.WriteLine("      Simple:      GHost.exe console simple");
            Console.WriteLine();
            Console.WriteLine("  GUI MODE");
            Console.WriteLine("  ========");
            Console.WriteLine("      Normal:      GHost.exe gui");
            Console.WriteLine();
            Console.WriteLine("  SERVICE MODE");
            Console.WriteLine("  ============");
            Console.WriteLine("      Install:     GHost.exe service install");
            Console.WriteLine("      Uninstall:   GHost.exe service uninstall");
            Console.WriteLine();
            Console.WriteLine("      Start:       NET START GHost");
            Console.WriteLine("      Stop:        NET STOP GHost");
            Console.WriteLine();
            Console.WriteLine("      Pause:       NET PAUSE GHost");
            Console.WriteLine("      Resume:      NET CONTINUE GHost");
            Console.WriteLine();
            Console.WriteLine("Hit a key to quit");
            Console.ReadKey();
        }
    }
}
