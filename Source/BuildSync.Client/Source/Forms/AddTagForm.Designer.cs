namespace BuildSync.Client.Forms
{
    partial class AddTagForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddTagForm));
            this.nameTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.addGroupButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.uniqueCheckBox = new System.Windows.Forms.CheckBox();
            this.decayLabel = new System.Windows.Forms.Label();
            this.decayTagTextBox = new BuildSync.Client.Controls.TagTextBox();
            this.colorSelector = new BuildSync.Client.Controls.ColorSelector();
            this.SuspendLayout();
            // 
            // nameTextBox
            // 
            this.nameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.nameTextBox.Location = new System.Drawing.Point(11, 33);
            this.nameTextBox.Margin = new System.Windows.Forms.Padding(2);
            this.nameTextBox.Name = "nameTextBox";
            this.nameTextBox.Size = new System.Drawing.Size(379, 20);
            this.nameTextBox.TabIndex = 18;
            this.nameTextBox.TextChanged += new System.EventHandler(this.StateChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 18);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 17;
            this.label2.Text = "Name";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.SystemColors.ControlDark;
            this.panel1.Location = new System.Drawing.Point(10, 196);
            this.panel1.Margin = new System.Windows.Forms.Padding(2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(380, 1);
            this.panel1.TabIndex = 16;
            // 
            // addGroupButton
            // 
            this.addGroupButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.addGroupButton.Enabled = false;
            this.addGroupButton.Location = new System.Drawing.Point(295, 204);
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
            this.label1.Location = new System.Drawing.Point(8, 62);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 13);
            this.label1.TabIndex = 19;
            this.label1.Text = "Color";
            // 
            // uniqueCheckBox
            // 
            this.uniqueCheckBox.AutoSize = true;
            this.uniqueCheckBox.Location = new System.Drawing.Point(11, 112);
            this.uniqueCheckBox.Name = "uniqueCheckBox";
            this.uniqueCheckBox.Size = new System.Drawing.Size(168, 17);
            this.uniqueCheckBox.TabIndex = 21;
            this.uniqueCheckBox.Text = "Unique within build container?";
            this.uniqueCheckBox.UseVisualStyleBackColor = true;
            this.uniqueCheckBox.CheckedChanged += new System.EventHandler(this.StateChanged);
            // 
            // decayLabel
            // 
            this.decayLabel.AutoSize = true;
            this.decayLabel.Location = new System.Drawing.Point(8, 135);
            this.decayLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.decayLabel.Name = "decayLabel";
            this.decayLabel.Size = new System.Drawing.Size(185, 13);
            this.decayLabel.TabIndex = 23;
            this.decayLabel.Text = "Decay into tag when new tag added?";
            // 
            // decayTagTextBox
            // 
            this.decayTagTextBox.Location = new System.Drawing.Point(10, 151);
            this.decayTagTextBox.Name = "decayTagTextBox";
            this.decayTagTextBox.Size = new System.Drawing.Size(380, 21);
            this.decayTagTextBox.TabIndex = 22;
            this.decayTagTextBox.TagIds = ((System.Collections.Generic.List<System.Guid>)(resources.GetObject("decayTagTextBox.TagIds")));
            this.decayTagTextBox.OnTagsChanged += new System.EventHandler(this.StateChanged);
            // 
            // colorSelector
            // 
            this.colorSelector.Location = new System.Drawing.Point(11, 78);
            this.colorSelector.Name = "colorSelector";
            this.colorSelector.Size = new System.Drawing.Size(379, 23);
            this.colorSelector.TabIndex = 20;
            this.colorSelector.OnColorChanged += new System.EventHandler(this.StateChanged);
            // 
            // AddTagForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(402, 242);
            this.Controls.Add(this.decayLabel);
            this.Controls.Add(this.decayTagTextBox);
            this.Controls.Add(this.uniqueCheckBox);
            this.Controls.Add(this.colorSelector);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.nameTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.addGroupButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "AddTagForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Modify Tag";
            this.Shown += new System.EventHandler(this.OnShown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox nameTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button addGroupButton;
        private System.Windows.Forms.Label label1;
        private Controls.ColorSelector colorSelector;
        private System.Windows.Forms.CheckBox uniqueCheckBox;
        private Controls.TagTextBox decayTagTextBox;
        private System.Windows.Forms.Label decayLabel;
    }
}