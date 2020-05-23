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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BuildSync.Core.Manifests;
using BuildSync.Core.Client;
using BuildSync.Core.Utils;

namespace BuildSync.Client.Forms
{
    /// <summary>
    /// 
    /// </summary>
    public partial class RemoteInstallForm : Form
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid ManifestId = Guid.Empty;

        /// <summary>
        /// 
        /// </summary>
        public string ManifestPath = "";

        /// <summary>
        /// 
        /// </summary>
        public Guid InstallId = Guid.Empty;

        /// <summary>
        /// 
        /// </summary>
        private InstallRateEstimater InstallRate = new InstallRateEstimater();

        /// <summary>
        /// 
        /// </summary>
        public RemoteInstallForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnShown(object sender, EventArgs e)
        {
            manifestLabel.Text = ManifestPath;
            deviceNameTextBox.Text = Program.Settings.RemoteInstallDeviceName;
            locationTextBox.Text = Program.Settings.RemoteInstallLocation;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnStartClicked(object sender, EventArgs e)
        {
            if (InstallId != Guid.Empty)
            {
                Close();
            }
            else
            {
                InstallId = Program.RemoteActionClient.RequestRemoteInstall(ManifestId, deviceNameTextBox.Text, locationTextBox.Text);
                deviceNameTextBox.Enabled = false;
                locationTextBox.Enabled = false;
                startButton.Text = "Cancel";

                progressLabel.Visible = true;
                label3.Visible = true;
                installTimeLabel.Visible = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateTicked(object sender, EventArgs e)
        {
            if (InstallId == Guid.Empty)
            {
                return;
            }

            if (!Program.NetClient.IsConnected)
            {
                FailInstall("Lost connected to server, install aborted.");
                return;
            }

            RemoteActionClientState State = Program.RemoteActionClient.GetActionState(InstallId);
            if (State == null)
            {
                FailInstall("Remote install failed.");
                return;
            }

            InstallRate.SetProgress(State.Progress);
            InstallRate.Poll();

            //progressBar.Value = Math.Max(0, Math.Min(100, (int)(State.Progress * 100.0f)));
            progressBar.Value = Math.Max(0, Math.Min(100, (int)(InstallRate.EstimatedProgress * 100.0f)));
            progressLabel.Text = State.ProgressText == "" ? "Installing ..." : State.ProgressText;
            installTimeLabel.Text = (InstallRate.EstimatedSeconds == 0.0f ? "Unknown" : StringUtils.FormatAsDuration((long)InstallRate.EstimatedSeconds));

            progressBar.Style = (InstallRate.EstimatedProgress == 0.0f) ? ProgressBarStyle.Marquee : ProgressBarStyle.Continuous;

            if (State.Completed)
            {
                if (!State.Failed)
                {
                    SuccessInstall("Remote install completed.");
                    return;
                }
                else
                {
                    FailInstall("Remote install failed with error:\n\n" + State.ResultMessage);
                    return;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void FailInstall(string Message)
        {
            updateTimer.Enabled = false;
            MessageBox.Show(Message, "Install Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Close();
        }

        /// <summary>
        /// 
        /// </summary>
        private void SuccessInstall(string Message)
        {
            updateTimer.Enabled = false;
            MessageBox.Show(Message, "Install Completed", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Close();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClosed(object sender, FormClosedEventArgs e)
        {
            if (InstallId != Guid.Empty)
            {
                Program.RemoteActionClient.CancelRemoteAction(InstallId);
                InstallId = Guid.Empty;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClosing(object sender, FormClosingEventArgs e)
        {
            Program.Settings.RemoteInstallDeviceName = deviceNameTextBox.Text;
            Program.Settings.RemoteInstallLocation = locationTextBox.Text;
            Program.SaveSettings();
        }
    }
}
