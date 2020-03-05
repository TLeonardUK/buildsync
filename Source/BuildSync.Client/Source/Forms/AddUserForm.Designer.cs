namespace BuildSync.Client.Forms
{
    partial class AddUserForm
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
			System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("Test Test", 0);
			System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem("asdf", 0);
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddUserForm));
			this.nameTextBox = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.panel1 = new System.Windows.Forms.Panel();
			this.addGroupButton = new System.Windows.Forms.Button();
			this.potentialListView = new System.Windows.Forms.ListView();
			this.UserImageList = new System.Windows.Forms.ImageList(this.components);
			this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.SuspendLayout();
			// 
			// nameTextBox
			// 
			this.nameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.nameTextBox.Location = new System.Drawing.Point(11, 30);
			this.nameTextBox.Margin = new System.Windows.Forms.Padding(2);
			this.nameTextBox.Name = "nameTextBox";
			this.nameTextBox.Size = new System.Drawing.Size(359, 20);
			this.nameTextBox.TabIndex = 18;
			this.nameTextBox.TextChanged += new System.EventHandler(this.NameTextChanged);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(8, 15);
			this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(35, 13);
			this.label2.TabIndex = 17;
			this.label2.Text = "Name";
			// 
			// panel1
			// 
			this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panel1.BackColor = System.Drawing.SystemColors.ControlDark;
			this.panel1.Location = new System.Drawing.Point(10, 395);
			this.panel1.Margin = new System.Windows.Forms.Padding(2);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(360, 1);
			this.panel1.TabIndex = 16;
			// 
			// addGroupButton
			// 
			this.addGroupButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.addGroupButton.Enabled = false;
			this.addGroupButton.Location = new System.Drawing.Point(275, 403);
			this.addGroupButton.Margin = new System.Windows.Forms.Padding(2);
			this.addGroupButton.Name = "addGroupButton";
			this.addGroupButton.Size = new System.Drawing.Size(95, 29);
			this.addGroupButton.TabIndex = 15;
			this.addGroupButton.Text = "Add User";
			this.addGroupButton.UseVisualStyleBackColor = true;
			this.addGroupButton.Click += new System.EventHandler(this.OkClicked);
			// 
			// potentialListView
			// 
			this.potentialListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.potentialListView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.potentialListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
			this.potentialListView.FullRowSelect = true;
			this.potentialListView.HideSelection = false;
			this.potentialListView.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2});
			this.potentialListView.Location = new System.Drawing.Point(11, 58);
			this.potentialListView.Name = "potentialListView";
			this.potentialListView.Size = new System.Drawing.Size(359, 322);
			this.potentialListView.SmallImageList = this.UserImageList;
			this.potentialListView.TabIndex = 19;
			this.potentialListView.UseCompatibleStateImageBehavior = false;
			this.potentialListView.View = System.Windows.Forms.View.Details;
			this.potentialListView.Click += new System.EventHandler(this.OnClickUser);
			this.potentialListView.DoubleClick += new System.EventHandler(this.OnDoubleClickUser);
			// 
			// UserImageList
			// 
			this.UserImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("UserImageList.ImageStream")));
			this.UserImageList.TransparentColor = System.Drawing.Color.Transparent;
			this.UserImageList.Images.SetKeyName(0, "appbar.user.png");
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "Username";
			this.columnHeader1.Width = 350;
			// 
			// AddUserForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(382, 441);
			this.Controls.Add(this.potentialListView);
			this.Controls.Add(this.nameTextBox);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.addGroupButton);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "AddUserForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Add User";
			this.Shown += new System.EventHandler(this.OnShown);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox nameTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button addGroupButton;
        private System.Windows.Forms.ListView potentialListView;
        private System.Windows.Forms.ImageList UserImageList;
        private System.Windows.Forms.ColumnHeader columnHeader1;
    }
}