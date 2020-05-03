namespace BuildSync.Client.Controls.Settings
{
    partial class StorageSettings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StorageSettings));
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.deprioritizeTagsTextBox = new BuildSync.Client.Controls.TagTextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.prioritizeTagsTextBox = new BuildSync.Client.Controls.TagTextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label3 = new System.Windows.Forms.Label();
            this.HeuristicComboBox = new System.Windows.Forms.ComboBox();
            this.StorageMaxSizeIcon = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.StoragePathIcon = new System.Windows.Forms.PictureBox();
            this.savePathBrowseButton = new System.Windows.Forms.Button();
            this.StoragePathTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.StorageMaxSizeTextBox = new BuildSync.Core.Controls.SizeTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.StorageMaxSizeIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.StoragePathIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox3
            // 
            this.pictureBox3.Image = global::BuildSync.Client.Properties.Resources.ic_check_circle_2x;
            this.pictureBox3.Location = new System.Drawing.Point(14, 287);
            this.pictureBox3.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(32, 31);
            this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox3.TabIndex = 50;
            this.pictureBox3.TabStop = false;
            // 
            // deprioritizeTagsTextBox
            // 
            this.deprioritizeTagsTextBox.Location = new System.Drawing.Point(54, 294);
            this.deprioritizeTagsTextBox.Name = "deprioritizeTagsTextBox";
            this.deprioritizeTagsTextBox.Size = new System.Drawing.Size(337, 21);
            this.deprioritizeTagsTextBox.TabIndex = 49;
            this.deprioritizeTagsTextBox.TagIds = ((System.Collections.Generic.List<System.Guid>)(resources.GetObject("deprioritizeTagsTextBox.TagIds")));
            this.deprioritizeTagsTextBox.OnTagsChanged += new System.EventHandler(this.StateChanged);
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(13, 268);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(483, 17);
            this.label4.TabIndex = 48;
            this.label4.Text = "Prioritize deleting builds with any tags";
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::BuildSync.Client.Properties.Resources.ic_check_circle_2x;
            this.pictureBox2.Location = new System.Drawing.Point(14, 226);
            this.pictureBox2.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(32, 31);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 47;
            this.pictureBox2.TabStop = false;
            // 
            // prioritizeTagsTextBox
            // 
            this.prioritizeTagsTextBox.Location = new System.Drawing.Point(54, 233);
            this.prioritizeTagsTextBox.Name = "prioritizeTagsTextBox";
            this.prioritizeTagsTextBox.Size = new System.Drawing.Size(337, 21);
            this.prioritizeTagsTextBox.TabIndex = 46;
            this.prioritizeTagsTextBox.TagIds = ((System.Collections.Generic.List<System.Guid>)(resources.GetObject("prioritizeTagsTextBox.TagIds")));
            this.prioritizeTagsTextBox.OnTagsChanged += new System.EventHandler(this.StateChanged);
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(13, 207);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(483, 17);
            this.label5.TabIndex = 45;
            this.label5.Text = "Prioritize keeping builds with any tags";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::BuildSync.Client.Properties.Resources.ic_check_circle_2x;
            this.pictureBox1.Location = new System.Drawing.Point(14, 163);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(32, 31);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 44;
            this.pictureBox1.TabStop = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 145);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(243, 13);
            this.label3.TabIndex = 43;
            this.label3.Text = "What build to delete to make space for new builds";
            // 
            // HeuristicComboBox
            // 
            this.HeuristicComboBox.FormattingEnabled = true;
            this.HeuristicComboBox.Location = new System.Drawing.Point(54, 169);
            this.HeuristicComboBox.Name = "HeuristicComboBox";
            this.HeuristicComboBox.Size = new System.Drawing.Size(337, 21);
            this.HeuristicComboBox.TabIndex = 42;
            this.HeuristicComboBox.SelectedIndexChanged += new System.EventHandler(this.StateChanged);
            // 
            // StorageMaxSizeIcon
            // 
            this.StorageMaxSizeIcon.Image = global::BuildSync.Client.Properties.Resources.ic_check_circle_2x;
            this.StorageMaxSizeIcon.Location = new System.Drawing.Point(14, 99);
            this.StorageMaxSizeIcon.Margin = new System.Windows.Forms.Padding(2);
            this.StorageMaxSizeIcon.Name = "StorageMaxSizeIcon";
            this.StorageMaxSizeIcon.Size = new System.Drawing.Size(32, 31);
            this.StorageMaxSizeIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.StorageMaxSizeIcon.TabIndex = 19;
            this.StorageMaxSizeIcon.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 80);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(330, 13);
            this.label2.TabIndex = 18;
            this.label2.Text = "Maximum storage space to use, old builds will be deleted to maintain.";
            // 
            // StoragePathIcon
            // 
            this.StoragePathIcon.Image = global::BuildSync.Client.Properties.Resources.ic_check_circle_2x;
            this.StoragePathIcon.Location = new System.Drawing.Point(14, 34);
            this.StoragePathIcon.Margin = new System.Windows.Forms.Padding(2);
            this.StoragePathIcon.Name = "StoragePathIcon";
            this.StoragePathIcon.Size = new System.Drawing.Size(32, 31);
            this.StoragePathIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.StoragePathIcon.TabIndex = 17;
            this.StoragePathIcon.TabStop = false;
            // 
            // savePathBrowseButton
            // 
            this.savePathBrowseButton.Location = new System.Drawing.Point(403, 36);
            this.savePathBrowseButton.Margin = new System.Windows.Forms.Padding(2);
            this.savePathBrowseButton.Name = "savePathBrowseButton";
            this.savePathBrowseButton.Size = new System.Drawing.Size(93, 27);
            this.savePathBrowseButton.TabIndex = 16;
            this.savePathBrowseButton.Text = "Browse";
            this.savePathBrowseButton.UseVisualStyleBackColor = true;
            this.savePathBrowseButton.Click += new System.EventHandler(this.BrowseClicked);
            // 
            // StoragePathTextBox
            // 
            this.StoragePathTextBox.Location = new System.Drawing.Point(54, 42);
            this.StoragePathTextBox.Margin = new System.Windows.Forms.Padding(2);
            this.StoragePathTextBox.Name = "StoragePathTextBox";
            this.StoragePathTextBox.ReadOnly = true;
            this.StoragePathTextBox.Size = new System.Drawing.Size(338, 20);
            this.StoragePathTextBox.TabIndex = 15;
            this.StoragePathTextBox.TextChanged += new System.EventHandler(this.StateChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 15);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(202, 13);
            this.label1.TabIndex = 14;
            this.label1.Text = "Local folder to store downloaded builds in";
            // 
            // StorageMaxSizeTextBox
            // 
            this.StorageMaxSizeTextBox.DisplayAsTransferRate = false;
            this.StorageMaxSizeTextBox.Location = new System.Drawing.Point(54, 103);
            this.StorageMaxSizeTextBox.Name = "StorageMaxSizeTextBox";
            this.StorageMaxSizeTextBox.Size = new System.Drawing.Size(337, 27);
            this.StorageMaxSizeTextBox.TabIndex = 51;
            this.StorageMaxSizeTextBox.Value = ((long)(0));
            this.StorageMaxSizeTextBox.OnValueChanged += new System.EventHandler(this.StateChanged);
            // 
            // StorageSettings
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.StorageMaxSizeTextBox);
            this.Controls.Add(this.pictureBox3);
            this.Controls.Add(this.deprioritizeTagsTextBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.prioritizeTagsTextBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.HeuristicComboBox);
            this.Controls.Add(this.StorageMaxSizeIcon);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.StoragePathIcon);
            this.Controls.Add(this.savePathBrowseButton);
            this.Controls.Add(this.StoragePathTextBox);
            this.Controls.Add(this.label1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "StorageSettings";
            this.Size = new System.Drawing.Size(519, 323);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.StorageMaxSizeIcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.StoragePathIcon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox StoragePathIcon;
        private System.Windows.Forms.Button savePathBrowseButton;
        private System.Windows.Forms.TextBox StoragePathTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox StorageMaxSizeIcon;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox HeuristicComboBox;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private TagTextBox prioritizeTagsTextBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.PictureBox pictureBox3;
        private TagTextBox deprioritizeTagsTextBox;
        private System.Windows.Forms.Label label4;
        private Core.Controls.SizeTextBox StorageMaxSizeTextBox;
    }
}
