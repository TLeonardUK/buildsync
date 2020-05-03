namespace BuildSync.Core.Controls
{
    partial class SizeTextBox
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
            this.valueTextBox = new System.Windows.Forms.NumericUpDown();
            this.postfixComboBox = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.valueTextBox)).BeginInit();
            this.SuspendLayout();
            // 
            // valueTextBox
            // 
            this.valueTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.valueTextBox.Location = new System.Drawing.Point(0, 2);
            this.valueTextBox.Name = "valueTextBox";
            this.valueTextBox.Size = new System.Drawing.Size(243, 20);
            this.valueTextBox.TabIndex = 0;
            this.valueTextBox.ValueChanged += new System.EventHandler(this.ValueChanged);
            // 
            // postfixComboBox
            // 
            this.postfixComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.postfixComboBox.FormattingEnabled = true;
            this.postfixComboBox.Location = new System.Drawing.Point(249, 1);
            this.postfixComboBox.Name = "postfixComboBox";
            this.postfixComboBox.Size = new System.Drawing.Size(60, 21);
            this.postfixComboBox.TabIndex = 1;
            this.postfixComboBox.SelectedIndexChanged += new System.EventHandler(this.PostfixChanged);
            // 
            // SizeTextBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.postfixComboBox);
            this.Controls.Add(this.valueTextBox);
            this.Name = "SizeTextBox";
            this.Size = new System.Drawing.Size(309, 26);
            ((System.ComponentModel.ISupportInitialize)(this.valueTextBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NumericUpDown valueTextBox;
        private System.Windows.Forms.ComboBox postfixComboBox;
    }
}
