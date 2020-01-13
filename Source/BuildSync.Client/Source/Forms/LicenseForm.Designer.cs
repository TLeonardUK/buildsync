namespace BuildSync.Client.Forms
{
    partial class LicenseForm
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.closeButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.licensedToLabel = new System.Windows.Forms.Label();
            this.seatsLabel = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.expirationLabel = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.applyLicenseButton = new System.Windows.Forms.Button();
            this.buyLicenseButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.SystemColors.ControlDark;
            this.panel1.Location = new System.Drawing.Point(9, 116);
            this.panel1.Margin = new System.Windows.Forms.Padding(2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(425, 1);
            this.panel1.TabIndex = 5;
            // 
            // closeButton
            // 
            this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.closeButton.Location = new System.Drawing.Point(339, 128);
            this.closeButton.Margin = new System.Windows.Forms.Padding(2);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(95, 29);
            this.closeButton.TabIndex = 4;
            this.closeButton.Text = "Close";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.CloseClicked);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(26, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Licensed to: ";
            // 
            // licensedToLabel
            // 
            this.licensedToLabel.AutoSize = true;
            this.licensedToLabel.Location = new System.Drawing.Point(125, 32);
            this.licensedToLabel.Name = "licensedToLabel";
            this.licensedToLabel.Size = new System.Drawing.Size(55, 13);
            this.licensedToLabel.TabIndex = 7;
            this.licensedToLabel.Text = "Retrieving";
            // 
            // seatsLabel
            // 
            this.seatsLabel.AutoSize = true;
            this.seatsLabel.Location = new System.Drawing.Point(125, 52);
            this.seatsLabel.Margin = new System.Windows.Forms.Padding(3, 0, 3, 63);
            this.seatsLabel.Name = "seatsLabel";
            this.seatsLabel.Size = new System.Drawing.Size(55, 13);
            this.seatsLabel.TabIndex = 9;
            this.seatsLabel.Text = "Retrieving";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(26, 52);
            this.label4.Margin = new System.Windows.Forms.Padding(3, 0, 3, 63);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(37, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Seats:";
            // 
            // expirationLabel
            // 
            this.expirationLabel.AutoSize = true;
            this.expirationLabel.Location = new System.Drawing.Point(125, 72);
            this.expirationLabel.Name = "expirationLabel";
            this.expirationLabel.Size = new System.Drawing.Size(55, 13);
            this.expirationLabel.TabIndex = 11;
            this.expirationLabel.Text = "Retrieving";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(26, 72);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(44, 13);
            this.label6.TabIndex = 10;
            this.label6.Text = "Expires:";
            // 
            // applyLicenseButton
            // 
            this.applyLicenseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.applyLicenseButton.Location = new System.Drawing.Point(191, 128);
            this.applyLicenseButton.Margin = new System.Windows.Forms.Padding(2);
            this.applyLicenseButton.Name = "applyLicenseButton";
            this.applyLicenseButton.Size = new System.Drawing.Size(144, 29);
            this.applyLicenseButton.TabIndex = 12;
            this.applyLicenseButton.Text = "Apply New License File";
            this.applyLicenseButton.UseVisualStyleBackColor = true;
            this.applyLicenseButton.Click += new System.EventHandler(this.ApplyClicked);
            // 
            // buyLicenseButton
            // 
            this.buyLicenseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buyLicenseButton.Location = new System.Drawing.Point(99, 128);
            this.buyLicenseButton.Margin = new System.Windows.Forms.Padding(2);
            this.buyLicenseButton.Name = "buyLicenseButton";
            this.buyLicenseButton.Size = new System.Drawing.Size(88, 29);
            this.buyLicenseButton.TabIndex = 13;
            this.buyLicenseButton.Text = "Buy License";
            this.buyLicenseButton.UseVisualStyleBackColor = true;
            this.buyLicenseButton.Click += new System.EventHandler(this.BuyClicked);
            // 
            // LicenseForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(445, 170);
            this.Controls.Add(this.buyLicenseButton);
            this.Controls.Add(this.applyLicenseButton);
            this.Controls.Add(this.expirationLabel);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.seatsLabel);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.licensedToLabel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.closeButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "LicenseForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "License Information";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OnClosed);
            this.Load += new System.EventHandler(this.OnLoad);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label licensedToLabel;
        private System.Windows.Forms.Label seatsLabel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label expirationLabel;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button applyLicenseButton;
        private System.Windows.Forms.Button buyLicenseButton;
    }
}