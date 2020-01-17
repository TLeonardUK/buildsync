namespace BuildSync.Client.Forms
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
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.UserImageList = new System.Windows.Forms.ImageList(this.components);
            this.ListUpdateTimer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // MainListView
            // 
            this.MainListView.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.MainListView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.MainListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6,
            this.columnHeader2,
            this.columnHeader3});
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
            this.MainListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.MainListView.HideSelection = false;
            this.MainListView.HotTracking = true;
            this.MainListView.HoverSelection = true;
            this.MainListView.Location = new System.Drawing.Point(0, 0);
            this.MainListView.Name = "MainListView";
            this.MainListView.Size = new System.Drawing.Size(895, 204);
            this.MainListView.SmallImageList = this.UserImageList;
            this.MainListView.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.MainListView.TabIndex = 0;
            this.MainListView.UseCompatibleStateImageBehavior = false;
            this.MainListView.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Address";
            this.columnHeader1.Width = 190;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Download Speed";
            this.columnHeader4.Width = 150;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Upload Speed";
            this.columnHeader5.Width = 150;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Total Downloaded";
            this.columnHeader6.Width = 110;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Total Uploaded";
            this.columnHeader2.Width = 110;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Last Seen";
            this.columnHeader3.Width = 170;
            // 
            // UserImageList
            // 
            this.UserImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("UserImageList.ImageStream")));
            this.UserImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.UserImageList.Images.SetKeyName(0, "appbar.user.png");
            // 
            // ListUpdateTimer
            // 
            this.ListUpdateTimer.Enabled = true;
            this.ListUpdateTimer.Interval = 1000;
            this.ListUpdateTimer.Tick += new System.EventHandler(this.ListUpdateTimerTick);
            // 
            // PeersForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(895, 204);
            this.Controls.Add(this.MainListView);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PeersForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Peer Explorer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Closing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OnClosed);
            this.Shown += new System.EventHandler(this.OnShown);
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
    }
}