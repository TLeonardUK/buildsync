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
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using BuildSync.Client.Controls;
using BuildSync.Client.Properties;
using BuildSync.Client.Tasks;
using BuildSync.Core;
using BuildSync.Core.Client;
using BuildSync.Core.Downloads;
using BuildSync.Core.Networking;
using BuildSync.Core.Networking.Messages;
using BuildSync.Core.Users;
using BuildSync.Core.Utils;
using WeifenLuo.WinFormsUI.Docking;

namespace BuildSync.Client.Forms
{
    /// <summary>
    /// </summary>
    public partial class MainForm : Form
    {
        #region Fields

        /// <summary>
        /// </summary>
        private bool ForcedExit;

        /// <summary>
        /// </summary>
        private readonly ConsoleForm ConsoleOutputForm;

        /// <summary>
        /// </summary>
        private readonly StatisticsForm StatsForm;

        /// <summary>
        /// </summary>
        private readonly PeersForm PeersForm;

        /// <summary>
        /// </summary>
        private readonly ManifestsForm ManifestsForm;

        /// <summary>
        /// </summary>
        private readonly ManageUsersForm ManageUsersForm;

        /// <summary>
        /// </summary>
        private readonly ManageServerForm ManageServerForm;

        /// <summary>
        /// </summary>
        private readonly ManageBuildsForm ManageBuildsForm;

        /// <summary>
        /// </summary>
        private readonly ManageTagsForm ManageTagsForm;

        /// <summary>
        /// </summary>
        private DownloadListItem ContextMenuDownloadItem;

        /// <summary>
        /// </summary>
        private readonly DownloadList MainDownloadList;

        /// <summary>
        /// </summary>
        private bool WasMinimized;

        /// <summary>
        /// </summary>
        private bool DisableLayoutStoring;

        /// <summary>
        /// 
        /// </summary>
        private bool IsResizing = false;

        /// <summary>
        /// 
        /// </summary>
        private bool HasPolled = false;

        /// <summary>
        /// </summary>
        private readonly DeserializeDockContent DeserializedDockContent;

        #endregion

        #region Methods

        /// <summary>
        /// </summary>
        public MainForm()
        {
            InitializeComponent();

            MainDownloadList = new DownloadList();
            StatsForm = new StatisticsForm();
            ManageBuildsForm = new ManageBuildsForm();
            ManageTagsForm = new ManageTagsForm();
            ManageUsersForm = new ManageUsersForm();
            ManageServerForm = new ManageServerForm();
            ConsoleOutputForm = new ConsoleForm();
            PeersForm = new PeersForm();
            ManifestsForm = new ManifestsForm();

            DeserializedDockContent = GetLayoutContentFromPersistString;
        }

        /// <summary>
        /// </summary>
        /// <param name="persistString"></param>
        /// <returns></returns>
        private IDockContent GetLayoutContentFromPersistString(string persistString)
        {
            if (persistString == typeof(DownloadList).ToString())
            {
                return MainDownloadList;
            }

            if (persistString == typeof(StatisticsForm).ToString())
            {
                return StatsForm;
            }

            if (persistString == typeof(ManageBuildsForm).ToString())
            {
                return ManageBuildsForm;
            }

            if (persistString == typeof(ManageTagsForm).ToString())
            {
                return ManageTagsForm;
            }

            if (persistString == typeof(ManageUsersForm).ToString())
            {
                return ManageUsersForm;
            }

            if (persistString == typeof(ManageServerForm).ToString())
            {
                return ManageServerForm;
            }

            if (persistString == typeof(ConsoleForm).ToString())
            {
                return ConsoleOutputForm;
            }

            if (persistString == typeof(PeersForm).ToString())
            {
                return PeersForm;
            }

            if (persistString == typeof(ManifestsForm).ToString())
            {
                return ManifestsForm;
            }

            Logger.Log(LogLevel.Error, LogCategory.Main, "Dock layout deserialization failed due to unknown form: {0}", persistString);
            return null;
        }

