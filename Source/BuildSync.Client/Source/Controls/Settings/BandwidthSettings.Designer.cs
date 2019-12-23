namespace BuildSync.Client.Controls.Settings
{
    partial class BandwidthSettings
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
            this.MaxUploadBandwidthBox = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.MaxDownloadBandwidthBox = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.MaxDownloadBandwidthIcon = new System.Windows.Forms.PictureBox();
            this.MaxUploadBandwidthIcon = new System.Windows.Forms.PictureBox();
            this.BandwidthTimespanStartHourBox = new System.Windows.Forms.NumericUpDown();
            this.BandwidthTimespanIcon = new System.Windows.Forms.PictureBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.BandwidthTimespanStartMinBox = new System.Windows.Forms.NumericUpDown();
            this.BandwidthTimespanEndMinBox = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.BandwidthTimespanEndHourBox = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.MaxUploadBandwidthBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MaxDownloadBandwidthBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MaxDownloadBandwidthIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MaxUploadBandwidthIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BandwidthTimespanStartHourBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BandwidthTimespanIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BandwidthTimespanStartMinBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BandwidthTimespanEndMinBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BandwidthTimespanEndHourBox)).BeginInit();
            this.SuspendLayout();
            // 
            // MaxUploadBandwidthBox
            // 
            this.MaxUploadBandwidthBox.Location = new System.Drawing.Point(81, 64);
            this.MaxUploadBandwidthBox.Maximum = new decimal(new int[] {
            1073741824,
            0,
            0,
            0});
            this.MaxUploadBandwidthBox.Name = "MaxUploadBandwidthBox";
            this.MaxUploadBandwidthBox.Size = new System.Drawing.Size(505, 26);
            this.MaxUploadBandwidthBox.TabIndex = 20;
            this.MaxUploadBandwidthBox.ValueChanged += new System.EventHandler(this.StateChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(19, 23);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(350, 20);
            this.label2.TabIndex = 18;
            this.label2.Text = "Maximum upload bandwidith (kb/s), 0 is unlimited";
            // 
            // MaxDownloadBandwidthBox
            // 
            this.MaxDownloadBandwidthBox.Location = new System.Drawing.Point(81, 160);
            this.MaxDownloadBandwidthBox.Maximum = new decimal(new int[] {
            1073741824,
            0,
            0,
            0});
            this.MaxDownloadBandwidthBox.Name = "MaxDownloadBandwidthBox";
            this.MaxDownloadBandwidthBox.Size = new System.Drawing.Size(505, 26);
            this.MaxDownloadBandwidthBox.TabIndex = 23;
            this.MaxDownloadBandwidthBox.ValueChanged += new System.EventHandler(this.StateChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(19, 119);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(374, 20);
            this.label1.TabIndex = 21;
            this.label1.Text = "Maximum download bandwidith  (kb/s), 0 is unlimited";
            // 
            // MaxDownloadBandwidthIcon
            // 
            this.MaxDownloadBandwidthIcon.Image = global::BuildSync.Client.Properties.Resources.ic_check_circle_2x;
            this.MaxDownloadBandwidthIcon.Location = new System.Drawing.Point(21, 148);
            this.MaxDownloadBandwidthIcon.Name = "MaxDownloadBandwidthIcon";
            this.MaxDownloadBandwidthIcon.Size = new System.Drawing.Size(48, 48);
            this.MaxDownloadBandwidthIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.MaxDownloadBandwidthIcon.TabIndex = 22;
            this.MaxDownloadBandwidthIcon.TabStop = false;
            // 
            // MaxUploadBandwidthIcon
            // 
            this.MaxUploadBandwidthIcon.Image = global::BuildSync.Client.Properties.Resources.ic_check_circle_2x;
            this.MaxUploadBandwidthIcon.Location = new System.Drawing.Point(21, 52);
            this.MaxUploadBandwidthIcon.Name = "MaxUploadBandwidthIcon";
            this.MaxUploadBandwidthIcon.Size = new System.Drawing.Size(48, 48);
            this.MaxUploadBandwidthIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.MaxUploadBandwidthIcon.TabIndex = 19;
            this.MaxUploadBandwidthIcon.TabStop = false;
            // 
            // BandwidthTimespanStartHourBox
            // 
            this.BandwidthTimespanStartHourBox.Location = new System.Drawing.Point(81, 256);
            this.BandwidthTimespanStartHourBox.Maximum = new decimal(new int[] {
            23,
            0,
            0,
            0});
            this.BandwidthTimespanStartHourBox.Name = "BandwidthTimespanStartHourBox";
            this.BandwidthTimespanStartHourBox.Size = new System.Drawing.Size(50, 26);
            this.BandwidthTimespanStartHourBox.TabIndex = 26;
            this.BandwidthTimespanStartHourBox.ValueChanged += new System.EventHandler(this.StateChanged);
            // 
            // BandwidthTimespanIcon
            // 
            this.BandwidthTimespanIcon.Image = global::BuildSync.Client.Properties.Resources.ic_check_circle_2x;
            this.BandwidthTimespanIcon.Location = new System.Drawing.Point(21, 244);
            this.BandwidthTimespanIcon.Name = "BandwidthTimespanIcon";
            this.BandwidthTimespanIcon.Size = new System.Drawing.Size(48, 48);
            this.BandwidthTimespanIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.BandwidthTimespanIcon.TabIndex = 25;
            this.BandwidthTimespanIcon.TabStop = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(19, 215);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(532, 20);
            this.label3.TabIndex = 24;
            this.label3.Text = "Time span during which downloads can run, 0 allows downloads at anytime";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(138, 256);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(13, 20);
            this.label4.TabIndex = 27;
            this.label4.Text = ":";
            // 
            // BandwidthTimespanStartMinBox
            // 
            this.BandwidthTimespanStartMinBox.Location = new System.Drawing.Point(157, 256);
            this.BandwidthTimespanStartMinBox.Maximum = new decimal(new int[] {
            59,
            0,
            0,
            0});
            this.BandwidthTimespanStartMinBox.Name = "BandwidthTimespanStartMinBox";
            this.BandwidthTimespanStartMinBox.Size = new System.Drawing.Size(50, 26);
            this.BandwidthTimespanStartMinBox.TabIndex = 28;
            this.BandwidthTimespanStartMinBox.ValueChanged += new System.EventHandler(this.StateChanged);
            // 
            // BandwidthTimespanEndMinBox
            // 
            this.BandwidthTimespanEndMinBox.Location = new System.Drawing.Point(394, 256);
            this.BandwidthTimespanEndMinBox.Maximum = new decimal(new int[] {
            59,
            0,
            0,
            0});
            this.BandwidthTimespanEndMinBox.Name = "BandwidthTimespanEndMinBox";
            this.BandwidthTimespanEndMinBox.Size = new System.Drawing.Size(50, 26);
            this.BandwidthTimespanEndMinBox.TabIndex = 31;
            this.BandwidthTimespanEndMinBox.ValueChanged += new System.EventHandler(this.StateChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(375, 256);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(13, 20);
            this.label5.TabIndex = 30;
            this.label5.Text = ":";
            // 
            // BandwidthTimespanEndHourBox
            // 
            this.BandwidthTimespanEndHourBox.Location = new System.Drawing.Point(318, 256);
            this.BandwidthTimespanEndHourBox.Maximum = new decimal(new int[] {
            23,
            0,
            0,
            0});
            this.BandwidthTimespanEndHourBox.Name = "BandwidthTimespanEndHourBox";
            this.BandwidthTimespanEndHourBox.Size = new System.Drawing.Size(50, 26);
            this.BandwidthTimespanEndHourBox.TabIndex = 29;
            this.BandwidthTimespanEndHourBox.ValueChanged += new System.EventHandler(this.StateChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(251, 258);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(23, 20);
            this.label6.TabIndex = 32;
            this.label6.Text = "to";
            // 
            // BandwidthSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label6);
            this.Controls.Add(this.BandwidthTimespanEndMinBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.BandwidthTimespanEndHourBox);
            this.Controls.Add(this.BandwidthTimespanStartMinBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.BandwidthTimespanStartHourBox);
            this.Controls.Add(this.BandwidthTimespanIcon);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.MaxDownloadBandwidthBox);
            this.Controls.Add(this.MaxDownloadBandwidthIcon);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.MaxUploadBandwidthBox);
            this.Controls.Add(this.MaxUploadBandwidthIcon);
            this.Controls.Add(this.label2);
            this.Name = "BandwidthSettings";
            this.Size = new System.Drawing.Size(778, 315);
            ((System.ComponentModel.ISupportInitialize)(this.MaxUploadBandwidthBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MaxDownloadBandwidthBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MaxDownloadBandwidthIcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MaxUploadBandwidthIcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BandwidthTimespanStartHourBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BandwidthTimespanIcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BandwidthTimespanStartMinBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BandwidthTimespanEndMinBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BandwidthTimespanEndHourBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown MaxUploadBandwidthBox;
        private System.Windows.Forms.PictureBox MaxUploadBandwidthIcon;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown MaxDownloadBandwidthBox;
        private System.Windows.Forms.PictureBox MaxDownloadBandwidthIcon;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown BandwidthTimespanStartHourBox;
        private System.Windows.Forms.PictureBox BandwidthTimespanIcon;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown BandwidthTimespanStartMinBox;
        private System.Windows.Forms.NumericUpDown BandwidthTimespanEndMinBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown BandwidthTimespanEndHourBox;
        private System.Windows.Forms.Label label6;
    }
}
