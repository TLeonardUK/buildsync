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
using BuildSync.Core.Downloads;
using BuildSync.Core.Utils;

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
            (new PublishBuildForm()).ShowDialog(this);
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

            e.Cancel = true;
            this.Visible = false;
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
                this.Visible = true;
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
            /*foreach (DownloadState State in Program.DownloadManager.States.States)
            {
                State.Paused = true;
            }*/
        }

        /// <summary>
        ///     Invoked when the user clicks the resume all button
        /// </summary>
        /// <param name="sender">Object that invoked this event.</param>
        /// <param name="e">Event specific arguments.</param>
        private void ResumeAllClicked(object sender, EventArgs e)
        {
            /*foreach (DownloadState State in Program.DownloadManager.States.States)
            {
                State.Paused = false;
            }*/
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
        private void RefreshState()
        {
            bool Connected = Program.NetClient.IsConnected;

            if (!Connected)
            {
                peerCountLabel.Text = "Unable to connect to server '" + Program.Settings.ServerHostname + ":" + Program.Settings.ServerPort + "'";
                peerCountLabel.ForeColor = Color.Red;
            }
            else
            {
                peerCountLabel.Text = Program.NetClient.PeerCount + " peers connected";
                peerCountLabel.ForeColor = Color.Black;
            }

            publishBuildToolStripMenuItem.Enabled = Connected;
            manageBuildsToolStripMenuItem.Enabled = Connected;
            addDownloadToolStripMenuItem.Enabled = Connected;
            addDownloadToolStripMenuItem1.Enabled = Connected;

            totalUpBandwidthLabel.Text = StringUtils.FormatAsTransferRate(NetConnection.GlobalBandwidthStats.RateOut);
            totalDownBandwidthLabel.Text = StringUtils.FormatAsTransferRate(NetConnection.GlobalBandwidthStats.RateIn);

            totalDiskUpBandwidthLabel.Text = string.Format("{0} ({1})", StringUtils.FormatAsTransferRate(Program.IOQueue.BandwidthStats.RateOut), StringUtils.FormatAsSize(Program.IOQueue.QueuedOut));
            totalDiskDownBandwidthLabel.Text = string.Format("{0} ({1})", StringUtils.FormatAsTransferRate(Program.IOQueue.BandwidthStats.RateIn), StringUtils.FormatAsSize(Program.IOQueue.QueuedIn));
        }

        #endregion
    }
}
