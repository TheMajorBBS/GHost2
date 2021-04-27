using RandM.RMLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MajorBBS.GHost.Applications
{
    public partial class ConsoleForm : Form
    {
        private GHost _GHost = new();
        private int _CharWidth = 0;
        private System.Windows.Forms.ContextMenuStrip _contextMenuTray;
        private ToolStripMenuItem _consoleMenuItem;
        private ToolStripMenuItem _configureMenuItem;
        private ToolStripMenuItem _startMenuItem;
        private ToolStripMenuItem _pauseMenuItem;
        private ToolStripMenuItem _stopMenuItem;
        private ToolStripMenuItem _quitMenuItem;

        public ConsoleForm()
        {
            InitializeComponent();
            InitTrayMenu();

            // Determine the character width of our font (used for indenting wrapped lines)
            using (Graphics G = this.CreateGraphics())
            {
                G.PageUnit = GraphicsUnit.Pixel;
                _CharWidth = G.MeasureString("..", this.Font).ToSize().Width - G.MeasureString(".", this.Font).ToSize().Width;
            }

            RMLog.Handler += HandleLog;
            StatusText(Helpers.Copyright, Color.White, false);

            // Init titles
            this.Text = "GHost/2 Log Console v" + GHost.Version;
            tray.Text = "GHost/2 v" + GHost.Version;
            tray.ContextMenuStrip = _contextMenuTray;

            // Init GHost object
            NodeManager.ConnectionCountChangeEvent += NodeManager_ConnectionCountChangeEvent;
            NodeManager.NodeEvent += NodeManager_NodeEvent;
            _GHost.StatusChangeEvent += GHost_StatusChangeEvent;
            _GHost.Start();
            this.Hide();
        }

        private void InitTrayMenu()
        {
            _contextMenuTray = new();

            _consoleMenuItem = new("Open Console");
            _configureMenuItem = new("Configure");
            _startMenuItem = new("Start");
            _pauseMenuItem = new("Pause");
            _stopMenuItem = new("Stop");
            _quitMenuItem = new("Quit");

            _consoleMenuItem.Click += HandleConsole;
            _configureMenuItem.Click += HandleConfigure;
            _startMenuItem.Click += HandleStart;
            _pauseMenuItem.Click += HandlePause;
            _stopMenuItem.Click += HandleStop;
            _quitMenuItem.Click += HandleQuit;

            _contextMenuTray.Items.Add(_consoleMenuItem);
            _contextMenuTray.Items.Add(_configureMenuItem);
            _contextMenuTray.Items.Add(new ToolStripSeparator());
            _contextMenuTray.Items.Add(_startMenuItem);
            _contextMenuTray.Items.Add(_pauseMenuItem);
            _contextMenuTray.Items.Add(_stopMenuItem);
            _contextMenuTray.Items.Add(new ToolStripSeparator());
            _contextMenuTray.Items.Add(_quitMenuItem);
        }

        private void HandleConsole(object sender, EventArgs e)
        {
            this.Show();
        }

        private void HandleConfigure(object sender, EventArgs e)
        {
            Process.Start(StringUtils.PathCombine(ProcessUtils.StartupPath, "GHostConfig.exe"));
        }

        private void HandleStart(object sender, EventArgs e)
        {
            _GHost.Start();
        }

        private void HandlePause(object sender, EventArgs e)
        {
            _GHost.Pause();
        }

        private void HandleStop(object sender, EventArgs e)
        {
            _GHost.Stop();
        }

        private void HandleQuit(object sender, EventArgs e)
        {
            _GHost.Stop();
            _GHost.Dispose();
            Close();
            Application.Exit();
        }

        private void HandleLog(object sender, RMLogEventArgs e)
        {
            switch (e.Level)
            {
                case LogLevel.Debug:
                    StatusText("DEBUG: " + e.Message, Color.LightCyan);
                    break;
                case LogLevel.Error:
                    this.Show();
                    StatusText("ERROR: " + e.Message, Color.Red);
                    break;
                case LogLevel.Info:
                    StatusText(e.Message, Color.LightGray);
                    break;
                case LogLevel.Trace:
                    StatusText("TRACE: " + e.Message, Color.DarkGray);
                    break;
                case LogLevel.Warning:
                    StatusText("WARNING: " + e.Message, Color.Yellow);
                    break;
                default:
                    StatusText("UNKNOWN: " + e.Message, Color.White);
                    break;
            }
        }
        private void StatusText(string message, Color foreColour, bool prefixWithTime = true)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(delegate { StatusText(message, foreColour, prefixWithTime); }));
            }
            else
            {

                if (prefixWithTime)
                {
                    string Time = DateTime.Now.ToString(Config.Instance.TimeFormatUI) + "  ";
                    rtxtLog.SelectionHangingIndent = Time.Length * _CharWidth;
                    rtxtLog.ForeColor = Color.LightGray;
                    rtxtLog.AppendText(Time);
                }
                else
                {
                    rtxtLog.SelectionHangingIndent = 0;
                }

                rtxtLog.ForeColor = foreColour;
                rtxtLog.AppendText(message + "\r\n");
                rtxtLog.SelectionStart = rtxtLog.Text.Length;
                rtxtLog.ScrollToCaret();
            }
        }

        private void UpdateButtonsAndTrayIcon()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(delegate { UpdateButtonsAndTrayIcon(); }));
            }
            else
            {
                switch (_GHost.Status)
                {
                    case GHostStatus.Paused:
                        _startMenuItem.Enabled = true;
                        _pauseMenuItem.Enabled = true;
                        _stopMenuItem.Enabled = true;
                        this.Icon = Properties.Resources.GHost_Paused;
                        break;
                    case GHostStatus.Started:
                        _startMenuItem.Enabled = false;
                        _pauseMenuItem.Enabled = true;
                        _stopMenuItem.Enabled = true;
                        this.Icon = (NodeManager.ConnectionCount == 0) ? Properties.Resources.GHost_Started : Properties.Resources.GHost_InUse;
                        break;
                    case GHostStatus.Stopped:
                        _startMenuItem.Enabled = true;
                        _pauseMenuItem.Enabled = false;
                        _stopMenuItem.Enabled = false;
                        this.Icon = Properties.Resources.GHost_Stopped;
                        break;
                }
                tray.Icon = this.Icon;
            }
        }

        private void NodeManager_ConnectionCountChangeEvent(object sender, IntEventArgs e)
        {
            UpdateButtonsAndTrayIcon();
        }

        private void NodeManager_NodeEvent(object sender, NodeEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(delegate { NodeManager_NodeEvent(sender, e); }));
            }
            else
            {
                if (e.EventType == NodeEventType.LogOff)
                {
                    UpdateButtonsAndTrayIcon();
                }
                else if (e.EventType == NodeEventType.LogOn)
                {
                    UpdateButtonsAndTrayIcon();

                    //// Add history item
                    //ListViewItem LVIHistory = new ListViewItem(e.NodeInfo.Node.ToString());
                    //LVIHistory.SubItems.Add(e.NodeInfo.ConnectionType.ToString());
                    //LVIHistory.SubItems.Add(e.NodeInfo.Connection.GetRemoteIP());
                    //LVIHistory.SubItems.Add(e.NodeInfo.User.Alias);
                    //LVIHistory.SubItems.Add(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString());
                    //lvHistory.Items.Insert(0, LVIHistory);

                    // Keep a counter for number of connections
                    //switch (e.NodeInfo.ConnectionType)
                    //{
                    //    case ConnectionType.RLogin:
                    //        lblRLoginCount.Text = (Convert.ToInt32(lblRLoginCount.Text) + 1).ToString();
                    //        break;
                    //}

                }

                // Update status
                //ListViewItem LVI = lvNodes.Items[e.NodeInfo.Node - Config.Instance.FirstNode];
                //LVI.SubItems[1].Text = (e.EventType == NodeEventType.LogOff ? "" : e.NodeInfo.ConnectionType.ToString());
                //LVI.SubItems[2].Text = (e.EventType == NodeEventType.LogOff ? "" : e.NodeInfo.Connection.GetRemoteIP());
                //LVI.SubItems[3].Text = (e.EventType == NodeEventType.LogOff ? "" : e.NodeInfo.User.Alias);
                //LVI.SubItems[4].Text = (e.EventType == NodeEventType.LogOff ? "Waiting for a caller..." : e.Status);
            }
        }
        private void GHost_StatusChangeEvent(object sender, StatusEventArgs e)
        {
            UpdateButtonsAndTrayIcon();
        }

        private void ConsoleForm_Load(object sender, EventArgs e)
        {

        }

        private void ConsoleForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
            }
        }
    }
}
