namespace BuildSync.Client.Controls.Settings
{
    partial class ReplicationSettings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReplicationSettings));
            this.ButtonImageIndex = new System.Windows.Forms.ImageList(this.components);
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.ignoreTagsTextBox = new BuildSync.Client.Controls.TagTextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.selectTagsTextBox = new BuildSync.Client.Controls.TagTextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.AutoDownloadBuildsCheckbox = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // ButtonImageIndex
            // 
            this.ButtonImageIndex.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ButtonImageIndex.ImageStream")));
            this.ButtonImageIndex.TransparentColor = System.Drawing.Color.Transparent;
            this.ButtonImageIndex.Images.SetKeyName(0, "appbar.add.png");
            this.ButtonImageIndex.Images.SetKeyName(1, "appbar.delete.png");
            this.ButtonImageIndex.Images.SetKeyName(2, "appbar.user.add.png");
            this.ButtonImageIndex.Images.SetKeyName(3, "appbar.user.delete.png");
            // 
            // pictureBox3
            // 
            this.pictureBox3.Image = global::BuildSync.Client.Properties.Resources.ic_check_circle_2x;
            this.pictureBox3.Location = new System.Drawing.Point(14, 217);
            this.pictureBox3.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(32, 31);
            this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox3.TabIndex = 56;
            this.pictureBox3.TabStop = false;
            // 
            // ignoreTagsTextBox
            // 
            this.ignoreTagsTextBox.Location = new System.Drawing.Point(54, 225);
            this.ignoreTagsTextBox.Name = "ignoreTagsTextBox";
            this.ignoreTagsTextBox.Size = new System.Drawing.Size(337, 21);
            this.ignoreTagsTextBox.TabIndex = 55;
            this.ignoreTagsTextBox.TagIds = ((System.Collections.Generic.List<System.Guid>)(resources.GetObject("ignoreTagsTextBox.TagIds")));
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(13, 199);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(483, 17);
            this.label4.TabIndex = 54;
            this.label4.Text = "Do not download builds with tags";
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::BuildSync.Client.Properties.Resources.ic_check_circle_2x;
            this.pictureBox2.Location = new System.Drawing.Point(14, 156);
            this.pictureBox2.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(32, 31);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 53;
            this.pictureBox2.TabStop = false;
            // 
            // selectTagsTextBox
            // 
            this.selectTagsTextBox.Location = new System.Drawing.Point(54, 164);
            this.selectTagsTextBox.Name = "selectTagsTextBox";
            this.selectTagsTextBox.Size = new System.Drawing.Size(337, 21);
            this.selectTagsTextBox.TabIndex = 52;
            this.selectTagsTextBox.TagIds = ((System.Collections.Generic.List<System.Guid>)(resources.GetObject("selectTagsTextBox.TagIds")));
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(13, 138);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(483, 17);
            this.label5.TabIndex = 51;
            this.label5.Text = "Only download builds with tags";
            // 
            // AutoDownloadBuildsCheckbox
            // 
            this.AutoDownloadBuildsCheckbox.AutoSize = true;
            this.AutoDownloadBuildsCheckbox.Location = new System.Drawing.Point(14, 102);
            this.AutoDownloadBuildsCheckbox.Margin = new System.Windows.Forms.Padding(2);
            this.AutoDownloadBuildsCheckbox.Name = "AutoDownloadBuildsCheckbox";
            this.AutoDownloadBuildsCheckbox.Size = new System.Drawing.Size(349, 17);
            this.AutoDownloadBuildsCheckbox.TabIndex = 57;
            this.AutoDownloadBuildsCheckbox.Text = "Automatically create downloads for builds as they become available?";
            this.AutoDownloadBuildsCheckbox.UseVisualStyleBackColor = true;
            this.AutoDownloadBuildsCheckbox.CheckedChanged += new System.EventHandler(this.StateChanged);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(11, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(485, 74);
            this.label1.TabIndex = 58;
            this.label1.Text = resources.GetString("label1.Text");
            // 
            // ReplicationSettings
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.AutoDownloadBuildsCheckbox);
            this.Controls.Add(this.pictureBox3);
            this.Controls.Add(this.ignoreTagsTextBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.selectTagsTextBox);
            this.Controls.Add(this.label5);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "ReplicationSettings";
            this.Size = new System.Drawing.Size(519, 259);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ImageList ButtonImageIndex;
        private System.Windows.Forms.PictureBox pictureBox3;
        private TagTextBox ignoreTagsTextBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.PictureBox pictureBox2;
        private TagTextBox selectTagsTextBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox AutoDownloadBuildsCheckbox;
        private System.Windows.Forms.Label label1;
    }
}
