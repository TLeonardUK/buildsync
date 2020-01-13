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
            this.toolStripStatusLabel4 = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.adminToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.notifyIconContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.pauseAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resumeAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.closeApplicationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.downloadListContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.forceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.updateTimer = new System.Windows.Forms.Timer(this.components);
            this.mainDownloadList = new BuildSync.Client.Controls.DownloadList();
            this.aToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.peerCountLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.totalDiskDownBandwidthLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.totalDiskUpBandwidthLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.totalDownBandwidthLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.totalUpBandwidthLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.addDownloadToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.pauseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.forceRedownloadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.forceReinstallToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.forceRevalidateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addDownloadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pauseAllDownloadsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resumeAllDownloadsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.preferencesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.quitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.manageUsersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewConsoleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewPeersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewManifestsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpTopicsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewLicenseMenuToolstrip = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.manageBuildsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            // toolStripStatusLabel4
            // 
            this.toolStripStatusLabel4.AutoSize = false;
            this.toolStripStatusLabel4.Margin = new System.Windows.Forms.Padding(0, -3, 0, -3);
            this.toolStripStatusLabel4.Name = "toolStripStatusLabel4";
            this.toolStripStatusLabel4.Size = new System.Drawing.Size(286, 32);
            this.toolStripStatusLabel4.Spring = true;
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
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(200, 6);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(200, 6);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(200, 6);
            // 
            // adminToolStripMenuItem
            // 
            this.adminToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.manageBuildsToolStripMenuItem,
            this.manageUsersToolStripMenuItem,
            this.toolStripSeparator5,
            this.viewConsoleToolStripMenuItem,
            this.viewPeersToolStripMenuItem,
            this.viewManifestsToolStripMenuItem});
            this.adminToolStripMenuItem.Name = "adminToolStripMenuItem";
            this.adminToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.adminToolStripMenuItem.Text = "View";
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(185, 6);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.helpTopicsToolStripMenuItem,
            this.toolStripSeparator7,
            this.viewLicenseMenuToolstrip,
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(185, 6);
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
            this.pauseToolStripMenuItem,
            this.deleteToolStripMenuItem,
            this.toolStripSeparator9,
            this.forceToolStripMenuItem,
            this.settingsToolStripMenuItem});
            this.downloadListContextMenu.Name = "downloadListContextMenu";
            this.downloadListContextMenu.Size = new System.Drawing.Size(174, 166);
            this.downloadListContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.DownloadListContextMenuShowing);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(170, 6);
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            this.toolStripSeparator9.Size = new System.Drawing.Size(170, 6);
            // 
            // forceToolStripMenuItem
            // 
            this.forceToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.forceRedownloadToolStripMenuItem,
            this.forceReinstallToolStripMenuItem,
            this.forceRevalidateToolStripMenuItem});
            this.forceToolStripMenuItem.Name = "forceToolStripMenuItem";
            this.forceToolStripMenuItem.Size = new System.Drawing.Size(173, 30);
            this.forceToolStripMenuItem.Text = "Force ...";
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
            // aToolStripMenuItem
            // 
            this.aToolStripMenuItem.Name = "aToolStripMenuItem";
            this.aToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.aToolStripMenuItem.Text = "A";
            // 
            // peerCountLabel
            // 
            this.peerCountLabel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.peerCountLabel.Image = global::BuildSync.Client.Properties.Resources.appbar_people;
            this.peerCountLabel.Margin = new System.Windows.Forms.Padding(0, -3, 0, -3);
            this.peerCountLabel.Name = "peerCountLabel";
            this.peerCountLabel.Size = new System.Drawing.Size(145, 32);
            this.peerCountLabel.Text = "3 peers connected";
            this.peerCountLabel.ToolTipText = "The number of peers currently being downloaded from";
            // 
            // totalDiskDownBandwidthLabel
            // 
            this.totalDiskDownBandwidthLabel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.totalDiskDownBandwidthLabel.Image = global::BuildSync.Client.Properties.Resources.appbar_disk_download;
            this.totalDiskDownBandwidthLabel.Margin = new System.Windows.Forms.Padding(0, -3, 0, -3);
            this.totalDiskDownBandwidthLabel.Name = "totalDiskDownBandwidthLabel";
            this.totalDiskDownBandwidthLabel.Size = new System.Drawing.Size(87, 32);
            this.totalDiskDownBandwidthLabel.Text = "100 kb/s";
            this.totalDiskDownBandwidthLabel.ToolTipText = "Rate at which data is being written to disk";
            // 
            // totalDiskUpBandwidthLabel
            // 
            this.totalDiskUpBandwidthLabel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.totalDiskUpBandwidthLabel.Image = global::BuildSync.Client.Properties.Resources.appbar_disk_upload;
            this.totalDiskUpBandwidthLabel.Margin = new System.Windows.Forms.Padding(0, -3, 0, -3);
            this.totalDiskUpBandwidthLabel.Name = "totalDiskUpBandwidthLabel";
            this.totalDiskUpBandwidthLabel.Size = new System.Drawing.Size(87, 32);
            this.totalDiskUpBandwidthLabel.Text = "501 kb/s";
            this.totalDiskUpBandwidthLabel.ToolTipText = "Rate at which data is being read from disk";
            // 
            // totalDownBandwidthLabel
            // 
            this.totalDownBandwidthLabel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.totalDownBandwidthLabel.Image = global::BuildSync.Client.Properties.Resources.appbar_download;
            this.totalDownBandwidthLabel.Margin = new System.Windows.Forms.Padding(0, -3, 0, -3);
            this.totalDownBandwidthLabel.Name = "totalDownBandwidthLabel";
            this.totalDownBandwidthLabel.Size = new System.Drawing.Size(87, 32);
            this.totalDownBandwidthLabel.Text = "100 kb/s";
            this.totalDownBandwidthLabel.ToolTipText = "Rate at which data is being downloaded from peers";
            // 
            // totalUpBandwidthLabel
            // 
            this.totalUpBandwidthLabel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.totalUpBandwidthLabel.Image = global::BuildSync.Client.Properties.Resources.appbar_upload;
            this.totalUpBandwidthLabel.Margin = new System.Windows.Forms.Padding(0, -3, 0, -3);
            this.totalUpBandwidthLabel.Name = "totalUpBandwidthLabel";
            this.totalUpBandwidthLabel.Size = new System.Drawing.Size(87, 32);
            this.totalUpBandwidthLabel.Text = "501 kb/s";
            this.totalUpBandwidthLabel.ToolTipText = "Rate at which data is being uploaded from peers";
            // 
            // addDownloadToolStripMenuItem1
            // 
            this.addDownloadToolStripMenuItem1.Image = global::BuildSync.Client.Properties.Resources.appbar_add;
            this.addDownloadToolStripMenuItem1.Name = "addDownloadToolStripMenuItem1";
            this.addDownloadToolStripMenuItem1.Size = new System.Drawing.Size(173, 30);
            this.addDownloadToolStripMenuItem1.Text = "Add Download ...";
            this.addDownloadToolStripMenuItem1.Click += new System.EventHandler(this.AddDownloadClicked);
            // 
            // pauseToolStripMenuItem
            // 
            this.pauseToolStripMenuItem.Image = global::BuildSync.Client.Properties.Resources.appbar_control_pause;
            this.pauseToolStripMenuItem.Name = "pauseToolStripMenuItem";
            this.pauseToolStripMenuItem.Size = new System.Drawing.Size(173, 30);
            this.pauseToolStripMenuItem.Text = "Pause";
            this.pauseToolStripMenuItem.Click += new System.EventHandler(this.PauseClicked);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Image = global::BuildSync.Client.Properties.Resources.appbar_delete;
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(173, 30);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.DeleteClicked);
            // 
            // forceRedownloadToolStripMenuItem
            // 
            this.forceRedownloadToolStripMenuItem.Image = global::BuildSync.Client.Properties.Resources.appbar_disk_download;
            this.forceRedownloadToolStripMenuItem.Name = "forceRedownloadToolStripMenuItem";
            this.forceRedownloadToolStripMenuItem.Size = new System.Drawing.Size(188, 30);
            this.forceRedownloadToolStripMenuItem.Text = "Redownload";
            this.forceRedownloadToolStripMenuItem.Click += new System.EventHandler(this.ForceRedownloadClicked);
            // 
            // forceReinstallToolStripMenuItem
            // 
            this.forceReinstallToolStripMenuItem.Image = global::BuildSync.Client.Properties.Resources.appbar_refresh;
            this.forceReinstallToolStripMenuItem.Name = "forceReinstallToolStripMenuItem";
            this.forceReinstallToolStripMenuItem.Size = new System.Drawing.Size(188, 30);
            this.forceReinstallToolStripMenuItem.Text = "Reinstall";
            this.forceReinstallToolStripMenuItem.Click += new System.EventHandler(this.ForceReinstallClicked);
            // 
            // forceRevalidateToolStripMenuItem
            // 
            this.forceRevalidateToolStripMenuItem.Image = global::BuildSync.Client.Properties.Resources.appbar_link;
            this.forceRevalidateToolStripMenuItem.Name = "forceRevalidateToolStripMenuItem";
            this.forceRevalidateToolStripMenuItem.Size = new System.Drawing.Size(188, 30);
            this.forceRevalidateToolStripMenuItem.Text = "Revalidate";
            this.forceRevalidateToolStripMenuItem.Click += new System.EventHandler(this.ForceRevalidateClicked);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Image = global::BuildSync.Client.Properties.Resources.appbar_settings;
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(173, 30);
            this.settingsToolStripMenuItem.Text = "Settings ...";
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.SettingsClicked);
            // 
            // addDownloadToolStripMenuItem
            // 
            this.addDownloadToolStripMenuItem.Image = global::BuildSync.Client.Properties.Resources.appbar_add;
            this.addDownloadToolStripMenuItem.Name = "addDownloadToolStripMenuItem";
            this.addDownloadToolStripMenuItem.Size = new System.Drawing.Size(203, 30);
            this.addDownloadToolStripMenuItem.Text = "Add Download ...";
            this.addDownloadToolStripMenuItem.Click += new System.EventHandler(this.AddDownloadClicked);
            // 
            // pauseAllDownloadsToolStripMenuItem
            // 
            this.pauseAllDownloadsToolStripMenuItem.Image = global::BuildSync.Client.Properties.Resources.appbar_control_pause;
            this.pauseAllDownloadsToolStripMenuItem.Name = "pauseAllDownloadsToolStripMenuItem";
            this.pauseAllDownloadsToolStripMenuItem.Size = new System.Drawing.Size(203, 30);
            this.pauseAllDownloadsToolStripMenuItem.Text = "Pause All Downloads";
            this.pauseAllDownloadsToolStripMenuItem.Click += new System.EventHandler(this.PauseAllClicked);
            // 
            // resumeAllDownloadsToolStripMenuItem
            // 
            this.resumeAllDownloadsToolStripMenuItem.Image = global::BuildSync.Client.Properties.Resources.appbar_control_play;
            this.resumeAllDownloadsToolStripMenuItem.Name = "resumeAllDownloadsToolStripMenuItem";
            this.resumeAllDownloadsToolStripMenuItem.Size = new System.Drawing.Size(203, 30);
            this.resumeAllDownloadsToolStripMenuItem.Text = "Resume All Downloads";
            this.resumeAllDownloadsToolStripMenuItem.Click += new System.EventHandler(this.ResumeAllClicked);
            // 
            // preferencesToolStripMenuItem
            // 
            this.preferencesToolStripMenuItem.Image = global::BuildSync.Client.Properties.Resources.appbar_settings;
            this.preferencesToolStripMenuItem.Name = "preferencesToolStripMenuItem";
            this.preferencesToolStripMenuItem.Size = new System.Drawing.Size(203, 30);
            this.preferencesToolStripMenuItem.Text = "Preferences ...";
            this.preferencesToolStripMenuItem.Click += new System.EventHandler(this.PreferencesClicked);
            // 
            // quitToolStripMenuItem
            // 
            this.quitToolStripMenuItem.Image = global::BuildSync.Client.Properties.Resources.appbar_close;
            this.quitToolStripMenuItem.Name = "quitToolStripMenuItem";
            this.quitToolStripMenuItem.Size = new System.Drawing.Size(203, 30);
            this.quitToolStripMenuItem.Text = "Quit";
            this.quitToolStripMenuItem.Click += new System.EventHandler(this.QuitClicked);
            // 
            // manageUsersToolStripMenuItem
            // 
            this.manageUsersToolStripMenuItem.Image = global::BuildSync.Client.Properties.Resources.appbar_people;
            this.manageUsersToolStripMenuItem.Name = "manageUsersToolStripMenuItem";
            this.manageUsersToolStripMenuItem.Size = new System.Drawing.Size(188, 30);
            this.manageUsersToolStripMenuItem.Text = "User Manager";
            this.manageUsersToolStripMenuItem.Click += new System.EventHandler(this.ManageUsersClicked);
            // 
            // viewConsoleToolStripMenuItem
            // 
            this.viewConsoleToolStripMenuItem.Image = global::BuildSync.Client.Properties.Resources.appbar_console;
            this.viewConsoleToolStripMenuItem.Name = "viewConsoleToolStripMenuItem";
            this.viewConsoleToolStripMenuItem.Size = new System.Drawing.Size(188, 30);
            this.viewConsoleToolStripMenuItem.Text = "Console Window";
            this.viewConsoleToolStripMenuItem.Click += new System.EventHandler(this.ViewConsoleClicked);
            // 
            // viewPeersToolStripMenuItem
            // 
            this.viewPeersToolStripMenuItem.Image = global::BuildSync.Client.Properties.Resources.appbar_people_multiple;
            this.viewPeersToolStripMenuItem.Name = "viewPeersToolStripMenuItem";
            this.viewPeersToolStripMenuItem.Size = new System.Drawing.Size(188, 30);
            this.viewPeersToolStripMenuItem.Text = "Peer Explorer";
            this.viewPeersToolStripMenuItem.Click += new System.EventHandler(this.ViewPeersClicked);
            // 
            // viewManifestsToolStripMenuItem
            // 
            this.viewManifestsToolStripMenuItem.Image = global::BuildSync.Client.Properties.Resources.appbar_cabinet_files_variant;
            this.viewManifestsToolStripMenuItem.Name = "viewManifestsToolStripMenuItem";
            this.viewManifestsToolStripMenuItem.Size = new System.Drawing.Size(188, 30);
            this.viewManifestsToolStripMenuItem.Text = "Manifest Explorer";
            this.viewManifestsToolStripMenuItem.Click += new System.EventHandler(this.ViewManifestsClicked);
            // 
            // helpTopicsToolStripMenuItem
            // 
            this.helpTopicsToolStripMenuItem.Image = global::BuildSync.Client.Properties.Resources.appbar_book_perspective_help;
            this.helpTopicsToolStripMenuItem.Name = "helpTopicsToolStripMenuItem";
            this.helpTopicsToolStripMenuItem.Size = new System.Drawing.Size(188, 30);
            this.helpTopicsToolStripMenuItem.Text = "View Help ...";
            this.helpTopicsToolStripMenuItem.Click += new System.EventHandler(this.ViewHelpClickled);
            // 
            // viewLicenseMenuToolstrip
            // 
            this.viewLicenseMenuToolstrip.Image = global::BuildSync.Client.Properties.Resources.appbar_key;
            this.viewLicenseMenuToolstrip.Name = "viewLicenseMenuToolstrip";
            this.viewLicenseMenuToolstrip.Size = new System.Drawing.Size(188, 30);
            this.viewLicenseMenuToolstrip.Text = "Licensing ...";
            this.viewLicenseMenuToolstrip.Click += new System.EventHandler(this.ViewLicenseClicked);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Image = global::BuildSync.Client.Properties.Resources.appbar_question;
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(188, 30);
            this.aboutToolStripMenuItem.Text = "About ...";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.AboutClicked);
            // 
            // manageBuildsToolStripMenuItem
            // 
            this.manageBuildsToolStripMenuItem.Image = global::BuildSync.Client.Properties.Resources.appbar_box;
            this.manageBuildsToolStripMenuItem.Name = "manageBuildsToolStripMenuItem";
            this.manageBuildsToolStripMenuItem.Size = new System.Drawing.Size(188, 30);
            this.manageBuildsToolStripMenuItem.Text = "Build Manager";
            this.manageBuildsToolStripMenuItem.Click += new System.EventHandler(this.ManageBuildsClicked);
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
        private System.Windows.Forms.ToolStripStatusLabel totalDiskDownBandwidthLabel;
        private System.Windows.Forms.ToolStripStatusLabel totalDiskUpBandwidthLabel;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem viewConsoleToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripMenuItem forceRedownloadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewPeersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem manageUsersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem forceReinstallToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpTopicsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripMenuItem viewManifestsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewLicenseMenuToolstrip;
        private System.Windows.Forms.ToolStripMenuItem forceRevalidateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem forceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pauseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem manageBuildsToolStripMenuItem;
    }
}

