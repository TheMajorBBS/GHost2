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
using System;
using System.Windows.Forms;

namespace MajorBBS.GHost
{
    public partial class PlatformEditorForm : Form
    {
        PlatformInfo platformInfo = null;

        public PlatformEditorForm(string shortName)
        {
            InitializeComponent();

            platformInfo = new PlatformInfo(shortName);
            LoadPlatform();
            Text = "Platform Target Editor - " + shortName + ".ini";
        }

        private void LoadPlatform()
        {
            string[] strDelimiters = new string[] { "\\r", "%r%" };
            txtBootstrap.Lines = platformInfo.Bootstrap.Split(strDelimiters, StringSplitOptions.RemoveEmptyEntries);
            txtName.Text = platformInfo.Name;
            txtBootstrapFile.Text = platformInfo.BootstrapName;
            txtShell.Text = platformInfo.Shell;
            txtShellArgs.Text = platformInfo.Arguments;
            chkRedirectLocal.Checked = platformInfo.RedirectLocal;
        }

        private void StorePlatform()
        {
            platformInfo.Name = txtName.Text;
            platformInfo.Bootstrap = String.Join("%r%", txtBootstrap.Lines);
            platformInfo.BootstrapName = txtBootstrapFile.Text;
            platformInfo.Arguments = txtShellArgs.Text;
            platformInfo.Shell = txtShell.Text;
            platformInfo.RedirectLocal = chkRedirectLocal.Checked;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            StorePlatform();
            platformInfo.Save();
            Close();
        }
    }
}
