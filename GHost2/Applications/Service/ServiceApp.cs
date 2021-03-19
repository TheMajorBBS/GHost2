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
using System.Configuration.Install;
using System.ServiceProcess;

namespace MajorBBS.GHost
{
    static class ServiceApp
    {
        public static void Start()
        {
            ServiceBase.Run(new MainService());
        }

        public static void Install()
        {
            try
            {
                Console.WriteLine();
                Console.WriteLine("*********************");
                Console.WriteLine("Installing service...");
                Console.WriteLine("*********************");
                Console.WriteLine();

                ManagedInstallerClass.InstallHelper(new string[] { ProcessUtils.ExecutablePath });

                Console.WriteLine();
                Console.WriteLine("*******************************");
                Console.WriteLine("Service installed successfully!");
                Console.WriteLine("*******************************");
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine("*************************");
                Console.WriteLine("Error installing service!");
                Console.WriteLine("*************************");
                Console.WriteLine(ex.ToString());
                Console.WriteLine();
            }
        }

        public static void Uninstall()
        {
            try
            {
                Console.WriteLine();
                Console.WriteLine("***********************");
                Console.WriteLine("Uninstalling service...");
                Console.WriteLine("***********************");
                Console.WriteLine();

                ManagedInstallerClass.InstallHelper(new string[] { "/u", ProcessUtils.ExecutablePath });

                Console.WriteLine();
                Console.WriteLine("*********************************");
                Console.WriteLine("Service uninstalled successfully!");
                Console.WriteLine("*********************************");
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine("***************************");
                Console.WriteLine("Error uninstalling service!");
                Console.WriteLine("***************************");
                Console.WriteLine(ex.ToString());
                Console.WriteLine();
            }
        }
    }
}
