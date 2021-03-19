/*/*
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
using System.IO;
using System.Windows.Forms;

namespace MajorBBS.GHost
{
    public partial class frmMain : Form
    {

        private void frmMain_Load(object sender, EventArgs e)
        {
            try
            {
                InitializeComponent();

                DoorInfo Door = new DoorInfo("RUNBBS");
                chkRUNBBS.Checked = Door.Loaded;

            }
            catch (Exception err)
            {
                ErrorAlert("frmMain_Load Error", err);
            }
        }


        private void OnDoorChanged(object source, FileSystemEventArgs e)
        {
            CreateDoorList();
        }

        private void OnPlatformChanged(object source, FileSystemEventArgs e)
        {
            CreatePlatformList();
        }

        private void chkRUNBBS_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                DoorInfo Door = new DoorInfo("RUNBBS");
                string EnabledFileName = Door.FileName;
                string DisabledFileName = StringUtils.PathCombine(Path.GetDirectoryName(Door.FileName), "_" + Path.GetFileName(Door.FileName));

                if (chkRUNBBS.Checked)
                {
                    // If we don't have a RUNBBS.INI but we have a _RUNBBS.INI, rename it to RUNBBS.INI
                    if ((!File.Exists(EnabledFileName)) && (File.Exists(DisabledFileName)))
                    {
                        FileUtils.FileMove(DisabledFileName, EnabledFileName);
                    }
                }
                else
                {
                    // If we have a RUNBBS.INI file, rename it to _RUNBBS.INI
                    if (File.Exists(EnabledFileName))
                    {
                        FileUtils.FileDelete(DisabledFileName);
                        FileUtils.FileMove(EnabledFileName, DisabledFileName);
                    }
                }
            }
            catch (Exception err)
            {
                ErrorAlert("chkRUNBBS_CheckedChanged Error", err);
            }
        }

        private void btnDoorsEdit_Click(object sender, EventArgs e)
        {
            if (lsboxDoors.Text == "") return;

            string doorName = mapDoors[lsboxDoors.Text];
            Process.Start(StringUtils.PathCombine(ProcessUtils.StartupPath, "GameEditor.exe"), doorName);
        }

        private void btnPlatformEdit_Click(object sender, EventArgs e)
        {
            if (lsboxPlatforms.Text == "") return;

            using (PlatformEditorForm platformEditor = new PlatformEditorForm(mapPlatforms[lsboxPlatforms.Text]))
            {
                platformEditor.ShowDialog();
                //CreatePlatformList();
            }
        }

        private void btnPlatformAdd_Click(object sender, EventArgs e)
        {
            using (IniNameForm frm = new IniNameForm())
            {
                DialogResult res = frm.ShowDialog();
                if (res.Equals(DialogResult.OK))
                {
                    string fileName = frm.iniFileName;
                    string filepath = "platforms/" + fileName + ".ini";

                    string[] lines =
                    {
                        "[PLATFORM]",
                        "Name=New Platform Target",
                        "Type=",
                        "Command=",
                    };
                    File.WriteAllLines(filepath, lines);
                    using (PlatformEditorForm platformEditor = new PlatformEditorForm(fileName))
                    {
                        platformEditor.ShowDialog();
                        //CreatePlatformList();
                    }
                }

            }
        }

        private void btnPlatformDelete_Click(object sender, EventArgs e)
        {
            if (mapPlatforms[lsboxPlatforms.Text] == "") return;

            string fileName = "platforms/" + mapPlatforms[lsboxPlatforms.Text] + ".ini";
            File.Delete(fileName);
        }

        private void btnDoorsAdd_Click(object sender, EventArgs e)
        {
            using (IniNameForm frm = new IniNameForm())
            {
                DialogResult res = frm.ShowDialog();
                if (res.Equals(DialogResult.OK))
                {
                    string fileName = frm.iniFileName;
                    string filepath = "doors/" + fileName + ".ini";

                    string[] lines =
                    {
                        "[DOOR]",
                        "Name=New Door",
                        "Command=",
                        "Parameters=",
                        "Platform=native",
                        "Authorization=+[default]",
                    };
                    File.WriteAllLines(filepath, lines);
                    Process.Start(StringUtils.PathCombine(ProcessUtils.StartupPath, "GameEditor.exe"), fileName);
                }
            }
        }

        private void btnDoorsDelete_Click(object sender, EventArgs e)
        {
            if (lboxTimeEvents.Text == "") return;

            string doorName = mapDoors[lsboxDoors.Text];

            string fileName = "doors/" + doorName + ".ini";
            File.Delete(fileName);
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnTimeEventEdit_Click(object sender, EventArgs e)
        {
            if (lboxTimeEvents.Text == "") return;

            string eventName = mapTimeEvents[lboxTimeEvents.Text];
            TimedEvent evt = new TimedEvent(eventName);
            using (TimedEventForm evtFrm = new TimedEventForm(evt))
            {
                DialogResult res = evtFrm.ShowDialog();
                if (res == DialogResult.OK)
                {
                    evt = evtFrm.StoreForm();
                    evt.SaveEvent(eventName);
                    CreateTimedEventList();
                }
            }
        }

        private void btnTimeEventAdd_Click(object sender, EventArgs e)
        {
            using (IniNameForm frm = new IniNameForm("Adding new event.", "Enter the short name of event."))
            {
                DialogResult res = frm.ShowDialog();
                if (res.Equals(DialogResult.OK))
                {
                    string eventName = frm.iniFileName;
                    TimedEvent evt = new TimedEvent(eventName);
                    using (TimedEventForm evtFrm = new TimedEventForm(evt))
                    {
                        DialogResult resEvt = evtFrm.ShowDialog();
                        if (resEvt == DialogResult.OK)
                        {
                            evt = evtFrm.StoreForm();
                            evt.SaveEvent(eventName);
                            CreateTimedEventList();
                        }
                    }
                }
            }

        }

        private void btnTimeEventDelete_Click(object sender, EventArgs e)
        {
            if (lboxTimeEvents.Text == "") return;
            string eventName = mapTimeEvents[lboxTimeEvents.Text];
            TimedEvent.RemoveSection(eventName);
            CreateTimedEventList();
        }
    }
}
