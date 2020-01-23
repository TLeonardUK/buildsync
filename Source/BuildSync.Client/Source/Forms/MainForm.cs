﻿using System;
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

        /// <summary>
        /// 
        /// </summary>
        private bool WasMinimized = false;

        /// <summary>
        /// 
        /// </summary>
        private bool DisableLayoutStoring = false;

        /// <summary>
        /// 
        /// </summary>
        private DeserializeDockContent DeserializedDockContent = null;

        #endregion
        #region Methods

        /// <summary>
        /// 
        /// </summary>
        public MainForm()
        {
            InitializeComponent();

            MainDownloadList = new DownloadList();
            StatsForm = new StatisticsForm();
            ManageBuildsForm = new ManageBuildsForm();
            ManageUsersForm = new ManageUsersForm();
            ManageServerForm = new ManageServerForm();
            ConsoleOutputForm = new ConsoleForm();
            PeersForm = new PeersForm();
            ManifestsForm = new ManifestsForm();

            DeserializedDockContent = new DeserializeDockContent(GetLayoutContentFromPersistString);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="persistString"></param>
        /// <returns></returns>
        private IDockContent GetLayoutContentFromPersistString(string persistString)
        {
            if (persistString == typeof(DownloadList).ToString())
            {
                return MainDownloadList;
            }
            else if (persistString == typeof(StatisticsForm).ToString())
            {
                return StatsForm;
            }
            else if (persistString == typeof(ManageBuildsForm).ToString())
            {
                return ManageBuildsForm;
            }
            else if (persistString == typeof(ManageUsersForm).ToString())
            {
                return ManageUsersForm;
            }
            else if (persistString == typeof(ManageServerForm).ToString())
            {
                return ManageServerForm;
            }
            else if (persistString == typeof(ConsoleForm).ToString())
            {
                return ConsoleOutputForm;
            }
            else if (persistString == typeof(PeersForm).ToString())
            {
                return PeersForm;
            }
            else if (persistString == typeof(ManifestsForm).ToString())
            {
                return ManifestsForm;
            }

            Logger.Log(LogLevel.Error, LogCategory.Main, "Dock layout deserialization failed due to unknown form: {0}", persistString);
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        private bool RestoreLayout()
        {
            if (Program.Settings.LayoutState != null)
            {
                using (MemoryStream Stream = new MemoryStream(Program.Settings.LayoutState))
                {
                    this.Size = Program.Settings.LayoutSize;
                    CloseAllContent();
                    DockPanel.LoadFromXml(Stream, DeserializedDockContent);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        private void CloseAllContent()
        {
            foreach (DockContent contents in DockPanel.Contents.ToArray())
            {
                contents.DockPanel = null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void StoreLayout(Size mainSize)
        {
            if (DisableLayoutStoring)
            {
                return;
            }

            using (MemoryStream Stream = new MemoryStream())
            {
                DockPanel.SaveAsXml(Stream, System.Text.Encoding.UTF8);
                Program.Settings.LayoutState = Stream.ToArray();
                Program.Settings.LayoutSize = mainSize;
            }
        }

        /// <summary>
        ///     Invoked when the form is loaded.
        /// </summary>
        /// <param name="sender">Object that invoked this event.</param>
        /// <param name="e">Event specific arguments.</param>
        private void FormLoaded(object sender, EventArgs e)
        {
            DockPanel.Theme = new VS2015LightTheme();

            if (!RestoreLayout())
            {
                MainDownloadList.Show(DockPanel, DockState.Document);
                MainDownloadList.ContextMenuStrip = downloadListContextMenu;

                StatsForm.Show(DockPanel, DockState.Document);

                MainDownloadList.Activate();
            }
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
        /// <param name="Content"></param>
        private void ShowContent(DockContent Content, DockState DefaultDockState)
        {
            if (Content.DockState == DockState.Unknown)
            {
                Content.Show(DockPanel, DefaultDockState);
            }

            Content.Show();
            if (Content.DockState == DockState.DockLeftAutoHide ||
                Content.DockState == DockState.DockRightAutoHide ||
                Content.DockState == DockState.DockTopAutoHide ||
                Content.DockState == DockState.DockBottomAutoHide)
            {
                DockPanel.ActiveAutoHideContent = Content;
            }
            Content.Activate();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ManageBuildsClicked(object sender, EventArgs e)
        {
            ShowContent(ManageBuildsForm, DockState.DockRight);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ManageUsersClicked(object sender, EventArgs e)
        {
            ShowContent(ManageUsersForm, DockState.Document);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ServerManagerClicked(object sender, EventArgs e)
        {
            ShowContent(ManageServerForm, DockState.Document);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DownloadListClicked(object sender, EventArgs e)
        {
            ShowContent(MainDownloadList, DockState.Document);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ViewConsoleClicked(object sender, EventArgs e)
        {
            ShowContent(ConsoleOutputForm, DockState.DockBottom);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StatisticsClicked(object sender, EventArgs e)
        {
            ShowContent(StatsForm, DockState.Document);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ViewPeersClicked(object sender, EventArgs e)
        {
            ShowContent(PeersForm, DockState.DockBottom);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ViewManifestsClicked(object sender, EventArgs e)
        {
            ShowContent(ManifestsForm, DockState.DockBottom);
        }

        /// <summary>
        ///     Invoked when the user clicks the notify icon
        /// </summary>
        /// <param name="sender">Object that invoked this event.</param>
        /// <param name="e">Event specific arguments.</param>
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            StoreLayout(this.Size);

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
            if (WindowState != FormWindowState.Minimized)
            {
                if (WasMinimized)
                {
                    RestoreLayout();
                    WasMinimized = false;
                }
            }
            else if (WindowState == FormWindowState.Minimized)
            {
                if (!WasMinimized)
                {
                    StoreLayout(this.RestoreBounds.Size);
                    WasMinimized = true;
                }
            }
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

            // Show/Hide autoupdate stuff
            if (Program.AutoUpdateAvailable)
            {
                StoreLayout(this.Size);
                DisableLayoutStoring = true;

                UpdatePanel.Visible = true;
                DockPanel.Visible = false;
                fileToolStripMenuItem.Enabled = false;
                viewToolStripMenuItem.Enabled = false;
                adminBaseToolStripMenuItem.Enabled = false;
                helpToolStripMenuItem.Enabled = false;

                peerCountLabel.Text = "Update is pending";
                peerCountLabel.ForeColor = Color.Red;

                CloseAllContent();
            }
            else
            {
                UpdatePanel.Visible = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PollTimerTick(object sender, EventArgs e)
        {
            Program.OnPoll();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AutoUpdaterClicked(object sender, EventArgs e)
        {
            Program.InstallAutoUpdate();
        }

        #endregion
    }
}
