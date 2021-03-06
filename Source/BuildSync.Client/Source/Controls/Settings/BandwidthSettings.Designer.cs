﻿namespace BuildSync.Client.Controls.Settings
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
            this.label2 = new System.Windows.Forms.Label();
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
            this.compressDataCheckBox = new System.Windows.Forms.CheckBox();
            this.MaxUploadBandwidthBox = new BuildSync.Core.Controls.SizeTextBox();
            this.MaxDownloadBandwidthBox = new BuildSync.Core.Controls.SizeTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.MaxDownloadBandwidthIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MaxUploadBandwidthIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BandwidthTimespanStartHourBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BandwidthTimespanIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BandwidthTimespanStartMinBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BandwidthTimespanEndMinBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BandwidthTimespanEndHourBox)).BeginInit();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 15);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(206, 13);
            this.label2.TabIndex = 18;
            this.label2.Text = "Maximum upload bandwidith, 0 is unlimited";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 77);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(220, 13);
            this.label1.TabIndex = 21;
            this.label1.Text = "Maximum download bandwidith, 0 is unlimited";
            // 
            // MaxDownloadBandwidthIcon
            // 
            this.MaxDownloadBandwidthIcon.Image = global::BuildSync.Client.Properties.Resources.ic_check_circle_2x;
            this.MaxDownloadBandwidthIcon.Location = new System.Drawing.Point(14, 96);
            this.MaxDownloadBandwidthIcon.Margin = new System.Windows.Forms.Padding(2);
            this.MaxDownloadBandwidthIcon.Name = "MaxDownloadBandwidthIcon";
            this.MaxDownloadBandwidthIcon.Size = new System.Drawing.Size(32, 31);
            this.MaxDownloadBandwidthIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.MaxDownloadBandwidthIcon.TabIndex = 22;
            this.MaxDownloadBandwidthIcon.TabStop = false;
            // 
            // MaxUploadBandwidthIcon
            // 
            this.MaxUploadBandwidthIcon.Image = global::BuildSync.Client.Properties.Resources.ic_check_circle_2x;
            this.MaxUploadBandwidthIcon.Location = new System.Drawing.Point(14, 34);
            this.MaxUploadBandwidthIcon.Margin = new System.Windows.Forms.Padding(2);
            this.MaxUploadBandwidthIcon.Name = "MaxUploadBandwidthIcon";
            this.MaxUploadBandwidthIcon.Size = new System.Drawing.Size(32, 31);
            this.MaxUploadBandwidthIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.MaxUploadBandwidthIcon.TabIndex = 19;
            this.MaxUploadBandwidthIcon.TabStop = false;
            // 
            // BandwidthTimespanStartHourBox
            // 
            this.BandwidthTimespanStartHourBox.Location = new System.Drawing.Point(54, 166);
            this.BandwidthTimespanStartHourBox.Margin = new System.Windows.Forms.Padding(2);
            this.BandwidthTimespanStartHourBox.Maximum = new decimal(new int[] {
            23,
            0,
            0,
            0});
            this.BandwidthTimespanStartHourBox.Name = "BandwidthTimespanStartHourBox";
            this.BandwidthTimespanStartHourBox.Size = new System.Drawing.Size(33, 20);
            this.BandwidthTimespanStartHourBox.TabIndex = 26;
            this.BandwidthTimespanStartHourBox.ValueChanged += new System.EventHandler(this.StateChanged);
            // 
            // BandwidthTimespanIcon
            // 
            this.BandwidthTimespanIcon.Image = global::BuildSync.Client.Properties.Resources.ic_check_circle_2x;
            this.BandwidthTimespanIcon.Location = new System.Drawing.Point(14, 159);
            this.BandwidthTimespanIcon.Margin = new System.Windows.Forms.Padding(2);
            this.BandwidthTimespanIcon.Name = "BandwidthTimespanIcon";
            this.BandwidthTimespanIcon.Size = new System.Drawing.Size(32, 31);
            this.BandwidthTimespanIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.BandwidthTimespanIcon.TabIndex = 25;
            this.BandwidthTimespanIcon.TabStop = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 140);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(361, 13);
            this.label3.TabIndex = 24;
            this.label3.Text = "Time span during which downloads can run, 0 allows downloads at anytime";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(92, 166);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(10, 13);
            this.label4.TabIndex = 27;
            this.label4.Text = ":";
            // 
            // BandwidthTimespanStartMinBox
            // 
            this.BandwidthTimespanStartMinBox.Location = new System.Drawing.Point(105, 166);
            this.BandwidthTimespanStartMinBox.Margin = new System.Windows.Forms.Padding(2);
            this.BandwidthTimespanStartMinBox.Maximum = new decimal(new int[] {
            59,
            0,
            0,
            0});
            this.BandwidthTimespanStartMinBox.Name = "BandwidthTimespanStartMinBox";
            this.BandwidthTimespanStartMinBox.Size = new System.Drawing.Size(33, 20);
            this.BandwidthTimespanStartMinBox.TabIndex = 28;
            this.BandwidthTimespanStartMinBox.ValueChanged += new System.EventHandler(this.StateChanged);
            // 
            // BandwidthTimespanEndMinBox
            // 
            this.BandwidthTimespanEndMinBox.Location = new System.Drawing.Point(263, 166);
            this.BandwidthTimespanEndMinBox.Margin = new System.Windows.Forms.Padding(2);
            this.BandwidthTimespanEndMinBox.Maximum = new decimal(new int[] {
            59,
            0,
            0,
            0});
            this.BandwidthTimespanEndMinBox.Name = "BandwidthTimespanEndMinBox";
            this.BandwidthTimespanEndMinBox.Size = new System.Drawing.Size(33, 20);
            this.BandwidthTimespanEndMinBox.TabIndex = 31;
            this.BandwidthTimespanEndMinBox.ValueChanged += new System.EventHandler(this.StateChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(250, 166);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(10, 13);
            this.label5.TabIndex = 30;
            this.label5.Text = ":";
            // 
            // BandwidthTimespanEndHourBox
            // 
            this.BandwidthTimespanEndHourBox.Location = new System.Drawing.Point(212, 166);
            this.BandwidthTimespanEndHourBox.Margin = new System.Windows.Forms.Padding(2);
            this.BandwidthTimespanEndHourBox.Maximum = new decimal(new int[] {
            23,
            0,
            0,
            0});
            this.BandwidthTimespanEndHourBox.Name = "BandwidthTimespanEndHourBox";
            this.BandwidthTimespanEndHourBox.Size = new System.Drawing.Size(33, 20);
            this.BandwidthTimespanEndHourBox.TabIndex = 29;
            this.BandwidthTimespanEndHourBox.ValueChanged += new System.EventHandler(this.StateChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(167, 168);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(16, 13);
            this.label6.TabIndex = 32;
            this.label6.Text = "to";
            // 
            // compressDataCheckBox
            // 
            this.compressDataCheckBox.AutoSize = true;
            this.compressDataCheckBox.Location = new System.Drawing.Point(14, 218);
            this.compressDataCheckBox.Margin = new System.Windows.Forms.Padding(2);
            this.compressDataCheckBox.Name = "compressDataCheckBox";
            this.compressDataCheckBox.Size = new System.Drawing.Size(429, 17);
            this.compressDataCheckBox.TabIndex = 43;
            this.compressDataCheckBox.Text = "Compress data during transfer (lowers bandwidth usage, but causes high cpu usage)" +
    "?";
            this.compressDataCheckBox.UseVisualStyleBackColor = true;
            this.compressDataCheckBox.CheckedChanged += new System.EventHandler(this.StateChanged);
            // 
            // MaxUploadBandwidthBox
            // 
            this.MaxUploadBandwidthBox.DisplayAsTransferRate = true;
            this.MaxUploadBandwidthBox.Location = new System.Drawing.Point(51, 38);
            this.MaxUploadBandwidthBox.Name = "MaxUploadBandwidthBox";
            this.MaxUploadBandwidthBox.Size = new System.Drawing.Size(340, 27);
            this.MaxUploadBandwidthBox.TabIndex = 52;
            this.MaxUploadBandwidthBox.Value = ((long)(0));
            this.MaxUploadBandwidthBox.OnValueChanged += new System.EventHandler(this.StateChanged);
            // 
            // MaxDownloadBandwidthBox
            // 
            this.MaxDownloadBandwidthBox.DisplayAsTransferRate = true;
            this.MaxDownloadBandwidthBox.Location = new System.Drawing.Point(51, 100);
            this.MaxDownloadBandwidthBox.Name = "MaxDownloadBandwidthBox";
            this.MaxDownloadBandwidthBox.Size = new System.Drawing.Size(340, 27);
            this.MaxDownloadBandwidthBox.TabIndex = 53;
            this.MaxDownloadBandwidthBox.Value = ((long)(0));
            this.MaxDownloadBandwidthBox.OnValueChanged += new System.EventHandler(this.StateChanged);
            // 
            // BandwidthSettings
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.MaxDownloadBandwidthBox);
            this.Controls.Add(this.MaxUploadBandwidthBox);
            this.Controls.Add(this.compressDataCheckBox);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.BandwidthTimespanEndMinBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.BandwidthTimespanEndHourBox);
            this.Controls.Add(this.BandwidthTimespanStartMinBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.BandwidthTimespanStartHourBox);
            this.Controls.Add(this.BandwidthTimespanIcon);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.MaxDownloadBandwidthIcon);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.MaxUploadBandwidthIcon);
            this.Controls.Add(this.label2);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "BandwidthSettings";
            this.Size = new System.Drawing.Size(519, 248);
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
        private System.Windows.Forms.PictureBox MaxUploadBandwidthIcon;
        private System.Windows.Forms.Label label2;
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
        private System.Windows.Forms.CheckBox compressDataCheckBox;
        private Core.Controls.SizeTextBox MaxUploadBandwidthBox;
        private Core.Controls.SizeTextBox MaxDownloadBandwidthBox;
    }
}
