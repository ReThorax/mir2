using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Windows.Forms;
using System.Drawing;

namespace Launcher
{
    partial class AMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AMain));
            this.ProgressCurrent_pb = new System.Windows.Forms.PictureBox();
            this.SpeedLabel = new System.Windows.Forms.Label();
            this.InterfaceTimer = new System.Windows.Forms.Timer(this.components);
            this.Version_label = new System.Windows.Forms.Label();
            this.Main_browser = new System.Windows.Forms.WebBrowser();
            this.CurrentFile_label = new System.Windows.Forms.Label();
            this.Credit_label = new System.Windows.Forms.Label();
            this.TotalProg_pb = new System.Windows.Forms.PictureBox();
            this.Launch_pb = new System.Windows.Forms.PictureBox();
            this.Close_pb = new System.Windows.Forms.PictureBox();
            this.Config_pb = new System.Windows.Forms.PictureBox();
            this.Movement_panel = new System.Windows.Forms.Panel();
            this.ActionLabel = new System.Windows.Forms.Label();
            this.IncorrectWebsite_Image = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.ProgressCurrent_pb)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TotalProg_pb)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Launch_pb)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Close_pb)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Config_pb)).BeginInit();
            this.Movement_panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.IncorrectWebsite_Image)).BeginInit();
            this.SuspendLayout();
            // 
            // ProgressCurrent_pb
            // 
            this.ProgressCurrent_pb.BackColor = System.Drawing.Color.Transparent;
            this.ProgressCurrent_pb.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.ProgressCurrent_pb.Image = global::Client.Properties.Resources.Current;
            this.ProgressCurrent_pb.Location = new System.Drawing.Point(99, 414);
            this.ProgressCurrent_pb.Name = "ProgressCurrent_pb";
            this.ProgressCurrent_pb.Size = new System.Drawing.Size(252, 11);
            this.ProgressCurrent_pb.TabIndex = 23;
            this.ProgressCurrent_pb.TabStop = false;
            // 
            // SpeedLabel
            // 
            this.SpeedLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.SpeedLabel.BackColor = System.Drawing.Color.Transparent;
            this.SpeedLabel.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SpeedLabel.ForeColor = System.Drawing.Color.Gray;
            this.SpeedLabel.Location = new System.Drawing.Point(420, 394);
            this.SpeedLabel.Name = "SpeedLabel";
            this.SpeedLabel.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.SpeedLabel.Size = new System.Drawing.Size(139, 16);
            this.SpeedLabel.TabIndex = 13;
            this.SpeedLabel.Text = "Speed";
            this.SpeedLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.SpeedLabel.Visible = false;
            // 
            // InterfaceTimer
            // 
            this.InterfaceTimer.Enabled = true;
            this.InterfaceTimer.Interval = 50;
            this.InterfaceTimer.Tick += new System.EventHandler(this.InterfaceTimer_Tick);
            // 
            // Version_label
            // 
            this.Version_label.BackColor = System.Drawing.Color.Transparent;
            this.Version_label.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Version_label.ForeColor = System.Drawing.Color.Gray;
            this.Version_label.Location = new System.Drawing.Point(556, 16);
            this.Version_label.Name = "Version_label";
            this.Version_label.Size = new System.Drawing.Size(125, 13);
            this.Version_label.TabIndex = 31;
            this.Version_label.Text = "Version 0000.00.00 0000";
            this.Version_label.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.Version_label.Click += new System.EventHandler(this.Version_label_Click);
            // 
            // Main_browser
            // 
            this.Main_browser.AllowWebBrowserDrop = false;
            this.Main_browser.IsWebBrowserContextMenuEnabled = false;
            this.Main_browser.Location = new System.Drawing.Point(170, 94);
            this.Main_browser.MinimumSize = new System.Drawing.Size(20, 20);
            this.Main_browser.Name = "Main_browser";
            this.Main_browser.ScriptErrorsSuppressed = true;
            this.Main_browser.ScrollBarsEnabled = false;
            this.Main_browser.Size = new System.Drawing.Size(588, 293);
            this.Main_browser.TabIndex = 24;
            this.Main_browser.Url = new System.Uri("", System.UriKind.Relative);
            this.Main_browser.Visible = false;
            this.Main_browser.WebBrowserShortcutsEnabled = false;
            this.Main_browser.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.Main_browser_DocumentCompleted);
            // 
            // CurrentFile_label
            // 
            this.CurrentFile_label.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.CurrentFile_label.BackColor = System.Drawing.Color.Transparent;
            this.CurrentFile_label.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CurrentFile_label.ForeColor = System.Drawing.Color.Gray;
            this.CurrentFile_label.Location = new System.Drawing.Point(94, 392);
            this.CurrentFile_label.Name = "CurrentFile_label";
            this.CurrentFile_label.Size = new System.Drawing.Size(258, 17);
            this.CurrentFile_label.TabIndex = 27;
            this.CurrentFile_label.Text = "Up to date.";
            this.CurrentFile_label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.CurrentFile_label.Visible = false;
            // 
            // Credit_label
            // 
            this.Credit_label.AutoSize = true;
            this.Credit_label.BackColor = System.Drawing.Color.Transparent;
            this.Credit_label.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Credit_label.ForeColor = System.Drawing.Color.Gray;
            this.Credit_label.Location = new System.Drawing.Point(9, 16);
            this.Credit_label.Name = "Credit_label";
            this.Credit_label.Size = new System.Drawing.Size(168, 13);
            this.Credit_label.TabIndex = 30;
            this.Credit_label.Text = "Nexus Mir - Powered by Crystal M2";
            this.Credit_label.Click += new System.EventHandler(this.Credit_label_Click);
            // 
            // TotalProg_pb
            // 
            this.TotalProg_pb.BackColor = System.Drawing.Color.Transparent;
            this.TotalProg_pb.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.TotalProg_pb.Image = global::Client.Properties.Resources.Total;
            this.TotalProg_pb.Location = new System.Drawing.Point(420, 414);
            this.TotalProg_pb.Name = "TotalProg_pb";
            this.TotalProg_pb.Size = new System.Drawing.Size(252, 11);
            this.TotalProg_pb.TabIndex = 22;
            this.TotalProg_pb.TabStop = false;
            // 
            // Launch_pb
            // 
            this.Launch_pb.BackColor = System.Drawing.Color.Transparent;
            this.Launch_pb.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.Launch_pb.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Launch_pb.Image = global::Client.Properties.Resources.Start_Normal;
            this.Launch_pb.Location = new System.Drawing.Point(683, 397);
            this.Launch_pb.Name = "Launch_pb";
            this.Launch_pb.Size = new System.Drawing.Size(75, 24);
            this.Launch_pb.TabIndex = 19;
            this.Launch_pb.TabStop = false;
            this.Launch_pb.Click += new System.EventHandler(this.Launch_pb_Click);
            this.Launch_pb.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Launch_pb_MouseDown);
            this.Launch_pb.MouseEnter += new System.EventHandler(this.Launch_pb_MouseEnter);
            this.Launch_pb.MouseLeave += new System.EventHandler(this.Launch_pb_MouseLeave);
            this.Launch_pb.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Launch_pb_MouseUp);
            // 
            // Close_pb
            // 
            this.Close_pb.BackColor = System.Drawing.Color.Transparent;
            this.Close_pb.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.Close_pb.Image = global::Client.Properties.Resources.Close_Normal;
            this.Close_pb.Location = new System.Drawing.Point(717, 12);
            this.Close_pb.Name = "Close_pb";
            this.Close_pb.Size = new System.Drawing.Size(27, 22);
            this.Close_pb.TabIndex = 20;
            this.Close_pb.TabStop = false;
            this.Close_pb.Click += new System.EventHandler(this.Close_pb_Click);
            this.Close_pb.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Close_pb_MouseDown);
            this.Close_pb.MouseEnter += new System.EventHandler(this.Close_pb_MouseEnter);
            this.Close_pb.MouseLeave += new System.EventHandler(this.Close_pb_MouseLeave);
            this.Close_pb.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Close_pb_MouseUp);
            // 
            // Config_pb
            // 
            this.Config_pb.BackColor = System.Drawing.Color.Transparent;
            this.Config_pb.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.Config_pb.Image = global::Client.Properties.Resources.Config_Normal;
            this.Config_pb.Location = new System.Drawing.Point(688, 12);
            this.Config_pb.Name = "Config_pb";
            this.Config_pb.Size = new System.Drawing.Size(27, 22);
            this.Config_pb.TabIndex = 32;
            this.Config_pb.TabStop = false;
            this.Config_pb.Click += new System.EventHandler(this.Config_pb_Click);
            this.Config_pb.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Config_pb_MouseDown);
            this.Config_pb.MouseEnter += new System.EventHandler(this.Config_pb_MouseEnter);
            this.Config_pb.MouseLeave += new System.EventHandler(this.Config_pb_MouseLeave);
            this.Config_pb.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Config_pb_MouseUp);
            // 
            // Movement_panel
            // 
            this.Movement_panel.BackColor = System.Drawing.Color.Transparent;
            this.Movement_panel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.Movement_panel.Controls.Add(this.Version_label);
            this.Movement_panel.Controls.Add(this.Credit_label);
            this.Movement_panel.Controls.Add(this.Config_pb);
            this.Movement_panel.Controls.Add(this.Close_pb);
            this.Movement_panel.Location = new System.Drawing.Point(23, 46);
            this.Movement_panel.Name = "Movement_panel";
            this.Movement_panel.Size = new System.Drawing.Size(749, 46);
            this.Movement_panel.TabIndex = 21;
            this.Movement_panel.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Movement_panel_MouseClick);
            this.Movement_panel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Movement_panel_MouseClick);
            this.Movement_panel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Movement_panel_MouseMove);
            this.Movement_panel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Movement_panel_MouseUp);
            // 
            // ActionLabel
            // 
            this.ActionLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.ActionLabel.BackColor = System.Drawing.Color.Transparent;
            this.ActionLabel.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ActionLabel.ForeColor = System.Drawing.Color.Gray;
            this.ActionLabel.Location = new System.Drawing.Point(176, 391);
            this.ActionLabel.Name = "ActionLabel";
            this.ActionLabel.Size = new System.Drawing.Size(173, 18);
            this.ActionLabel.TabIndex = 4;
            this.ActionLabel.Text = "1423MB/2000MB";
            this.ActionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.ActionLabel.Visible = false;
            this.ActionLabel.Click += new System.EventHandler(this.ActionLabel_Click);
            // 
            // IncorrectWebsite_Image
            // 
            this.IncorrectWebsite_Image.Image = global::Client.Properties.Resources.Incorrect_website;
            this.IncorrectWebsite_Image.Location = new System.Drawing.Point(170, 94);
            this.IncorrectWebsite_Image.Name = "IncorrectWebsite_Image";
            this.IncorrectWebsite_Image.Size = new System.Drawing.Size(588, 293);
            this.IncorrectWebsite_Image.TabIndex = 28;
            this.IncorrectWebsite_Image.TabStop = false;
            // 
            // AMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.BackgroundImage = global::Client.Properties.Resources.Background;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.ClientSize = new System.Drawing.Size(800, 482);
            this.Controls.Add(this.Main_browser);
            this.Controls.Add(this.IncorrectWebsite_Image);
            this.Controls.Add(this.ActionLabel);
            this.Controls.Add(this.CurrentFile_label);
            this.Controls.Add(this.ProgressCurrent_pb);
            this.Controls.Add(this.TotalProg_pb);
            this.Controls.Add(this.Launch_pb);
            this.Controls.Add(this.SpeedLabel);
            this.Controls.Add(this.Movement_panel);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimizeBox = false;
            this.Name = "AMain";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Nexus Mir | Launcher";
            this.TransparencyKey = System.Drawing.Color.Black;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.AMain_FormClosed);
            this.Load += new System.EventHandler(this.AMain_Load);
            this.Click += new System.EventHandler(this.AMain_Click);
            ((System.ComponentModel.ISupportInitialize)(this.ProgressCurrent_pb)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TotalProg_pb)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Launch_pb)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Close_pb)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Config_pb)).EndInit();
            this.Movement_panel.ResumeLayout(false);
            this.Movement_panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.IncorrectWebsite_Image)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label SpeedLabel;
        public System.Windows.Forms.Timer InterfaceTimer;
        public System.Windows.Forms.PictureBox Launch_pb;
        private System.Windows.Forms.PictureBox TotalProg_pb;
        private System.Windows.Forms.WebBrowser Main_browser;
        private System.Windows.Forms.Label CurrentFile_label;
        private System.Windows.Forms.Label Credit_label;
        private System.Windows.Forms.Label Version_label;
        private PictureBox Close_pb;
        private PictureBox Config_pb;
        private Panel Movement_panel;
        private PictureBox ProgressCurrent_pb;
        private Label ActionLabel;
        private PictureBox IncorrectWebsite_Image;
    }
}

