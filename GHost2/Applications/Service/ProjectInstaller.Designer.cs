/*
  GHost/2: A BBS Door Game Server
  Copyright (C) Shawn Rapp, WMSI  (original: Rick Parrish, R&M Software)

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
namespace MajorBBS.GHost
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.spiGHost = new System.ServiceProcess.ServiceProcessInstaller();
            this.siGHost = new System.ServiceProcess.ServiceInstaller();
            // 
            // spiGHost
            // 
            this.spiGHost.Account = System.ServiceProcess.ServiceAccount.NetworkService;
            this.spiGHost.Password = null;
            this.spiGHost.Username = null;
            // 
            // siGHost
            // 
            this.siGHost.Description = "GHost/2 BBS Door Game Server";
            this.siGHost.DisplayName = "GHost/2";
            this.siGHost.ServiceName = "GHost/2";
            this.siGHost.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.spiGHost,
            this.siGHost});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller spiGHost;
        private System.ServiceProcess.ServiceInstaller siGHost;
    }
}