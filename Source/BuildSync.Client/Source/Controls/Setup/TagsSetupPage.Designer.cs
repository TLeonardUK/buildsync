namespace BuildSync.Client.Controls.Setup
{
    partial class TagsSetupPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TagsSetupPage));
            this.label1 = new System.Windows.Forms.Label();
            this.ServerHostnameIcon = new System.Windows.Forms.PictureBox();
            this.TagsTextBox = new BuildSync.Client.Controls.TagTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.ServerHostnameIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(13, 15);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(621, 57);
            this.label1.TabIndex = 10;
            this.label1.Text = resources.GetString("label1.Text");
            // 
            // ServerHostnameIcon
            // 
            this.ServerHostnameIcon.Image = global::BuildSync.Client.Properties.Resources.ic_check_circle_2x;
            this.ServerHostnameIcon.Location = new System.Drawing.Point(19, 90);
            this.ServerHostnameIcon.Margin = new System.Windows.Forms.Padding(2);
            this.ServerHostnameIcon.Name = "ServerHostnameIcon";
            this.ServerHostnameIcon.Size = new System.Drawing.Size(32, 31);
            this.ServerHostnameIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.ServerHostnameIcon.TabIndex = 13;
            this.ServerHostnameIcon.TabStop = false;
            // 
            // TagsTextBox
            // 
            this.TagsTextBox.Location = new System.Drawing.Point(57, 95);
            this.TagsTextBox.Name = "TagsTextBox";
            this.TagsTextBox.Size = new System.Drawing.Size(577, 21);
            this.TagsTextBox.TabIndex = 14;
            this.TagsTextBox.TagIds = ((System.Collections.Generic.List<System.Guid>)(resources.GetObject("TagsTextBox.TagIds")));
            this.TagsTextBox.OnTagsChanged += new System.EventHandler(this.StateChanged);
            // 
            // TagsSetupPage
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.TagsTextBox);
            this.Controls.Add(this.ServerHostnameIcon);
            this.Controls.Add(this.label1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "TagsSetupPage";
            this.Size = new System.Drawing.Size(656, 139);
            ((System.ComponentModel.ISupportInitialize)(this.ServerHostnameIcon)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox ServerHostnameIcon;
        private System.Windows.Forms.Label label1;
        private TagTextBox TagsTextBox;
    }
}
