/*
  GHost/2: Door Server
  Copyleft 2021 Major BBS (GPL3)

  This file is part of GHost/2 solution, GHostConfig project.

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
    partial class PlatformEditorForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.txtBootstrap = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.txtShell = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtShellArgs = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtBootstrapFile = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.chkRedirectLocal = new System.Windows.Forms.CheckBox();
            this.chkSurpressErrors = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(152, 23);
            this.label1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 25);
            this.label1.TabIndex = 0;
            this.label1.Text = "Name";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(74, 203);
            this.label3.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(148, 25);
            this.label3.TabIndex = 2;
            this.label3.Text = "Bootstrap Code";
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(228, 20);
            this.txtName.Margin = new System.Windows.Forms.Padding(6);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(411, 29);
            this.txtName.TabIndex = 3;
            // 
            // txtBootstrap
            // 
            this.txtBootstrap.Location = new System.Drawing.Point(228, 200);
            this.txtBootstrap.Margin = new System.Windows.Forms.Padding(6);
            this.txtBootstrap.Multiline = true;
            this.txtBootstrap.Name = "txtBootstrap";
            this.txtBootstrap.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtBootstrap.Size = new System.Drawing.Size(619, 197);
            this.txtBootstrap.TabIndex = 5;
            this.txtBootstrap.WordWrap = false;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(539, 450);
            this.btnSave.Margin = new System.Windows.Forms.Padding(6);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(138, 42);
            this.btnSave.TabIndex = 6;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(709, 450);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(6);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(138, 42);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // txtShell
            // 
            this.txtShell.Location = new System.Drawing.Point(228, 63);
            this.txtShell.Name = "txtShell";
            this.txtShell.Size = new System.Drawing.Size(619, 29);
            this.txtShell.TabIndex = 8;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(115, 66);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(101, 25);
            this.label2.TabIndex = 9;
            this.label2.Text = "Shell Path";
            // 
            // txtShellArgs
            // 
            this.txtShellArgs.Location = new System.Drawing.Point(228, 106);
            this.txtShellArgs.Name = "txtShellArgs";
            this.txtShellArgs.Size = new System.Drawing.Size(619, 29);
            this.txtShellArgs.TabIndex = 10;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(66, 109);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(156, 25);
            this.label4.TabIndex = 11;
            this.label4.Text = "Shell Arguments";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(34, 156);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(188, 25);
            this.label5.TabIndex = 12;
            this.label5.Text = "Bootstrap File Name";
            // 
            // txtBootstrapFile
            // 
            this.txtBootstrapFile.Location = new System.Drawing.Point(228, 153);
            this.txtBootstrapFile.Name = "txtBootstrapFile";
            this.txtBootstrapFile.Size = new System.Drawing.Size(286, 29);
            this.txtBootstrapFile.TabIndex = 13;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(520, 156);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(240, 25);
            this.label6.TabIndex = 14;
            this.label6.Text = "Example: run.bat or run.sh";
            // 
            // chkRedirectLocal
            // 
            this.chkRedirectLocal.AutoSize = true;
            this.chkRedirectLocal.Location = new System.Drawing.Point(228, 423);
            this.chkRedirectLocal.Name = "chkRedirectLocal";
            this.chkRedirectLocal.Size = new System.Drawing.Size(187, 29);
            this.chkRedirectLocal.TabIndex = 15;
            this.chkRedirectLocal.Text = "Redirect Local IO";
            this.chkRedirectLocal.UseVisualStyleBackColor = true;
            // 
            // chkSurpressErrors
            // 
            this.chkSurpressErrors.AutoSize = true;
            this.chkSurpressErrors.Location = new System.Drawing.Point(228, 458);
            this.chkSurpressErrors.Name = "chkSurpressErrors";
            this.chkSurpressErrors.Size = new System.Drawing.Size(168, 29);
            this.chkSurpressErrors.TabIndex = 16;
            this.chkSurpressErrors.Text = "Supress Errors";
            this.chkSurpressErrors.UseVisualStyleBackColor = true;
            // 
            // PlatformEditorForm
            // 
            this.AcceptButton = this.btnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(937, 528);
            this.Controls.Add(this.chkSurpressErrors);
            this.Controls.Add(this.chkRedirectLocal);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtBootstrapFile);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtShellArgs);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtShell);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.txtBootstrap);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PlatformEditorForm";
            this.Text = "PlatformEditorForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.TextBox txtBootstrap;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TextBox txtShell;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtShellArgs;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtBootstrapFile;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox chkRedirectLocal;
        private System.Windows.Forms.CheckBox chkSurpressErrors;
    }
}