namespace BuildSync.Client.Forms
{
    partial class ManageUsersForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ManageUsersForm));
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.closeButton = new System.Windows.Forms.Button();
            this.ButtonImageIndex = new System.Windows.Forms.ImageList(this.components);
            this.UserListView = new System.Windows.Forms.ListView();
            this.UserImageList = new System.Windows.Forms.ImageList(this.components);
            this.PermissionListView = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label2 = new System.Windows.Forms.Label();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.RemoveUserButton = new System.Windows.Forms.Button();
            this.AddUserButton = new System.Windows.Forms.Button();
            this.AddPermissionButton = new System.Windows.Forms.Button();
            this.RemovePermissionButton = new System.Windows.Forms.Button();
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 13);
            this.label1.TabIndex = 32;
            this.label1.Text = "Users";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.SystemColors.ControlDark;
            this.panel1.Location = new System.Drawing.Point(9, 264);
            this.panel1.Margin = new System.Windows.Forms.Padding(2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(638, 1);
            this.panel1.TabIndex = 31;
            // 
            // closeButton
            // 
            this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.closeButton.Location = new System.Drawing.Point(552, 275);
            this.closeButton.Margin = new System.Windows.Forms.Padding(2);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(95, 29);
            this.closeButton.TabIndex = 30;
            this.closeButton.Text = "Close";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.CloseClicked);
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
            // UserListView
            // 
            this.UserListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.UserListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3});
            this.UserListView.FullRowSelect = true;
            this.UserListView.GridLines = true;
            this.UserListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.UserListView.HideSelection = false;
            this.UserListView.LabelEdit = true;
            this.UserListView.Location = new System.Drawing.Point(0, 39);
            this.UserListView.Name = "UserListView";
            this.UserListView.Size = new System.Drawing.Size(224, 209);
            this.UserListView.SmallImageList = this.UserImageList;
            this.UserListView.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.UserListView.TabIndex = 33;
            this.UserListView.UseCompatibleStateImageBehavior = false;
            this.UserListView.View = System.Windows.Forms.View.Details;
            this.UserListView.AfterLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.FinishLabelEdit);
            this.UserListView.BeforeLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.BeginLabelEdit);
            this.UserListView.SelectedIndexChanged += new System.EventHandler(this.UserListSelectedItemChanged);
            // 
            // UserImageList
            // 
            this.UserImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("UserImageList.ImageStream")));
            this.UserImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.UserImageList.Images.SetKeyName(0, "appbar.user.png");
            // 
            // PermissionListView
            // 
            this.PermissionListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PermissionListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.PermissionListView.FullRowSelect = true;
            this.PermissionListView.GridLines = true;
            this.PermissionListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.PermissionListView.HideSelection = false;
            this.PermissionListView.Location = new System.Drawing.Point(0, 39);
            this.PermissionListView.Name = "PermissionListView";
            this.PermissionListView.Size = new System.Drawing.Size(407, 209);
            this.PermissionListView.TabIndex = 36;
            this.PermissionListView.UseCompatibleStateImageBehavior = false;
            this.PermissionListView.View = System.Windows.Forms.View.Details;
            this.PermissionListView.SelectedIndexChanged += new System.EventHandler(this.PermissionListSelectedItemChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Permission Type";
            this.columnHeader1.Width = 150;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Virtual Path";
            this.columnHeader2.Width = 250;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 13);
            this.label2.TabIndex = 37;
            this.label2.Text = "Permissions";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(9, 2);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.UserListView);
            this.splitContainer1.Panel1.Controls.Add(this.RemoveUserButton);
            this.splitContainer1.Panel1.Controls.Add(this.AddUserButton);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.AddPermissionButton);
            this.splitContainer1.Panel2.Controls.Add(this.PermissionListView);
            this.splitContainer1.Panel2.Controls.Add(this.label2);
            this.splitContainer1.Panel2.Controls.Add(this.RemovePermissionButton);
            this.splitContainer1.Size = new System.Drawing.Size(637, 248);
            this.splitContainer1.SplitterDistance = 226;
            this.splitContainer1.TabIndex = 40;
            // 
            // RemoveUserButton
            // 
            this.RemoveUserButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.RemoveUserButton.ImageIndex = 3;
            this.RemoveUserButton.ImageList = this.ButtonImageIndex;
            this.RemoveUserButton.Location = new System.Drawing.Point(194, 5);
            this.RemoveUserButton.Margin = new System.Windows.Forms.Padding(2);
            this.RemoveUserButton.Name = "RemoveUserButton";
            this.RemoveUserButton.Size = new System.Drawing.Size(30, 29);
            this.RemoveUserButton.TabIndex = 27;
            this.RemoveUserButton.UseVisualStyleBackColor = true;
            this.RemoveUserButton.Click += new System.EventHandler(this.RemoveUserButtonClicked);
            // 
            // AddUserButton
            // 
            this.AddUserButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.AddUserButton.ImageIndex = 2;
            this.AddUserButton.ImageList = this.ButtonImageIndex;
            this.AddUserButton.Location = new System.Drawing.Point(157, 5);
            this.AddUserButton.Margin = new System.Windows.Forms.Padding(2);
            this.AddUserButton.Name = "AddUserButton";
            this.AddUserButton.Size = new System.Drawing.Size(30, 29);
            this.AddUserButton.TabIndex = 29;
            this.AddUserButton.UseVisualStyleBackColor = true;
            this.AddUserButton.Click += new System.EventHandler(this.AddUserButtonClicked);
            // 
            // AddPermissionButton
            // 
            this.AddPermissionButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.AddPermissionButton.ImageIndex = 0;
            this.AddPermissionButton.ImageList = this.ButtonImageIndex;
            this.AddPermissionButton.Location = new System.Drawing.Point(342, 5);
            this.AddPermissionButton.Margin = new System.Windows.Forms.Padding(2);
            this.AddPermissionButton.Name = "AddPermissionButton";
            this.AddPermissionButton.Size = new System.Drawing.Size(30, 29);
            this.AddPermissionButton.TabIndex = 45;
            this.AddPermissionButton.UseVisualStyleBackColor = true;
            this.AddPermissionButton.Click += new System.EventHandler(this.AddPermissionButtonClicked);
            // 
            // RemovePermissionButton
            // 
            this.RemovePermissionButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.RemovePermissionButton.ImageIndex = 1;
            this.RemovePermissionButton.ImageList = this.ButtonImageIndex;
            this.RemovePermissionButton.Location = new System.Drawing.Point(377, 5);
            this.RemovePermissionButton.Margin = new System.Windows.Forms.Padding(2);
            this.RemovePermissionButton.Name = "RemovePermissionButton";
            this.RemovePermissionButton.Size = new System.Drawing.Size(30, 29);
            this.RemovePermissionButton.TabIndex = 38;
            this.RemovePermissionButton.UseVisualStyleBackColor = true;
            this.RemovePermissionButton.Click += new System.EventHandler(this.RemovePermissionButtonClicked);
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Username";
            this.columnHeader3.Width = 220;
            // 
            // ManageUsersForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(658, 315);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.closeButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ManageUsersForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Manage Users";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OnClosed);
            this.Shown += new System.EventHandler(this.OnShown);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Button AddUserButton;
        private System.Windows.Forms.Button RemoveUserButton;
        private System.Windows.Forms.ListView UserListView;
        private System.Windows.Forms.ListView PermissionListView;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button RemovePermissionButton;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button AddPermissionButton;
        private System.Windows.Forms.ImageList UserImageList;
        private System.Windows.Forms.ImageList ButtonImageIndex;
        private System.Windows.Forms.ColumnHeader columnHeader3;
    }
}