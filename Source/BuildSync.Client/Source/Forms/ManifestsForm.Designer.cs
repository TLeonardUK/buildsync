﻿namespace BuildSync.Client.Forms
{
    partial class ManifestsForm
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
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("Test", 0);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ManifestsForm));
            this.mainListView = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.listContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.openInExplorerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.listImageList = new System.Windows.Forms.ImageList(this.components);
            this.UpdateTimer = new System.Windows.Forms.Timer(this.components);
            this.listContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainListView
            // 
            this.mainListView.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.mainListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader5,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader6,
            this.columnHeader7,
            this.columnHeader8});
            this.mainListView.ContextMenuStrip = this.listContextMenu;
            this.mainListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainListView.FullRowSelect = true;
            this.mainListView.GridLines = true;
            this.mainListView.HideSelection = false;
            this.mainListView.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1});
            this.mainListView.Location = new System.Drawing.Point(0, 0);
            this.mainListView.Name = "mainListView";
            this.mainListView.Size = new System.Drawing.Size(1068, 211);
            this.mainListView.SmallImageList = this.listImageList;
            this.mainListView.TabIndex = 0;
            this.mainListView.UseCompatibleStateImageBehavior = false;
            this.mainListView.View = System.Windows.Forms.View.Details;
            this.mainListView.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.ColumnClicked);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Id";
            this.columnHeader1.Width = 240;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Path";
            this.columnHeader2.Width = 200;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Local Path";
            this.columnHeader5.Width = 200;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Progress";
            this.columnHeader3.Width = 80;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "State";
            this.columnHeader4.Width = 130;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Total Size";
            this.columnHeader6.Width = 80;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "Upload";
            this.columnHeader7.Width = 70;
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "Download";
            this.columnHeader8.Width = 70;
            // 
            // listContextMenu
            // 
            this.listContextMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.listContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openInExplorerToolStripMenuItem,
            this.toolStripSeparator9,
            this.deleteToolStripMenuItem});
            this.listContextMenu.Name = "downloadListContextMenu";
            this.listContextMenu.Size = new System.Drawing.Size(183, 70);
            this.listContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.ContextMenuOpening);
            // 
            // openInExplorerToolStripMenuItem
            // 
            this.openInExplorerToolStripMenuItem.Image = global::BuildSync.Client.Properties.Resources.appbar_folder_open;
            this.openInExplorerToolStripMenuItem.Name = "openInExplorerToolStripMenuItem";
            this.openInExplorerToolStripMenuItem.Size = new System.Drawing.Size(182, 30);
            this.openInExplorerToolStripMenuItem.Text = "Open in Explorer ...";
            this.openInExplorerToolStripMenuItem.Click += new System.EventHandler(this.OpenInExplorerClicked);
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            this.toolStripSeparator9.Size = new System.Drawing.Size(179, 6);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Image = global::BuildSync.Client.Properties.Resources.appbar_delete;
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(182, 30);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.DeleteClicked);
            // 
            // listImageList
            // 
            this.listImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("listImageList.ImageStream")));
            this.listImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.listImageList.Images.SetKeyName(0, "appbar.box.png");
            // 
            // UpdateTimer
            // 
            this.UpdateTimer.Interval = 1000;
            this.UpdateTimer.Tick += new System.EventHandler(this.UpdateTimerTick);
            // 
            // ManifestsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1068, 211);
            this.Controls.Add(this.mainListView);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.HideOnClose = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ManifestsForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Manifest Explorer";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OnClosed);
            this.Load += new System.EventHandler(this.OnLoaded);
            this.Shown += new System.EventHandler(this.OnShown);
            this.listContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView mainListView;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.Timer UpdateTimer;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.ImageList listImageList;
        private System.Windows.Forms.ContextMenuStrip listContextMenu;
        private System.Windows.Forms.ToolStripMenuItem openInExplorerToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
    }
}