        /// <summary>
        /// </summary>
        private bool RestoreLayout()
        {
            SuspendLayout();

            if (Program.Settings.LayoutState != null)
            {
                using (MemoryStream Stream = new MemoryStream(Program.Settings.LayoutState))
                {
                    Size = Program.Settings.LayoutSize;
                    CloseAllContent();
                    DockPanel.LoadFromXml(Stream, DeserializedDockContent);
                    ResumeLayout();
                    return true;
                }
            }

            ResumeLayout();

            return false;
        }

        /// <summary>
        /// </summary>
        private void CloseAllContent()
        {
            foreach (DockContent contents in DockPanel.Contents.ToArray())
            {
                contents.DockPanel = null;
            }
        }

        /// <summary>
        /// </summary>
        private void StoreLayout(Size mainSize)
        {
            if (DisableLayoutStoring)
            {
                return;
            }

            using (MemoryStream Stream = new MemoryStream())
            {
                DockPanel.SaveAsXml(Stream, Encoding.UTF8);
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
            Logger.Log(LogLevel.Info, LogCategory.Main, "MainForm: Setting up docking themes");
            DockPanel.Theme = new VS2015LightTheme();

#if !SHIPPING
            Text = "Build Sync - Client (PID " + Process.GetCurrentProcess().Id + ")";
#endif

            MainDownloadList.ContextMenuStrip = downloadListContextMenu;
            
            if (AppVersion.NonLicensed)
            {
                viewLicenseMenuToolstrip.Visible = false;   
            }

            Logger.Log(LogLevel.Info, LogCategory.Main, "MainForm: Restoring layout");
            if (!RestoreLayout())
            {
                MainDownloadList.Show(DockPanel, DockState.Document);

                StatsForm.Show(DockPanel, DockState.Document);

                MainDownloadList.Activate();
            }


            Logger.Log(LogLevel.Info, LogCategory.Main, "MainForm: Hooking download events");
            Program.DownloadManager.OnDownloadFinished += (DownloadState State) => 
            {
                if (!State.VirtualPath.StartsWith("$")) // Ignore internal downloads.
                {
                    notifyIcon.ShowBalloonTip(5000, "Download Finished", "Finished downloading newest build for '" + State.Name + "'", ToolTipIcon.Info);
                }
            };
            Program.DownloadManager.OnDownloadStarted += (DownloadState State) =>
            {
                if (!State.VirtualPath.StartsWith("$")) // Ignore internal downloads.
                {
                    notifyIcon.ShowBalloonTip(5000, "Download Started", "Started downloading new build for '" + State.Name + "'", ToolTipIcon.Info);
                }
            };
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
            new AddDownloadForm().ShowDialog(this);
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
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PushUpdateClicked(object sender, EventArgs e)
        {
            OpenFileDialog Dialog = new OpenFileDialog();
            Dialog.Title = "Select Update File";
            Dialog.Filter = "Installer File (*.msi)|*.msi";
            //Dialog.InitialDirectory = Environment.CurrentDirectory;
            Dialog.CheckFileExists = true;
            Dialog.ShowHelp = true;

            if (Dialog.ShowDialog(this) == DialogResult.OK)
            {
                string msiVersion = InstallUtils.GetMsiProperty(Dialog.FileName, "ProductVersion");
                if (msiVersion.Length == 0)
                {
                    MessageBox.Show("Failed to retrieve version data from msi.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                BuildsRecievedHandler Handler = null;
                Handler = (RootPath, Builds) =>
                {
                    Program.NetClient.OnBuildsRecieved -= Handler;

                    // Find the next sequential build index.
                    string NewVirtualPath = RootPath + "/" + msiVersion;

                    bool Exists = false;
                    foreach (NetMessage_GetBuildsResponse.BuildInfo Build in Builds)
                    {
                        if (Build.VirtualPath.ToUpper() == NewVirtualPath.ToUpper())
                        {
                            Exists = true;
                            break;
                        }
                    }

                    if (Exists)
                    {
                        MessageBox.Show("Version is already uploaded.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Publish a folder.
                    string TempFolder = FileUtils.GetTempDirectory();
                    File.Copy(Dialog.FileName, Path.Combine(TempFolder, "installer.msi"));

                    // Publish the build.
                    PublishBuildTask Publisher = new PublishBuildTask();
                    Publisher.Start(NewVirtualPath, TempFolder);

                    Task.Run(
                        () =>
                        {
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
                                        Invoke((MethodInvoker) (() => { Publisher.Commit(); }));
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
                        }
                    );
                };

                Program.NetClient.OnBuildsRecieved += Handler;
                Program.NetClient.RequestBuilds("$Internal$/Updates");
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ViewLicenseClicked(object sender, EventArgs e)
        {
            new LicenseForm().ShowDialog(this);
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ViewHelpClickled(object sender, EventArgs e)
        {
            string ExePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            while (true)
            {
                string HelpDocs = Path.Combine(ExePath, "Docs/Build Sync Help.chm");
                Console.WriteLine("Trying: {0}", HelpDocs);
                if (File.Exists(HelpDocs))
                {
                    Process.Start(HelpDocs);
                    break;
                }

                ExePath = Path.GetDirectoryName(ExePath);
                if (ExePath == null || !ExePath.Contains('\\') && !ExePath.Contains('/'))
                {
                    MessageBox.Show("Failed to find help chm file, installation may be corrupt.", "Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
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
            new PreferencesForm().ShowDialog(this);
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
            new AboutForm().ShowDialog(this);
        }

        /// <summary>
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
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ManageBuildsClicked(object sender, EventArgs e)
        {
            ShowContent(ManageBuildsForm, DockState.Document);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ManageTagsClicked(object sender, EventArgs e)
        {
            ShowContent(ManageTagsForm, DockState.Document);
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ManageUsersClicked(object sender, EventArgs e)
        {
            ShowContent(ManageUsersForm, DockState.Document);
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ServerManagerClicked(object sender, EventArgs e)
        {
            ShowContent(ManageServerForm, DockState.Document);
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DownloadListClicked(object sender, EventArgs e)
        {
            ShowContent(MainDownloadList, DockState.Document);
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ViewConsoleClicked(object sender, EventArgs e)
        {
            ShowContent(ConsoleOutputForm, DockState.DockBottom);
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StatisticsClicked(object sender, EventArgs e)
        {
            ShowContent(StatsForm, DockState.Document);
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ViewPeersClicked(object sender, EventArgs e)
        {
            ShowContent(PeersForm, DockState.DockBottom);
        }

        /// <summary>
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
            StoreLayout(Size);

            if (ForcedExit)
            {
                return;
            }

            if (Program.Settings.MinimizeToTrayOnClose)
            {
                e.Cancel = true;
                WindowState = FormWindowState.Minimized;
            }
            else
            {
                e.Cancel = false;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClientSizeHasChanged(object sender, EventArgs e)
        {
            ShowInTaskbar = WindowState != FormWindowState.Minimized;
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
                    StoreLayout(RestoreBounds.Size);
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
                WindowState = FormWindowState.Normal;
                BringToFront();
                Focus();
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
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DownloadListContextMenuShowing(object sender, CancelEventArgs e)
        {
            ContextMenuDownloadItem = MainDownloadList.SelectedItem;
            if (ContextMenuDownloadItem != null)
            {
                ManifestDownloadState Downloader = Program.ManifestDownloadManager.GetDownload(ContextMenuDownloadItem.State.ActiveManifestId);

                forceRedownloadToolStripMenuItem.Enabled = Downloader != null && Downloader.State != ManifestDownloadProgressState.Validating && Downloader.State != ManifestDownloadProgressState.Initializing && Downloader.State != ManifestDownloadProgressState.Installing;
                forceRevalidateToolStripMenuItem.Enabled = Downloader != null && (Downloader.State == ManifestDownloadProgressState.Complete || Downloader.State == ManifestDownloadProgressState.ValidationFailed);
                forceReinstallToolStripMenuItem.Enabled = Downloader != null && (Downloader.State == ManifestDownloadProgressState.Complete || Downloader.State == ManifestDownloadProgressState.InstallFailed);
                pauseToolStripMenuItem.Enabled = Downloader != null;
                pauseToolStripMenuItem.Text = Downloader != null && Downloader.Paused ? "Resume" : "Pause";
                pauseToolStripMenuItem.Image = Downloader != null && Downloader.Paused ? Resources.appbar_control_play : Resources.appbar_control_pause;
                deleteToolStripMenuItem.Enabled = Downloader != null;
                settingsToolStripMenuItem.Enabled = Downloader != null;
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
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PauseClicked(object sender, EventArgs e)
        {
            ContextMenuDownloadItem.State.Paused = !ContextMenuDownloadItem.State.Paused;
        }

        /// <summary>
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
                peerCountLabel.Text = Program.NetClient.ConnectedPeerCount + " peers connected (" + Program.NetClient.PeerCount + " acknowledged)";
                peerCountLabel.ForeColor = Color.Black;
            }

            //publishBuildToolStripMenuItem.Enabled = Connected;
            manageUsersToolStripMenuItem.Text = "User Manager";
            manageUsersToolStripMenuItem.Enabled = Connected;
            if (Connected)
            {
                if (!Program.NetClient.Permissions.HasPermission(UserPermissionType.ModifyUsers, "") &&
                    !Program.NetClient.Permissions.HasAnyPermissionOfType(UserPermissionType.AddUsersToGroup))
                {
                    manageUsersToolStripMenuItem.Enabled = false;
                    manageUsersToolStripMenuItem.Text = "User Manager (Permission Required)";
                }
            }

            serverManagerToolStripMenuItem.Text = "Server Manager";
            serverManagerToolStripMenuItem.Enabled = Connected;
            if (Connected)
            {
                if (!Program.NetClient.Permissions.HasPermission(UserPermissionType.ModifyServer, ""))
                {
                    serverManagerToolStripMenuItem.Enabled = false;
                    serverManagerToolStripMenuItem.Text = "Server Manager (Permission Required)";
                }
            }

            tagManagerToolStripMenuItem.Text = "Tag Manager";
            tagManagerToolStripMenuItem.Enabled = Connected;
            if (Connected)
            {
                if (!Program.NetClient.Permissions.HasPermission(UserPermissionType.ModifyTags, ""))
                {
                    tagManagerToolStripMenuItem.Enabled = false;
                    tagManagerToolStripMenuItem.Text = "Tag Manager (Permission Required)";
                }
            }

            pushUpdateToolStripMenuItem.Text = "Push Update";
            pushUpdateToolStripMenuItem.Enabled = Connected;
            if (Connected)
            {
                if (!Program.NetClient.Permissions.HasPermission(UserPermissionType.PushUpdate, ""))
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
                StoreLayout(Size);
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
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PollTimerTick(object sender, EventArgs e)
        {
            //if (IsResizing)
            //{
            //ResumeLayout(true);
            //SuspendLayout();
            //}

            if (!HasPolled)
            {
                Logger.Log(LogLevel.Info, LogCategory.Main, "MainForm: Running first poll");
                HasPolled = true;
            }

            Program.OnPoll();
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AutoUpdaterClicked(object sender, EventArgs e)
        {
            Program.InstallAutoUpdate();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnBeginResize(object sender, EventArgs e)
        {
            //SuspendLayout();
            //IsResizing = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnEndResize(object sender, EventArgs e)
        {
            //ResumeLayout(true);
            //IsResizing = false;
        }

        #endregion
    }
}