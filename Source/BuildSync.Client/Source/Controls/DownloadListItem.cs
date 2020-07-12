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
using System.Drawing;
using System.Windows.Forms;
using BuildSync.Client.Forms;
using BuildSync.Core.Downloads;
using BuildSync.Core.Utils;
using BuildSync.Core.Controls;

namespace BuildSync.Client.Controls
{
    /// <summary>
    /// </summary>
    public partial class DownloadListItem : UserControl
    {
        /// <summary>
        /// </summary>
        private enum LaunchOption
        {
            Pause,
            Resume,
            Launch
        }

        /// <summary>
        /// </summary>
        private enum StateColoring
        {
            Success,
            Error,
            Warning,
            Info
        }

        /// <summary>
        /// </summary>
        public DownloadState State;

        /// <summary>
        /// </summary>
        private bool Collapsed = true;

        /// <summary>
        /// </summary>
        private bool InternalSelected;
        
        /// <summary>
        /// 
        /// </summary>
        private bool SkipDiskAllocation = false;

        /// <summary>
        /// 
        /// </summary>
        private bool SkipValidation = false;

        /// <summary>
        /// 
        /// </summary>
        private bool InstallAutomatically = false;

        /// <summary>
        /// 
        /// </summary>
        private SegmentedProgressBar.Segment AllocatingSegment = null;

        /// <summary>
        /// 
        /// </summary>
        private SegmentedProgressBar.Segment CopyingSegment = null;

        /// <summary>
        /// 
        /// </summary>
        private SegmentedProgressBar.Segment DowloadingSegment = null;

        /// <summary>
        /// 
        /// </summary>
        private SegmentedProgressBar.Segment ValidatingSegment = null;
        
        /// <summary>
        /// 
        /// </summary>
        private SegmentedProgressBar.Segment InstallingSegment = null;

