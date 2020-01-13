namespace BuildSync.Client.Controls.Settings
{
    partial class ScmSettings
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
            System.Windows.Forms.ListViewGroup listViewGroup1 = new System.Windows.Forms.ListViewGroup("Perforce", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup2 = new System.Windows.Forms.ListViewGroup("Git", System.Windows.Forms.HorizontalAlignment.Left);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScmSettings));
            this.workspaceList = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label1 = new System.Windows.Forms.Label();
            this.ButtonImageIndex = new System.Windows.Forms.ImageList(this.components);
            this.AddServerButton = new System.Windows.Forms.Button();
            this.RemoveServerButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // workspaceList
            // 
            this.workspaceList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.workspaceList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4});
            this.workspaceList.FullRowSelect = true;
            listViewGroup1.Header = "Perforce";
            listViewGroup1.Name = "Perforce";
            listViewGroup2.Header = "Git";
            listViewGroup2.Name = "Git";
            this.workspaceList.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup1,
            listViewGroup2});
            this.workspaceList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.workspaceList.HideSelection = false;
            this.workspaceList.Location = new System.Drawing.Point(3, 42);
            this.workspaceList.Name = "workspaceList";
            this.workspaceList.Size = new System.Drawing.Size(510, 191);
            this.workspaceList.TabIndex = 0;
            this.workspaceList.UseCompatibleStateImageBehavior = false;
            this.workspaceList.View = System.Windows.Forms.View.Details;
            this.workspaceList.SelectedIndexChanged += new System.EventHandler(this.WorkspaceSelectionChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Server";
            this.columnHeader1.Width = 120;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Username";
            this.columnHeader2.Width = 80;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Password";
            this.columnHeader3.Width = 80;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Root";
            this.columnHeader4.Width = 220;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(140, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Source Control Workspaces";
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
            // AddServerButton
            // 
            this.AddServerButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.AddServerButton.ImageIndex = 0;
            this.AddServerButton.ImageList = this.ButtonImageIndex;
            this.AddServerButton.Location = new System.Drawing.Point(448, 8);
            this.AddServerButton.Margin = new System.Windows.Forms.Padding(2);
            this.AddServerButton.Name = "AddServerButton";
            this.AddServerButton.Size = new System.Drawing.Size(30, 29);
            this.AddServerButton.TabIndex = 47;
            this.AddServerButton.UseVisualStyleBackColor = true;
            this.AddServerButton.Click += new System.EventHandler(this.AddClicked);
            // 
            // RemoveServerButton
            // 
            this.RemoveServerButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.RemoveServerButton.ImageIndex = 1;
            this.RemoveServerButton.ImageList = this.ButtonImageIndex;
            this.RemoveServerButton.Location = new System.Drawing.Point(483, 8);
            this.RemoveServerButton.Margin = new System.Windows.Forms.Padding(2);
            this.RemoveServerButton.Name = "RemoveServerButton";
            this.RemoveServerButton.Size = new System.Drawing.Size(30, 29);
            this.RemoveServerButton.TabIndex = 46;
            this.RemoveServerButton.UseVisualStyleBackColor = true;
            this.RemoveServerButton.Click += new System.EventHandler(this.RemoveClicked);
            // 
            // ScmSettings
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.AddServerButton);
            this.Controls.Add(this.RemoveServerButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.workspaceList);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "ScmSettings";
            this.Size = new System.Drawing.Size(519, 236);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView workspaceList;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button AddServerButton;
        private System.Windows.Forms.Button RemoveServerButton;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ImageList ButtonImageIndex;
    }
}
