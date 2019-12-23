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
        public DownloadListItem()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        public void RefreshState()
        {
            NameLabel.Text = State.Name;

            EtaLabel.ForeColor = Color.Black;

            ManifestDownloadState Downloader = Program.ManifestDownloadManager.GetDownload(State.ActiveManifestId);
            if (Downloader != null)
            {
                if (Downloader.Paused)
                {
                    switch (Downloader.State)
                    {
                        case ManifestDownloadProgressState.InitializeFailed:
                            {
                                EtaLabel.Text = "Disk Error";
                                EtaLabel.ForeColor = Color.Red;
                                BuildLabel.Text = Downloader.Manifest.VirtualPath;
                                ProgressBar.Style = ProgressBarStyle.Continuous;
                                ProgressBar.Value = 0;
                                PlayButton.Image = global::BuildSync.Client.Properties.Resources.appbar_control_play;
                                break;
                            }
                        case ManifestDownloadProgressState.ValidationFailed:
                            {
                                EtaLabel.Text = "Validation Error";
                                EtaLabel.ForeColor = Color.Red;
                                BuildLabel.Text = Downloader.Manifest.VirtualPath;
                                ProgressBar.Style = ProgressBarStyle.Continuous;
                                ProgressBar.Value = 0;
                                PlayButton.Image = global::BuildSync.Client.Properties.Resources.appbar_control_play;
                                break;
                            }
                        default:
                            {
                                EtaLabel.Text = "-";
                                BuildLabel.Text = Downloader.Manifest == null ? "-" : Downloader.Manifest.VirtualPath;
                                ProgressBar.Style = ProgressBarStyle.Continuous;
                                ProgressBar.Value = (int)(Downloader.Progress * 100);
                                PlayButton.Image = global::BuildSync.Client.Properties.Resources.appbar_control_play;
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
                                EtaLabel.Text = "-";
                                BuildLabel.Text = Downloader.Manifest.VirtualPath;
                                ProgressBar.Style = ProgressBarStyle.Continuous;
                                ProgressBar.Value = 100;
                                PlayButton.Image = global::BuildSync.Client.Properties.Resources.appbar_rocket_rotated_45;
                                break;
                            }
                        case ManifestDownloadProgressState.Initializing:
                            {
                                EtaLabel.Text = "Initializing";
                                BuildLabel.Text = Downloader.Manifest.VirtualPath;
                                ProgressBar.Style = ProgressBarStyle.Marquee;
                                ProgressBar.Value = 0;
                                PlayButton.Image = global::BuildSync.Client.Properties.Resources.appbar_control_pause;
                                break;
                            }
                        case ManifestDownloadProgressState.Validating:
                            {
                                EtaLabel.Text = "Validating";
                                BuildLabel.Text = Downloader.Manifest.VirtualPath;
                                ProgressBar.Style = ProgressBarStyle.Marquee;
                                ProgressBar.Value = 0;
                                PlayButton.Image = global::BuildSync.Client.Properties.Resources.appbar_control_pause;
                                break;
                            }
                        case ManifestDownloadProgressState.InitializeFailed:
                            {
                                EtaLabel.Text = "Disk Error";
                                EtaLabel.ForeColor = Color.Red;
                                BuildLabel.Text = Downloader.Manifest.VirtualPath;
                                ProgressBar.Style = ProgressBarStyle.Continuous;
                                ProgressBar.Value = 0;
                                PlayButton.Image = global::BuildSync.Client.Properties.Resources.appbar_control_pause;
                                break;
                            }
                        case ManifestDownloadProgressState.Downloading:
                            {
                                long SecondsToDownload = (long)((double)Downloader.BytesRemaining / (double)Downloader.BandwidthStats.RateIn);
                                if (Downloader.BandwidthStats.RateIn == 0)
                                {
                                    if (Program.NetClient.IsConnected)
                                    {
                                        EtaLabel.Text = "Calculating";
                                        BuildLabel.Text = "Calculating";
                                    }
                                    else
                                    {
                                        EtaLabel.Text = "No Connection";
                                        EtaLabel.ForeColor = Color.Red;
                                        BuildLabel.Text = "Unknown";
                                    }
                                }
                                else
                                {
                                    EtaLabel.Text = StringUtils.FormatAsDuration(SecondsToDownload);
                                }

                                BuildLabel.Text = Downloader.Manifest.VirtualPath;
                                ProgressBar.Style = ProgressBarStyle.Continuous;
                                ProgressBar.Value = (int)(Downloader.Progress * 100);
                                PlayButton.Image = global::BuildSync.Client.Properties.Resources.appbar_control_pause;
                                break;
                            }
                        case ManifestDownloadProgressState.RetrievingManifest:
                            {
                                if (Program.NetClient.IsConnected)
                                {
                                    EtaLabel.Text = "Calculating";
                                    BuildLabel.Text = "Calculating";
                                }
                                else
                                {
                                    EtaLabel.Text = "No Connection";
                                    EtaLabel.ForeColor = Color.Red;
                                    BuildLabel.Text = "Unknown";
                                }
                                ProgressBar.Style = ProgressBarStyle.Marquee;
                                ProgressBar.Value = 0;
                                PlayButton.Image = global::BuildSync.Client.Properties.Resources.appbar_control_pause;
                                break;
                            }
                    }
                }

                UploadSpeedLabel.Text = StringUtils.FormatAsTransferRate(Downloader.BandwidthStats.RateOut);
                DownloadSpeedLabel.Text = StringUtils.FormatAsTransferRate(Downloader.BandwidthStats.RateIn);
            }
            else
            {
                EtaLabel.Text = "Calculating";
                BuildLabel.Text = "Calculating";
                ProgressBar.Value = 0;
                ProgressBar.Style = ProgressBarStyle.Marquee;
                PlayButton.Image = global::BuildSync.Client.Properties.Resources.appbar_control_pause;

                UploadSpeedLabel.Text = "0 kb/s";
                DownloadSpeedLabel.Text = "0 kb/s";
            }

            SettingsButton.Enabled = (Program.NetClient.IsConnected);
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
                                // TODO: Launch
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
        }
    }
}
