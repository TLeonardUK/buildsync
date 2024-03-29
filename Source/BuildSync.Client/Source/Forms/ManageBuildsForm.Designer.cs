﻿namespace BuildSync.Client.Forms
{
    partial class ManageBuildsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ManageBuildsForm));
            this.ButtonImageList = new System.Windows.Forms.ImageList(this.components);
            this.UpdateTimer = new System.Windows.Forms.Timer(this.components);
            this.downloadListContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addDownloadToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.addTagToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.downloadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.remoteInstallToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.diffManifestsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.examineBuildToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.downloadFileSystemTree = new BuildSync.Client.Controls.DownloadFileSystemTree();
            this.downloadListContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // ButtonImageList
            // 
            this.ButtonImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ButtonImageList.ImageStream")));
            this.ButtonImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.ButtonImageList.Images.SetKeyName(0, "appbar.add.png");
            this.ButtonImageList.Images.SetKeyName(1, "appbar.delete.png");
            // 
            // UpdateTimer
            // 
            this.UpdateTimer.Enabled = true;
            this.UpdateTimer.Interval = 1000;
            this.UpdateTimer.Tick += new System.EventHandler(this.UpdateTimer_Tick);
            // 
            // downloadListContextMenu
            // 
            this.downloadListContextMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.downloadListContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addDownloadToolStripMenuItem1,
            this.addTagToolStripMenuItem,
            this.toolStripSeparator6,
            this.downloadToolStripMenuItem,
            this.remoteInstallToolStripMenuItem,
            this.toolStripSeparator1,
            this.diffManifestsToolStripMenuItem,
            this.examineBuildToolStripMenuItem,
            this.toolStripSeparator9,
            this.deleteToolStripMenuItem});
            this.downloadListContextMenu.Name = "downloadListContextMenu";
            this.downloadListContextMenu.Size = new System.Drawing.Size(189, 254);
            this.downloadListContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.ContextMenuOpening);
            // 
            // addDownloadToolStripMenuItem1
            // 
            this.addDownloadToolStripMenuItem1.Image = global::BuildSync.Client.Properties.Resources.appbar_add;
            this.addDownloadToolStripMenuItem1.Name = "addDownloadToolStripMenuItem1";
            this.addDownloadToolStripMenuItem1.Size = new System.Drawing.Size(188, 30);
            this.addDownloadToolStripMenuItem1.Text = "Add Build ...";
            this.addDownloadToolStripMenuItem1.Click += new System.EventHandler(this.AddBuildClicked);
            // 
            // addTagToolStripMenuItem
            // 
            this.addTagToolStripMenuItem.Image = global::BuildSync.Client.Properties.Resources.appbar_tag;
            this.addTagToolStripMenuItem.Name = "addTagToolStripMenuItem";
            this.addTagToolStripMenuItem.Size = new System.Drawing.Size(188, 30);
            this.addTagToolStripMenuItem.Text = "Toggle Tag ...";
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(185, 6);
            // 
            // downloadToolStripMenuItem
            // 
            this.downloadToolStripMenuItem.Image = global::BuildSync.Client.Properties.Resources.appbar_download;
            this.downloadToolStripMenuItem.Name = "downloadToolStripMenuItem";
            this.downloadToolStripMenuItem.Size = new System.Drawing.Size(188, 30);
            this.downloadToolStripMenuItem.Text = "Download ...";
            this.downloadToolStripMenuItem.Click += new System.EventHandler(this.DownloadClicked);
            // 
            // remoteInstallToolStripMenuItem
            // 
            this.remoteInstallToolStripMenuItem.Image = global::BuildSync.Client.Properties.Resources.appbar_network_server_connecting;
            this.remoteInstallToolStripMenuItem.Name = "remoteInstallToolStripMenuItem";
            this.remoteInstallToolStripMenuItem.Size = new System.Drawing.Size(188, 30);
            this.remoteInstallToolStripMenuItem.Text = "Remote Install ...";
            this.remoteInstallToolStripMenuItem.Click += new System.EventHandler(this.RemoteInstallClicked);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(185, 6);
            // 
            // diffManifestsToolStripMenuItem
            // 
            this.diffManifestsToolStripMenuItem.Image = global::BuildSync.Client.Properties.Resources.appbar_page_search;
            this.diffManifestsToolStripMenuItem.Name = "diffManifestsToolStripMenuItem";
            this.diffManifestsToolStripMenuItem.Size = new System.Drawing.Size(188, 30);
            this.diffManifestsToolStripMenuItem.Text = "Diff Manifests ...";
            this.diffManifestsToolStripMenuItem.Click += new System.EventHandler(this.DiffBuildsClicked);
            // 
            // examineBuildToolStripMenuItem
            // 
            this.examineBuildToolStripMenuItem.Image = global::BuildSync.Client.Properties.Resources.appbar_folder_open;
            this.examineBuildToolStripMenuItem.Name = "examineBuildToolStripMenuItem";
            this.examineBuildToolStripMenuItem.Size = new System.Drawing.Size(188, 30);
            this.examineBuildToolStripMenuItem.Text = "Examine Manifest ...";
            this.examineBuildToolStripMenuItem.Click += new System.EventHandler(this.ExampleBuildClicked);
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            this.toolStripSeparator9.Size = new System.Drawing.Size(185, 6);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Image = global::BuildSync.Client.Properties.Resources.appbar_delete;
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(188, 30);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.RemoveBuildClicked);
            // 
            // downloadFileSystemTree
            // 
            this.downloadFileSystemTree.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.downloadFileSystemTree.CanSelectBuildContainers = true;
            this.downloadFileSystemTree.ContextMenuStrip = this.downloadListContextMenu;
            this.downloadFileSystemTree.Location = new System.Drawing.Point(0, 0);
            this.downloadFileSystemTree.Margin = new System.Windows.Forms.Padding(1);
            this.downloadFileSystemTree.Name = "downloadFileSystemTree";
            this.downloadFileSystemTree.SelectedPath = "";
            this.downloadFileSystemTree.SelectedPathRaw = "";
            this.downloadFileSystemTree.ShowInternal = true;
            this.downloadFileSystemTree.Size = new System.Drawing.Size(1093, 460);
            this.downloadFileSystemTree.TabIndex = 22;
            this.downloadFileSystemTree.OnSelectedNodeChanged += new System.EventHandler(this.DateStateChanged);
            // 
            // ManageBuildsForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1093, 460);
            this.Controls.Add(this.downloadFileSystemTree);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.HideOnClose = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ManageBuildsForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Build Explorer";
            this.Shown += new System.EventHandler(this.FormShown);
            this.downloadListContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Controls.DownloadFileSystemTree downloadFileSystemTree;
        private System.Windows.Forms.ImageList ButtonImageList;
        private System.Windows.Forms.Timer UpdateTimer;
        private System.Windows.Forms.ContextMenuStrip downloadListContextMenu;
        private System.Windows.Forms.ToolStripMenuItem addDownloadToolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem downloadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addTagToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem remoteInstallToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem diffManifestsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem examineBuildToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    }
}