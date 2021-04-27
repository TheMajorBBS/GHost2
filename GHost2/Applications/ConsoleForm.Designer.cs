
namespace MajorBBS.GHost.Applications
{
    partial class ConsoleForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConsoleForm));
            this.tray = new System.Windows.Forms.NotifyIcon(this.components);
            this.rtxtLog = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // tray
            // 
            this.tray.Icon = ((System.Drawing.Icon)(resources.GetObject("tray.Icon")));
            this.tray.Text = "GHost/2";
            this.tray.Visible = true;
            // 
            // rtxtLog
            // 
            this.rtxtLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtxtLog.BackColor = System.Drawing.Color.Black;
            this.rtxtLog.ForeColor = System.Drawing.Color.Silver;
            this.rtxtLog.Location = new System.Drawing.Point(12, 12);
            this.rtxtLog.Name = "rtxtLog";
            this.rtxtLog.Size = new System.Drawing.Size(776, 426);
            this.rtxtLog.TabIndex = 0;
            this.rtxtLog.Text = "";
            // 
            // ConsoleForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 30F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.SlateGray;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.rtxtLog);
            this.Name = "ConsoleForm";
            this.Text = "ConsoleForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ConsoleForm_FormClosing);
            this.Load += new System.EventHandler(this.ConsoleForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox rtxtLog;
        private System.Windows.Forms.NotifyIcon tray;
    }
}