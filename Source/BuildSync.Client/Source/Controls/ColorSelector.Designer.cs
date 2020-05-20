namespace BuildSync.Client.Controls
{
    partial class ColorSelector
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
            this.MainComboBox = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // MainComboBox
            // 
            this.MainComboBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.MainComboBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.MainComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.MainComboBox.FormattingEnabled = true;
            this.MainComboBox.Location = new System.Drawing.Point(0, 0);
            this.MainComboBox.Name = "MainComboBox";
            this.MainComboBox.Size = new System.Drawing.Size(383, 21);
            this.MainComboBox.TabIndex = 0;
            this.MainComboBox.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.DrawItem);
            this.MainComboBox.SelectedIndexChanged += new System.EventHandler(this.SelectedIndexChanged);
            // 
            // ColorSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.MainComboBox);
            this.Name = "ColorSelector";
            this.Size = new System.Drawing.Size(383, 28);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox MainComboBox;
    }
}
