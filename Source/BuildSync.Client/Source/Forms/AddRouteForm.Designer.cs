namespace BuildSync.Client.Forms
{
    partial class AddRouteForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddRouteForm));
            this.panel1 = new System.Windows.Forms.Panel();
            this.addGroupButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.blacklistCheckBox = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.destinationTagTextBox = new BuildSync.Client.Controls.TagTextBox();
            this.sourceTagTextBox = new BuildSync.Client.Controls.TagTextBox();
            this.bandwidthLimitTextBox = new BuildSync.Core.Controls.SizeTextBox();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.SystemColors.ControlDark;
            this.panel1.Location = new System.Drawing.Point(10, 184);
            this.panel1.Margin = new System.Windows.Forms.Padding(2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(519, 1);
            this.panel1.TabIndex = 16;
            // 
            // addGroupButton
            // 
            this.addGroupButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.addGroupButton.Enabled = false;
            this.addGroupButton.Location = new System.Drawing.Point(434, 192);
            this.addGroupButton.Margin = new System.Windows.Forms.Padding(2);
            this.addGroupButton.Name = "addGroupButton";
            this.addGroupButton.Size = new System.Drawing.Size(95, 29);
            this.addGroupButton.TabIndex = 15;
            this.addGroupButton.Text = "OK";
            this.addGroupButton.UseVisualStyleBackColor = true;
            this.addGroupButton.Click += new System.EventHandler(this.OkClicked);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(92, 13);
            this.label1.TabIndex = 18;
            this.label1.Text = "Source Client Tag";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 58);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(111, 13);
            this.label2.TabIndex = 20;
            this.label2.Text = "Destination Client Tag";
            // 
            // blacklistCheckBox
            // 
            this.blacklistCheckBox.AutoSize = true;
            this.blacklistCheckBox.Location = new System.Drawing.Point(10, 156);
            this.blacklistCheckBox.Name = "blacklistCheckBox";
            this.blacklistCheckBox.Size = new System.Drawing.Size(83, 17);
            this.blacklistCheckBox.TabIndex = 21;
            this.blacklistCheckBox.Text = "Blacklisted?";
            this.blacklistCheckBox.UseVisualStyleBackColor = true;
            this.blacklistCheckBox.CheckedChanged += new System.EventHandler(this.StateChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 105);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(150, 13);
            this.label3.TabIndex = 24;
            this.label3.Text = "Bandwidth Limit (0 is unlimited)";
            // 
            // destinationTagTextBox
            // 
            this.destinationTagTextBox.Location = new System.Drawing.Point(10, 74);
            this.destinationTagTextBox.Name = "destinationTagTextBox";
            this.destinationTagTextBox.Size = new System.Drawing.Size(519, 24);
            this.destinationTagTextBox.TabIndex = 19;
            this.destinationTagTextBox.TagIds = ((System.Collections.Generic.List<System.Guid>)(resources.GetObject("destinationTagTextBox.TagIds")));
            this.destinationTagTextBox.OnTagsChanged += new System.EventHandler(this.StateChanged);
            // 
            // sourceTagTextBox
            // 
            this.sourceTagTextBox.Location = new System.Drawing.Point(10, 28);
            this.sourceTagTextBox.Name = "sourceTagTextBox";
            this.sourceTagTextBox.Size = new System.Drawing.Size(519, 24);
            this.sourceTagTextBox.TabIndex = 17;
            this.sourceTagTextBox.TagIds = ((System.Collections.Generic.List<System.Guid>)(resources.GetObject("sourceTagTextBox.TagIds")));
            this.sourceTagTextBox.OnTagsChanged += new System.EventHandler(this.StateChanged);
            // 
            // bandwidthLimitTextBox
            // 
            this.bandwidthLimitTextBox.DisplayAsTransferRate = false;
            this.bandwidthLimitTextBox.Location = new System.Drawing.Point(10, 121);
            this.bandwidthLimitTextBox.Name = "bandwidthLimitTextBox";
            this.bandwidthLimitTextBox.Size = new System.Drawing.Size(519, 26);
            this.bandwidthLimitTextBox.TabIndex = 25;
            this.bandwidthLimitTextBox.Value = ((long)(0));
            this.bandwidthLimitTextBox.OnValueChanged += new System.EventHandler(this.StateChanged);
            // 
            // AddRouteForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(541, 230);
            this.Controls.Add(this.bandwidthLimitTextBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.blacklistCheckBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.destinationTagTextBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.sourceTagTextBox);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.addGroupButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "AddRouteForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Edit Route";
            this.Shown += new System.EventHandler(this.OnShown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button addGroupButton;
        private Controls.TagTextBox sourceTagTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private Controls.TagTextBox destinationTagTextBox;
        private System.Windows.Forms.CheckBox blacklistCheckBox;
        private System.Windows.Forms.Label label3;
        private Core.Controls.SizeTextBox bandwidthLimitTextBox;
    }
}