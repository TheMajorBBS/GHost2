using RandM.RMLib;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Management.Automation;
using System.Text;

namespace MajorBBS.GHost.Classes
{
    /**
     * The FixedEvent is triggered in code and executes a powershell script at a OS level
     */
    class FixedEvent
    {
        public FixedEvent(string eventName)
        {
            string eventFileName = string.Format(
                "fixed-events\\{0}.ps1",
                eventName
            );
            string eventCmd = Path.Combine(ProcessUtils.StartupPath, eventFileName);
            if (!File.Exists(eventCmd)) return;

            using (PowerShell powerShell = PowerShell.Create())
            {
                powerShell.AddScript(System.IO.File.ReadAllText(eventCmd));
                powerShell.AddCommand("Out-String");
                Collection<PSObject> PSOutput = powerShell.Invoke();
                StringBuilder stringBuilder = new StringBuilder();
                foreach (PSObject pSObject in PSOutput)
                    stringBuilder.AppendLine(pSObject.ToString());
                RMLog.Info(stringBuilder.ToString());
            }
        }
    }
}
