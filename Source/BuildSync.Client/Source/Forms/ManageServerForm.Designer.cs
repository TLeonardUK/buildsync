namespace BuildSync.Client.Forms
{
    partial class ManageServerForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ManageServerForm));
            System.Windows.Forms.ListViewGroup listViewGroup1 = new System.Windows.Forms.ListViewGroup("Connected", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup2 = new System.Windows.Forms.ListViewGroup("Disconnected", System.Windows.Forms.HorizontalAlignment.Left);
            this.ButtonImageIndex = new System.Windows.Forms.ImageList(this.components);
            this.UserImageList = new System.Windows.Forms.ImageList(this.components);
            this.MainListView = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader10 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader9 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clientLstContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addTagToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.RefreshTimer = new System.Windows.Forms.Timer(this.components);
            this.BandwithGraph = new BuildSync.Core.Controls.Graph.GraphControl();
            this.MaxBandwidthBox = new BuildSync.Core.Controls.SizeTextBox();
            this.columnHeader11 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clientLstContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // ButtonImageIndex
            // 
            this.ButtonImageIndex.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ButtonImageIndex.ImageStream")));
            this.ButtonImageIndex.TransparentColor = System.Drawing.Color.Transparent;
            this.ButtonImageIndex.Images.SetKeyName(0, "appbar.add.png");
            this.ButtonImageIndex.Images.SetKeyName(1, "appbar.delete.png");
            this.ButtonImageIndex.Images.SetKeyName(2, "appbar.user.add.png");
            this.ButtonImageIndex.Images.SetKeyName(3, "appbar.user.delete.png");
            // 
            // UserImageList
            // 
            this.UserImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("UserImageList.ImageStream")));
            this.UserImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.UserImageList.Images.SetKeyName(0, "appbar.user.png");
            // 
            // MainListView
            // 
            this.MainListView.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.MainListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MainListView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.MainListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader10,
            this.columnHeader9,
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader7,
            this.columnHeader11,
            this.columnHeader8});
            this.MainListView.ContextMenuStrip = this.clientLstContextMenu;
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
            this.MainListView.Location = new System.Drawing.Point(0, 111);
            this.MainListView.Name = "MainListView";
            this.MainListView.ShowGroups = false;
            this.MainListView.Size = new System.Drawing.Size(1147, 204);
            this.MainListView.SmallImageList = this.UserImageList;
            this.MainListView.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.MainListView.TabIndex = 1;
            this.MainListView.UseCompatibleStateImageBehavior = false;
            this.MainListView.View = System.Windows.Forms.View.Details;
            this.MainListView.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.ColumnClicked);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Username";
            this.columnHeader1.Width = 190;
            // 
            // columnHeader10
            // 
            this.columnHeader10.Text = "Hostname";
            this.columnHeader10.Width = 190;
            // 
            // columnHeader9
            // 
            this.columnHeader9.Text = "Tags";
            this.columnHeader9.Width = 170;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Down Speed";
            this.columnHeader4.Width = 80;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Up Speed";
            this.columnHeader5.Width = 80;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Total Down";
            this.columnHeader6.Width = 80;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Total Up";
            this.columnHeader2.Width = 80;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Peers";
            this.columnHeader3.Width = 50;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "Disk Usage";
            this.columnHeader7.Width = 80;
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "Version";
            this.columnHeader8.Width = 70;
            // 
            // clientLstContextMenu
            // 
            this.clientLstContextMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.clientLstContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addTagToolStripMenuItem});
            this.clientLstContextMenu.Name = "downloadListContextMenu";
            this.clientLstContextMenu.Size = new System.Drawing.Size(151, 34);
            this.clientLstContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.ContextMenuOpening);
            // 
            // addTagToolStripMenuItem
            // 
            this.addTagToolStripMenuItem.Image = global::BuildSync.Client.Properties.Resources.appbar_tag;
            this.addTagToolStripMenuItem.Name = "addTagToolStripMenuItem";
            this.addTagToolStripMenuItem.Size = new System.Drawing.Size(150, 30);
            this.addTagToolStripMenuItem.Text = "Toggle Tag ...";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 93);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(93, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Connected Clients";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 10);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(169, 13);
            this.label3.TabIndex = 24;
            this.label3.Text = "Maximum bandwidth, 0 is unlimited\r\n";
            // 
            // label2
            // 
            this.label2.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.label2.Location = new System.Drawing.Point(4, 28);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(422, 26);
            this.label2.TabIndex = 27;
            this.label2.Text = "This bandwidth will be split across all downloading peers to cap \r\ntotal network " +
    "bandwidth utilization. This takes priority over route specific bandwidth limits\r" +
    "\n";
            // 
            // RefreshTimer
            // 
            this.RefreshTimer.Interval = 1000;
            this.RefreshTimer.Tick += new System.EventHandler(this.RefreshTicked);
            // 
            // BandwithGraph
            // 
            this.BandwithGraph.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.BandwithGraph.DrawGridLines = true;
            this.BandwithGraph.DrawLabels = true;
            this.BandwithGraph.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(223)))), ((int)(((byte)(238)))), ((int)(((byte)(246)))));
            this.BandwithGraph.Location = new System.Drawing.Point(441, 2);
            this.BandwithGraph.Name = "BandwithGraph";
            this.BandwithGraph.Series = new BuildSync.Core.Controls.Graph.GraphSeries[] {
        null};
            this.BandwithGraph.Size = new System.Drawing.Size(706, 104);
            this.BandwithGraph.TabIndex = 3;
            // 
            // MaxBandwidthBox
            // 
            this.MaxBandwidthBox.DisplayAsTransferRate = true;
            this.MaxBandwidthBox.Location = new System.Drawing.Point(7, 61);
            this.MaxBandwidthBox.Name = "MaxBandwidthBox";
            this.MaxBandwidthBox.Size = new System.Drawing.Size(419, 26);
            this.MaxBandwidthBox.TabIndex = 28;
            this.MaxBandwidthBox.Value = ((long)(0));
            this.MaxBandwidthBox.OnValueChanged += new System.EventHandler(this.MaxBandwidthBoxChanged);
            // 
            // columnHeader11
            // 
            this.columnHeader11.Text = "Disk Quota";
            this.columnHeader11.Width = 70;
            // 
            // ManageServerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1149, 315);
            this.Controls.Add(this.MaxBandwidthBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.BandwithGraph);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.MainListView);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.HideOnClose = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ManageServerForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Server Manager";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OnClosed);
            this.Shown += new System.EventHandler(this.OnShown);
            this.clientLstContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ImageList UserImageList;
        private System.Windows.Forms.ImageList ButtonImageIndex;
        private System.Windows.Forms.ListView MainListView;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.Label label1;
        private Core.Controls.Graph.GraphControl BandwithGraph;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Timer RefreshTimer;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.ColumnHeader columnHeader8;
		private System.Windows.Forms.ColumnHeader columnHeader9;
        private System.Windows.Forms.ContextMenuStrip clientLstContextMenu;
        private System.Windows.Forms.ToolStripMenuItem addTagToolStripMenuItem;
        private System.Windows.Forms.ColumnHeader columnHeader10;
        private Core.Controls.SizeTextBox MaxBandwidthBox;
        private System.Windows.Forms.ColumnHeader columnHeader11;
    }
}