/*
  buildsync
  Copyright (C) 2020 Tim Leonard <me@timleonard.uk>

  This software is provided 'as-is', without any express or implied
  warranty.  In no event will the authors be held liable for any damages
  arising from the use of this software.
  
  Permission is granted to anyone to use this software for any purpose,
  including commercial applications, and to alter it and redistribute it
  freely, subject to the following restrictions:

  1. The origin of this software must not be misrepresented; you must not
     claim that you wrote the original software. If you use this software
     in a product, an acknowledgment in the product documentation would be
     appreciated but is not required.
  2. Altered source versions must be plainly marked as such, and must not be
     misrepresented as being the original software.
  3. This notice may not be removed or altered from any source distribution.
*/

namespace BuildSync.Client.Controls
{
    partial class ManifestAvailabilityPanel
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
            this.NameLabel = new System.Windows.Forms.Label();
            this.AvailabilityStatus = new BuildSync.Client.Controls.BlockStatusPanelLinear();
            this.SuspendLayout();
            // 
            // NameLabel
            // 
            this.NameLabel.Location = new System.Drawing.Point(3, 5);
            this.NameLabel.Name = "NameLabel";
            this.NameLabel.Size = new System.Drawing.Size(263, 13);
            this.NameLabel.TabIndex = 1;
            this.NameLabel.Text = "Something/Something";
            // 
            // AvailabilityStatus
            // 
            this.AvailabilityStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.AvailabilityStatus.Location = new System.Drawing.Point(272, 4);
            this.AvailabilityStatus.Name = "AvailabilityStatus";
            this.AvailabilityStatus.Size = new System.Drawing.Size(322, 16);
            this.AvailabilityStatus.TabIndex = 0;
            // 
            // ManifestAvailabilityPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.AvailabilityStatus);
            this.Controls.Add(this.NameLabel);
            this.Name = "ManifestAvailabilityPanel";
            this.Size = new System.Drawing.Size(600, 24);
            this.ResumeLayout(false);

        }

        #endregion

        private BlockStatusPanelLinear AvailabilityStatus;
        private System.Windows.Forms.Label NameLabel;
    }
}
