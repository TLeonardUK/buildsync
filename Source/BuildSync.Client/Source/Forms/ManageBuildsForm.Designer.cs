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
            this.downloadFileSystemTree = new BuildSync.Client.Controls.DownloadFileSystemTree();
            this.RemoveBuildButton = new System.Windows.Forms.Button();
            this.AddBuildButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // downloadFileSystemTree
            // 
            this.downloadFileSystemTree.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.downloadFileSystemTree.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.downloadFileSystemTree.CanSelectBuildContainers = true;
            this.downloadFileSystemTree.Location = new System.Drawing.Point(-1, 0);
            this.downloadFileSystemTree.Margin = new System.Windows.Forms.Padding(1, 1, 1, 1);
            this.downloadFileSystemTree.Name = "downloadFileSystemTree";
            this.downloadFileSystemTree.SelectedPath = "";
            this.downloadFileSystemTree.Size = new System.Drawing.Size(570, 200);
            this.downloadFileSystemTree.TabIndex = 22;
            this.downloadFileSystemTree.OnSelectedNodeChanged += new System.EventHandler(this.DateStateChanged);
            // 
            // RemoveBuildButton
            // 
            this.RemoveBuildButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.RemoveBuildButton.Enabled = false;
            this.RemoveBuildButton.Image = global::BuildSync.Client.Properties.Resources.appbar_delete;
            this.RemoveBuildButton.Location = new System.Drawing.Point(44, 207);
            this.RemoveBuildButton.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.RemoveBuildButton.Name = "RemoveBuildButton";
            this.RemoveBuildButton.Size = new System.Drawing.Size(30, 29);
            this.RemoveBuildButton.TabIndex = 15;
            this.RemoveBuildButton.UseVisualStyleBackColor = true;
            this.RemoveBuildButton.Click += new System.EventHandler(this.RemoveBuildClicked);
            // 
            // AddBuildButton
            // 
            this.AddBuildButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.AddBuildButton.Image = global::BuildSync.Client.Properties.Resources.appbar_add;
            this.AddBuildButton.Location = new System.Drawing.Point(7, 207);
            this.AddBuildButton.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.AddBuildButton.Name = "AddBuildButton";
            this.AddBuildButton.Size = new System.Drawing.Size(30, 29);
            this.AddBuildButton.TabIndex = 23;
            this.AddBuildButton.UseVisualStyleBackColor = true;
            this.AddBuildButton.Click += new System.EventHandler(this.AddBuildClicked);
            // 
            // ManageBuildsForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(566, 243);
            this.Controls.Add(this.AddBuildButton);
            this.Controls.Add(this.downloadFileSystemTree);
            this.Controls.Add(this.RemoveBuildButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ManageBuildsForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Manage Builds";
            this.ResumeLayout(false);

        }

        #endregion

        private Controls.DownloadFileSystemTree downloadFileSystemTree;
        private System.Windows.Forms.Button RemoveBuildButton;
        private System.Windows.Forms.Button AddBuildButton;
    }
}