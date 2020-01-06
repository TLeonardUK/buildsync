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
            this.manageBuildsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.manageUsersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.viewPeersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewConsoleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpTopicsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.notifyIconContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.pauseAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resumeAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.closeApplicationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.downloadListContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addDownloadToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.forceRevalidateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.forceRedownloadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.forceReinstallToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.updateTimer = new System.Windows.Forms.Timer(this.components);
            this.mainDownloadList = new BuildSync.Client.Controls.DownloadList();
            this.viewManifestsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip.SuspendLayout();
            this.menuStrip.SuspendLayout();
            this.notifyIconContextMenu.SuspendLayout();
            this.downloadListContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip
            // 
            this.statusStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.peerCountLabel,
            this.toolStripStatusLabel4,
            this.totalDiskDownBandwidthLabel,
            this.totalDiskUpBandwidthLabel,
            this.totalDownBandwidthLabel,
            this.totalUpBandwidthLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 238);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(794, 26);
            this.statusStrip.TabIndex = 2;
            this.statusStrip.Text = "statusStrip1";
            // 
            // peerCountLabel
            // 
            this.peerCountLabel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.peerCountLabel.Image = global::BuildSync.Client.Properties.Resources.appbar_people;
            this.peerCountLabel.Margin = new System.Windows.Forms.Padding(0, -3, 0, -3);
            this.peerCountLabel.Name = "peerCountLabel";
            this.peerCountLabel.Size = new System.Drawing.Size(145, 32);
            this.peerCountLabel.Text = "3 peers connected";
            // 
            // toolStripStatusLabel4
            // 
            this.toolStripStatusLabel4.AutoSize = false;
            this.toolStripStatusLabel4.Margin = new System.Windows.Forms.Padding(0, -3, 0, -3);
            this.toolStripStatusLabel4.Name = "toolStripStatusLabel4";
            this.toolStripStatusLabel4.Size = new System.Drawing.Size(286, 32);
            this.toolStripStatusLabel4.Spring = true;
            // 
            // totalDiskDownBandwidthLabel
            // 
            this.totalDiskDownBandwidthLabel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.totalDiskDownBandwidthLabel.Image = global::BuildSync.Client.Properties.Resources.appbar_disk_download;
            this.totalDiskDownBandwidthLabel.Margin = new System.Windows.Forms.Padding(0, -3, 0, -3);
            this.totalDiskDownBandwidthLabel.Name = "totalDiskDownBandwidthLabel";
            this.totalDiskDownBandwidthLabel.Size = new System.Drawing.Size(87, 32);
            this.totalDiskDownBandwidthLabel.Text = "100 kb/s";
            // 
            // totalDiskUpBandwidthLabel
            // 
            this.totalDiskUpBandwidthLabel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.totalDiskUpBandwidthLabel.Image = global::BuildSync.Client.Properties.Resources.appbar_disk_upload;
            this.totalDiskUpBandwidthLabel.Margin = new System.Windows.Forms.Padding(0, -3, 0, -3);
            this.totalDiskUpBandwidthLabel.Name = "totalDiskUpBandwidthLabel";
            this.totalDiskUpBandwidthLabel.Size = new System.Drawing.Size(87, 32);
            this.totalDiskUpBandwidthLabel.Text = "501 kb/s";
            // 
            // totalDownBandwidthLabel
            // 
            this.totalDownBandwidthLabel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.totalDownBandwidthLabel.Image = global::BuildSync.Client.Properties.Resources.appbar_download;
            this.totalDownBandwidthLabel.Margin = new System.Windows.Forms.Padding(0, -3, 0, -3);
            this.totalDownBandwidthLabel.Name = "totalDownBandwidthLabel";
            this.totalDownBandwidthLabel.Size = new System.Drawing.Size(87, 32);
            this.totalDownBandwidthLabel.Text = "100 kb/s";
            // 
            // totalUpBandwidthLabel
            // 
            this.totalUpBandwidthLabel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.totalUpBandwidthLabel.Image = global::BuildSync.Client.Properties.Resources.appbar_upload;
            this.totalUpBandwidthLabel.Margin = new System.Windows.Forms.Padding(0, -3, 0, -3);
            this.totalUpBandwidthLabel.Name = "totalUpBandwidthLabel";
            this.totalUpBandwidthLabel.Size = new System.Drawing.Size(87, 32);
            this.totalUpBandwidthLabel.Text = "501 kb/s";
            // 
            // menuStrip
            // 
            this.menuStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.adminToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(794, 24);
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
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // addDownloadToolStripMenuItem
            // 
            this.addDownloadToolStripMenuItem.Name = "addDownloadToolStripMenuItem";
            this.addDownloadToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.addDownloadToolStripMenuItem.Text = "Add download ...";
            this.addDownloadToolStripMenuItem.Click += new System.EventHandler(this.AddDownloadClicked);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(189, 6);
            // 
            // pauseAllDownloadsToolStripMenuItem
            // 
            this.pauseAllDownloadsToolStripMenuItem.Name = "pauseAllDownloadsToolStripMenuItem";
            this.pauseAllDownloadsToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.pauseAllDownloadsToolStripMenuItem.Text = "Pause all downloads";
            this.pauseAllDownloadsToolStripMenuItem.Click += new System.EventHandler(this.PauseAllClicked);
            // 
            // resumeAllDownloadsToolStripMenuItem
            // 
            this.resumeAllDownloadsToolStripMenuItem.Name = "resumeAllDownloadsToolStripMenuItem";
            this.resumeAllDownloadsToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.resumeAllDownloadsToolStripMenuItem.Text = "Resume all downloads";
            this.resumeAllDownloadsToolStripMenuItem.Click += new System.EventHandler(this.ResumeAllClicked);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(189, 6);
            // 
            // preferencesToolStripMenuItem
            // 
            this.preferencesToolStripMenuItem.Name = "preferencesToolStripMenuItem";
            this.preferencesToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.preferencesToolStripMenuItem.Text = "Preferences ...";
            this.preferencesToolStripMenuItem.Click += new System.EventHandler(this.PreferencesClicked);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(189, 6);
            // 
            // quitToolStripMenuItem
            // 
            this.quitToolStripMenuItem.Name = "quitToolStripMenuItem";
            this.quitToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.quitToolStripMenuItem.Text = "Quit";
            this.quitToolStripMenuItem.Click += new System.EventHandler(this.QuitClicked);
            // 
            // adminToolStripMenuItem
            // 
            this.adminToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.manageBuildsToolStripMenuItem,
            this.manageUsersToolStripMenuItem,
            this.toolStripSeparator5,
            this.viewPeersToolStripMenuItem,
            this.viewConsoleToolStripMenuItem,
            this.viewManifestsToolStripMenuItem});
            this.adminToolStripMenuItem.Name = "adminToolStripMenuItem";
            this.adminToolStripMenuItem.Size = new System.Drawing.Size(72, 20);
            this.adminToolStripMenuItem.Text = "Advanced";
            // 
            // manageBuildsToolStripMenuItem
            // 
            this.manageBuildsToolStripMenuItem.Name = "manageBuildsToolStripMenuItem";
            this.manageBuildsToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.manageBuildsToolStripMenuItem.Text = "Manage builds ...";
            this.manageBuildsToolStripMenuItem.Click += new System.EventHandler(this.ManageBuildsClicked);
            // 
            // manageUsersToolStripMenuItem
            // 
            this.manageUsersToolStripMenuItem.Name = "manageUsersToolStripMenuItem";
            this.manageUsersToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.manageUsersToolStripMenuItem.Text = "Manage users ...";
            this.manageUsersToolStripMenuItem.Click += new System.EventHandler(this.ManageUsersClicked);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(177, 6);
            // 
            // viewPeersToolStripMenuItem
            // 
            this.viewPeersToolStripMenuItem.Name = "viewPeersToolStripMenuItem";
            this.viewPeersToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.viewPeersToolStripMenuItem.Text = "View peers ...";
            this.viewPeersToolStripMenuItem.Click += new System.EventHandler(this.ViewPeersClicked);
            // 
            // viewConsoleToolStripMenuItem
            // 
            this.viewConsoleToolStripMenuItem.Name = "viewConsoleToolStripMenuItem";
            this.viewConsoleToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.viewConsoleToolStripMenuItem.Text = "View console ...";
            this.viewConsoleToolStripMenuItem.Click += new System.EventHandler(this.ViewConsoleClicked);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.helpTopicsToolStripMenuItem,
            this.toolStripSeparator7,
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // helpTopicsToolStripMenuItem
            // 
            this.helpTopicsToolStripMenuItem.Name = "helpTopicsToolStripMenuItem";
            this.helpTopicsToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            this.helpTopicsToolStripMenuItem.Text = "View help ...";
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(134, 6);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
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
            this.notifyIconContextMenu.Size = new System.Drawing.Size(193, 76);
            // 
            // pauseAllToolStripMenuItem
            // 
            this.pauseAllToolStripMenuItem.Name = "pauseAllToolStripMenuItem";
            this.pauseAllToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.pauseAllToolStripMenuItem.Text = "Pause all downloads";
            this.pauseAllToolStripMenuItem.Click += new System.EventHandler(this.PauseAllClicked);
            // 
            // resumeAllToolStripMenuItem
            // 
            this.resumeAllToolStripMenuItem.Name = "resumeAllToolStripMenuItem";
            this.resumeAllToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.resumeAllToolStripMenuItem.Text = "Resume all downloads";
            this.resumeAllToolStripMenuItem.Click += new System.EventHandler(this.ResumeAllClicked);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(189, 6);
            // 
            // closeApplicationToolStripMenuItem
            // 
            this.closeApplicationToolStripMenuItem.Name = "closeApplicationToolStripMenuItem";
            this.closeApplicationToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.closeApplicationToolStripMenuItem.Text = "Quit";
            this.closeApplicationToolStripMenuItem.Click += new System.EventHandler(this.QuitClicked);
            // 
            // downloadListContextMenu
            // 
            this.downloadListContextMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.downloadListContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addDownloadToolStripMenuItem1,
            this.toolStripSeparator6,
            this.forceRevalidateToolStripMenuItem,
            this.forceRedownloadToolStripMenuItem,
            this.forceReinstallToolStripMenuItem});
            this.downloadListContextMenu.Name = "downloadListContextMenu";
            this.downloadListContextMenu.Size = new System.Drawing.Size(185, 98);
            this.downloadListContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.DownloadListContextMenuShowing);
            // 
            // addDownloadToolStripMenuItem1
            // 
            this.addDownloadToolStripMenuItem1.Name = "addDownloadToolStripMenuItem1";
            this.addDownloadToolStripMenuItem1.Size = new System.Drawing.Size(184, 22);
            this.addDownloadToolStripMenuItem1.Text = "Add Download ...";
            this.addDownloadToolStripMenuItem1.Click += new System.EventHandler(this.AddDownloadClicked);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(181, 6);
            // 
            // forceRevalidateToolStripMenuItem
            // 
            this.forceRevalidateToolStripMenuItem.Name = "forceRevalidateToolStripMenuItem";
            this.forceRevalidateToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.forceRevalidateToolStripMenuItem.Text = "Force Revalidate ...";
            this.forceRevalidateToolStripMenuItem.Click += new System.EventHandler(this.ForceRevalidateClicked);
            // 
            // forceRedownloadToolStripMenuItem
            // 
            this.forceRedownloadToolStripMenuItem.Name = "forceRedownloadToolStripMenuItem";
            this.forceRedownloadToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.forceRedownloadToolStripMenuItem.Text = "Force Redownload ...";
            this.forceRedownloadToolStripMenuItem.Click += new System.EventHandler(this.ForceRedownloadClicked);
            // 
            // forceReinstallToolStripMenuItem
            // 
            this.forceReinstallToolStripMenuItem.Name = "forceReinstallToolStripMenuItem";
            this.forceReinstallToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.forceReinstallToolStripMenuItem.Text = "Force Reinstall ...";
            this.forceReinstallToolStripMenuItem.Click += new System.EventHandler(this.ForceReinstallClicked);
            // 
            // updateTimer
            // 
            this.updateTimer.Enabled = true;
            this.updateTimer.Interval = 1000;
            this.updateTimer.Tick += new System.EventHandler(this.UpdateTimerTick);
            // 
            // mainDownloadList
            // 
            this.mainDownloadList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mainDownloadList.AutoScroll = true;
            this.mainDownloadList.BackColor = System.Drawing.SystemColors.ControlLight;
            this.mainDownloadList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mainDownloadList.ContextMenuStrip = this.downloadListContextMenu;
            this.mainDownloadList.Location = new System.Drawing.Point(-1, 23);
            this.mainDownloadList.Margin = new System.Windows.Forms.Padding(20);
            this.mainDownloadList.Name = "mainDownloadList";
            this.mainDownloadList.Size = new System.Drawing.Size(796, 216);
            this.mainDownloadList.TabIndex = 6;
            // 
            // viewManifestsToolStripMenuItem
            // 
            this.viewManifestsToolStripMenuItem.Name = "viewManifestsToolStripMenuItem";
            this.viewManifestsToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.viewManifestsToolStripMenuItem.Text = "View manifests ...";
            this.viewManifestsToolStripMenuItem.Click += new System.EventHandler(this.ViewManifestsClicked);
            // 
            // MainForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(794, 264);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.mainDownloadList);
            this.Controls.Add(this.menuStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip;
            this.MinimumSize = new System.Drawing.Size(810, 303);
            this.Name = "MainForm";
            this.Text = "Build Sync - Client";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.FormLoaded);
            this.Shown += new System.EventHandler(this.FormShown);
            this.ClientSizeChanged += new System.EventHandler(this.ClientSizeHasChanged);
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
        private Controls.DownloadList mainDownloadList;
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
        private System.Windows.Forms.ToolStripMenuItem manageBuildsToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel totalDiskDownBandwidthLabel;
        private System.Windows.Forms.ToolStripStatusLabel totalDiskUpBandwidthLabel;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem viewConsoleToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripMenuItem forceRevalidateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem forceRedownloadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewPeersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem manageUsersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem forceReinstallToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpTopicsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripMenuItem viewManifestsToolStripMenuItem;
    }
}

