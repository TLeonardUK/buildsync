namespace BuildSync.Client.Forms
{
    partial class RemoteInstallForm
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
            this.deviceNameTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.startButton = new System.Windows.Forms.Button();
            this.label13 = new System.Windows.Forms.Label();
            this.locationTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.manifestLabel = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.progressLabel = new System.Windows.Forms.Label();
            this.updateTimer = new System.Windows.Forms.Timer(this.components);
            this.label3 = new System.Windows.Forms.Label();
            this.installTimeLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // deviceNameTextBox
            // 
            this.deviceNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.deviceNameTextBox.Location = new System.Drawing.Point(11, 62);
            this.deviceNameTextBox.Margin = new System.Windows.Forms.Padding(2);
            this.deviceNameTextBox.Name = "deviceNameTextBox";
            this.deviceNameTextBox.Size = new System.Drawing.Size(619, 20);
            this.deviceNameTextBox.TabIndex = 22;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 47);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(308, 13);
            this.label2.TabIndex = 21;
            this.label2.Text = "Device name or ip to install to (use commas to seperate multiple)";
            // 
            // startButton
            // 
            this.startButton.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.startButton.Location = new System.Drawing.Point(488, 168);
            this.startButton.Margin = new System.Windows.Forms.Padding(2);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(142, 34);
            this.startButton.TabIndex = 19;
            this.startButton.Text = "Start";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.OnStartClicked);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(8, 93);
            this.label13.Margin = new System.Windows.Forms.Padding(0, 0, 2, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(168, 13);
            this.label13.TabIndex = 33;
            this.label13.Text = "Location or workspace to install to";
            // 
            // locationTextBox
            // 
            this.locationTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.locationTextBox.Location = new System.Drawing.Point(11, 108);
            this.locationTextBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 8);
            this.locationTextBox.Name = "locationTextBox";
            this.locationTextBox.Size = new System.Drawing.Size(619, 20);
            this.locationTextBox.TabIndex = 34;
            this.locationTextBox.Text = "workspace0";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 17);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(116, 13);
            this.label1.TabIndex = 35;
            this.label1.Text = "Remote install of build: ";
            // 
            // manifestLabel
            // 
            this.manifestLabel.AutoSize = true;
            this.manifestLabel.BackColor = System.Drawing.SystemColors.Control;
            this.manifestLabel.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.manifestLabel.Location = new System.Drawing.Point(128, 17);
            this.manifestLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.manifestLabel.Name = "manifestLabel";
            this.manifestLabel.Size = new System.Drawing.Size(113, 13);
            this.manifestLabel.TabIndex = 36;
            this.manifestLabel.Text = "VirutalPath/Test/1231";
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.BackColor = System.Drawing.SystemColors.ControlDark;
            this.panel2.Location = new System.Drawing.Point(10, 144);
            this.panel2.Margin = new System.Windows.Forms.Padding(2);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(620, 1);
            this.panel2.TabIndex = 21;
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(10, 189);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(461, 13);
            this.progressBar.TabIndex = 37;
            // 
            // progressLabel
            // 
            this.progressLabel.Location = new System.Drawing.Point(9, 153);
            this.progressLabel.Margin = new System.Windows.Forms.Padding(0, 0, 2, 0);
            this.progressLabel.Name = "progressLabel";
            this.progressLabel.Size = new System.Drawing.Size(462, 13);
            this.progressLabel.TabIndex = 38;
            this.progressLabel.Text = "Installation Progress";
            this.progressLabel.Visible = false;
            // 
            // updateTimer
            // 
            this.updateTimer.Enabled = true;
            this.updateTimer.Tick += new System.EventHandler(this.UpdateTicked);
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(9, 170);
            this.label3.Margin = new System.Windows.Forms.Padding(0, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(90, 13);
            this.label3.TabIndex = 39;
            this.label3.Text = "Time Remaining: ";
            this.label3.Visible = false;
            // 
            // installTimeLabel
            // 
            this.installTimeLabel.Location = new System.Drawing.Point(95, 171);
            this.installTimeLabel.Margin = new System.Windows.Forms.Padding(0, 0, 2, 0);
            this.installTimeLabel.Name = "installTimeLabel";
            this.installTimeLabel.Size = new System.Drawing.Size(370, 13);
            this.installTimeLabel.TabIndex = 40;
            this.installTimeLabel.Text = "00:00:00";
            this.installTimeLabel.Visible = false;
            // 
            // RemoteInstallForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(641, 213);
            this.Controls.Add(this.installTimeLabel);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.progressLabel);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.manifestLabel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.locationTextBox);
            this.Controls.Add(this.deviceNameTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.startButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RemoteInstallForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Remote Install";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OnClosed);
            this.Shown += new System.EventHandler(this.OnShown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox deviceNameTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox locationTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label manifestLabel;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label progressLabel;
        private System.Windows.Forms.Timer updateTimer;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label installTimeLabel;
    }
}