﻿namespace BuildSync.Client.Forms
{
    partial class PeersForm
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
            System.Windows.Forms.ListViewGroup listViewGroup1 = new System.Windows.Forms.ListViewGroup("Connected", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup2 = new System.Windows.Forms.ListViewGroup("Disconnected", System.Windows.Forms.HorizontalAlignment.Left);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PeersForm));
            this.MainListView = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader13 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader10 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader11 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader9 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader12 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.MainContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.availabilityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.UserImageList = new System.Windows.Forms.ImageList(this.components);
            this.ListUpdateTimer = new System.Windows.Forms.Timer(this.components);
            this.columnHeader14 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.MainContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainListView
            // 
            this.MainListView.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.MainListView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.MainListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader13,
            this.columnHeader4,
            this.columnHeader10,
            this.columnHeader5,
            this.columnHeader11,
            this.columnHeader6,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader7,
            this.columnHeader8,
            this.columnHeader14,
            this.columnHeader9,
            this.columnHeader12});
            this.MainListView.ContextMenuStrip = this.MainContextMenu;
            this.MainListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainListView.FullRowSelect = true;
            this.MainListView.GridLines = true;
            listViewGroup1.Header = "Connected";
            listViewGroup1.Name = "Connected";
            listViewGroup2.Header = "Disconnected";
            listViewGroup2.Name = "Disconnected";
            this.MainListView.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup1,
            listViewGroup2});
            this.MainListView.HideSelection = false;
            this.MainListView.Location = new System.Drawing.Point(0, 0);
            this.MainListView.Name = "MainListView";
            this.MainListView.Size = new System.Drawing.Size(1258, 204);
            this.MainListView.SmallImageList = this.UserImageList;
            this.MainListView.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.MainListView.TabIndex = 0;
            this.MainListView.UseCompatibleStateImageBehavior = false;
            this.MainListView.View = System.Windows.Forms.View.Details;
            this.MainListView.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.ColumnClicked);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Username";
            this.columnHeader1.Width = 130;
            // 
            // columnHeader13
            // 
            this.columnHeader13.Text = "Hostname";
            this.columnHeader13.Width = 130;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Down Rate";
            this.columnHeader4.Width = 75;
            // 
            // columnHeader10
            // 
            this.columnHeader10.Text = "Peak Down";
            this.columnHeader10.Width = 75;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Up Rate";
            this.columnHeader5.Width = 75;
            // 
            // columnHeader11
            // 
            this.columnHeader11.Text = "Peak Up";
            this.columnHeader11.Width = 75;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Total Down";
            this.columnHeader6.Width = 75;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Total Up";
            this.columnHeader2.Width = 75;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Last Seen";
            this.columnHeader3.Width = 110;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "Target Requests";
            this.columnHeader7.Width = 100;
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "Current Requests";
            this.columnHeader8.Width = 100;
            // 
            // columnHeader9
            // 
            this.columnHeader9.Text = "RTT";
            // 
            // columnHeader12
            // 
            this.columnHeader12.Text = "Min RTT";
            // 
            // MainContextMenu
            // 
            this.MainContextMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.MainContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.availabilityToolStripMenuItem});
            this.MainContextMenu.Name = "downloadListContextMenu";
            this.MainContextMenu.Size = new System.Drawing.Size(218, 34);
            this.MainContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.ContextMenuOpening);
            // 
            // availabilityToolStripMenuItem
            // 
            this.availabilityToolStripMenuItem.Image = global::BuildSync.Client.Properties.Resources.appbar_network_server_connecting;
            this.availabilityToolStripMenuItem.Name = "availabilityToolStripMenuItem";
            this.availabilityToolStripMenuItem.Size = new System.Drawing.Size(217, 30);
            this.availabilityToolStripMenuItem.Text = "View Manifest Availability";
            this.availabilityToolStripMenuItem.Click += new System.EventHandler(this.PeerAvailabilityClicked);
            // 
            // UserImageList
            // 
            this.UserImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("UserImageList.ImageStream")));
            this.UserImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.UserImageList.Images.SetKeyName(0, "appbar.user.png");
            // 
            // ListUpdateTimer
            // 
            this.ListUpdateTimer.Interval = 1000;
            this.ListUpdateTimer.Tick += new System.EventHandler(this.ListUpdateTimerTick);
            // 
            // columnHeader14
            // 
            this.columnHeader14.Text = "Queue Size";
            this.columnHeader14.Width = 100;
            // 
            // PeersForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1258, 204);
            this.Controls.Add(this.MainListView);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.HideOnClose = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PeersForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Peer Explorer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnStartClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OnClosed);
            this.Shown += new System.EventHandler(this.OnShown);
            this.MainContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView MainListView;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.Timer ListUpdateTimer;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ImageList UserImageList;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.ColumnHeader columnHeader9;
        private System.Windows.Forms.ColumnHeader columnHeader10;
        private System.Windows.Forms.ColumnHeader columnHeader11;
        private System.Windows.Forms.ColumnHeader columnHeader12;
        private System.Windows.Forms.ColumnHeader columnHeader13;
        private System.Windows.Forms.ContextMenuStrip MainContextMenu;
        private System.Windows.Forms.ToolStripMenuItem availabilityToolStripMenuItem;
        private System.Windows.Forms.ColumnHeader columnHeader14;
    }
}