using RandM.RMLib;
using System;
using System.Diagnostics;
using System.IO;

namespace MajorBBS.GHost.Classes
{
    /**
     * The FixedEvent is triggered in code and executes a powershell script at a OS level
     */
    class FixedEvent
    {
        static string powerShellCmd = "C:\\Windows\\System32\\WindowsPowerShell\\v1.0\\powershell.exe";

        public FixedEvent(string eventName)
        {
            string eventFileName = string.Format(
                "fixed-events\\{0}.ps1",
                eventName
            );
            string eventCmd = Path.Combine(ProcessUtils.StartupPath, eventFileName);
            if (!File.Exists(eventCmd)) return;

            Process proc = new Process();
            ProcessStartInfo PSI = new ProcessStartInfo(FixedEvent.powerShellCmd, eventCmd)
            {
                WindowStyle = ProcessWindowStyle.Minimized,
                WorkingDirectory = ProcessUtils.StartupPath,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
            };

            proc.StartInfo = PSI;
            proc.Start();

            while (!proc.HasExited)
            {
                string errorStr = proc.StandardError.ReadLine();
                if (errorStr != "" && errorStr != null)
                {
                    string errFormat = String.Format(
                        "FIXED EVENT ERROR [{0}]: {1}",
                        eventName,
                        errorStr
                    );
                    RMLog.Error(errFormat);
                }

                string conStr = proc.StandardOutput.ReadLine();
                if (conStr != "" && conStr != null)
                {
                    string conFormat = string.Format(
                        "FIXED EVENT {0}: {1}",
                        eventName,
                        conStr
                    );
                    RMLog.Info(conStr);
                }
            }
        }
    }
}
