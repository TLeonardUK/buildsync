namespace BuildSync.Client.Controls.Settings
{
    partial class StorageSettings
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
            this.savePathBrowseButton = new System.Windows.Forms.Button();
            this.StoragePathTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.StorageMaxSizeTextBox = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.StorageMaxSizeIcon = new System.Windows.Forms.PictureBox();
            this.StoragePathIcon = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.StorageMaxSizeTextBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.StorageMaxSizeIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.StoragePathIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // savePathBrowseButton
            // 
            this.savePathBrowseButton.Location = new System.Drawing.Point(403, 36);
            this.savePathBrowseButton.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.savePathBrowseButton.Name = "savePathBrowseButton";
            this.savePathBrowseButton.Size = new System.Drawing.Size(93, 27);
            this.savePathBrowseButton.TabIndex = 16;
            this.savePathBrowseButton.Text = "Browse";
            this.savePathBrowseButton.UseVisualStyleBackColor = true;
            this.savePathBrowseButton.Click += new System.EventHandler(this.BrowseClicked);
            // 
            // StoragePathTextBox
            // 
            this.StoragePathTextBox.Location = new System.Drawing.Point(54, 42);
            this.StoragePathTextBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.StoragePathTextBox.Name = "StoragePathTextBox";
            this.StoragePathTextBox.ReadOnly = true;
            this.StoragePathTextBox.Size = new System.Drawing.Size(338, 20);
            this.StoragePathTextBox.TabIndex = 15;
            this.StoragePathTextBox.TextChanged += new System.EventHandler(this.StateChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 15);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(202, 13);
            this.label1.TabIndex = 14;
            this.label1.Text = "Local folder to store downloaded builds in";
            // 
            // StorageMaxSizeTextBox
            // 
            this.StorageMaxSizeTextBox.Location = new System.Drawing.Point(54, 107);
            this.StorageMaxSizeTextBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.StorageMaxSizeTextBox.Maximum = new decimal(new int[] {
            100000000,
            0,
            0,
            0});
            this.StorageMaxSizeTextBox.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.StorageMaxSizeTextBox.Name = "StorageMaxSizeTextBox";
            this.StorageMaxSizeTextBox.Size = new System.Drawing.Size(337, 20);
            this.StorageMaxSizeTextBox.TabIndex = 20;
            this.StorageMaxSizeTextBox.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.StorageMaxSizeTextBox.ValueChanged += new System.EventHandler(this.StateChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 80);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(361, 13);
            this.label2.TabIndex = 18;
            this.label2.Text = "Maximum storage space to use in GiB, old builds will be deleted to maintain.";
            // 
            // StorageMaxSizeIcon
            // 
            this.StorageMaxSizeIcon.Image = global::BuildSync.Client.Properties.Resources.ic_check_circle_2x;
            this.StorageMaxSizeIcon.Location = new System.Drawing.Point(14, 99);
            this.StorageMaxSizeIcon.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.StorageMaxSizeIcon.Name = "StorageMaxSizeIcon";
            this.StorageMaxSizeIcon.Size = new System.Drawing.Size(32, 31);
            this.StorageMaxSizeIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.StorageMaxSizeIcon.TabIndex = 19;
            this.StorageMaxSizeIcon.TabStop = false;
            // 
            // StoragePathIcon
            // 
            this.StoragePathIcon.Image = global::BuildSync.Client.Properties.Resources.ic_check_circle_2x;
            this.StoragePathIcon.Location = new System.Drawing.Point(14, 34);
            this.StoragePathIcon.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.StoragePathIcon.Name = "StoragePathIcon";
            this.StoragePathIcon.Size = new System.Drawing.Size(32, 31);
            this.StoragePathIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.StoragePathIcon.TabIndex = 17;
            this.StoragePathIcon.TabStop = false;
            // 
            // StorageSettings
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.StorageMaxSizeTextBox);
            this.Controls.Add(this.StorageMaxSizeIcon);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.StoragePathIcon);
            this.Controls.Add(this.savePathBrowseButton);
            this.Controls.Add(this.StoragePathTextBox);
            this.Controls.Add(this.label1);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "StorageSettings";
            this.Size = new System.Drawing.Size(519, 138);
            ((System.ComponentModel.ISupportInitialize)(this.StorageMaxSizeTextBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.StorageMaxSizeIcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.StoragePathIcon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox StoragePathIcon;
        private System.Windows.Forms.Button savePathBrowseButton;
        private System.Windows.Forms.TextBox StoragePathTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown StorageMaxSizeTextBox;
        private System.Windows.Forms.PictureBox StorageMaxSizeIcon;
        private System.Windows.Forms.Label label2;
    }
}
