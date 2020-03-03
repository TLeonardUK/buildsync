namespace BuildSync.Client.Controls
{
    partial class DownloadFileSystemTree
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DownloadFileSystemTree));
			this.TreeImageList = new System.Windows.Forms.ImageList(this.components);
			this.MainTreeView = new Aga.Controls.Tree.TreeViewAdv();
			this.pathTextBox = new System.Windows.Forms.TextBox();
			this.refreshButton = new System.Windows.Forms.Button();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.SuspendLayout();
			// 
			// TreeImageList
			// 
			this.TreeImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("TreeImageList.ImageStream")));
			this.TreeImageList.TransparentColor = System.Drawing.Color.Transparent;
			this.TreeImageList.Images.SetKeyName(0, "appbar.box.png");
			this.TreeImageList.Images.SetKeyName(1, "appbar.folder.png");
			this.TreeImageList.Images.SetKeyName(2, "appbar.folder.open.png");
			this.TreeImageList.Images.SetKeyName(3, "appbar.database.png");
			this.TreeImageList.Images.SetKeyName(4, "appbar.refresh.png");
			// 
			// MainTreeView
			// 
			this.MainTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.MainTreeView.BackColor = System.Drawing.SystemColors.Window;
			this.MainTreeView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.MainTreeView.DefaultToolTipProvider = null;
			this.MainTreeView.DragDropMarkColor = System.Drawing.Color.Black;
			this.MainTreeView.FullRowSelect = true;
			this.MainTreeView.LineColor = System.Drawing.SystemColors.ControlDark;
			this.MainTreeView.Location = new System.Drawing.Point(0, 23);
			this.MainTreeView.Model = null;
			this.MainTreeView.Name = "MainTreeView";
			this.MainTreeView.RowHeight = 22;
			this.MainTreeView.SelectedNode = null;
			this.MainTreeView.Size = new System.Drawing.Size(445, 220);
			this.MainTreeView.TabIndex = 1;
			this.MainTreeView.Text = "treeViewAdv1";
			this.MainTreeView.UseColumns = true;
			this.MainTreeView.SelectionChanged += new System.EventHandler(this.AfterNodeSelected);
			this.MainTreeView.Expanded += new System.EventHandler<Aga.Controls.Tree.TreeViewAdvEventArgs>(this.OnNodeExpanded);
			// 
			// pathTextBox
			// 
			this.pathTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.pathTextBox.BackColor = System.Drawing.SystemColors.Control;
			this.pathTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pathTextBox.Location = new System.Drawing.Point(30, 2);
			this.pathTextBox.Name = "pathTextBox";
			this.pathTextBox.Size = new System.Drawing.Size(388, 20);
			this.pathTextBox.TabIndex = 2;
			this.pathTextBox.TextChanged += new System.EventHandler(this.PathTextChanged);
			// 
			// refreshButton
			// 
			this.refreshButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.refreshButton.ImageIndex = 4;
			this.refreshButton.ImageList = this.TreeImageList;
			this.refreshButton.Location = new System.Drawing.Point(420, 1);
			this.refreshButton.Name = "refreshButton";
			this.refreshButton.Size = new System.Drawing.Size(26, 22);
			this.refreshButton.TabIndex = 4;
			this.refreshButton.UseVisualStyleBackColor = true;
			this.refreshButton.Click += new System.EventHandler(this.RefreshClicked);
			// 
			// pictureBox1
			// 
			this.pictureBox1.BackgroundImage = global::BuildSync.Client.Properties.Resources.appbar_folder_open;
			this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.pictureBox1.Location = new System.Drawing.Point(0, -2);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(28, 28);
			this.pictureBox1.TabIndex = 3;
			this.pictureBox1.TabStop = false;
			// 
			// DownloadFileSystemTree
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.Controls.Add(this.refreshButton);
			this.Controls.Add(this.pathTextBox);
			this.Controls.Add(this.MainTreeView);
			this.Controls.Add(this.pictureBox1);
			this.Margin = new System.Windows.Forms.Padding(2);
			this.Name = "DownloadFileSystemTree";
			this.Size = new System.Drawing.Size(445, 243);
			this.Load += new System.EventHandler(this.OnLoaded);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ImageList TreeImageList;
        private Aga.Controls.Tree.TreeViewAdv MainTreeView;
        private System.Windows.Forms.TextBox pathTextBox;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button refreshButton;
    }
}
