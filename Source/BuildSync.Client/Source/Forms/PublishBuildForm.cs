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

using System;
using System.IO;
using System.Windows.Forms;
using BuildSync.Client.Tasks;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace BuildSync.Client.Forms
{
    /// <summary>
    /// </summary>
    public partial class PublishBuildForm : Form
    {
        /// <summary>
        /// </summary>
        private PublishBuildTask Publisher;

        /// <summary>
        /// 
        /// </summary>
        public string VirtualPath
        {
            get { return VirtualPathTextBox.Text; }
            set { VirtualPathTextBox.Text = value; }
        }

        /// <summary>
        /// </summary>
        public PublishBuildForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BrowseForLocalFolderClicked(object sender, EventArgs e)
        {
            CommonOpenFileDialog Dialog = new CommonOpenFileDialog();
            Dialog.AllowNonFileSystemItems = true;
            Dialog.Multiselect = true;
            Dialog.IsFolderPicker = true;
            Dialog.Title = "Select Build Folder";

            if (Dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                LocalFolderTextBox.Text = Dialog.FileName;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataStateChanged(object sender, EventArgs e)
        {
            ValidateState();
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormAboutToClose(object sender, FormClosingEventArgs e)
        {
            if (Publisher != null)
            {
                MessageBox.Show("Cannot close, please wait for publishing to complete.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Cancel = true;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProgressTimerTick(object sender, EventArgs e)
        {
            if (Publisher != null)
            {
                PublishProgressBar.Value = Math.Max(0, Math.Min(100, (int) Publisher.Progress));

                switch (Publisher.State)
                {
                    case BuildPublishingState.CopyingFiles:
                    {
                        PublishProgressLabel.Text = "Copying file: " + Publisher.CurrentFile;
                        break;
                    }
                    case BuildPublishingState.ScanningFiles:
                    {
                        PublishProgressLabel.Text = "Scanning file: " + Publisher.CurrentFile;
                        break;
                    }
                    case BuildPublishingState.UploadingManifest:
                    {
                        PublishProgressLabel.Text = "Publishing manifest to server";
                        break;
                    }
                    case BuildPublishingState.FailedVirtualPathAlreadyExists:
                    {
                        Publisher = null;
                        MessageBox.Show("Failed to publish manifest, one already exists at virtual path.", "Failed to publish", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    }
                    case BuildPublishingState.PermissionDenied:
                    {
                        Publisher = null;
                        MessageBox.Show("Failed to publish manifest, you do not have permission to publish at that virtual path.", "Failed to publish", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    }
                    case BuildPublishingState.FailedGuidAlreadyExists:
                    {
                        Publisher = null;
                        MessageBox.Show("Failed to publish manifest, one already exists with the same guid.", "Failed to publish", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    }
                    case BuildPublishingState.Success:
                    {
                        Publisher.Commit();
                        Publisher = null;
                        MessageBox.Show("Finished publishing manifest.", "Manifest published", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Close();
                        break;
                    }
                    case BuildPublishingState.Failed:
                    default:
                    {
                        Publisher = null;
                        MessageBox.Show("Failed to publish manifest for undefined reason.", "Failed to publish", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    }
                }

                if (Publisher == null)
                {
                    PublishButton.Text = "Publish Build";
                    PublishButton.Enabled = true;
                    PublishProgressBar.Visible = false;
                    PublishProgressLabel.Visible = false;

                    VirtualPathTextBox.Enabled = true;
                    LocalFolderBrowseButton.Enabled = true;
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PublishClicked(object sender, EventArgs e)
        {
            PublishButton.Text = "Publishing ...";
            PublishButton.Enabled = false;
            PublishProgressBar.Visible = true;
            PublishProgressLabel.Visible = true;

            VirtualPathTextBox.Enabled = false;
            LocalFolderBrowseButton.Enabled = false;

            Publisher = new PublishBuildTask();
            Publisher.Start(VirtualPathTextBox.Text, LocalFolderTextBox.Text);
        }

        /// <summary>
        /// </summary>
        private void ValidateState()
        {
            PublishButton.Enabled = VirtualPathTextBox.Text.Trim().Length >= 3 &&
                                    LocalFolderTextBox.Text.Length > 0 &&
                                    Directory.Exists(LocalFolderTextBox.Text);
        }
    }
}