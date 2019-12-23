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
            this.savePathBrowseButton.Location = new System.Drawing.Point(605, 56);
            this.savePathBrowseButton.Name = "savePathBrowseButton";
            this.savePathBrowseButton.Size = new System.Drawing.Size(140, 42);
            this.savePathBrowseButton.TabIndex = 16;
            this.savePathBrowseButton.Text = "Browse";
            this.savePathBrowseButton.UseVisualStyleBackColor = true;
            this.savePathBrowseButton.Click += new System.EventHandler(this.BrowseClicked);
            // 
            // StoragePathTextBox
            // 
            this.StoragePathTextBox.Location = new System.Drawing.Point(81, 64);
            this.StoragePathTextBox.Name = "StoragePathTextBox";
            this.StoragePathTextBox.ReadOnly = true;
            this.StoragePathTextBox.Size = new System.Drawing.Size(505, 26);
            this.StoragePathTextBox.TabIndex = 15;
            this.StoragePathTextBox.TextChanged += new System.EventHandler(this.StateChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(19, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(300, 20);
            this.label1.TabIndex = 14;
            this.label1.Text = "Local folder to store downloaded builds in";
            // 
            // StorageMaxSizeTextBox
            // 
            this.StorageMaxSizeTextBox.Location = new System.Drawing.Point(81, 164);
            this.StorageMaxSizeTextBox.Maximum = new decimal(new int[] {
            100000000,
            0,
            0,
            0});
            this.StorageMaxSizeTextBox.Minimum = new decimal(new int[] {
            512,
            0,
            0,
            0});
            this.StorageMaxSizeTextBox.Name = "StorageMaxSizeTextBox";
            this.StorageMaxSizeTextBox.Size = new System.Drawing.Size(505, 26);
            this.StorageMaxSizeTextBox.TabIndex = 20;
            this.StorageMaxSizeTextBox.Value = new decimal(new int[] {
            512,
            0,
            0,
            0});
            this.StorageMaxSizeTextBox.ValueChanged += new System.EventHandler(this.StateChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(19, 123);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(539, 20);
            this.label2.TabIndex = 18;
            this.label2.Text = "Maximum storage space to use in MiB, old builds will be deleted to maintain.";
            // 
            // StorageMaxSizeIcon
            // 
            this.StorageMaxSizeIcon.Image = global::BuildSync.Client.Properties.Resources.ic_check_circle_2x;
            this.StorageMaxSizeIcon.Location = new System.Drawing.Point(21, 152);
            this.StorageMaxSizeIcon.Name = "StorageMaxSizeIcon";
            this.StorageMaxSizeIcon.Size = new System.Drawing.Size(48, 48);
            this.StorageMaxSizeIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.StorageMaxSizeIcon.TabIndex = 19;
            this.StorageMaxSizeIcon.TabStop = false;
            // 
            // StoragePathIcon
            // 
            this.StoragePathIcon.Image = global::BuildSync.Client.Properties.Resources.ic_check_circle_2x;
            this.StoragePathIcon.Location = new System.Drawing.Point(21, 52);
            this.StoragePathIcon.Name = "StoragePathIcon";
            this.StoragePathIcon.Size = new System.Drawing.Size(48, 48);
            this.StoragePathIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.StoragePathIcon.TabIndex = 17;
            this.StoragePathIcon.TabStop = false;
            // 
            // StorageSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.StorageMaxSizeTextBox);
            this.Controls.Add(this.StorageMaxSizeIcon);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.StoragePathIcon);
            this.Controls.Add(this.savePathBrowseButton);
            this.Controls.Add(this.StoragePathTextBox);
            this.Controls.Add(this.label1);
            this.Name = "StorageSettings";
            this.Size = new System.Drawing.Size(778, 213);
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
