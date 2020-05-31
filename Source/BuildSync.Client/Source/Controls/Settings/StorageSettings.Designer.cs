namespace BuildSync.Client.Controls.Settings
{
    partial class StorageSettings
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StorageSettings));
            System.Windows.Forms.ListViewGroup listViewGroup1 = new System.Windows.Forms.ListViewGroup("Perforce", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup2 = new System.Windows.Forms.ListViewGroup("Git", System.Windows.Forms.HorizontalAlignment.Left);
            this.deprioritizeTagsTextBox = new BuildSync.Client.Controls.TagTextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.prioritizeTagsTextBox = new BuildSync.Client.Controls.TagTextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.HeuristicComboBox = new System.Windows.Forms.ComboBox();
            this.ButtonImageIndex = new System.Windows.Forms.ImageList(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.storageLocationList = new System.Windows.Forms.ListView();
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.locationListContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addLocationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editLocationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.deleteLocationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AddStorageLocationButton = new System.Windows.Forms.Button();
            this.RemoveStorageLocationButton = new System.Windows.Forms.Button();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.locationListContextMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // deprioritizeTagsTextBox
            // 
            this.deprioritizeTagsTextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.deprioritizeTagsTextBox.Location = new System.Drawing.Point(54, 164);
            this.deprioritizeTagsTextBox.Name = "deprioritizeTagsTextBox";
            this.deprioritizeTagsTextBox.Size = new System.Drawing.Size(337, 21);
            this.deprioritizeTagsTextBox.TabIndex = 49;
            this.deprioritizeTagsTextBox.TagIds = ((System.Collections.Generic.List<System.Guid>)(resources.GetObject("deprioritizeTagsTextBox.TagIds")));
            this.deprioritizeTagsTextBox.OnTagsChanged += new System.EventHandler(this.StateChanged);
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(13, 138);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(483, 17);
            this.label4.TabIndex = 48;
            this.label4.Text = "Prioritize deleting builds with any tags";
            // 
            // prioritizeTagsTextBox
            // 
            this.prioritizeTagsTextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.prioritizeTagsTextBox.Location = new System.Drawing.Point(54, 103);
            this.prioritizeTagsTextBox.Name = "prioritizeTagsTextBox";
            this.prioritizeTagsTextBox.Size = new System.Drawing.Size(337, 21);
            this.prioritizeTagsTextBox.TabIndex = 46;
            this.prioritizeTagsTextBox.TagIds = ((System.Collections.Generic.List<System.Guid>)(resources.GetObject("prioritizeTagsTextBox.TagIds")));
            this.prioritizeTagsTextBox.OnTagsChanged += new System.EventHandler(this.StateChanged);
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(13, 77);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(483, 17);
            this.label5.TabIndex = 45;
            this.label5.Text = "Prioritize keeping builds with any tags";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 15);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(243, 13);
            this.label3.TabIndex = 43;
            this.label3.Text = "What build to delete to make space for new builds";
            // 
            // HeuristicComboBox
            // 
            this.HeuristicComboBox.FormattingEnabled = true;
            this.HeuristicComboBox.Location = new System.Drawing.Point(54, 39);
            this.HeuristicComboBox.Name = "HeuristicComboBox";
            this.HeuristicComboBox.Size = new System.Drawing.Size(337, 21);
            this.HeuristicComboBox.TabIndex = 42;
            this.HeuristicComboBox.SelectedIndexChanged += new System.EventHandler(this.StateChanged);
            // 
            // ButtonImageIndex
            // 
            this.ButtonImageIndex.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ButtonImageIndex.ImageStream")));
            this.ButtonImageIndex.TransparentColor = System.Drawing.Color.Transparent;
            this.ButtonImageIndex.Images.SetKeyName(0, "appbar.add.png");
            this.ButtonImageIndex.Images.SetKeyName(1, "appbar.delete.png");
            this.ButtonImageIndex.Images.SetKeyName(2, "appbar.user.add.png");
            this.ButtonImageIndex.Images.SetKeyName(3, "appbar.user.delete.png");
            this.ButtonImageIndex.Images.SetKeyName(4, "appbar.database.png");
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 200);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(93, 13);
            this.label1.TabIndex = 52;
            this.label1.Text = "Storage Locations";
            // 
            // storageLocationList
            // 
            this.storageLocationList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.storageLocationList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader5,
            this.columnHeader1,
            this.columnHeader6,
            this.columnHeader2});
            this.storageLocationList.ContextMenuStrip = this.locationListContextMenu;
            this.storageLocationList.FullRowSelect = true;
            listViewGroup1.Header = "Perforce";
            listViewGroup1.Name = "Perforce";
            listViewGroup2.Header = "Git";
            listViewGroup2.Name = "Git";
            this.storageLocationList.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup1,
            listViewGroup2});
            this.storageLocationList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.storageLocationList.HideSelection = false;
            this.storageLocationList.Location = new System.Drawing.Point(14, 221);
            this.storageLocationList.Name = "storageLocationList";
            this.storageLocationList.ShowGroups = false;
            this.storageLocationList.Size = new System.Drawing.Size(490, 113);
            this.storageLocationList.SmallImageList = this.ButtonImageIndex;
            this.storageLocationList.TabIndex = 51;
            this.storageLocationList.UseCompatibleStateImageBehavior = false;
            this.storageLocationList.View = System.Windows.Forms.View.Details;
            this.storageLocationList.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.StorageLocationItemChanged);
            this.storageLocationList.DoubleClick += new System.EventHandler(this.EditClicked);
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Location";
            this.columnHeader5.Width = 235;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Current Size";
            this.columnHeader1.Width = 80;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Max Size";
            this.columnHeader6.Width = 80;
            // 
            // locationListContextMenu
            // 
            this.locationListContextMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.locationListContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addLocationToolStripMenuItem,
            this.editLocationToolStripMenuItem,
            this.toolStripSeparator6,
            this.deleteLocationToolStripMenuItem});
            this.locationListContextMenu.Name = "downloadListContextMenu";
            this.locationListContextMenu.Size = new System.Drawing.Size(166, 100);
            // 
            // addLocationToolStripMenuItem
            // 
            this.addLocationToolStripMenuItem.Image = global::BuildSync.Client.Properties.Resources.appbar_add;
            this.addLocationToolStripMenuItem.Name = "addLocationToolStripMenuItem";
            this.addLocationToolStripMenuItem.Size = new System.Drawing.Size(165, 30);
            this.addLocationToolStripMenuItem.Text = "Add Location ...";
            this.addLocationToolStripMenuItem.Click += new System.EventHandler(this.AddStorageClicked);
            // 
            // editLocationToolStripMenuItem
            // 
            this.editLocationToolStripMenuItem.Image = global::BuildSync.Client.Properties.Resources.appbar_draw_pencil;
            this.editLocationToolStripMenuItem.Name = "editLocationToolStripMenuItem";
            this.editLocationToolStripMenuItem.Size = new System.Drawing.Size(165, 30);
            this.editLocationToolStripMenuItem.Text = "Edit Location ...";
            this.editLocationToolStripMenuItem.Click += new System.EventHandler(this.EditClicked);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(162, 6);
            // 
            // deleteLocationToolStripMenuItem
            // 
            this.deleteLocationToolStripMenuItem.Image = global::BuildSync.Client.Properties.Resources.appbar_delete;
            this.deleteLocationToolStripMenuItem.Name = "deleteLocationToolStripMenuItem";
            this.deleteLocationToolStripMenuItem.Size = new System.Drawing.Size(165, 30);
            this.deleteLocationToolStripMenuItem.Text = "Delete";
            this.deleteLocationToolStripMenuItem.Click += new System.EventHandler(this.RemoveStorageClicked);
            // 
            // AddStorageLocationButton
            // 
            this.AddStorageLocationButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.AddStorageLocationButton.ImageIndex = 0;
            this.AddStorageLocationButton.ImageList = this.ButtonImageIndex;
            this.AddStorageLocationButton.Location = new System.Drawing.Point(439, 187);
            this.AddStorageLocationButton.Margin = new System.Windows.Forms.Padding(2);
            this.AddStorageLocationButton.Name = "AddStorageLocationButton";
            this.AddStorageLocationButton.Size = new System.Drawing.Size(30, 29);
            this.AddStorageLocationButton.TabIndex = 54;
            this.AddStorageLocationButton.UseVisualStyleBackColor = true;
            this.AddStorageLocationButton.Click += new System.EventHandler(this.AddStorageClicked);
            // 
            // RemoveStorageLocationButton
            // 
            this.RemoveStorageLocationButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.RemoveStorageLocationButton.ImageIndex = 1;
            this.RemoveStorageLocationButton.ImageList = this.ButtonImageIndex;
            this.RemoveStorageLocationButton.Location = new System.Drawing.Point(474, 187);
            this.RemoveStorageLocationButton.Margin = new System.Windows.Forms.Padding(2);
            this.RemoveStorageLocationButton.Name = "RemoveStorageLocationButton";
            this.RemoveStorageLocationButton.Size = new System.Drawing.Size(30, 29);
            this.RemoveStorageLocationButton.TabIndex = 53;
            this.RemoveStorageLocationButton.UseVisualStyleBackColor = true;
            this.RemoveStorageLocationButton.Click += new System.EventHandler(this.RemoveStorageClicked);
            // 
            // pictureBox3
            // 
            this.pictureBox3.Image = global::BuildSync.Client.Properties.Resources.ic_check_circle_2x;
            this.pictureBox3.Location = new System.Drawing.Point(14, 157);
            this.pictureBox3.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(32, 31);
            this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox3.TabIndex = 50;
            this.pictureBox3.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::BuildSync.Client.Properties.Resources.ic_check_circle_2x;
            this.pictureBox2.Location = new System.Drawing.Point(14, 96);
            this.pictureBox2.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(32, 31);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 47;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::BuildSync.Client.Properties.Resources.ic_check_circle_2x;
            this.pictureBox1.Location = new System.Drawing.Point(14, 33);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(32, 31);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 44;
            this.pictureBox1.TabStop = false;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Min Size";
            this.columnHeader2.Width = 80;
            // 
            // StorageSettings
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.AddStorageLocationButton);
            this.Controls.Add(this.RemoveStorageLocationButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.storageLocationList);
            this.Controls.Add(this.pictureBox3);
            this.Controls.Add(this.deprioritizeTagsTextBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.prioritizeTagsTextBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.HeuristicComboBox);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "StorageSettings";
            this.Size = new System.Drawing.Size(519, 337);
            this.locationListContextMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox HeuristicComboBox;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private TagTextBox prioritizeTagsTextBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.PictureBox pictureBox3;
        private TagTextBox deprioritizeTagsTextBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button AddStorageLocationButton;
        private System.Windows.Forms.Button RemoveStorageLocationButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListView storageLocationList;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ImageList ButtonImageIndex;
        private System.Windows.Forms.ContextMenuStrip locationListContextMenu;
        private System.Windows.Forms.ToolStripMenuItem addLocationToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripMenuItem deleteLocationToolStripMenuItem;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ToolStripMenuItem editLocationToolStripMenuItem;
        private System.Windows.Forms.ColumnHeader columnHeader2;
    }
}
