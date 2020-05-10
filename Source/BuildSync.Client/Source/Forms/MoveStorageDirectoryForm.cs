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
using System.Windows.Forms;
using BuildSync.Client.Tasks;

namespace BuildSync.Client.Forms
{
    /// <summary>
    /// </summary>
    public partial class MoveStorageDirectoryForm : Form
    {
        private bool Finished;

        private readonly MoveStorageTask MoveTask = new MoveStorageTask();

        /// <summary>
        /// </summary>
        public MoveStorageDirectoryForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// </summary>
        /// <param name="Message"></param>
        /// <param name="Progress"></param>
        /// <param name="TotalMessage"></param>
        /// <param name="TotalProgress"></param>
        public void SetProgress(string Message, float Progress, float TotalProgress)
        {
            Invoke(
                (MethodInvoker) (() =>
                {
                    TaskProgressLabel.Text = Message;
                    TaskProgressBar.Value = (int) (Progress * 100);

                    TotalProgressLabel.Text = "Total Progress";
                    TotalProgressBar.Value = (int) (TotalProgress * 100);
                })
            );
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormClosingRequested(object sender, FormClosingEventArgs e)
        {
            if (!Finished)
            {
                MessageBox.Show("This process cannot be interrupted.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Cancel = true;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormLoaded(object sender, EventArgs e)
        {
            // Disconnect from everybody.
            MoveTask.Start();
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateTimerTick(object sender, EventArgs e)
        {
            string Message = "";
            switch (MoveTask.State)
            {
                case MoveStorageState.WaitingForIOQueueToDrain:
                {
                    Message = "Waiting for io queue to empty";
                    break;
                }
                case MoveStorageState.WaitingForDownloadInitToFinish:
                {
                    Message = "Waiting for download initialization to finish.";
                    break;
                }
                case MoveStorageState.WaitingForDownloadValidationToFinish:
                {
                    Message = "Waiting for download validation to finish.";
                    break;
                }
                case MoveStorageState.WaitingForDownloadInstallToFinish:
                {
                    Message = "Waiting for download install to finish.";
                    break;
                }
                case MoveStorageState.CopyingFiles:
                {
                    Message = "Copying: " + MoveTask.CurrentFile;
                    break;
                }
                case MoveStorageState.CleaningUpOldDirectory:
                {
                    Message = "Cleaning up old directory.";
                    break;
                }
                case MoveStorageState.Success:
                {
                    DialogResult = DialogResult.OK;
                    Finished = true;
                    UpdateTimer.Enabled = false;
                    Close();
                    return;
                }
                case MoveStorageState.FailedDiskError:
                case MoveStorageState.Failed:
                default:
                {
                    DialogResult = DialogResult.Abort;
                    Finished = true;
                    UpdateTimer.Enabled = false;
                    MessageBox.Show("Failed to change storage directory due to disk error.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Close();
                    return;
                }
            }

            SetProgress(Message, MoveTask.SubProgress, MoveTask.Progress);
        }
    }
}