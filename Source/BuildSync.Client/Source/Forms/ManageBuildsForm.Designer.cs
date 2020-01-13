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
            this.panel1 = new System.Windows.Forms.Panel();
            this.closeButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // downloadFileSystemTree
            // 
            this.downloadFileSystemTree.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.downloadFileSystemTree.CanSelectBuildContainers = true;
            this.downloadFileSystemTree.Location = new System.Drawing.Point(10, 43);
            this.downloadFileSystemTree.Margin = new System.Windows.Forms.Padding(1);
            this.downloadFileSystemTree.Name = "downloadFileSystemTree";
            this.downloadFileSystemTree.SelectedPath = "";
            this.downloadFileSystemTree.Size = new System.Drawing.Size(545, 216);
            this.downloadFileSystemTree.TabIndex = 22;
            this.downloadFileSystemTree.OnSelectedNodeChanged += new System.EventHandler(this.DateStateChanged);
            // 
            // RemoveBuildButton
            // 
            this.RemoveBuildButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.RemoveBuildButton.Enabled = false;
            this.RemoveBuildButton.ImageIndex = 1;
            this.RemoveBuildButton.ImageList = this.ButtonImageList;
            this.RemoveBuildButton.Location = new System.Drawing.Point(525, 8);
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
            this.AddBuildButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.AddBuildButton.ImageIndex = 0;
            this.AddBuildButton.ImageList = this.ButtonImageList;
            this.AddBuildButton.Location = new System.Drawing.Point(488, 8);
            this.AddBuildButton.Margin = new System.Windows.Forms.Padding(2);
            this.AddBuildButton.Name = "AddBuildButton";
            this.AddBuildButton.Size = new System.Drawing.Size(30, 29);
            this.AddBuildButton.TabIndex = 23;
            this.AddBuildButton.UseVisualStyleBackColor = true;
            this.AddBuildButton.Click += new System.EventHandler(this.AddBuildClicked);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.SystemColors.ControlDark;
            this.panel1.Location = new System.Drawing.Point(9, 273);
            this.panel1.Margin = new System.Windows.Forms.Padding(2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(546, 1);
            this.panel1.TabIndex = 25;
            // 
            // closeButton
            // 
            this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.closeButton.Location = new System.Drawing.Point(460, 281);
            this.closeButton.Margin = new System.Windows.Forms.Padding(2);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(95, 29);
            this.closeButton.TabIndex = 24;
            this.closeButton.Text = "Close";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.CloseButtonClicked);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 26;
            this.label1.Text = "Builds";
            // 
            // ManageBuildsForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(566, 321);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.AddBuildButton);
            this.Controls.Add(this.downloadFileSystemTree);
            this.Controls.Add(this.RemoveBuildButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ManageBuildsForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Build Manager";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Controls.DownloadFileSystemTree downloadFileSystemTree;
        private System.Windows.Forms.Button RemoveBuildButton;
        private System.Windows.Forms.Button AddBuildButton;
        private System.Windows.Forms.ImageList ButtonImageList;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Label label1;
    }
}