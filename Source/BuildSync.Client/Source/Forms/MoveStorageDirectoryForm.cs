using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using BuildSync.Core.Utils;
using BuildSync.Client.Tasks;

namespace BuildSync.Client.Forms
{
    /// <summary>
    /// 
    /// </summary>
    public partial class MoveStorageDirectoryForm : Form
    {
        private string OldDirectory = "";
        private string NewDirectory = "";
        private bool Finished = false;

        private MoveStorageTask MoveTask = new MoveStorageTask();

        /// <summary>
        /// 
        /// </summary>
        public MoveStorageDirectoryForm(string InOldDirectory, string InNewDirectory)
        {
            OldDirectory = InOldDirectory;
            NewDirectory = InNewDirectory;

            InitializeComponent();
        }

        /// <summary>
        /// 
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
        /// 
        /// </summary>
        /// <param name="Message"></param>
        /// <param name="Progress"></param>
        /// <param name="TotalMessage"></param>
        /// <param name="TotalProgress"></param>
        public void SetProgress(string Message, float Progress, float TotalProgress)
        {
            Invoke((MethodInvoker)(() =>
            {
                TaskProgressLabel.Text = Message;
                TaskProgressBar.Value = (int)(Progress * 100);

                TotalProgressLabel.Text = "Total Progress";
                TotalProgressBar.Value = (int)(TotalProgress * 100);
            }));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormLoaded(object sender, EventArgs e)
        {
            // Disconnect from everybody.
            MoveTask.Start(OldDirectory, NewDirectory);
/*
            // Run the rest async.
            Task.Run(() =>
            {
                bool Success = false;
                try
                {
                    // Drain the IO queue.
                    SetProgress("Waiting for io queue to empty", 0, 0);
                    while (!Program.IOQueue.IsEmpty)
                    {
                        Thread.Sleep(1);
                    }

                    // Wait for initialization or validation of any manifests to complete.
                    SetProgress("Waiting for download initialization to finish.", 0, 0);
                    while (Program.ManifestDownloadManager.DownloadInitializationInProgress)
                    {
                        Thread.Sleep(1);
                    }
                    SetProgress("Waiting for download validation to finish.", 0, 0);
                    while (Program.ManifestDownloadManager.DownloadValidationInProgress)
                    {
                        Thread.Sleep(1);
                    }

                    // Incase someone has parented the storage directory to a root drive or something
                    // super stupid like that, make sure we only move our sub directories around.
                    string[] OldDirectories =
                    {
                        Path.Combine(OldDirectory, "Builds"),
                        Path.Combine(OldDirectory, "Manifests"),
                    };
                    string[] NewDirectories =
                    {
                        Path.Combine(NewDirectory, "Builds"),
                        Path.Combine(NewDirectory, "Manifests"),
                    };

                    try
                    {
                        // Make list of everything we need to copy around.
                        List<Tuple<string, string, string>> FilesToCopy = new List<Tuple<string, string, string>>();
                        foreach (string Dir in OldDirectories)
                        {
                            string[] Files = Directory.GetFiles(OldDirectory, "*", SearchOption.AllDirectories);
                            foreach (string File in Files)
                            {
                                string RelativePath = File.Substring(OldDirectory.Length).Trim('\\', '/');
                                string NewPath = Path.Combine(NewDirectory, RelativePath);
                                FilesToCopy.Add(new Tuple<string, string, string>(File, NewPath, RelativePath));
                            }
                        }

                        // Get copying.
                        for (int i = 0; i < FilesToCopy.Count; i++)
                        {
                            string OldPath = FilesToCopy[i].Item1;
                            string NewPath = FilesToCopy[i].Item2;
                            string RelativePath = FilesToCopy[i].Item3;

                            string NewPathDir = Path.GetDirectoryName(NewPath);

                            if (!Directory.Exists(NewPathDir))
                            {
                                Directory.CreateDirectory(NewPathDir);
                            }

                            byte[] CopyBuffer = new byte[1024 * 1024];
                            using (FileStream Source = new FileStream(OldPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite|FileShare.Delete))
                            {
                                long FileLength = Source.Length;
                                using (FileStream Dest = new FileStream(NewPath, FileMode.Create, FileAccess.Write))
                                {
                                    long TotalBytes = 0;
                                    int CurrentBlockSize = 0;

                                    while ((CurrentBlockSize = Source.Read(CopyBuffer, 0, CopyBuffer.Length)) > 0)
                                    {
                                        TotalBytes += CurrentBlockSize;

                                        SetProgress("Copying: " + RelativePath, (float)TotalBytes / (float)FileLength, (float)i / (float)FilesToCopy.Count);

                                        Dest.Write(CopyBuffer, 0, CurrentBlockSize);
                                    }

                                    Dest.Flush();
                                    Dest.Close();
                                }

                                Source.Flush();
                                Source.Close();
                            }
                        }

                        // Delete old directory.
                        SetProgress("Cleaning up old directory.", 0.0f, 1.0f);
                        foreach (string Dir in OldDirectories)
                        {
                            try
                            {
                                FileUtils.DeleteDirectory(Dir);
                            }
                            catch (Exception Ex)
                            {
                                Logger.Log(LogLevel.Error, LogCategory.Main, "Failed to delete directory {0} with error: {1}", Dir, Ex.Message);
                            }
                        }

                        // Update local paths of all manifests.
                        Program.ManifestDownloadManager.UpdateStoragePath(Path.Combine(NewDirectory, "Builds"));
                        Program.BuildRegistry.UpdateStoragePath(Path.Combine(NewDirectory, "Manifests"));

                        Success = true;
                    }
                    catch (Exception Ex)
                    {
                        Logger.Log(LogLevel.Error, LogCategory.Main, "Failed to move storage directory with error: {0}", Ex.Message);

                        // Delete all the copied files.
                        foreach (string Dir in NewDirectories)
                        {
                            try
                            {
                                FileUtils.DeleteDirectory(Dir);
                            }
                            catch (Exception SubEx)
                            {
                                Logger.Log(LogLevel.Error, LogCategory.Main, "Failed to delete directory {0} with error: {1}", Dir, SubEx.Message);
                            }
                        }
                    }
                    finally
                    {
                        // Renable connections.
                        Program.NetClient.ConnectionsDisabled = false;

                        Invoke((MethodInvoker)(() => {
                            DialogResult = Success ? DialogResult.OK : DialogResult.Abort;
                            Finished = true;
                            Close();

                            if (!Success)
                            {
                                MessageBox.Show("Failed to change storage directory due to disk error.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }));
                    }                        
                }
                catch (Exception Ex)
                {
                }
            });
            */
        }

        /// <summary>
        /// 
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
