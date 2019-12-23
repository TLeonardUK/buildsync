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
            this.MainTreeView = new System.Windows.Forms.TreeView();
            this.TreeImageList = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // MainTreeView
            // 
            this.MainTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainTreeView.FullRowSelect = true;
            this.MainTreeView.HideSelection = false;
            this.MainTreeView.HotTracking = true;
            this.MainTreeView.ImageIndex = 0;
            this.MainTreeView.ImageList = this.TreeImageList;
            this.MainTreeView.Indent = 10;
            this.MainTreeView.Location = new System.Drawing.Point(0, 0);
            this.MainTreeView.Name = "MainTreeView";
            this.MainTreeView.SelectedImageIndex = 0;
            this.MainTreeView.Size = new System.Drawing.Size(150, 150);
            this.MainTreeView.TabIndex = 0;
            this.MainTreeView.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.OnNodeExpanded);
            this.MainTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.AfterNodeSelected);
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
            // DownloadFileSystemTree
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.MainTreeView);
            this.Name = "DownloadFileSystemTree";
            this.Load += new System.EventHandler(this.OnLoaded);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView MainTreeView;
        private System.Windows.Forms.ImageList TreeImageList;
    }
}
