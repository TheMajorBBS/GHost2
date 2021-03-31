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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using RandM.RMLib;

namespace MajorBBS.GHost
{
    partial class frmMain
    {
        private Dictionary<string, string> mapPlatforms = new Dictionary<string, string>();
        private Dictionary<string, string> mapDoors = new Dictionary<string, string>();
        private Dictionary<string, string> mapTimeEvents = new Dictionary<string, string>();

        public frmMain()
        {
            InitializeComponent();

            PopulateConfigForm();
            PopulateIPAddresses();
            CreatePlatformList();
            CreateDoorList();
            CreateTimedEventList();
        }

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

        public void InitRunBBS()
        {
            lblTimePerCall.Visible = false;
            lblTimePerCallMinutes.Visible = false;
            txtTimePerCall.Visible = false;
        }

        private void PopulateIPAddresses()
        {
            try
            {
                IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (IPAddress ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        cboRLoginServerIP.Items.Add(ip.ToString());
                    }
                }
            }
            catch (Exception e)
            {
                ErrorAlert("PopulateIPAddresses Error", e);
            }
        }

        private void txtTimeFormatLog_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtTimeFormatUI_TextChanged(object sender, EventArgs e)
        {
            try
            {
                lblTimeFormatUISample.Text = DateTime.Now.ToString(txtTimeFormatUI.Text);
            }
            catch (Exception)
            {
                lblTimeFormatUISample.Text = "Invalid format string";
            }
        }

        private void cmdSave_Click(object sender, EventArgs e)
        {
            StoreConfig();
        }

        private void PopulateConfigForm()
        {
            try
            {
                txtBBSName.Text = Config.Instance.BBSName;
                txtSysopFirstName.Text = Config.Instance.SysopFirstName;
                txtSysopLastName.Text = Config.Instance.SysopLastName;
                txtSysopEmail.Text = Config.Instance.SysopEmail;
                txtFirstNode.Text = Config.Instance.FirstNode.ToString();
                txtLastNode.Text = Config.Instance.LastNode.ToString();
                txtTimePerCall.Text = Config.Instance.TimePerCall.ToString();
                if (Config.Instance.RLoginServerIP != "0.0.0.0")
                {
                    if (!cboRLoginServerIP.Items.Contains(Config.Instance.RLoginServerIP)) cboRLoginServerIP.Items.Add(Config.Instance.RLoginServerIP);
                    cboRLoginServerIP.Text = Config.Instance.RLoginServerIP;
                }
                txtRLoginServerPort.Text = Config.Instance.RLoginServerPort.ToString();
                txtTimeFormatLog.Text = Config.Instance.TimeFormatLog;
                txtTimeFormatUI.Text = Config.Instance.TimeFormatUI;
            }
            catch (Exception e)
            {
                ErrorAlert("Main Form Constructor", e);
            }
        }

        private void StoreConfig()
        {
            Config.Instance.BBSName = txtBBSName.Text;
            Config.Instance.SysopFirstName = txtSysopFirstName.Text;
            Config.Instance.SysopLastName = txtSysopLastName.Text;
            Config.Instance.SysopEmail = txtSysopEmail.Text;
            Config.Instance.FirstNode = Int16.Parse(txtFirstNode.Text);
            Config.Instance.LastNode = Int16.Parse(txtLastNode.Text);
            Config.Instance.TimePerCall = Int16.Parse(txtTimePerCall.Text);
            
            if (cboRLoginServerIP.Text == "All IP addresses")
            {
                Config.Instance.RLoginServerIP = "0.0.0.0";
            } 
            else
            {
                Config.Instance.RLoginServerIP = cboRLoginServerIP.Text;
            }
            Config.Instance.RLoginServerPort = Int32.Parse(txtRLoginServerPort.Text);
            Config.Instance.TimeFormatLog = txtTimeFormatLog.Text;
            Config.Instance.TimeFormatUI = txtTimeFormatUI.Text;
            Config.Instance.Save();
        }

        private void CreatePlatformList()
        {
            try
            {
                string[] platformList = Directory.GetFiles("platforms");
                lsboxPlatforms.Items.Clear();
                mapPlatforms.Clear();
                foreach (string platformPath in platformList)
                {
                    if (platformPath.EndsWith(".ini"))
                    {
                        string platformName = platformPath.Substring(10, platformPath.Length - 14);
                        PlatformInfo platformInfo = new PlatformInfo(platformName);

                        lsboxPlatforms.Items.Add(platformInfo.Name);
                        mapPlatforms.Add(platformInfo.Name, platformName);
                    }
                }
            }
            catch(Exception e)
            {
                ErrorAlert("CrearePlatformList Error", e);
            }
        }

