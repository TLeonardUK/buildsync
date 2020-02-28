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
            // 
            // MainTreeView
            // 
            this.MainTreeView.BackColor = System.Drawing.SystemColors.Window;
            this.MainTreeView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.MainTreeView.DefaultToolTipProvider = null;
            this.MainTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainTreeView.DragDropMarkColor = System.Drawing.Color.Black;
            this.MainTreeView.FullRowSelect = true;
            this.MainTreeView.LineColor = System.Drawing.SystemColors.ControlDark;
            this.MainTreeView.Location = new System.Drawing.Point(0, 0);
            this.MainTreeView.Model = null;
            this.MainTreeView.Name = "MainTreeView";
            this.MainTreeView.RowHeight = 22;
            this.MainTreeView.SelectedNode = null;
            this.MainTreeView.Size = new System.Drawing.Size(445, 243);
            this.MainTreeView.TabIndex = 1;
            this.MainTreeView.Text = "treeViewAdv1";
            this.MainTreeView.UseColumns = true;
            this.MainTreeView.SelectionChanged += new System.EventHandler(this.AfterNodeSelected);
            this.MainTreeView.Expanded += new System.EventHandler<Aga.Controls.Tree.TreeViewAdvEventArgs>(this.OnNodeExpanded);
            // 
            // DownloadFileSystemTree
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.MainTreeView);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "DownloadFileSystemTree";
            this.Size = new System.Drawing.Size(445, 243);
            this.Load += new System.EventHandler(this.OnLoaded);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ImageList TreeImageList;
        private Aga.Controls.Tree.TreeViewAdv MainTreeView;
    }
}
