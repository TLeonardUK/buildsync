namespace BuildSync.Client.Forms
{
    partial class ManageBuildsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ManageBuildsForm));
            this.downloadFileSystemTree = new BuildSync.Client.Controls.DownloadFileSystemTree();
            this.RemoveBuildButton = new System.Windows.Forms.Button();
            this.ButtonImageList = new System.Windows.Forms.ImageList(this.components);
            this.AddBuildButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // downloadFileSystemTree
            // 
            this.downloadFileSystemTree.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.downloadFileSystemTree.CanSelectBuildContainers = true;
            this.downloadFileSystemTree.Location = new System.Drawing.Point(6, 38);
            this.downloadFileSystemTree.Margin = new System.Windows.Forms.Padding(1);
            this.downloadFileSystemTree.Name = "downloadFileSystemTree";
            this.downloadFileSystemTree.SelectedPath = "";
            this.downloadFileSystemTree.ShowInternal = true;
            this.downloadFileSystemTree.Size = new System.Drawing.Size(344, 417);
            this.downloadFileSystemTree.TabIndex = 22;
            this.downloadFileSystemTree.OnSelectedNodeChanged += new System.EventHandler(this.DateStateChanged);
            // 
            // RemoveBuildButton
            // 
            this.RemoveBuildButton.Enabled = false;
            this.RemoveBuildButton.ImageIndex = 1;
            this.RemoveBuildButton.ImageList = this.ButtonImageList;
            this.RemoveBuildButton.Location = new System.Drawing.Point(42, 5);
            this.RemoveBuildButton.Margin = new System.Windows.Forms.Padding(2);
            this.RemoveBuildButton.Name = "RemoveBuildButton";
            this.RemoveBuildButton.Size = new System.Drawing.Size(30, 29);
            this.RemoveBuildButton.TabIndex = 15;
            this.RemoveBuildButton.UseVisualStyleBackColor = true;
            this.RemoveBuildButton.Click += new System.EventHandler(this.RemoveBuildClicked);
            // 
            // ButtonImageList
            // 
            this.ButtonImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ButtonImageList.ImageStream")));
            this.ButtonImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.ButtonImageList.Images.SetKeyName(0, "appbar.add.png");
            this.ButtonImageList.Images.SetKeyName(1, "appbar.delete.png");
            // 
            // AddBuildButton
            // 
            this.AddBuildButton.ImageIndex = 0;
            this.AddBuildButton.ImageList = this.ButtonImageList;
            this.AddBuildButton.Location = new System.Drawing.Point(5, 5);
            this.AddBuildButton.Margin = new System.Windows.Forms.Padding(2);
            this.AddBuildButton.Name = "AddBuildButton";
            this.AddBuildButton.Size = new System.Drawing.Size(30, 29);
            this.AddBuildButton.TabIndex = 23;
            this.AddBuildButton.UseVisualStyleBackColor = true;
            this.AddBuildButton.Click += new System.EventHandler(this.AddBuildClicked);
            // 
            // ManageBuildsForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(355, 460);
            this.Controls.Add(this.AddBuildButton);
            this.Controls.Add(this.downloadFileSystemTree);
            this.Controls.Add(this.RemoveBuildButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ManageBuildsForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Build Manager";
            this.ResumeLayout(false);

        }

        #endregion

        private Controls.DownloadFileSystemTree downloadFileSystemTree;
        private System.Windows.Forms.Button RemoveBuildButton;
        private System.Windows.Forms.Button AddBuildButton;
        private System.Windows.Forms.ImageList ButtonImageList;
    }
}