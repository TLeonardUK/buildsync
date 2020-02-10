using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BuildSync.Core.Downloads;
using BuildSync.Client.Forms;
using BuildSync.Core.Utils;

namespace BuildSync.Client.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public partial class DownloadListItem : UserControl
    {
        /// <summary>
        /// 
        /// </summary>
        public DownloadState State;

        /// <summary>
        /// 
        /// </summary>
        private bool Collapsed = true;

        /// <summary>
        /// 
        /// </summary>
        private bool InternalSelected = false;
        public bool Selected
        {
            get
            {
                return InternalSelected;
            }
            set
            {
                if (InternalSelected != value)
                {
                    InternalSelected = value;
                    MainPanel.Invalidate();
                    MainPanel.Refresh();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public DownloadListItem()
        {
            InitializeComponent();

            WindowUtils.EnableDoubleBuffering(MainPanel);

            //CollapseButtonClicked(null, null);
        }

        /// <summary>
        /// 
        /// </summary>
        private enum LaunchOption
        {
            Pause,
            Resume,
            Launch
        }

        /// <summary>
        /// 
        /// </summary>
        private enum StateColoring
        { 
            Success,
            Error,
            Warning,
            Info
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="State"></param>
        /// <param name="Build"></param>
        /// <param name="Progress"></param>
        private void SetStatus(string State, StateColoring Coloring, string Build, float Progress, bool ContinuousWork, LaunchOption LaunchType, string UpRate, string DownRate)
        {
            if (EtaLabel.Text != State)
            {
                EtaLabel.Text = State;
            }

            if (BuildLabel.Text != Build)
            {
                BuildLabel.Text = Build;
            }

            Color TargetColor = Color.Black;
            if (Coloring == StateColoring.Error)
            {
                TargetColor = Color.Red;
                ProgressBar.SetState(2);
            }
            else if (Coloring == StateColoring.Info)
            {
                TargetColor = Color.Black;
                ProgressBar.SetState(1);
            }
            else if (Coloring == StateColoring.Warning)
            {
                TargetColor = Color.Orange;
                ProgressBar.SetState(3);
            }
            else if (Coloring == StateColoring.Success)
            {
                TargetColor = Color.Green;
                ProgressBar.SetState(1);
            }

            if (EtaLabel.ForeColor != TargetColor)
            {
                EtaLabel.ForeColor = TargetColor;
            }

            if (!ContinuousWork)
            {
                if (ProgressBar.Style != ProgressBarStyle.Marquee)
                {
                    ProgressBar.Style = ProgressBarStyle.Marquee;
                    ProgressBar.Value = 0;
                    ProgressBar.Refresh();
                }
            }
            else
            {
                if (ProgressBar.Style != ProgressBarStyle.Continuous)
                {
                    ProgressBar.Style = ProgressBarStyle.Continuous;
                }

                if (ProgressBar.Value != (int)Progress)
                {
                    ProgressBar.Value = Math.Max(0, Math.Min(100, (int)Progress));
                    ProgressBar.Refresh();
                }
            }

            int ImageIndex = 0;
            if (LaunchType == LaunchOption.Launch)
            {
                ImageIndex = 0;
            }
            else if (LaunchType == LaunchOption.Resume)
            {
                ImageIndex = 3;
            }
            else if (LaunchType == LaunchOption.Pause)
            {
                ImageIndex = 4;
            }
            if (PlayButton.ImageIndex != ImageIndex)
            {
                PlayButton.ImageIndex = ImageIndex;
            }

            if (DownloadSpeedLabel.Text != DownRate)
            {
                DownloadSpeedLabel.Text = DownRate;
            }
            if (UploadSpeedLabel.Text != UpRate)
            {
                UploadSpeedLabel.Text = UpRate;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void RefreshState()
        {
            if (State.Name != NameLabel.Text)
            {
                NameLabel.Text = State.Name;
            }

            blockStatusPanel.State = State;

            ManifestDownloadState Downloader = Program.ManifestDownloadManager.GetDownload(State.ActiveManifestId);
            if (Downloader != null)
            {
                string UpRate = StringUtils.FormatAsTransferRate(Downloader.BandwidthStats.RateOut);
                string DownRate = StringUtils.FormatAsTransferRate(Downloader.BandwidthStats.RateIn);

                if (Downloader.Paused)
                {
                    switch (Downloader.State)
                    {
                        case ManifestDownloadProgressState.DiskError:
                        case ManifestDownloadProgressState.InitializeFailed:
                            {
                                SetStatus("Disk Error", StateColoring.Error, Downloader.Manifest.VirtualPath, 100, true, LaunchOption.Resume, UpRate, DownRate);
                                break;
                            }
                        case ManifestDownloadProgressState.ValidationFailed:
                            {
                                SetStatus("Validation Error", StateColoring.Error, Downloader.Manifest.VirtualPath, 100, true, LaunchOption.Resume, UpRate, DownRate);
                                break;
                            }
                        case ManifestDownloadProgressState.InstallFailed:
                            {
                                SetStatus("Install Error", StateColoring.Error, Downloader.Manifest.VirtualPath, 100, true, LaunchOption.Resume, UpRate, DownRate);
                                break;
                            }
                        default:
                            {
                                SetStatus("-", StateColoring.Info, (Downloader.Manifest == null ? "-" : Downloader.Manifest.VirtualPath), Downloader.Progress * 100, true, LaunchOption.Resume, UpRate, DownRate);
                                break;
                            }
                    }
                }
                else
                {
                    switch (Downloader.State)
                    {
                        case ManifestDownloadProgressState.Complete:
                            {
                                SetStatus("Complete", StateColoring.Success, Downloader.Manifest.VirtualPath, 100, true, LaunchOption.Launch, UpRate, DownRate);
                                break;
                            }
                        case ManifestDownloadProgressState.Initializing:
                            {
                                double SecondsToInitialize = (double)Downloader.InitializeBytesRemaining / (double)Downloader.InitializeRateStats.RateIn;
                                if (Downloader.InitializeRateStats.RateIn == 0)
                                {
                                    SecondsToInitialize = 0;
                                }

                                string Status = string.Format("Allocating Disk Space - {0}", StringUtils.FormatAsDuration((long)SecondsToInitialize));

                                SetStatus(Status, StateColoring.Warning, Downloader.Manifest.VirtualPath, Downloader.InitializeProgress * 100, true, LaunchOption.Pause, UpRate, DownRate);
                                break;
                            }
                        case ManifestDownloadProgressState.Validating:
                            {
                                long SecondsToValidate = (long)((double)Downloader.ValidateBytesRemaining / (double)Downloader.ValidateRateStats.RateOut);
                                if (Downloader.ValidateRateStats.RateOut == 0)
                                {
                                    SecondsToValidate = 0;
                                }

                                string Status = string.Format("Validating - {0}", StringUtils.FormatAsDuration(SecondsToValidate));

                                SetStatus(Status, StateColoring.Warning, Downloader.Manifest.VirtualPath, Downloader.ValidateProgress * 100, true, LaunchOption.Pause, UpRate, DownRate);
                                break;
                            }
                        case ManifestDownloadProgressState.Installing:
                            {
                                SetStatus("Installing", StateColoring.Info, Downloader.Manifest.VirtualPath, 0, false, LaunchOption.Pause, UpRate, DownRate);
                                break;
                            }
                        case ManifestDownloadProgressState.Downloading:
                            {
                                long SecondsToDownload = (long)((double)Downloader.BytesRemaining / (double)Downloader.BandwidthStats.RateIn);
                                if (Downloader.BandwidthStats.RateIn == 0)
                                {
                                    SecondsToDownload = 0;
                                }

                                string Status = "";
                                StateColoring StatusColor = StateColoring.Info;
                                if (Downloader.BandwidthStats.RateIn == 0)
                                {
                                    if (Program.NetClient.IsConnected)
                                    {
                                        Status = "Locating Data";
                                    }
                                    else
                                    {
                                        Status = "No Connection";
                                        StatusColor = StateColoring.Error;
                                    }
                                }
                                else
                                {
                                    Status = string.Format("Downloading - {0}", StringUtils.FormatAsDuration(SecondsToDownload));
                                }

                                SetStatus(Status, StatusColor, Downloader.Manifest.VirtualPath, Downloader.Progress * 100, true, LaunchOption.Pause, UpRate, DownRate);
                                break;
                            }
                        case ManifestDownloadProgressState.RetrievingManifest:
                            {
                                string Status = "";
                                StateColoring StatusColor = StateColoring.Info;
                                if (Program.NetClient.IsConnected)
                                {
                                    Status = "Locating Blocks";
                                }
                                else
                                {
                                    Status = "No Connection";
                                    StatusColor = StateColoring.Error;
                                }

                                SetStatus(Status, StatusColor, "Locating", 0, false, LaunchOption.Pause, UpRate, DownRate);
                                break;
                            }
                    }
                }
            }
            else
            {
                string Status = "";
                StateColoring StatusColor = StateColoring.Info;

                if (Program.NetClient.IsConnected)
                {
                    Status = "Waiting for available download";
                }
                else
                {
                    Status = "No Connection";
                    StatusColor = StateColoring.Error;
                }

                SetStatus(Status, StatusColor, "Unknown", 0, false, State.Paused ? LaunchOption.Resume : LaunchOption.Pause, "0 kb/s", "0 kb/s");
            }

            if (SettingsButton.Enabled != Program.NetClient.IsConnected)
            {
                SettingsButton.Enabled = (Program.NetClient.IsConnected);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteClicked(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you wish to delete '" + State.Name + "'?", "Delete Download?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                Program.DownloadManager.RemoveDownload(State);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SettingsClicked(object sender, EventArgs e)
        {
            AddDownloadForm Dialog = new AddDownloadForm();
            Dialog.EditState = State;
            Dialog.ShowDialog(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlayClicked(object sender, EventArgs e)
        {
            ManifestDownloadState Downloader = Program.ManifestDownloadManager.GetDownload(State.ActiveManifestId);
            if (Downloader != null)
            {
                if (Downloader.Paused)
                {
                    State.Paused = false;
                }
                else
                {
                    switch (Downloader.State)
                    {
                        case ManifestDownloadProgressState.Complete:
                            {
                                LaunchForm Form = new LaunchForm();
                                Form.Downloader = Downloader;
                                Form.DownloadState = State;
                                Form.ShowDialog();
                                break;
                            }
                        default:
                            {
                                State.Paused = true;
                                break;
                            }
                    }
                }
            }
            else
            {
                State.Paused = !State.Paused;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainPanel_Paint(object sender, PaintEventArgs e)
        {
            //if (Selected)
            //{
            //    ControlPaint.DrawBorder(e.Graphics, MainPanel.ClientRectangle, SystemColors.ActiveBorder, ButtonBorderStyle.Solid);
            //}
            //else
            //{
                ControlPaint.DrawBorder(e.Graphics, MainPanel.ClientRectangle, SystemColors.ControlLight, ButtonBorderStyle.Solid);
            //}
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CollapseButtonClicked(object sender, EventArgs e)
        {
            Collapsed = !Collapsed;
            if (Collapsed)
            {
                Size = new Size(610, 50);
            }
            else
            {
                Size = new Size(610, 150);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BlockRefreshTimer(object sender, EventArgs e)
        {
            if (!Collapsed)
            {
                blockStatusPanel.Refresh();
            }
        }
    }
}
