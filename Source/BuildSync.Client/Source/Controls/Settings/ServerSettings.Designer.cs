namespace BuildSync.Client.Controls.Settings
{
    partial class ServerSettings
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
            this.ServerHostnameTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.savePathBrowseButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.ServerPortTextBox = new System.Windows.Forms.NumericUpDown();
            this.ServerPortIcon = new System.Windows.Forms.PictureBox();
            this.ServerHostnameIcon = new System.Windows.Forms.PictureBox();
            this.PeerPortRangeStartBox = new System.Windows.Forms.NumericUpDown();
            this.PeerPortRangeIcon = new System.Windows.Forms.PictureBox();
            this.label3 = new System.Windows.Forms.Label();
            this.PeerPortRangeEndBox = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.ServerPortTextBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ServerPortIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ServerHostnameIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PeerPortRangeStartBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PeerPortRangeIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PeerPortRangeEndBox)).BeginInit();
            this.SuspendLayout();
            // 
            // ServerHostnameTextBox
            // 
            this.ServerHostnameTextBox.Location = new System.Drawing.Point(81, 64);
            this.ServerHostnameTextBox.Name = "ServerHostnameTextBox";
            this.ServerHostnameTextBox.Size = new System.Drawing.Size(505, 26);
            this.ServerHostnameTextBox.TabIndex = 11;
            this.ServerHostnameTextBox.TextChanged += new System.EventHandler(this.StateChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(19, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(229, 20);
            this.label1.TabIndex = 10;
            this.label1.Text = "Hostname of distribution server";
            // 
            // savePathBrowseButton
            // 
            this.savePathBrowseButton.Enabled = false;
            this.savePathBrowseButton.Location = new System.Drawing.Point(605, 56);
            this.savePathBrowseButton.Name = "savePathBrowseButton";
            this.savePathBrowseButton.Size = new System.Drawing.Size(140, 42);
            this.savePathBrowseButton.TabIndex = 12;
            this.savePathBrowseButton.Text = "Search Network";
            this.savePathBrowseButton.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(19, 119);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(388, 20);
            this.label2.TabIndex = 14;
            this.label2.Text = "Network port to communication with distribution server";
            // 
            // ServerPortTextBox
            // 
            this.ServerPortTextBox.Location = new System.Drawing.Point(81, 160);
            this.ServerPortTextBox.Maximum = new decimal(new int[] {
            65534,
            0,
            0,
            0});
            this.ServerPortTextBox.Minimum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.ServerPortTextBox.Name = "ServerPortTextBox";
            this.ServerPortTextBox.Size = new System.Drawing.Size(505, 26);
            this.ServerPortTextBox.TabIndex = 17;
            this.ServerPortTextBox.Value = new decimal(new int[] {
            65534,
            0,
            0,
            0});
            this.ServerPortTextBox.ValueChanged += new System.EventHandler(this.StateChanged);
            // 
            // ServerPortIcon
            // 
            this.ServerPortIcon.Image = global::BuildSync.Client.Properties.Resources.ic_check_circle_2x;
            this.ServerPortIcon.Location = new System.Drawing.Point(21, 148);
            this.ServerPortIcon.Name = "ServerPortIcon";
            this.ServerPortIcon.Size = new System.Drawing.Size(48, 48);
            this.ServerPortIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.ServerPortIcon.TabIndex = 16;
            this.ServerPortIcon.TabStop = false;
            // 
            // ServerHostnameIcon
            // 
            this.ServerHostnameIcon.Image = global::BuildSync.Client.Properties.Resources.ic_check_circle_2x;
            this.ServerHostnameIcon.Location = new System.Drawing.Point(21, 52);
            this.ServerHostnameIcon.Name = "ServerHostnameIcon";
            this.ServerHostnameIcon.Size = new System.Drawing.Size(48, 48);
            this.ServerHostnameIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.ServerHostnameIcon.TabIndex = 13;
            this.ServerHostnameIcon.TabStop = false;
            // 
            // PeerPortRangeStartBox
            // 
            this.PeerPortRangeStartBox.Location = new System.Drawing.Point(81, 254);
            this.PeerPortRangeStartBox.Maximum = new decimal(new int[] {
            65534,
            0,
            0,
            0});
            this.PeerPortRangeStartBox.Minimum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.PeerPortRangeStartBox.Name = "PeerPortRangeStartBox";
            this.PeerPortRangeStartBox.Size = new System.Drawing.Size(106, 26);
            this.PeerPortRangeStartBox.TabIndex = 20;
            this.PeerPortRangeStartBox.Value = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.PeerPortRangeStartBox.ValueChanged += new System.EventHandler(this.StateChanged);
            // 
            // PeerPortRangeIcon
            // 
            this.PeerPortRangeIcon.Image = global::BuildSync.Client.Properties.Resources.ic_check_circle_2x;
            this.PeerPortRangeIcon.Location = new System.Drawing.Point(21, 242);
            this.PeerPortRangeIcon.Name = "PeerPortRangeIcon";
            this.PeerPortRangeIcon.Size = new System.Drawing.Size(48, 48);
            this.PeerPortRangeIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.PeerPortRangeIcon.TabIndex = 19;
            this.PeerPortRangeIcon.TabStop = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(19, 213);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(298, 20);
            this.label3.TabIndex = 18;
            this.label3.Text = "Port range to use for connecting to peers";
            // 
            // PeerPortRangeEndBox
            // 
            this.PeerPortRangeEndBox.Location = new System.Drawing.Point(224, 254);
            this.PeerPortRangeEndBox.Maximum = new decimal(new int[] {
            65534,
            0,
            0,
            0});
            this.PeerPortRangeEndBox.Minimum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.PeerPortRangeEndBox.Name = "PeerPortRangeEndBox";
            this.PeerPortRangeEndBox.Size = new System.Drawing.Size(106, 26);
            this.PeerPortRangeEndBox.TabIndex = 21;
            this.PeerPortRangeEndBox.Value = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.PeerPortRangeEndBox.ValueChanged += new System.EventHandler(this.StateChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(193, 256);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(23, 20);
            this.label6.TabIndex = 33;
            this.label6.Text = "to";
            // 
            // ServerSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label6);
            this.Controls.Add(this.PeerPortRangeEndBox);
            this.Controls.Add(this.PeerPortRangeStartBox);
            this.Controls.Add(this.PeerPortRangeIcon);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.ServerPortTextBox);
            this.Controls.Add(this.ServerPortIcon);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.ServerHostnameIcon);
            this.Controls.Add(this.savePathBrowseButton);
            this.Controls.Add(this.ServerHostnameTextBox);
            this.Controls.Add(this.label1);
            this.Name = "ServerSettings";
            this.Size = new System.Drawing.Size(778, 312);
            ((System.ComponentModel.ISupportInitialize)(this.ServerPortTextBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ServerPortIcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ServerHostnameIcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PeerPortRangeStartBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PeerPortRangeIcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PeerPortRangeEndBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox ServerHostnameIcon;
        private System.Windows.Forms.TextBox ServerHostnameTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button savePathBrowseButton;
        private System.Windows.Forms.PictureBox ServerPortIcon;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown ServerPortTextBox;
        private System.Windows.Forms.NumericUpDown PeerPortRangeStartBox;
        private System.Windows.Forms.PictureBox PeerPortRangeIcon;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown PeerPortRangeEndBox;
        private System.Windows.Forms.Label label6;
    }
}