        /// <summary>
        /// 
        /// </summary>
        public bool Selected
        {
            get => InternalSelected;
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
        /// </summary>
        public DownloadListItem()
        {
            InitializeComponent();

            WindowUtils.EnableDoubleBuffering(MainPanel);
        }

        /// <summary>
        /// 
        /// </summary>
        public void UpdateProgressSegments()
        {
            if (SkipDiskAllocation == Program.Settings.SkipDiskAllocation &&
                SkipValidation == Program.Settings.SkipValidation &&
                InstallAutomatically == State.InstallAutomatically &&
                MainProgressBar.Segments.Count != 0)
            {
                return;
            }

            SkipDiskAllocation = Program.Settings.SkipDiskAllocation;
            SkipValidation = Program.Settings.SkipValidation;
            InstallAutomatically = State.InstallAutomatically;

            MainProgressBar.Segments.Clear();

            if (!SkipDiskAllocation)
            {
                AllocatingSegment = new SegmentedProgressBar.Segment() { Proportion = 1.0f, Text = "Disk Allocation", Color = Color.LightGreen, Progress = 0.0f };
                MainProgressBar.Segments.Add(AllocatingSegment);
            }
            else
            {
                AllocatingSegment = null;
            }

            CopyingSegment = new SegmentedProgressBar.Segment() { Proportion = 1.0f, Text = "Copy Files", Color = Color.LightGreen, Progress = 0.0f };
            MainProgressBar.Segments.Add(CopyingSegment);

            DowloadingSegment = new SegmentedProgressBar.Segment() { Proportion = 3.0f, Text = "Download", Color = Color.LightGreen, Progress = 0.0f };
            MainProgressBar.Segments.Add(DowloadingSegment);

            if (!SkipValidation)
            {
                ValidatingSegment = new SegmentedProgressBar.Segment() { Proportion = 1.0f, Text = "Validate", Color = Color.LightGreen, Progress = 0.0f };
                MainProgressBar.Segments.Add(ValidatingSegment);
            }
            else
            {
                ValidatingSegment = null;
            }

            if (InstallAutomatically)
            {
                InstallingSegment = new SegmentedProgressBar.Segment() { Proportion = 0.5f, Text = "Install", Color = Color.LightGreen, Progress = 0.0f };
                MainProgressBar.Segments.Add(InstallingSegment);
            }
            else
            {
                InstallingSegment = null;
            }
        }

        /// <summary>
        /// </summary>
        public void RefreshState()
        {
            if (State.Name != NameLabel.Text)
            {
                NameLabel.Text = State.Name;
            }

            blockStatusPanel.State = State;

            UpdateProgressSegments();

            ManifestDownloadState Downloader = Program.ManifestDownloadManager.GetDownload(State.ActiveManifestId);

            Color ErrorColor = Color.FromArgb(255, 238, 114, 76); // Yellow
            Color InProgressColor = Color.FromArgb(255, 103, 182, 234); // Blue
            Color FinishedColor = Color.FromArgb(255, 128, 216, 127); // Green

            // Update allocation progress.
            if (AllocatingSegment != null && Downloader != null)
            {
                switch (Downloader.State)
                {
                    case ManifestDownloadProgressState.DiskError:
                    case ManifestDownloadProgressState.InitializeFailed:
                        {
                            AllocatingSegment.Progress = 1.0f;
                            AllocatingSegment.Color = ErrorColor;
                            break;
                        }
                    case ManifestDownloadProgressState.RetrievingManifest:
                    case ManifestDownloadProgressState.Initializing:
                        {
                            AllocatingSegment.Progress = Downloader.InitializeProgress;
                            AllocatingSegment.Color = InProgressColor;
                            break;
                        }
                    case ManifestDownloadProgressState.DeltaCopying:
                    case ManifestDownloadProgressState.Downloading:
                    case ManifestDownloadProgressState.Complete:
                    case ManifestDownloadProgressState.Validating:
                    case ManifestDownloadProgressState.Installing:
                    case ManifestDownloadProgressState.ValidationFailed:
                    case ManifestDownloadProgressState.InstallFailed:
                        {
                            AllocatingSegment.Progress = 1.0f;
                            AllocatingSegment.Color = FinishedColor;
                            break;
                        }
                }
            }

            // Update copying progress.
            if (CopyingSegment != null && Downloader != null)
            {
                switch (Downloader.State)
                {
                    case ManifestDownloadProgressState.RetrievingManifest:
                    case ManifestDownloadProgressState.Initializing:
                    case ManifestDownloadProgressState.DiskError:
                    case ManifestDownloadProgressState.InitializeFailed:
                        {
                            CopyingSegment.Progress = 0.0f;
                            CopyingSegment.Marquee = false;
                            break;
                        }
                    case ManifestDownloadProgressState.DeltaCopying:
                        {
                            CopyingSegment.Progress = Downloader.DeltaCopyProgress;
                            CopyingSegment.Marquee = (Downloader.DeltaCopyProgress <= 0.01f);
                            CopyingSegment.Color = InProgressColor;
                            break;
                        }
                    case ManifestDownloadProgressState.Downloading:
                    case ManifestDownloadProgressState.Complete:
                    case ManifestDownloadProgressState.Validating:
                    case ManifestDownloadProgressState.Installing:
                    case ManifestDownloadProgressState.ValidationFailed:
                    case ManifestDownloadProgressState.InstallFailed:
                        {
                            CopyingSegment.Progress = 1.0f;
                            CopyingSegment.Marquee = false;
                            CopyingSegment.Color = FinishedColor;
                            break;
                        }
                }
            }

            // Update downloading progress.
            if (DowloadingSegment != null && Downloader != null)
            {
                switch (Downloader.State)
                {
                    case ManifestDownloadProgressState.RetrievingManifest:
                    case ManifestDownloadProgressState.Initializing:
                    case ManifestDownloadProgressState.InitializeFailed:
                    case ManifestDownloadProgressState.DeltaCopying:
                        {
                            DowloadingSegment.Text = "Download";
                            DowloadingSegment.Progress = 0.0f;
                            break;
                        }
                    case ManifestDownloadProgressState.DiskError:
                        {
                            DowloadingSegment.Text = "Download";
                            DowloadingSegment.Progress = Downloader.Progress;
                            DowloadingSegment.Color = ErrorColor;
                            break;
                        }
                    case ManifestDownloadProgressState.Downloading:
                        {
                            DowloadingSegment.Text = string.Format("{0} / {1}", StringUtils.FormatAsSize(Downloader.BytesTotal - Downloader.BytesRemaining), StringUtils.FormatAsSize(Downloader.BytesTotal)); 
                            DowloadingSegment.Progress = Downloader.Progress;
                            DowloadingSegment.Color = InProgressColor;
                            break;
                        }
                    case ManifestDownloadProgressState.Complete:
                    case ManifestDownloadProgressState.Validating:
                    case ManifestDownloadProgressState.Installing:
                    case ManifestDownloadProgressState.ValidationFailed:
                    case ManifestDownloadProgressState.InstallFailed:
                        {
                            DowloadingSegment.Text = "Download";
                            DowloadingSegment.Progress = 1.0f;
                            DowloadingSegment.Color = FinishedColor;
                            break;
                        }
                }
            }

            // Update validating progress.
            if (ValidatingSegment != null && Downloader != null)
            {
                switch (Downloader.State)
                {
                    case ManifestDownloadProgressState.RetrievingManifest:
                    case ManifestDownloadProgressState.Initializing:
                    case ManifestDownloadProgressState.DiskError:
                    case ManifestDownloadProgressState.InitializeFailed:
                    case ManifestDownloadProgressState.DeltaCopying:
                    case ManifestDownloadProgressState.Downloading:
                        {
                            ValidatingSegment.Progress = 0.0f;
                            break;
                        }
                    case ManifestDownloadProgressState.ValidationFailed:
                        {
                            ValidatingSegment.Progress = 1.0f;
                            ValidatingSegment.Color = ErrorColor;
                            break;
                        }
                    case ManifestDownloadProgressState.Validating:
                        {
                            ValidatingSegment.Progress = Downloader.ValidateProgress;
                            ValidatingSegment.Color = InProgressColor;
                            break;
                        }
                    case ManifestDownloadProgressState.Complete:
                    case ManifestDownloadProgressState.Installing:
                    case ManifestDownloadProgressState.InstallFailed:
                        {
                            ValidatingSegment.Progress = 1.0f;
                            ValidatingSegment.Color = FinishedColor;
                            break;
                        }
                }
            }

            // Update installing progress.
            if (InstallingSegment != null && Downloader != null)
            {
                switch (Downloader.State)
                {
                    case ManifestDownloadProgressState.RetrievingManifest:
                    case ManifestDownloadProgressState.Initializing:
                    case ManifestDownloadProgressState.DiskError:
                    case ManifestDownloadProgressState.InitializeFailed:
                    case ManifestDownloadProgressState.DeltaCopying:
                    case ManifestDownloadProgressState.Downloading:
                    case ManifestDownloadProgressState.ValidationFailed:
                    case ManifestDownloadProgressState.Validating:
                        {
                            InstallingSegment.Progress = 0.0f;
                            InstallingSegment.Marquee = false;
                            break;
                        }
                    case ManifestDownloadProgressState.InstallFailed:
                        {
                            InstallingSegment.Progress = 1.0f;
                            InstallingSegment.Color = ErrorColor;
                            InstallingSegment.Marquee = false;
                            break;
                        }
                    case ManifestDownloadProgressState.Installing:
                        {
                            InstallingSegment.Progress = Downloader.InstallProgress;
                            InstallingSegment.Color = InProgressColor;
                            InstallingSegment.Marquee = Downloader.InstallProgress < 0.0f;
                            break;
                        }
                    case ManifestDownloadProgressState.Complete:
                        {
                            InstallingSegment.Progress = 1.0f;
                            InstallingSegment.Color = FinishedColor;
                            InstallingSegment.Marquee = false;
                            break;
                        }
                }
            }

            // Update status.
            string Status = "";
            string BuildInfo = "Unknown";
            StateColoring StatusColor = StateColoring.Info;
            if (Downloader != null)
            {
                if (Downloader.Paused)
                {
                    switch (Downloader.State)
                    {
                        case ManifestDownloadProgressState.DiskError:
                        case ManifestDownloadProgressState.InitializeFailed:
                        {
                            Status = "Disk Error";         
                            StatusColor = StateColoring.Error;
                            BuildInfo = Downloader.Manifest.VirtualPath;
                            break;
                        }
                        case ManifestDownloadProgressState.ValidationFailed:
                        {
                            Status = "Validation Error";         
                            StatusColor = StateColoring.Error;
                            BuildInfo = Downloader.Manifest.VirtualPath;
                            break;
                        }
                        case ManifestDownloadProgressState.InstallFailed:
                        {
                            Status = "Install Error";         
                            StatusColor = StateColoring.Error;
                            BuildInfo = Downloader.Manifest.VirtualPath;
                            break;
                        }
                        default:
                        {
                            Status = "-";         
                            StatusColor = StateColoring.Info;
                            BuildInfo = Downloader.Manifest == null ? "-" : Downloader.Manifest.VirtualPath;
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
                            Status = "Complete";         
                            StatusColor = StateColoring.Success;
                            BuildInfo = Downloader.Manifest.VirtualPath;
                            break;
                        }
                        case ManifestDownloadProgressState.Initializing:
                        case ManifestDownloadProgressState.DeltaCopying:
                        case ManifestDownloadProgressState.Validating:
                        case ManifestDownloadProgressState.Installing:
                        {
                            long TimeRemaining = Program.DownloadManager.GetEstimatedTimeRemainingForState(State, Downloader.State);
                            /*if (State.DurationHistory.Count > 0)
                            {
                                long TotalTimeRemaining = Program.DownloadManager.GetEstimatedTimeRemaining(State);
                                Status = string.Format("{0} (Previous {1})", StringUtils.FormatAsDuration(TimeRemaining), StringUtils.FormatAsDuration(TotalTimeRemaining));
                            }
                            else
                            {
                                Status = string.Format("{0}", StringUtils.FormatAsDuration(TimeRemaining));
                            }*/

                            Status = string.Format("{0}", StringUtils.FormatAsTextDuration(TimeRemaining));

                            StatusColor = StateColoring.Info;
                            BuildInfo = Downloader.Manifest.VirtualPath;
                            break;
                        }
                        case ManifestDownloadProgressState.Downloading:
                        {
                            if (Downloader.BandwidthStats.RateIn == 0 )
                            {
                                if (Program.NetClient.IsConnected)
                                {
                                    int Blocks = Program.NetClient.GetRequestedBlocksForDownload(Downloader.ManifestId);
                                    if (Blocks > 0)
                                    {
                                        Status = string.Format("Slow Download ({0} Blocks)", Blocks);
                                    }
                                    else
                                    {
                                        Status = "Locating Data";
                                    }
                                }
                                else
                                {
                                    Status = "No Connection";
                                    StatusColor = StateColoring.Error;
                                }
                            }
                            else
                            {
                                long TimeRemaining = Program.DownloadManager.GetEstimatedTimeRemainingForState(State, Downloader.State);
                                /*if (State.DurationHistory.Count > 0)
                                {
                                    long TotalTimeRemaining = Program.DownloadManager.GetEstimatedTimeRemaining(State);
                                    Status = string.Format("{0} (Previous {1})", StringUtils.FormatAsDuration(TimeRemaining), StringUtils.FormatAsDuration(TotalTimeRemaining));
                                }
                                else
                                {
                                    Status = string.Format("{0}", StringUtils.FormatAsDuration(TimeRemaining));
                                }*/
                                
                                Status = string.Format("{0}", StringUtils.FormatAsTextDuration(TimeRemaining));
                            }

                            BuildInfo = Downloader.Manifest.VirtualPath;
                            break;
                        }
                        case ManifestDownloadProgressState.RetrievingManifest:
                        {
                            BuildInfo = "Locating";
                            if (Program.NetClient.IsConnected)
                            {
                                Status = "Locating Blocks";
                            }
                            else
                            {
                                Status = "No Connection";
                                StatusColor = StateColoring.Error;
                            }
                            break;
                        }
                    }
                }
            }
            else
            {
                if (Program.NetClient.IsConnected)
                {
                    Status = "Waiting for available download";
                }
                else
                {
                    Status = "No Connection";
                    StatusColor = StateColoring.Error;
                }
            }

            // Apply status.
            SetStatus(Status, StatusColor, BuildInfo);

            // Update play icons.
            LaunchOption LaunchType = State.Paused ? LaunchOption.Resume : LaunchOption.Pause;
            if (Downloader != null)
            {
                switch (Downloader.State)
                {
                    case ManifestDownloadProgressState.DiskError:
                    case ManifestDownloadProgressState.InitializeFailed:
                    case ManifestDownloadProgressState.ValidationFailed:
                    case ManifestDownloadProgressState.InstallFailed:
                        {
                            LaunchType = LaunchOption.Resume;
                            break;
                        }
                    case ManifestDownloadProgressState.Complete:
                        {
                            LaunchType = LaunchOption.Launch;
                            break;
                        }
                    case ManifestDownloadProgressState.Initializing:
                    case ManifestDownloadProgressState.DeltaCopying:
                    case ManifestDownloadProgressState.Validating:
                    case ManifestDownloadProgressState.Installing:
                    case ManifestDownloadProgressState.Downloading:
                    case ManifestDownloadProgressState.RetrievingManifest:
                    default:
                        {
                            LaunchType = LaunchOption.Pause;
                            break;
                        }
                }
            }

            if (Downloader != null && Downloader.Paused)
            {
                LaunchType = LaunchOption.Resume;
            }

            int ImageIndex = 0;
            if      (LaunchType == LaunchOption.Launch) ImageIndex = 0;
            else if (LaunchType == LaunchOption.Resume) ImageIndex = 3;
            else if (LaunchType == LaunchOption.Pause)  ImageIndex = 4;

            if (PlayButton.ImageIndex != ImageIndex)
            {
                PlayButton.ImageIndex = ImageIndex;
            }

            // Update download rate.
            string UpRate = StringUtils.FormatAsTransferRate(Downloader == null ? 0 : Downloader.BandwidthStats.RateOut);
            string DownRate = StringUtils.FormatAsTransferRate(Downloader == null ? 0 : Downloader.BandwidthStats.RateIn);

            if (DownloadSpeedLabel.Text != DownRate)
            {
                DownloadSpeedLabel.Text = DownRate;
            }
            if (UploadSpeedLabel.Text != UpRate)
            {
                UploadSpeedLabel.Text = UpRate;
            }

            // Only allow changing settings when connected to server.
            if (SettingsButton.Enabled != Program.NetClient.IsConnected)
            {
                SettingsButton.Enabled = Program.NetClient.IsConnected;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="State"></param>
        /// <param name="Build"></param>
        /// <param name="Progress"></param>
        private void SetStatus(string State, StateColoring Coloring, string Build)
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
                //ProgressBar.SetState(2);
            }
            else if (Coloring == StateColoring.Info)
            {
                TargetColor = Color.Black;
                //ProgressBar.SetState(1);
            }
            else if (Coloring == StateColoring.Warning)
            {
                TargetColor = Color.Orange;
                //ProgressBar.SetState(3);
            }
            else if (Coloring == StateColoring.Success)
            {
                TargetColor = Color.Green;
                //ProgressBar.SetState(1);
            }

            if (EtaLabel.ForeColor != TargetColor)
            {
                EtaLabel.ForeColor = TargetColor;
            }
        }

        /// <summary>
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

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CollapseButtonClicked(object sender, EventArgs e)
        {
            Collapsed = !Collapsed;
            blockStatusPanel.Active = !Collapsed;
            if (Collapsed)
            {
                Size = new Size(610, 50);
            }
            else
            {
                Size = new Size(610, 80);
            }
        }

        /// <summary>
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
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainPanel_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, MainPanel.ClientRectangle, SystemColors.ControlLight, ButtonBorderStyle.Solid);
        }

        /// <summary>
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
                            using (LaunchForm Form = new LaunchForm())
                            {
                                Form.Downloader = Downloader;
                                Form.DownloadState = State;
                                if (Form.Init())
                                {
                                    Form.ShowDialog();
                                }
                            }
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
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SettingsClicked(object sender, EventArgs e)
        {
            using (AddDownloadForm Dialog = new AddDownloadForm())
            {
                Dialog.EditState = State;
                Dialog.ShowDialog(this);
            }
        }
    }
}