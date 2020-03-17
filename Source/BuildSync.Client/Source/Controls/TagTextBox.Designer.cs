﻿namespace BuildSync.Client.Controls
{
    partial class TagTextBox
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
            this.components = new System.ComponentModel.Container();
            this.MainTextBox = new System.Windows.Forms.TextBox();
            this.TagContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.SuspendLayout();
            // 
            // MainTextBox
            // 
            this.MainTextBox.ContextMenuStrip = this.TagContextMenuStrip;
            this.MainTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainTextBox.Location = new System.Drawing.Point(0, 0);
            this.MainTextBox.Name = "MainTextBox";
            this.MainTextBox.ReadOnly = true;
            this.MainTextBox.Size = new System.Drawing.Size(150, 20);
            this.MainTextBox.TabIndex = 0;
            this.MainTextBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.OnClicked);
            // 
            // TagContextMenuStrip
            // 
            this.TagContextMenuStrip.Name = "TagContextMenuStrip";
            this.TagContextMenuStrip.Size = new System.Drawing.Size(61, 4);
            this.TagContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.ContextMenuStripOpening);
            // 
            // TagTextBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.MainTextBox);
            this.Name = "TagTextBox";
            this.Size = new System.Drawing.Size(150, 21);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox MainTextBox;
        private System.Windows.Forms.ContextMenuStrip TagContextMenuStrip;
    }
}