        private void CreateDoorList()
        {
            try
            {
                string[] doorList = Directory.GetFiles("doors");
                lsboxDoors.Items.Clear();
                mapDoors.Clear();
                foreach (string doorPath in doorList)
                {
                    if (doorPath.EndsWith(".ini"))
                    {
                        string doorName = doorPath.Substring(6, doorPath.Length - 10);
                        DoorInfo doorInfo = new DoorInfo(doorName);

                        lsboxDoors.Items.Add(doorInfo.Name);
                        mapDoors.Add(doorInfo.Name, doorName);
                    }
                }
            } catch(Exception e)
            {
                ErrorAlert("CreateDoorList Error", e);
            }
        }

        private void CreateTimedEventList()
        {
            try
            {
                string[] timedEventList = TimedEvent.GetEventNames();
                lboxTimeEvents.Items.Clear();
                mapTimeEvents.Clear();

                foreach (string evtName in timedEventList) 
                {
                    TimedEvent evt = new TimedEvent(evtName);
                    lboxTimeEvents.Items.Add(evt.Name);
                    mapTimeEvents.Add(evt.Name, evtName);
                }
            } catch(Exception e)
            {
                ErrorAlert("CreateTimedEventList Error", e);
            }
        }

        private void ErrorAlert(string caption, Exception e)
        {
            string message = "Error Occurred: " + e.Message;
            var result = MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (lsboxDoors.Text == "") return;

            string doorName = mapDoors[lsboxDoors.Text];
            Process.Start(StringUtils.PathCombine(ProcessUtils.StartupPath, "DoorEditor.exe"), doorName);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabAbout = new System.Windows.Forms.TabPage();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.tabServerSettings = new System.Windows.Forms.TabPage();
            this.lblTimePerCallMinutes = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.cmdSave = new System.Windows.Forms.Button();
            this.lblTimeFormatUISample = new System.Windows.Forms.Label();
            this.lblTimeFormatLogSample = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.txtTimeFormatUI = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.txtTimeFormatLog = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.cboRLoginServerIP = new System.Windows.Forms.ComboBox();
            this.txtRLoginServerPort = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtTimePerCall = new System.Windows.Forms.TextBox();
            this.lblTimePerCall = new System.Windows.Forms.Label();
            this.txtLastNode = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtFirstNode = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtSysopEmail = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtSysopLastName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtSysopFirstName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtBBSName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPlatforms = new System.Windows.Forms.TabPage();
            this.btnPlatformDelete = new System.Windows.Forms.Button();
            this.btnPlatformEdit = new System.Windows.Forms.Button();
            this.btnPlatformAdd = new System.Windows.Forms.Button();
            this.lsboxPlatforms = new System.Windows.Forms.ListBox();
            this.tabDoors = new System.Windows.Forms.TabPage();
            this.btnDoorsTest = new System.Windows.Forms.Button();
            this.btnDoorsAdd = new System.Windows.Forms.Button();
            this.btnDoorsDelete = new System.Windows.Forms.Button();
            this.btnDoorsEdit = new System.Windows.Forms.Button();
            this.lsboxDoors = new System.Windows.Forms.ListBox();
            this.tabEvents = new System.Windows.Forms.TabPage();
            this.label17 = new System.Windows.Forms.Label();
            this.btnTimeEventDelete = new System.Windows.Forms.Button();
            this.btnTimeEventEdit = new System.Windows.Forms.Button();
            this.btnTimeEventAdd = new System.Windows.Forms.Button();
            this.lboxTimeEvents = new System.Windows.Forms.ListBox();
            this.watcherDoors = new System.IO.FileSystemWatcher();
            this.watcherPlatforms = new System.IO.FileSystemWatcher();
            this.tabControl1.SuspendLayout();
            this.tabAbout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.tabServerSettings.SuspendLayout();
            this.tabPlatforms.SuspendLayout();
            this.tabDoors.SuspendLayout();
            this.tabEvents.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.watcherDoors)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.watcherPlatforms)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabAbout);
            this.tabControl1.Controls.Add(this.tabServerSettings);
            this.tabControl1.Controls.Add(this.tabPlatforms);
            this.tabControl1.Controls.Add(this.tabDoors);
            this.tabControl1.Controls.Add(this.tabEvents);
            this.tabControl1.Location = new System.Drawing.Point(16, 16);
            this.tabControl1.Multiline = true;
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(459, 390);
            this.tabControl1.TabIndex = 8;
            // 
            // tabAbout
            // 
            this.tabAbout.Controls.Add(this.pictureBox1);
            this.tabAbout.Controls.Add(this.textBox1);
            this.tabAbout.Controls.Add(this.label7);
            this.tabAbout.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabAbout.Location = new System.Drawing.Point(4, 22);
            this.tabAbout.Name = "tabAbout";
            this.tabAbout.Size = new System.Drawing.Size(451, 364);
            this.tabAbout.TabIndex = 3;
            this.tabAbout.Text = "About";
            this.tabAbout.UseVisualStyleBackColor = true;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pictureBox1.Image = global::MajorBBS.GHost.Properties.Resources.ghost_png;
            this.pictureBox1.Location = new System.Drawing.Point(83, 39);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(70, 69);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.SystemColors.Window;
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox1.Cursor = System.Windows.Forms.Cursors.Default;
            this.textBox1.Location = new System.Drawing.Point(62, 118);
            this.textBox1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(292, 106);
            this.textBox1.TabIndex = 1;
            this.textBox1.Text = "CopyLeft 2021 Major BBS (GPL3)\r\nOriginated from the GameSrv code (GPL3) by Rick P" +
    "arrish\r\n\r\nCredits: \r\nRMLib and RMUILib by Rick Parrish\r\nNetFoss: by PC Micro Sys" +
    "tems\r\n\r\n";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(158, 69);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(92, 26);
            this.label7.TabIndex = 0;
            this.label7.Text = "GHost/2";
            // 
            // tabServerSettings
            // 
            this.tabServerSettings.Controls.Add(this.lblTimePerCallMinutes);
            this.tabServerSettings.Controls.Add(this.label16);
            this.tabServerSettings.Controls.Add(this.cmdSave);
            this.tabServerSettings.Controls.Add(this.lblTimeFormatUISample);
            this.tabServerSettings.Controls.Add(this.lblTimeFormatLogSample);
            this.tabServerSettings.Controls.Add(this.label15);
            this.tabServerSettings.Controls.Add(this.label14);
            this.tabServerSettings.Controls.Add(this.txtTimeFormatUI);
            this.tabServerSettings.Controls.Add(this.label13);
            this.tabServerSettings.Controls.Add(this.txtTimeFormatLog);
            this.tabServerSettings.Controls.Add(this.label12);
            this.tabServerSettings.Controls.Add(this.cboRLoginServerIP);
            this.tabServerSettings.Controls.Add(this.txtRLoginServerPort);
            this.tabServerSettings.Controls.Add(this.label9);
            this.tabServerSettings.Controls.Add(this.txtTimePerCall);
            this.tabServerSettings.Controls.Add(this.lblTimePerCall);
            this.tabServerSettings.Controls.Add(this.txtLastNode);
            this.tabServerSettings.Controls.Add(this.label6);
            this.tabServerSettings.Controls.Add(this.txtFirstNode);
            this.tabServerSettings.Controls.Add(this.label5);
            this.tabServerSettings.Controls.Add(this.txtSysopEmail);
            this.tabServerSettings.Controls.Add(this.label4);
            this.tabServerSettings.Controls.Add(this.txtSysopLastName);
            this.tabServerSettings.Controls.Add(this.label3);
            this.tabServerSettings.Controls.Add(this.txtSysopFirstName);
            this.tabServerSettings.Controls.Add(this.label2);
            this.tabServerSettings.Controls.Add(this.txtBBSName);
            this.tabServerSettings.Controls.Add(this.label1);
            this.tabServerSettings.Font = new System.Drawing.Font("Arial", 8.249F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabServerSettings.Location = new System.Drawing.Point(4, 22);
            this.tabServerSettings.Name = "tabServerSettings";
            this.tabServerSettings.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.tabServerSettings.Size = new System.Drawing.Size(451, 364);
            this.tabServerSettings.TabIndex = 0;
            this.tabServerSettings.Text = "Server Settings";
            this.tabServerSettings.UseVisualStyleBackColor = true;
            // 
            // lblTimePerCallMinutes
            // 
            this.lblTimePerCallMinutes.AutoSize = true;
            this.lblTimePerCallMinutes.Location = new System.Drawing.Point(79, 244);
            this.lblTimePerCallMinutes.Name = "lblTimePerCallMinutes";
            this.lblTimePerCallMinutes.Size = new System.Drawing.Size(52, 14);
            this.lblTimePerCallMinutes.TabIndex = 73;
            this.lblTimePerCallMinutes.Text = "(minutes)";
            // 
            // label16
            // 
            this.label16.Location = new System.Drawing.Point(132, 314);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(203, 30);
            this.label16.TabIndex = 72;
            this.label16.Text = "These settings will require GHost/2 to be restarted before they will take effect." +
    "";
            // 
            // cmdSave
            // 
            this.cmdSave.Location = new System.Drawing.Point(341, 321);
            this.cmdSave.Name = "cmdSave";
            this.cmdSave.Size = new System.Drawing.Size(75, 23);
            this.cmdSave.TabIndex = 63;
            this.cmdSave.Text = "&Save";
            this.cmdSave.UseVisualStyleBackColor = true;
            this.cmdSave.Click += new System.EventHandler(this.cmdSave_Click);
            // 
            // lblTimeFormatUISample
            // 
            this.lblTimeFormatUISample.AutoSize = true;
            this.lblTimeFormatUISample.Location = new System.Drawing.Point(250, 241);
            this.lblTimeFormatUISample.Name = "lblTimeFormatUISample";
            this.lblTimeFormatUISample.Size = new System.Drawing.Size(75, 14);
            this.lblTimeFormatUISample.TabIndex = 71;
            this.lblTimeFormatUISample.Text = "UI sample time";
            // 
            // lblTimeFormatLogSample
            // 
            this.lblTimeFormatLogSample.AutoSize = true;
            this.lblTimeFormatLogSample.Location = new System.Drawing.Point(250, 222);
            this.lblTimeFormatLogSample.Name = "lblTimeFormatLogSample";
            this.lblTimeFormatLogSample.Size = new System.Drawing.Size(108, 14);
            this.lblTimeFormatLogSample.TabIndex = 70;
            this.lblTimeFormatLogSample.Text = "Log sample date/time";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(216, 241);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(19, 14);
            this.label15.TabIndex = 69;
            this.label15.Text = "UI:";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(216, 222);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(28, 14);
            this.label14.TabIndex = 68;
            this.label14.Text = "Log:";
            // 
            // txtTimeFormatUI
            // 
            this.txtTimeFormatUI.Location = new System.Drawing.Point(330, 190);
            this.txtTimeFormatUI.Name = "txtTimeFormatUI";
            this.txtTimeFormatUI.Size = new System.Drawing.Size(86, 20);
            this.txtTimeFormatUI.TabIndex = 61;
            this.txtTimeFormatUI.Tag = "UI Time Format";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(327, 174);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(77, 14);
            this.label13.TabIndex = 67;
            this.label13.Text = "UI Time Format";
            // 
            // txtTimeFormatLog
            // 
            this.txtTimeFormatLog.Location = new System.Drawing.Point(216, 190);
            this.txtTimeFormatLog.Name = "txtTimeFormatLog";
            this.txtTimeFormatLog.Size = new System.Drawing.Size(86, 20);
            this.txtTimeFormatLog.TabIndex = 59;
            this.txtTimeFormatLog.Tag = "Log Time Format";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(216, 174);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(86, 14);
            this.label12.TabIndex = 66;
            this.label12.Text = "Log Time Format";
            // 
            // cboRLoginServerIP
            // 
            this.cboRLoginServerIP.FormattingEnabled = true;
            this.cboRLoginServerIP.Items.AddRange(new object[] {
            "All IP addresses"});
            this.cboRLoginServerIP.Location = new System.Drawing.Point(233, 40);
            this.cboRLoginServerIP.Name = "cboRLoginServerIP";
            this.cboRLoginServerIP.Size = new System.Drawing.Size(135, 22);
            this.cboRLoginServerIP.TabIndex = 52;
            this.cboRLoginServerIP.Tag = "RLogin Server IP";
            this.cboRLoginServerIP.Text = "All IP addresses";
            // 
            // txtRLoginServerPort
            // 
            this.txtRLoginServerPort.Location = new System.Drawing.Point(374, 41);
            this.txtRLoginServerPort.Name = "txtRLoginServerPort";
            this.txtRLoginServerPort.Size = new System.Drawing.Size(59, 20);
            this.txtRLoginServerPort.TabIndex = 54;
            this.txtRLoginServerPort.Tag = "RLogin Server Port";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(233, 25);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(174, 14);
            this.label9.TabIndex = 62;
            this.label9.Text = "RLogin Server IP Address and Port";
            // 
            // txtTimePerCall
            // 
            this.txtTimePerCall.Location = new System.Drawing.Point(14, 241);
            this.txtTimePerCall.Name = "txtTimePerCall";
            this.txtTimePerCall.Size = new System.Drawing.Size(59, 20);
            this.txtTimePerCall.TabIndex = 49;
            this.txtTimePerCall.Tag = "Time Per Call";
            // 
            // lblTimePerCall
            // 
            this.lblTimePerCall.AutoSize = true;
            this.lblTimePerCall.Location = new System.Drawing.Point(14, 225);
            this.lblTimePerCall.Name = "lblTimePerCall";
            this.lblTimePerCall.Size = new System.Drawing.Size(68, 14);
            this.lblTimePerCall.TabIndex = 56;
            this.lblTimePerCall.Text = "Time Per Call";
            // 
            // txtLastNode
            // 
            this.txtLastNode.Location = new System.Drawing.Point(101, 193);
            this.txtLastNode.Name = "txtLastNode";
            this.txtLastNode.Size = new System.Drawing.Size(59, 20);
            this.txtLastNode.TabIndex = 48;
            this.txtLastNode.Tag = "End Node Number";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(79, 196);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(16, 14);
            this.label6.TabIndex = 53;
            this.label6.Text = "to";
            // 
            // txtFirstNode
            // 
            this.txtFirstNode.Location = new System.Drawing.Point(14, 193);
            this.txtFirstNode.Name = "txtFirstNode";
            this.txtFirstNode.Size = new System.Drawing.Size(59, 20);
            this.txtFirstNode.TabIndex = 46;
            this.txtFirstNode.Tag = "Start Node Number";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(14, 177);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(146, 14);
            this.label5.TabIndex = 50;
            this.label5.Text = "Start and End Node Numbers";
            // 
            // txtSysopEmail
            // 
            this.txtSysopEmail.Location = new System.Drawing.Point(14, 145);
            this.txtSysopEmail.Name = "txtSysopEmail";
            this.txtSysopEmail.Size = new System.Drawing.Size(200, 20);
            this.txtSysopEmail.TabIndex = 45;
            this.txtSysopEmail.Tag = "Sysop Email Address";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(14, 129);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(109, 14);
            this.label4.TabIndex = 47;
            this.label4.Text = "Sysop Email Address";
            // 
            // txtSysopLastName
            // 
            this.txtSysopLastName.Location = new System.Drawing.Point(14, 106);
            this.txtSysopLastName.Name = "txtSysopLastName";
            this.txtSysopLastName.Size = new System.Drawing.Size(200, 20);
            this.txtSysopLastName.TabIndex = 43;
            this.txtSysopLastName.Tag = "Sysop Last Name";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 90);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(92, 14);
            this.label3.TabIndex = 44;
            this.label3.Text = "Sysop Last Name";
            // 
            // txtSysopFirstName
            // 
            this.txtSysopFirstName.Location = new System.Drawing.Point(14, 67);
            this.txtSysopFirstName.Name = "txtSysopFirstName";
            this.txtSysopFirstName.Size = new System.Drawing.Size(200, 20);
            this.txtSysopFirstName.TabIndex = 42;
            this.txtSysopFirstName.Tag = "Sysop First Name";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 51);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(92, 14);
            this.label2.TabIndex = 41;
            this.label2.Text = "Sysop First Name";
            // 
            // txtBBSName
            // 
            this.txtBBSName.Location = new System.Drawing.Point(14, 28);
            this.txtBBSName.Name = "txtBBSName";
            this.txtBBSName.Size = new System.Drawing.Size(200, 20);
            this.txtBBSName.TabIndex = 40;
            this.txtBBSName.Tag = "BBS Name";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 14);
            this.label1.TabIndex = 39;
            this.label1.Text = "BBS Name";
            // 
            // tabPlatforms
            // 
            this.tabPlatforms.Controls.Add(this.btnPlatformDelete);
            this.tabPlatforms.Controls.Add(this.btnPlatformEdit);
            this.tabPlatforms.Controls.Add(this.btnPlatformAdd);
            this.tabPlatforms.Controls.Add(this.lsboxPlatforms);
            this.tabPlatforms.Location = new System.Drawing.Point(4, 22);
            this.tabPlatforms.Name = "tabPlatforms";
            this.tabPlatforms.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.tabPlatforms.Size = new System.Drawing.Size(451, 364);
            this.tabPlatforms.TabIndex = 1;
            this.tabPlatforms.Text = "Platform Targets";
            this.tabPlatforms.UseVisualStyleBackColor = true;
            // 
            // btnPlatformDelete
            // 
            this.btnPlatformDelete.Location = new System.Drawing.Point(279, 105);
            this.btnPlatformDelete.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnPlatformDelete.Name = "btnPlatformDelete";
            this.btnPlatformDelete.Size = new System.Drawing.Size(66, 29);
            this.btnPlatformDelete.TabIndex = 7;
            this.btnPlatformDelete.Text = "Delete";
            this.btnPlatformDelete.UseVisualStyleBackColor = true;
            this.btnPlatformDelete.Click += new System.EventHandler(this.btnPlatformDelete_Click);
            // 
            // btnPlatformEdit
            // 
            this.btnPlatformEdit.Location = new System.Drawing.Point(279, 73);
            this.btnPlatformEdit.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnPlatformEdit.Name = "btnPlatformEdit";
            this.btnPlatformEdit.Size = new System.Drawing.Size(66, 29);
            this.btnPlatformEdit.TabIndex = 6;
            this.btnPlatformEdit.Text = "Edit";
            this.btnPlatformEdit.UseVisualStyleBackColor = true;
            this.btnPlatformEdit.Click += new System.EventHandler(this.btnPlatformEdit_Click);
            // 
            // btnPlatformAdd
            // 
            this.btnPlatformAdd.Location = new System.Drawing.Point(279, 40);
            this.btnPlatformAdd.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnPlatformAdd.Name = "btnPlatformAdd";
            this.btnPlatformAdd.Size = new System.Drawing.Size(66, 29);
            this.btnPlatformAdd.TabIndex = 5;
            this.btnPlatformAdd.Text = "Add";
            this.btnPlatformAdd.UseVisualStyleBackColor = true;
            this.btnPlatformAdd.Click += new System.EventHandler(this.btnPlatformAdd_Click);
            // 
            // lsboxPlatforms
            // 
            this.lsboxPlatforms.FormattingEnabled = true;
            this.lsboxPlatforms.Location = new System.Drawing.Point(99, 40);
            this.lsboxPlatforms.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.lsboxPlatforms.Name = "lsboxPlatforms";
            this.lsboxPlatforms.Size = new System.Drawing.Size(179, 225);
            this.lsboxPlatforms.TabIndex = 4;
            // 
            // tabDoors
            // 
            this.tabDoors.Controls.Add(this.btnDoorsTest);
            this.tabDoors.Controls.Add(this.btnDoorsAdd);
            this.tabDoors.Controls.Add(this.btnDoorsDelete);
            this.tabDoors.Controls.Add(this.btnDoorsEdit);
            this.tabDoors.Controls.Add(this.lsboxDoors);
            this.tabDoors.Location = new System.Drawing.Point(4, 22);
            this.tabDoors.Name = "tabDoors";
            this.tabDoors.Size = new System.Drawing.Size(451, 364);
            this.tabDoors.TabIndex = 2;
            this.tabDoors.Text = "Doors";
            this.tabDoors.UseVisualStyleBackColor = true;
            // 
            // btnDoorsTest
            // 
            this.btnDoorsTest.Location = new System.Drawing.Point(339, 157);
            this.btnDoorsTest.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnDoorsTest.Name = "btnDoorsTest";
            this.btnDoorsTest.Size = new System.Drawing.Size(68, 29);
            this.btnDoorsTest.TabIndex = 7;
            this.btnDoorsTest.Text = "Test";
            this.btnDoorsTest.UseVisualStyleBackColor = true;
            this.btnDoorsTest.Click += new System.EventHandler(this.btnDoorsTest_Click);
            // 
            // btnDoorsAdd
            // 
            this.btnDoorsAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDoorsAdd.Location = new System.Drawing.Point(339, 58);
            this.btnDoorsAdd.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnDoorsAdd.Name = "btnDoorsAdd";
            this.btnDoorsAdd.Size = new System.Drawing.Size(68, 29);
            this.btnDoorsAdd.TabIndex = 6;
            this.btnDoorsAdd.Text = "Add";
            this.btnDoorsAdd.UseVisualStyleBackColor = true;
            this.btnDoorsAdd.Click += new System.EventHandler(this.btnDoorsAdd_Click);
            // 
            // btnDoorsDelete
            // 
            this.btnDoorsDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDoorsDelete.Location = new System.Drawing.Point(339, 124);
            this.btnDoorsDelete.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnDoorsDelete.Name = "btnDoorsDelete";
            this.btnDoorsDelete.Size = new System.Drawing.Size(68, 29);
            this.btnDoorsDelete.TabIndex = 5;
            this.btnDoorsDelete.Text = "Delete";
            this.btnDoorsDelete.UseVisualStyleBackColor = true;
            this.btnDoorsDelete.Click += new System.EventHandler(this.btnDoorsDelete_Click);
            // 
            // btnDoorsEdit
            // 
            this.btnDoorsEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDoorsEdit.Location = new System.Drawing.Point(339, 91);
            this.btnDoorsEdit.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnDoorsEdit.Name = "btnDoorsEdit";
            this.btnDoorsEdit.Size = new System.Drawing.Size(68, 29);
            this.btnDoorsEdit.TabIndex = 4;
            this.btnDoorsEdit.Text = "Edit";
            this.btnDoorsEdit.UseVisualStyleBackColor = true;
            this.btnDoorsEdit.Click += new System.EventHandler(this.btnDoorsEdit_Click);
            // 
            // lsboxDoors
            // 
            this.lsboxDoors.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lsboxDoors.FormattingEnabled = true;
            this.lsboxDoors.Location = new System.Drawing.Point(63, 58);
            this.lsboxDoors.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.lsboxDoors.Name = "lsboxDoors";
            this.lsboxDoors.Size = new System.Drawing.Size(268, 264);
            this.lsboxDoors.TabIndex = 3;
            // 
            // tabEvents
            // 
            this.tabEvents.Controls.Add(this.label17);
            this.tabEvents.Controls.Add(this.btnTimeEventDelete);
            this.tabEvents.Controls.Add(this.btnTimeEventEdit);
            this.tabEvents.Controls.Add(this.btnTimeEventAdd);
            this.tabEvents.Controls.Add(this.lboxTimeEvents);
            this.tabEvents.Location = new System.Drawing.Point(4, 22);
            this.tabEvents.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tabEvents.Name = "tabEvents";
            this.tabEvents.Size = new System.Drawing.Size(451, 364);
            this.tabEvents.TabIndex = 4;
            this.tabEvents.Text = "Events";
            this.tabEvents.UseVisualStyleBackColor = true;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(46, 14);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(72, 13);
            this.label17.TabIndex = 6;
            this.label17.Text = "Timed Events";
            // 
            // btnTimeEventDelete
            // 
            this.btnTimeEventDelete.Location = new System.Drawing.Point(334, 105);
            this.btnTimeEventDelete.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnTimeEventDelete.Name = "btnTimeEventDelete";
            this.btnTimeEventDelete.Size = new System.Drawing.Size(75, 30);
            this.btnTimeEventDelete.TabIndex = 3;
            this.btnTimeEventDelete.Text = "Delete";
            this.btnTimeEventDelete.UseVisualStyleBackColor = true;
            this.btnTimeEventDelete.Click += new System.EventHandler(this.btnTimeEventDelete_Click);
            // 
            // btnTimeEventEdit
            // 
            this.btnTimeEventEdit.Location = new System.Drawing.Point(334, 68);
            this.btnTimeEventEdit.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnTimeEventEdit.Name = "btnTimeEventEdit";
            this.btnTimeEventEdit.Size = new System.Drawing.Size(75, 30);
            this.btnTimeEventEdit.TabIndex = 2;
            this.btnTimeEventEdit.Text = "Edit";
            this.btnTimeEventEdit.UseVisualStyleBackColor = true;
            this.btnTimeEventEdit.Click += new System.EventHandler(this.btnTimeEventEdit_Click);
            // 
            // btnTimeEventAdd
            // 
            this.btnTimeEventAdd.Location = new System.Drawing.Point(334, 30);
            this.btnTimeEventAdd.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnTimeEventAdd.Name = "btnTimeEventAdd";
            this.btnTimeEventAdd.Size = new System.Drawing.Size(75, 30);
            this.btnTimeEventAdd.TabIndex = 1;
            this.btnTimeEventAdd.Text = "Add";
            this.btnTimeEventAdd.UseVisualStyleBackColor = true;
            this.btnTimeEventAdd.Click += new System.EventHandler(this.btnTimeEventAdd_Click);
            // 
            // lboxTimeEvents
            // 
            this.lboxTimeEvents.FormattingEnabled = true;
            this.lboxTimeEvents.Location = new System.Drawing.Point(49, 30);
            this.lboxTimeEvents.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.lboxTimeEvents.Name = "lboxTimeEvents";
            this.lboxTimeEvents.Size = new System.Drawing.Size(281, 290);
            this.lboxTimeEvents.TabIndex = 0;
            this.lboxTimeEvents.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // watcherDoors
            // 
            this.watcherDoors.EnableRaisingEvents = true;
            this.watcherDoors.Filter = "*.ini";
            this.watcherDoors.Path = "doors";
            this.watcherDoors.SynchronizingObject = this;
            this.watcherDoors.Changed += new System.IO.FileSystemEventHandler(this.OnDoorChanged);
            this.watcherDoors.Created += new System.IO.FileSystemEventHandler(this.OnDoorChanged);
            this.watcherDoors.Deleted += new System.IO.FileSystemEventHandler(this.OnDoorChanged);
            // 
            // watcherPlatforms
            // 
            this.watcherPlatforms.EnableRaisingEvents = true;
            this.watcherPlatforms.Filter = "*.ini";
            this.watcherPlatforms.Path = "platforms";
            this.watcherPlatforms.SynchronizingObject = this;
            this.watcherPlatforms.Changed += new System.IO.FileSystemEventHandler(this.OnPlatformChanged);
            this.watcherPlatforms.Created += new System.IO.FileSystemEventHandler(this.OnPlatformChanged);
            this.watcherPlatforms.Deleted += new System.IO.FileSystemEventHandler(this.OnPlatformChanged);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(487, 418);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "GHost/2 Configuration";
            this.tabControl1.ResumeLayout(false);
            this.tabAbout.ResumeLayout(false);
            this.tabAbout.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.tabServerSettings.ResumeLayout(false);
            this.tabServerSettings.PerformLayout();
            this.tabPlatforms.ResumeLayout(false);
            this.tabDoors.ResumeLayout(false);
            this.tabEvents.ResumeLayout(false);
            this.tabEvents.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.watcherDoors)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.watcherPlatforms)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabServerSettings;
        private System.Windows.Forms.TabPage tabPlatforms;
        private System.Windows.Forms.TabPage tabDoors;
        private System.Windows.Forms.Label lblTimePerCallMinutes;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Button cmdSave;
        private System.Windows.Forms.Label lblTimeFormatUISample;
        private System.Windows.Forms.Label lblTimeFormatLogSample;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox txtTimeFormatUI;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox txtTimeFormatLog;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.ComboBox cboRLoginServerIP;
        private System.Windows.Forms.TextBox txtRLoginServerPort;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtTimePerCall;
        private System.Windows.Forms.Label lblTimePerCall;
        private System.Windows.Forms.TextBox txtLastNode;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtFirstNode;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtSysopEmail;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtSysopLastName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtSysopFirstName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtBBSName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage tabAbout;
        private System.Windows.Forms.Label label7;
        private Button btnPlatformDelete;
        private Button btnPlatformEdit;
        private Button btnPlatformAdd;
        private ListBox lsboxPlatforms;
        private Button btnDoorsAdd;
        private Button btnDoorsDelete;
        private Button btnDoorsEdit;
        private ListBox lsboxDoors;
        private FileSystemWatcher watcherDoors;
        private FileSystemWatcher watcherPlatforms;
        private TabPage tabEvents;
        private ListBox lboxTimeEvents;
        private TextBox textBox1;
        private Button btnTimeEventDelete;
        private Button btnTimeEventEdit;
        private Button btnTimeEventAdd;
        private Label label17;
        private PictureBox pictureBox1;
        private Button btnDoorsTest;
    }
}

