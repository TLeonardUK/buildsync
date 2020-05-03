namespace BuildSync.Client.Controls.Setup
{
    partial class StorageSetupPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StorageSetupPage));
            this.label2 = new System.Windows.Forms.Label();
            this.savePathBrowseButton = new System.Windows.Forms.Button();
            this.StoragePathTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.StorageMaxSizeIcon = new System.Windows.Forms.PictureBox();
            this.StoragePathIcon = new System.Windows.Forms.PictureBox();
            this.label3 = new System.Windows.Forms.Label();
            this.StorageMaxSizeTextBox = new BuildSync.Core.Controls.SizeTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.StorageMaxSizeIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.StoragePathIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(18, 135);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(617, 55);
            this.label2.TabIndex = 25;
            this.label2.Text = resources.GetString("label2.Text");
            // 
            // savePathBrowseButton
            // 
            this.savePathBrowseButton.Location = new System.Drawing.Point(542, 79);
            this.savePathBrowseButton.Margin = new System.Windows.Forms.Padding(2);
            this.savePathBrowseButton.Name = "savePathBrowseButton";
            this.savePathBrowseButton.Size = new System.Drawing.Size(93, 27);
            this.savePathBrowseButton.TabIndex = 23;
            this.savePathBrowseButton.Text = "Browse";
            this.savePathBrowseButton.UseVisualStyleBackColor = true;
            this.savePathBrowseButton.Click += new System.EventHandler(this.BrowseClicked);
            // 
            // StoragePathTextBox
            // 
            this.StoragePathTextBox.Location = new System.Drawing.Point(59, 83);
            this.StoragePathTextBox.Margin = new System.Windows.Forms.Padding(2);
            this.StoragePathTextBox.Name = "StoragePathTextBox";
            this.StoragePathTextBox.ReadOnly = true;
            this.StoragePathTextBox.Size = new System.Drawing.Size(469, 20);
            this.StoragePathTextBox.TabIndex = 22;
            this.StoragePathTextBox.TextChanged += new System.EventHandler(this.StateChanged);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(18, 17);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(617, 40);
            this.label1.TabIndex = 21;
            this.label1.Text = resources.GetString("label1.Text");
            // 
            // StorageMaxSizeIcon
            // 
            this.StorageMaxSizeIcon.Image = global::BuildSync.Client.Properties.Resources.ic_check_circle_2x;
            this.StorageMaxSizeIcon.Location = new System.Drawing.Point(21, 196);
            this.StorageMaxSizeIcon.Margin = new System.Windows.Forms.Padding(2);
            this.StorageMaxSizeIcon.Name = "StorageMaxSizeIcon";
            this.StorageMaxSizeIcon.Size = new System.Drawing.Size(32, 31);
            this.StorageMaxSizeIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.StorageMaxSizeIcon.TabIndex = 26;
            this.StorageMaxSizeIcon.TabStop = false;
            // 
            // StoragePathIcon
            // 
            this.StoragePathIcon.Image = global::BuildSync.Client.Properties.Resources.ic_check_circle_2x;
            this.StoragePathIcon.Location = new System.Drawing.Point(19, 75);
            this.StoragePathIcon.Margin = new System.Windows.Forms.Padding(2);
            this.StoragePathIcon.Name = "StoragePathIcon";
            this.StoragePathIcon.Size = new System.Drawing.Size(32, 31);
            this.StoragePathIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.StoragePathIcon.TabIndex = 24;
            this.StoragePathIcon.TabStop = false;
            // 
            // label3
            // 
            this.label3.ForeColor = System.Drawing.Color.Red;
            this.label3.Location = new System.Drawing.Point(18, 54);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(617, 16);
            this.label3.TabIndex = 28;
            this.label3.Text = "WARNING: Please do not delete or modify the contents of this folder manually. Fil" +
    "es in it may be used for seeding builds.\r\n";
            // 
            // StorageMaxSizeTextBox
            // 
            this.StorageMaxSizeTextBox.DisplayAsTransferRate = false;
            this.StorageMaxSizeTextBox.Location = new System.Drawing.Point(59, 201);
            this.StorageMaxSizeTextBox.Name = "StorageMaxSizeTextBox";
            this.StorageMaxSizeTextBox.Size = new System.Drawing.Size(469, 26);
            this.StorageMaxSizeTextBox.TabIndex = 29;
            this.StorageMaxSizeTextBox.Value = ((long)(0));
            this.StorageMaxSizeTextBox.Load += new System.EventHandler(this.StateChanged);
            // 
            // StorageSetupPage
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.StorageMaxSizeTextBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.StorageMaxSizeIcon);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.StoragePathIcon);
            this.Controls.Add(this.savePathBrowseButton);
            this.Controls.Add(this.StoragePathTextBox);
            this.Controls.Add(this.label1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "StorageSetupPage";
            this.Size = new System.Drawing.Size(656, 238);
            ((System.ComponentModel.ISupportInitialize)(this.StorageMaxSizeIcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.StoragePathIcon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.PictureBox StorageMaxSizeIcon;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox StoragePathIcon;
        private System.Windows.Forms.Button savePathBrowseButton;
        private System.Windows.Forms.TextBox StoragePathTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private Core.Controls.SizeTextBox StorageMaxSizeTextBox;
    }
}
