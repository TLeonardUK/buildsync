using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using BuildSync.Client.Tasks;
using BuildSync.Core;
using BuildSync.Core.Networking;
using BuildSync.Core.Networking.Messages;
using BuildSync.Core.Downloads;
using BuildSync.Core.Utils;
using BuildSync.Core.Users;
using BuildSync.Client.Controls;
using WeifenLuo.WinFormsUI.Docking;

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
        private StatisticsForm StatsForm = null;

        /// <summary>
        /// 
        /// </summary>
        private PeersForm PeersForm = null;

        /// <summary>
        /// 
        /// </summary>
        private ManifestsForm ManifestsForm = null;

        /// <summary>
        /// 
        /// </summary>
        private ManageUsersForm ManageUsersForm = null;

        /// <summary>
        /// 
        /// </summary>
        private ManageServerForm ManageServerForm = null;

        /// <summary>
        /// 
        /// </summary>
        private ManageBuildsForm ManageBuildsForm = null;

        /// <summary>
        /// 
        /// </summary>
        private DownloadListItem ContextMenuDownloadItem = null;

        /// <summary>
        /// 
        /// </summary>
        private DownloadList MainDownloadList = null;

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
            DockPanel.Theme = new VS2015LightTheme();

            MainDownloadList = new DownloadList();
            MainDownloadList.Show(DockPanel, DockState.Document);
            MainDownloadList.ContextMenuStrip = downloadListContextMenu;

            //ManifestsForm = new ManifestsForm();
            //ManifestsForm.Show(DockPanel, DockState.DockBottomAutoHide);
            //ManifestsForm.Hide();

            StatsForm = new StatisticsForm();
            StatsForm.Show(DockPanel, DockState.Document);

            MainDownloadList.Activate();

            //ConsoleOutputForm = new ConsoleForm();
            //ConsoleOutputForm.Show(DockPanel, DockState.DockBottomAutoHide);
            //ConsoleOutputForm.Hide();

            //PeersForm = new PeersForm();
            //PeersForm.Show(DockPanel, DockState.DockBottomAutoHide);
            //PeersForm.Hide();

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
            if (ManageBuildsForm == null)
            {
                ManageBuildsForm = new ManageBuildsForm();
            }
            ManageBuildsForm.Show(DockPanel, DockState.DockRight);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PushUpdateClicked(object sender, EventArgs e)
        {
            OpenFileDialog Dialog = new OpenFileDialog();
            Dialog.Title = "Select Update File";
            Dialog.Filter = "Installer File (*.msi)|*.msi|All files (*.*)|*.*";
            //Dialog.InitialDirectory = Environment.CurrentDirectory;
            Dialog.CheckFileExists = true;
            Dialog.ShowHelp = true;

            if (Dialog.ShowDialog(this) == DialogResult.OK)
            {
                BuildsRecievedHandler Handler = null;                
                Handler = (string RootPath, NetMessage_GetBuildsResponse.BuildInfo[] Builds) => {

                    Program.NetClient.OnBuildsRecieved -= Handler;

                    // Find the next sequential build index.
                    string NewVirtualPath = "";
                    for (int i = 0; true; i++)
                    {
                        NewVirtualPath = RootPath + "/" + i.ToString();

                        bool Exists = false;
                        foreach (NetMessage_GetBuildsResponse.BuildInfo Build in Builds)
                        {
                            if (Build.VirtualPath.ToUpper() == NewVirtualPath.ToUpper())
                            {
                                Exists = true;
                                break;
                            }
                        }

                        if (!Exists)
                        {
                            break;
                        }
                    }

                    // Publish a folder.
                    string TempFolder = FileUtils.GetTempDirectory();
                    File.Copy(Dialog.FileName, Path.Combine(TempFolder, "installer.msi"));

                    // Publish the build.
                    PublishBuildTask Publisher = new PublishBuildTask();
                    Publisher.Start(NewVirtualPath, TempFolder);

                    Task.Run(() => { 

                        bool PublishComplete = false;
                        while (!PublishComplete)
                        {
                            switch (Publisher.State)
                            {
                                case BuildPublishingState.CopyingFiles:
                                case BuildPublishingState.ScanningFiles:
                                case BuildPublishingState.UploadingManifest:
                                    {
                                        // Ignore
                                        break;
                                    }
                                case BuildPublishingState.Success:
                                    {
                                        this.Invoke((MethodInvoker)(() => { 
                                            Publisher.Commit(); 
                                        }));
                                        Publisher = null;
                                        PublishComplete = true;
                                        break;
                                    }
                                default:
                                    {
                                        Publisher = null;
                                        PublishComplete = true;
                                        break;
                                    }
                            }

                            Thread.Sleep(1000);
                        }

                    });
                }; 

                Program.NetClient.OnBuildsRecieved += Handler;
                Program.NetClient.RequestBuilds("$Internal$/Updates");
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ViewLicenseClicked(object sender, EventArgs e)
        {
            (new LicenseForm()).ShowDialog(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ViewHelpClickled(object sender, EventArgs e)
        {
            string ExePath = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            while (true)
            {
                string HelpDocs = Path.Combine(ExePath, "Docs/Build Sync Help.chm");
                Console.WriteLine("Trying: {0}", HelpDocs);
                if (File.Exists(HelpDocs))
                {
                    Process.Start(HelpDocs);
                    break;
                }
                else
                {
                    ExePath = Path.GetDirectoryName(ExePath);
                    if (ExePath == null || !ExePath.Contains('\\') && !ExePath.Contains('/'))
                    {
                        MessageBox.Show("Failed to find help chm file, installation may be corrupt.", "Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ManageUsersClicked(object sender, EventArgs e)
        {
            if (ManageUsersForm == null)
            {
                ManageUsersForm = new ManageUsersForm();
            }
            ManageUsersForm.Show(DockPanel, DockState.Document);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ServerManagerClicked(object sender, EventArgs e)
        {
            if (ManageServerForm == null)
            {
                ManageServerForm = new ManageServerForm();
            }
            ManageServerForm.Show(DockPanel, DockState.Document);
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
            if (ConsoleOutputForm == null)
            {
                ConsoleOutputForm = new ConsoleForm();
                ConsoleOutputForm.Show(DockPanel, DockState.DockBottom);
            }

            ConsoleOutputForm.Show();
            if (ConsoleOutputForm.DockState == DockState.DockLeftAutoHide ||
                ConsoleOutputForm.DockState == DockState.DockRightAutoHide ||
                ConsoleOutputForm.DockState == DockState.DockTopAutoHide ||
                ConsoleOutputForm.DockState == DockState.DockBottomAutoHide)
            {
                DockPanel.ActiveAutoHideContent = ConsoleOutputForm;
            }
            ConsoleOutputForm.Activate();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StatisticsClicked(object sender, EventArgs e)
        {
            StatsForm.Show();
            if (StatsForm.DockState == DockState.DockLeftAutoHide ||
                StatsForm.DockState == DockState.DockRightAutoHide ||
                StatsForm.DockState == DockState.DockTopAutoHide ||
                StatsForm.DockState == DockState.DockBottomAutoHide)
            {
                DockPanel.ActiveAutoHideContent = StatsForm;
            }
            StatsForm.Activate();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ViewPeersClicked(object sender, EventArgs e)
        {
            if (PeersForm == null)
            {
                PeersForm = new PeersForm();
                PeersForm.Show(DockPanel, DockState.DockBottom);
            }

            PeersForm.Show();
            if (PeersForm.DockState == DockState.DockLeftAutoHide ||
                PeersForm.DockState == DockState.DockRightAutoHide ||
                PeersForm.DockState == DockState.DockTopAutoHide ||
                PeersForm.DockState == DockState.DockBottomAutoHide)
            {
                DockPanel.ActiveAutoHideContent = PeersForm;
            }
            PeersForm.Activate();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ViewManifestsClicked(object sender, EventArgs e)
        {
            if (ManifestsForm == null)
            {
                ManifestsForm = new ManifestsForm();
                ManifestsForm.Show(DockPanel, DockState.DockBottom);
            }

            ManifestsForm.Show();
            if (ManifestsForm.DockState == DockState.DockLeftAutoHide ||
                ManifestsForm.DockState == DockState.DockRightAutoHide ||
                ManifestsForm.DockState == DockState.DockTopAutoHide ||
                ManifestsForm.DockState == DockState.DockBottomAutoHide)
            {
                DockPanel.ActiveAutoHideContent = ManifestsForm;
            }
            ManifestsForm.Activate();
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
                if (Downloader != null && Downloader.State == ManifestDownloadProgressState.Downloading && !State.IsInternal)
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
                if (!State.IsInternal)
                {
                    State.Paused = false;
                }
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
            ContextMenuDownloadItem = MainDownloadList.SelectedItem;
            if (ContextMenuDownloadItem != null)
            {
                ManifestDownloadState Downloader = Program.ManifestDownloadManager.GetDownload(ContextMenuDownloadItem.State.ActiveManifestId);

                forceRedownloadToolStripMenuItem.Enabled = (Downloader != null && Downloader.State != ManifestDownloadProgressState.Validating && Downloader.State != ManifestDownloadProgressState.Initializing && Downloader.State != ManifestDownloadProgressState.Installing);
                forceRevalidateToolStripMenuItem.Enabled = (Downloader != null && (Downloader.State == ManifestDownloadProgressState.Complete || Downloader.State == ManifestDownloadProgressState.ValidationFailed));
                forceReinstallToolStripMenuItem.Enabled = (Downloader != null && (Downloader.State == ManifestDownloadProgressState.Complete || Downloader.State == ManifestDownloadProgressState.InstallFailed));
                pauseToolStripMenuItem.Enabled = (Downloader != null);
                pauseToolStripMenuItem.Text = (Downloader != null && Downloader.Paused ? "Resume" : "Pause");
                pauseToolStripMenuItem.Image = (Downloader != null && Downloader.Paused ? global::BuildSync.Client.Properties.Resources.appbar_control_play : global::BuildSync.Client.Properties.Resources.appbar_control_pause);
                deleteToolStripMenuItem.Enabled = (Downloader != null);
                settingsToolStripMenuItem.Enabled = (Downloader != null);
            }
            else
            {
                forceRedownloadToolStripMenuItem.Enabled = false;
                forceRevalidateToolStripMenuItem.Enabled = false;
                forceReinstallToolStripMenuItem.Enabled = false;
                pauseToolStripMenuItem.Enabled = false;
                pauseToolStripMenuItem.Text = "Pause";
                deleteToolStripMenuItem.Enabled = false;
                settingsToolStripMenuItem.Enabled = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PauseClicked(object sender, EventArgs e)
        {
            ContextMenuDownloadItem.State.Paused = !ContextMenuDownloadItem.State.Paused;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteClicked(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you wish to delete '" + ContextMenuDownloadItem.State.Name + "'?", "Delete Download?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                Program.DownloadManager.RemoveDownload(ContextMenuDownloadItem.State);
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
            Dialog.EditState = ContextMenuDownloadItem.State;
            Dialog.ShowDialog(this);
        }

        /// <summary>
        /// 
        /// </summary>
        private void RefreshState()
        {
            bool Connected = Program.NetClient.IsReadyForData;

            if (!Connected)
            {
                if (Program.NetClient.IsConnected)
                {
                    peerCountLabel.Text = "Handshaking with '" + Program.Settings.ServerHostname + ":" + Program.Settings.ServerPort + "'";
                    peerCountLabel.ForeColor = Color.DarkCyan;
                }
                else if (Program.NetClient.IsConnecting)
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
                        case HandshakeResultType.MaxSeatsExceeded:
                            {
                                peerCountLabel.Text = "Server has run out of seat licenses.";
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
            manageBuildsToolStripMenuItem.Text = "Build Manager";
            manageBuildsToolStripMenuItem.Enabled = Connected;
            if (Connected)
            {
                if (!Program.NetClient.Permissions.HasPermission(UserPermissionType.ManageBuilds, ""))
                {
                    manageBuildsToolStripMenuItem.Enabled = false;
                    manageBuildsToolStripMenuItem.Text = "Build Manager (Permission Required)";
                }
            }

            manageUsersToolStripMenuItem.Text = "User Manager";
            manageUsersToolStripMenuItem.Enabled = Connected;
            if (Connected)
            {
                if (!Program.NetClient.Permissions.HasPermission(UserPermissionType.ManageUsers, ""))
                {
                    manageUsersToolStripMenuItem.Enabled = false;
                    manageUsersToolStripMenuItem.Text = "User Manager (Permission Required)";
                }
            }

            serverManagerToolStripMenuItem.Text = "Server Manager";
            serverManagerToolStripMenuItem.Enabled = Connected;
            if (Connected)
            {
                if (!Program.NetClient.Permissions.HasPermission(UserPermissionType.ManageServer, ""))
                {
                    serverManagerToolStripMenuItem.Enabled = false;
                    serverManagerToolStripMenuItem.Text = "Server Manager (Permission Required)";
                }
            }

            pushUpdateToolStripMenuItem.Text = "Push Update";
            pushUpdateToolStripMenuItem.Enabled = Connected;
            if (Connected)
            {
                if (!Program.NetClient.Permissions.HasPermission(UserPermissionType.ForceUpdate, ""))
                {
                    pushUpdateToolStripMenuItem.Enabled = false;
                    pushUpdateToolStripMenuItem.Text = "Push Update (Permission Required)";
                }
            }

            addDownloadToolStripMenuItem.Enabled = Connected;
            addDownloadToolStripMenuItem1.Enabled = Connected;

            viewLicenseMenuToolstrip.Enabled = Connected;

            totalUpBandwidthLabel.Text = string.Format("{0} ({1})", StringUtils.FormatAsTransferRate(NetConnection.GlobalBandwidthStats.RateOut), StringUtils.FormatAsSize(NetConnection.GlobalBandwidthStats.TotalOut));
            totalDownBandwidthLabel.Text = string.Format("{0} ({1})", StringUtils.FormatAsTransferRate(NetConnection.GlobalBandwidthStats.RateIn), StringUtils.FormatAsSize(NetConnection.GlobalBandwidthStats.TotalIn));

            if (AsyncIOQueue.QueuedOut > 0)
            {
                totalDiskUpBandwidthLabel.Text = string.Format("{0} (Q {1})", StringUtils.FormatAsTransferRate(AsyncIOQueue.GlobalBandwidthStats.RateOut), StringUtils.FormatAsSize(AsyncIOQueue.QueuedOut));
            }
            else
            {
                totalDiskUpBandwidthLabel.Text = string.Format("{0} ", StringUtils.FormatAsTransferRate(AsyncIOQueue.GlobalBandwidthStats.RateOut));
            }

            if (AsyncIOQueue.QueuedIn > 0)
            {
                totalDiskDownBandwidthLabel.Text = string.Format("{0} (Q {1})", StringUtils.FormatAsTransferRate(AsyncIOQueue.GlobalBandwidthStats.RateIn), StringUtils.FormatAsSize(AsyncIOQueue.QueuedIn));
            }
            else
            {
                totalDiskDownBandwidthLabel.Text = string.Format("{0}", StringUtils.FormatAsTransferRate(AsyncIOQueue.GlobalBandwidthStats.RateIn));
            }
        }

        #endregion
    }
}
