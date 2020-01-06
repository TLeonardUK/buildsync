using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BuildSync.Core.Networking;
using BuildSync.Core.Networking.Messages;
using BuildSync.Core.Downloads;
using BuildSync.Core.Utils;
using BuildSync.Core.Users;
using BuildSync.Client.Controls;

namespace BuildSync.Client.Forms
{
    /// <summary>
    /// 
    /// </summary>
    public partial class MainForm : Form
    {
        #region Fields

        /// <summary>
        /// 
        /// </summary>
        private bool ForcedExit = false;

        /// <summary>
        /// 
        /// </summary>
        private ConsoleForm ConsoleOutputForm = null;

        /// <summary>
        /// 
        /// </summary>
        private DownloadListItem ContextMenuDownloadItem = null;

        #endregion
        #region Methods

        /// <summary>
        /// 
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Invoked when the form is loaded.
        /// </summary>
        /// <param name="sender">Object that invoked this event.</param>
        /// <param name="e">Event specific arguments.</param>
        private void FormLoaded(object sender, EventArgs e)
        {
        }

        /// <summary>
        ///     Invoked when the form is shown.
        /// </summary>
        /// <param name="sender">Object that invoked this event.</param>
        /// <param name="e">Event specific arguments.</param>
        private void FormShown(object sender, EventArgs e)
        {
            RefreshState();

            if (Program.Settings.FirstRun)
            {
                if (MessageBox.Show(this, "Server connection information has not yet been set.\n\nWould you like to view settings and configure?", "No Server Configured", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    PreferencesForm form = new PreferencesForm();
                    form.ShowDialog(this);
                }

                Program.Settings.FirstRun = false;
                Program.SaveSettings();
            }
        }

        /// <summary>
        ///     Invoked when the user clicks the add download button.
        /// </summary>
        /// <param name="sender">Object that invoked this event.</param>
        /// <param name="e">Event specific arguments.</param>
        private void AddDownloadClicked(object sender, EventArgs e)
        {
            (new AddDownloadForm()).ShowDialog(this);
        }

        /// <summary>
        ///     Invoked when the user clicks the publish build button.
        /// </summary>
        /// <param name="sender">Object that invoked this event.</param>
        /// <param name="e">Event specific arguments.</param>
        private void PublishBuildClicked(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ManageBuildsClicked(object sender, EventArgs e)
        {
            (new ManageBuildsForm()).ShowDialog(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ManageUsersClicked(object sender, EventArgs e)
        {
            (new ManageUsersForm()).ShowDialog(this);
        }

        /// <summary>
        ///     Invoked when the user clicks the settings menu item.
        /// </summary>
        /// <param name="sender">Object that invoked this event.</param>
        /// <param name="e">Event specific arguments.</param>
        private void PreferencesClicked(object sender, EventArgs e)
        {
            (new PreferencesForm()).ShowDialog(this);
        }

        /// <summary>
        ///     Invoked when the user clicks the quit menu item.
        /// </summary>
        /// <param name="sender">Object that invoked this event.</param>
        /// <param name="e">Event specific arguments.</param>
        private void QuitClicked(object sender, EventArgs e)
        {
            ForcedExit = true;
            Application.Exit();
        }

        /// <summary>
        ///     Invoked when the user clicks the about menu item.
        /// </summary>
        /// <param name="sender">Object that invoked this event.</param>
        /// <param name="e">Event specific arguments.</param>
        private void AboutClicked(object sender, EventArgs e)
        {
            (new AboutForm()).ShowDialog(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ViewConsoleClicked(object sender, EventArgs e)
        {
            if (ConsoleOutputForm != null)
            {
                ConsoleOutputForm.Show(this);
            }
            else
            {
                ConsoleOutputForm = new ConsoleForm();
                ConsoleOutputForm.Show(this);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ViewPeersClicked(object sender, EventArgs e)
        {
            PeersForm form = new PeersForm();
            form.Show(this);
        }

        /// <summary>
        ///     Invoked when the user clicks the notify icon
        /// </summary>
        /// <param name="sender">Object that invoked this event.</param>
        /// <param name="e">Event specific arguments.</param>
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (ForcedExit)
            {
                return;
            }

            if (Program.Settings.MinimizeToTrayOnClose)
            {
                e.Cancel = true;
                this.WindowState = FormWindowState.Minimized;
            }
            else
            {
                e.Cancel = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClientSizeHasChanged(object sender, EventArgs e)
        {
            ShowInTaskbar = (WindowState != FormWindowState.Minimized);
        }

        /// <summary>
        ///     Invoked when the user clicks the notify icon
        /// </summary>
        /// <param name="sender">Object that invoked this event.</param>
        /// <param name="e">Event specific arguments.</param>
        private void NotifyIconClicked(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.WindowState = FormWindowState.Normal;
                this.BringToFront();
                this.Focus();
            }
        }

        /// <summary>
        ///     Invoked when the user clicks the pause all button
        /// </summary>
        /// <param name="sender">Object that invoked this event.</param>
        /// <param name="e">Event specific arguments.</param>
        private void PauseAllClicked(object sender, EventArgs e)
        {
            foreach (DownloadState State in Program.DownloadManager.States.States)
            {
                ManifestDownloadState Downloader = Program.ManifestDownloadManager.GetDownload(State.ActiveManifestId);
                if (Downloader != null && Downloader.State == ManifestDownloadProgressState.Downloading)
                {
                    State.Paused = true;
                }
            }
        }

        /// <summary>
        ///     Invoked when the user clicks the resume all button
        /// </summary>
        /// <param name="sender">Object that invoked this event.</param>
        /// <param name="e">Event specific arguments.</param>
        private void ResumeAllClicked(object sender, EventArgs e)
        {
            foreach (DownloadState State in Program.DownloadManager.States.States)
            {
                State.Paused = false;
            }
        }

        /// <summary>
        ///     Invoked when the update timer runs.
        /// </summary>
        /// <param name="sender">Object that invoked this event.</param>
        /// <param name="e">Event specific arguments.</param>
        private void UpdateTimerTick(object sender, EventArgs e)
        {
            RefreshState();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ForceRevalidateClicked(object sender, EventArgs e)
        {
            ManifestDownloadState Downloader = Program.ManifestDownloadManager.GetDownload(ContextMenuDownloadItem.State.ActiveManifestId);
            if (Downloader != null)
            {
                ContextMenuDownloadItem.State.Paused = false;
                Program.ManifestDownloadManager.ValidateDownload(Downloader.ManifestId);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ForceRedownloadClicked(object sender, EventArgs e)
        {
            ManifestDownloadState Downloader = Program.ManifestDownloadManager.GetDownload(ContextMenuDownloadItem.State.ActiveManifestId);
            if (Downloader != null)
            {
                ContextMenuDownloadItem.State.Paused = false;
                Program.ManifestDownloadManager.RestartDownload(Downloader.ManifestId);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ForceReinstallClicked(object sender, EventArgs e)
        {
            ManifestDownloadState Downloader = Program.ManifestDownloadManager.GetDownload(ContextMenuDownloadItem.State.ActiveManifestId);
            if (Downloader != null)
            {
                ContextMenuDownloadItem.State.Paused = false;
                Program.ManifestDownloadManager.InstallDownload(Downloader.ManifestId);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DownloadListContextMenuShowing(object sender, CancelEventArgs e)
        {
            ContextMenuDownloadItem = mainDownloadList.SelectedItem;
            if (ContextMenuDownloadItem != null)
            {
                ManifestDownloadState Downloader = Program.ManifestDownloadManager.GetDownload(ContextMenuDownloadItem.State.ActiveManifestId);

                forceRedownloadToolStripMenuItem.Enabled = (Downloader != null && Downloader.State != ManifestDownloadProgressState.Validating && Downloader.State != ManifestDownloadProgressState.Initializing && Downloader.State != ManifestDownloadProgressState.Installing);
                forceRevalidateToolStripMenuItem.Enabled = (Downloader != null && (Downloader.State == ManifestDownloadProgressState.Complete || Downloader.State == ManifestDownloadProgressState.ValidationFailed));
                forceReinstallToolStripMenuItem.Enabled = (Downloader != null && (Downloader.State == ManifestDownloadProgressState.Complete || Downloader.State == ManifestDownloadProgressState.InstallFailed));
            }
            else
            {
                forceRedownloadToolStripMenuItem.Enabled = false;
                forceRevalidateToolStripMenuItem.Enabled = false;
                forceReinstallToolStripMenuItem.Enabled = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void RefreshState()
        {
            bool Connected = Program.NetClient.IsReadyForData;

            if (!Connected)
            {
                if (Program.NetClient.IsConnected || Program.NetClient.IsConnecting)
                {
                    peerCountLabel.Text = "Connecting to '" + Program.Settings.ServerHostname + ":" + Program.Settings.ServerPort + "'";

                    peerCountLabel.ForeColor = Color.DarkCyan;
                }
                else
                {
                    switch (Program.NetClient.HandshakeResult)
                    {
                        case HandshakeResultType.InvalidVersion:
                            {
                                peerCountLabel.Text = "Version is incompatible, please update application.";
                                break;
                            }
                        default:
                            {
                                peerCountLabel.Text = "Unable to connect to server '" + Program.Settings.ServerHostname + ":" + Program.Settings.ServerPort + "'";
                                break;
                            }
                    }

                    peerCountLabel.ForeColor = Color.Red;
                }
            }
            else if (!Program.AreDownloadsAllowed())
            {
                peerCountLabel.Text = "Traffic disabled by bandwidth settings";
                peerCountLabel.ForeColor = Color.Orange;
            }
            else
            {
                peerCountLabel.Text = Program.NetClient.PeerCount + " peers connected";
                peerCountLabel.ForeColor = Color.Black;
            }

            //publishBuildToolStripMenuItem.Enabled = Connected;
            manageBuildsToolStripMenuItem.Text = "Manage builds ...";
            manageBuildsToolStripMenuItem.Enabled = Connected;
            if (Connected)
            {
                if (!Program.NetClient.Permissions.HasPermission(UserPermissionType.ManageBuilds, ""))
                {
                    manageBuildsToolStripMenuItem.Enabled = false;
                    manageBuildsToolStripMenuItem.Text = "Manage builds (Permission Required)";
                }
            }

            manageUsersToolStripMenuItem.Text = "Manage users ...";
            manageUsersToolStripMenuItem.Enabled = Connected;
            if (Connected)
            {
                if (!Program.NetClient.Permissions.HasPermission(UserPermissionType.ManageUsers, ""))
                {
                    manageUsersToolStripMenuItem.Enabled = false;
                    manageUsersToolStripMenuItem.Text = "Manage users (Permission Required)";
                }
            }

            addDownloadToolStripMenuItem.Enabled = Connected;
            addDownloadToolStripMenuItem1.Enabled = Connected;

            totalUpBandwidthLabel.Text = string.Format("{0} ({1})", StringUtils.FormatAsTransferRate(NetConnection.GlobalBandwidthStats.RateOut), StringUtils.FormatAsSize(NetConnection.GlobalBandwidthStats.TotalOut));
            totalDownBandwidthLabel.Text = string.Format("{0} ({1})", StringUtils.FormatAsTransferRate(NetConnection.GlobalBandwidthStats.RateIn), StringUtils.FormatAsSize(NetConnection.GlobalBandwidthStats.TotalIn));

            if (Program.IOQueue.QueuedOut > 0)
            {
                totalDiskUpBandwidthLabel.Text = string.Format("{0} (Q {1})", StringUtils.FormatAsTransferRate(Program.IOQueue.BandwidthStats.RateOut), StringUtils.FormatAsSize(Program.IOQueue.QueuedOut));
            }
            else
            {
                totalDiskUpBandwidthLabel.Text = string.Format("{0} ", StringUtils.FormatAsTransferRate(Program.IOQueue.BandwidthStats.RateOut));
            }

            if (Program.IOQueue.QueuedIn > 0)
            {
                totalDiskDownBandwidthLabel.Text = string.Format("{0} (Q {1})", StringUtils.FormatAsTransferRate(Program.IOQueue.BandwidthStats.RateIn), StringUtils.FormatAsSize(Program.IOQueue.QueuedIn));
            }
            else
            {
                totalDiskDownBandwidthLabel.Text = string.Format("{0}", StringUtils.FormatAsTransferRate(Program.IOQueue.BandwidthStats.RateIn));
            }
        }

        #endregion
    }
}
