using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BuildSync.Client.Publishing;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace BuildSync.Client.Forms
{
    /// <summary>
    /// 
    /// </summary>
    public partial class PublishBuildForm : Form
    {
        /// <summary>
        /// 
        /// </summary>
        private BuildPublisher Publisher = null;

        /// <summary>
        /// 
        /// </summary>
        public PublishBuildForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        private void ValidateState()
        {
            PublishButton.Enabled = (
                NameTextBox.Text.Trim().Length > 3 &&
                VirtualPathTextBox.Text.Trim().Length >= 3 &&
                LocalFolderTextBox.Text.Length > 0 &&
                Directory.Exists(LocalFolderTextBox.Text)
            );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataStateChanged(object sender, EventArgs e)
        {
            ValidateState();
        }

        /// <summary>
        /// 
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
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PublishClicked(object sender, EventArgs e)
        {
            PublishButton.Text = "Publishing ...";
            PublishButton.Enabled = false;
            PublishProgressBar.Visible = true;
            PublishProgressLabel.Visible = true;

            NameTextBox.Enabled = false;
            VirtualPathTextBox.Enabled = false;
            LocalFolderBrowseButton.Enabled = false;

            Publisher = new BuildPublisher();
            Publisher.Start(NameTextBox.Text, VirtualPathTextBox.Text, LocalFolderTextBox.Text);
        }

        /// <summary>
        /// 
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
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProgressTimerTick(object sender, EventArgs e)
        {
            if (Publisher != null)
            {
                PublishProgressBar.Value = (int)Publisher.Progress;

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
                    case BuildPublishingState.FailedGuidAlreadyExists:
                        {
                            Publisher = null;
                            MessageBox.Show("Failed to publish manifest, one already exists with the same guid.", "Failed to publish", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break;
                        }
                    case BuildPublishingState.Success:
                        {
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

                    NameTextBox.Enabled = true;
                    VirtualPathTextBox.Enabled = true;
                    LocalFolderBrowseButton.Enabled = true;
                }
            }
        }
    }
}
