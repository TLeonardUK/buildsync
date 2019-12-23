namespace BuildSync.Client.Forms
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.peerCountLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel4 = new System.Windows.Forms.ToolStripStatusLabel();
            this.totalDiskDownBandwidthLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.totalDiskUpBandwidthLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.totalDownBandwidthLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.totalUpBandwidthLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addDownloadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.pauseAllDownloadsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resumeAllDownloadsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.preferencesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.quitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.adminToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.publishBuildToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.manageBuildsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.notifyIconContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.pauseAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resumeAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.closeApplicationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.downloadListContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addDownloadToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.updateTimer = new System.Windows.Forms.Timer(this.components);
            this.downloadList1 = new BuildSync.Client.Controls.DownloadList();
            this.statusStrip.SuspendLayout();
            this.menuStrip.SuspendLayout();
            this.notifyIconContextMenu.SuspendLayout();
            this.downloadListContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip
            // 
            this.statusStrip.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.peerCountLabel,
            this.toolStripStatusLabel4,
            this.totalDiskDownBandwidthLabel,
            this.totalDiskUpBandwidthLabel,
            this.totalDownBandwidthLabel,
            this.totalUpBandwidthLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 350);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(1004, 39);
            this.statusStrip.TabIndex = 2;
            this.statusStrip.Text = "statusStrip1";
            // 
            // peerCountLabel
            // 
            this.peerCountLabel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.peerCountLabel.Image = global::BuildSync.Client.Properties.Resources.appbar_people;
            this.peerCountLabel.Name = "peerCountLabel";
            this.peerCountLabel.Size = new System.Drawing.Size(203, 32);
            this.peerCountLabel.Text = "3 peers connected";
            // 
            // toolStripStatusLabel4
            // 
            this.toolStripStatusLabel4.Name = "toolStripStatusLabel4";
            this.toolStripStatusLabel4.Size = new System.Drawing.Size(306, 32);
            this.toolStripStatusLabel4.Spring = true;
            // 
            // totalDiskDownBandwidthLabel
            // 
            this.totalDiskDownBandwidthLabel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.totalDiskDownBandwidthLabel.Image = global::BuildSync.Client.Properties.Resources.appbar_disk_download;
            this.totalDiskDownBandwidthLabel.Name = "totalDiskDownBandwidthLabel";
            this.totalDiskDownBandwidthLabel.Size = new System.Drawing.Size(120, 32);
            this.totalDiskDownBandwidthLabel.Text = "100 kb/s";
            // 
            // totalDiskUpBandwidthLabel
            // 
            this.totalDiskUpBandwidthLabel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.totalDiskUpBandwidthLabel.Image = global::BuildSync.Client.Properties.Resources.appbar_disk_upload;
            this.totalDiskUpBandwidthLabel.Name = "totalDiskUpBandwidthLabel";
            this.totalDiskUpBandwidthLabel.Size = new System.Drawing.Size(120, 32);
            this.totalDiskUpBandwidthLabel.Text = "501 kb/s";
            // 
            // totalDownBandwidthLabel
            // 
            this.totalDownBandwidthLabel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.totalDownBandwidthLabel.Image = global::BuildSync.Client.Properties.Resources.appbar_download;
            this.totalDownBandwidthLabel.Name = "totalDownBandwidthLabel";
            this.totalDownBandwidthLabel.Size = new System.Drawing.Size(120, 32);
            this.totalDownBandwidthLabel.Text = "100 kb/s";
            // 
            // totalUpBandwidthLabel
            // 
            this.totalUpBandwidthLabel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.totalUpBandwidthLabel.Image = global::BuildSync.Client.Properties.Resources.appbar_upload;
            this.totalUpBandwidthLabel.Name = "totalUpBandwidthLabel";
            this.totalUpBandwidthLabel.Size = new System.Drawing.Size(120, 32);
            this.totalUpBandwidthLabel.Text = "501 kb/s";
            // 
            // menuStrip
            // 
            this.menuStrip.GripMargin = new System.Windows.Forms.Padding(2, 2, 0, 2);
            this.menuStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.adminToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(1004, 33);
            this.menuStrip.TabIndex = 5;
            this.menuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addDownloadToolStripMenuItem,
            this.toolStripSeparator4,
            this.pauseAllDownloadsToolStripMenuItem,
            this.resumeAllDownloadsToolStripMenuItem,
            this.toolStripSeparator1,
            this.preferencesToolStripMenuItem,
            this.toolStripSeparator2,
            this.quitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(54, 29);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // addDownloadToolStripMenuItem
            // 
            this.addDownloadToolStripMenuItem.Name = "addDownloadToolStripMenuItem";
            this.addDownloadToolStripMenuItem.Size = new System.Drawing.Size(291, 34);
            this.addDownloadToolStripMenuItem.Text = "Add download ...";
            this.addDownloadToolStripMenuItem.Click += new System.EventHandler(this.AddDownloadClicked);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(288, 6);
            // 
            // pauseAllDownloadsToolStripMenuItem
            // 
            this.pauseAllDownloadsToolStripMenuItem.Name = "pauseAllDownloadsToolStripMenuItem";
            this.pauseAllDownloadsToolStripMenuItem.Size = new System.Drawing.Size(291, 34);
            this.pauseAllDownloadsToolStripMenuItem.Text = "Pause all downloads";
            this.pauseAllDownloadsToolStripMenuItem.Click += new System.EventHandler(this.PauseAllClicked);
            // 
            // resumeAllDownloadsToolStripMenuItem
            // 
            this.resumeAllDownloadsToolStripMenuItem.Name = "resumeAllDownloadsToolStripMenuItem";
            this.resumeAllDownloadsToolStripMenuItem.Size = new System.Drawing.Size(291, 34);
            this.resumeAllDownloadsToolStripMenuItem.Text = "Resume all downloads";
            this.resumeAllDownloadsToolStripMenuItem.Click += new System.EventHandler(this.ResumeAllClicked);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(288, 6);
            // 
            // preferencesToolStripMenuItem
            // 
            this.preferencesToolStripMenuItem.Name = "preferencesToolStripMenuItem";
            this.preferencesToolStripMenuItem.Size = new System.Drawing.Size(291, 34);
            this.preferencesToolStripMenuItem.Text = "Preferences ...";
            this.preferencesToolStripMenuItem.Click += new System.EventHandler(this.PreferencesClicked);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(288, 6);
            // 
            // quitToolStripMenuItem
            // 
            this.quitToolStripMenuItem.Name = "quitToolStripMenuItem";
            this.quitToolStripMenuItem.Size = new System.Drawing.Size(291, 34);
            this.quitToolStripMenuItem.Text = "Quit";
            this.quitToolStripMenuItem.Click += new System.EventHandler(this.QuitClicked);
            // 
            // adminToolStripMenuItem
            // 
            this.adminToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.publishBuildToolStripMenuItem,
            this.manageBuildsToolStripMenuItem});
            this.adminToolStripMenuItem.Name = "adminToolStripMenuItem";
            this.adminToolStripMenuItem.Size = new System.Drawing.Size(81, 29);
            this.adminToolStripMenuItem.Text = "Admin";
            // 
            // publishBuildToolStripMenuItem
            // 
            this.publishBuildToolStripMenuItem.Name = "publishBuildToolStripMenuItem";
            this.publishBuildToolStripMenuItem.Size = new System.Drawing.Size(270, 34);
            this.publishBuildToolStripMenuItem.Text = "Publish build ...";
            this.publishBuildToolStripMenuItem.Click += new System.EventHandler(this.PublishBuildClicked);
            // 
            // manageBuildsToolStripMenuItem
            // 
            this.manageBuildsToolStripMenuItem.Name = "manageBuildsToolStripMenuItem";
            this.manageBuildsToolStripMenuItem.Size = new System.Drawing.Size(270, 34);
            this.manageBuildsToolStripMenuItem.Text = "Manage Builds ...";
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(65, 29);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(181, 34);
            this.aboutToolStripMenuItem.Text = "About ...";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.AboutClicked);
            // 
            // notifyIcon
            // 
            this.notifyIcon.ContextMenuStrip = this.notifyIconContextMenu;
            this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
            this.notifyIcon.Text = "Build Sync";
            this.notifyIcon.Visible = true;
            this.notifyIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(this.NotifyIconClicked);
            // 
            // notifyIconContextMenu
            // 
            this.notifyIconContextMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.notifyIconContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.pauseAllToolStripMenuItem,
            this.resumeAllToolStripMenuItem,
            this.toolStripSeparator3,
            this.closeApplicationToolStripMenuItem});
            this.notifyIconContextMenu.Name = "notifyIconContextMenu";
            this.notifyIconContextMenu.Size = new System.Drawing.Size(262, 106);
            // 
            // pauseAllToolStripMenuItem
            // 
            this.pauseAllToolStripMenuItem.Name = "pauseAllToolStripMenuItem";
            this.pauseAllToolStripMenuItem.Size = new System.Drawing.Size(261, 32);
            this.pauseAllToolStripMenuItem.Text = "Pause all downloads";
            this.pauseAllToolStripMenuItem.Click += new System.EventHandler(this.PauseAllClicked);
            // 
            // resumeAllToolStripMenuItem
            // 
            this.resumeAllToolStripMenuItem.Name = "resumeAllToolStripMenuItem";
            this.resumeAllToolStripMenuItem.Size = new System.Drawing.Size(261, 32);
            this.resumeAllToolStripMenuItem.Text = "Resume all downloads";
            this.resumeAllToolStripMenuItem.Click += new System.EventHandler(this.ResumeAllClicked);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(258, 6);
            // 
            // closeApplicationToolStripMenuItem
            // 
            this.closeApplicationToolStripMenuItem.Name = "closeApplicationToolStripMenuItem";
            this.closeApplicationToolStripMenuItem.Size = new System.Drawing.Size(261, 32);
            this.closeApplicationToolStripMenuItem.Text = "Quit";
            this.closeApplicationToolStripMenuItem.Click += new System.EventHandler(this.QuitClicked);
            // 
            // downloadListContextMenu
            // 
            this.downloadListContextMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.downloadListContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addDownloadToolStripMenuItem1});
            this.downloadListContextMenu.Name = "downloadListContextMenu";
            this.downloadListContextMenu.Size = new System.Drawing.Size(223, 36);
            // 
            // addDownloadToolStripMenuItem1
            // 
            this.addDownloadToolStripMenuItem1.Name = "addDownloadToolStripMenuItem1";
            this.addDownloadToolStripMenuItem1.Size = new System.Drawing.Size(222, 32);
            this.addDownloadToolStripMenuItem1.Text = "Add Download ...";
            this.addDownloadToolStripMenuItem1.Click += new System.EventHandler(this.AddDownloadClicked);
            // 
            // updateTimer
            // 
            this.updateTimer.Enabled = true;
            this.updateTimer.Interval = 1000;
            this.updateTimer.Tick += new System.EventHandler(this.UpdateTimerTick);
            // 
            // downloadList1
            // 
            this.downloadList1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.downloadList1.AutoScroll = true;
            this.downloadList1.BackColor = System.Drawing.SystemColors.ControlLight;
            this.downloadList1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.downloadList1.ContextMenuStrip = this.downloadListContextMenu;
            this.downloadList1.Location = new System.Drawing.Point(-2, 36);
            this.downloadList1.Margin = new System.Windows.Forms.Padding(20);
            this.downloadList1.Name = "downloadList1";
            this.downloadList1.Size = new System.Drawing.Size(1007, 316);
            this.downloadList1.TabIndex = 6;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1004, 389);
            this.Controls.Add(this.downloadList1);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.menuStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip;
            this.MinimumSize = new System.Drawing.Size(1026, 445);
            this.Name = "MainForm";
            this.Text = "Build Sync";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.FormLoaded);
            this.Shown += new System.EventHandler(this.FormShown);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.notifyIconContextMenu.ResumeLayout(false);
            this.downloadListContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel peerCountLabel;
        private System.Windows.Forms.ToolStripStatusLabel totalDownBandwidthLabel;
        private System.Windows.Forms.ToolStripStatusLabel totalUpBandwidthLabel;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel4;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addDownloadToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem preferencesToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem quitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private Controls.DownloadList downloadList1;
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.ContextMenuStrip notifyIconContextMenu;
        private System.Windows.Forms.ToolStripMenuItem pauseAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resumeAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem closeApplicationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pauseAllDownloadsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resumeAllDownloadsToolStripMenuItem;
        private System.Windows.Forms.Timer updateTimer;
        private System.Windows.Forms.ContextMenuStrip downloadListContextMenu;
        private System.Windows.Forms.ToolStripMenuItem addDownloadToolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem adminToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem publishBuildToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem manageBuildsToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel totalDiskDownBandwidthLabel;
        private System.Windows.Forms.ToolStripStatusLabel totalDiskUpBandwidthLabel;
    }
}

