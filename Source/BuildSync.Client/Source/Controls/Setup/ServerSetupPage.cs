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
using BuildSync.Client.Properties;
using BuildSync.Core.Networking.Messages;
using BuildSync.Core.Utils;

namespace BuildSync.Client.Controls.Setup
{
    /// <summary>
    ///     Control thats displayed in the <see cref="SettingsForm" /> to allow the user to configure all
    ///     server settings.
    /// </summary>
    public partial class ServerSetupPage : SetupPageControlBase
    {
        private readonly bool SkipValidity;

        /// <summary>
        ///     Gets the title displayed over the settings when this control is displayed.
        /// </summary>
        public override string Title => "Server Connection";

        /// <summary>
        /// 
        /// </summary>
        public override bool PreviousEnabled => true;

        /// <summary>
        /// 
        /// </summary>
        public override bool NextEnabled => (Program.NetClient.IsReadyForData && Program.NetClient.ServerHostname == Program.Settings.ServerHostname);

        /// <summary>
        /// 
        /// </summary>
        public ulong TimeLastChanged = 0;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ServerSettings" /> class.
        /// </summary>
        public ServerSetupPage()
        {
            InitializeComponent();

            SkipValidity = true;
            ServerHostnameTextBox.Text = Program.Settings.ServerHostname;
            SkipValidity = false;

            UpdateValidityState();
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FindServerClicked(object sender, EventArgs e)
        {
            using (FindServerForm form = new FindServerForm())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    ServerHostnameTextBox.Text = form.SelectedHostname;

                    UpdateValidityState();
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StateChanged(object sender, EventArgs e)
        {
            TimeLastChanged = TimeUtils.Ticks;

            UpdateValidityState();
        }

        /// <summary>
        /// </summary>
        private void UpdateValidityState()
        {
            if (SkipValidity)
            {
                return;
            }

            Program.Settings.ServerHostname = ServerHostnameTextBox.Text;

            if (Uri.CheckHostName(Program.Settings.ServerHostname) != UriHostNameType.Unknown)
            {
                ServerHostnameIcon.Image = Resources.ic_check_circle_2x;
            }
            else
            {
                ServerHostnameIcon.Image = Resources.ic_error_red_48pt;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConnectTimerTick(object sender, EventArgs e)
        {
            bool Connected = Program.NetClient.IsReadyForData;

            connectionLabelStatus.Visible = (Program.Settings.ServerHostname.Trim().Length > 0);

            if (Program.NetClient.ServerHostname != Program.Settings.ServerHostname)
            {
                connectionLabelStatus.Visible = false;
            }
            else if (!Connected)
            {
                if (Program.NetClient.IsConnected)
                {
                    connectionLabelStatus.Text = "Handshaking with '" + Program.Settings.ServerHostname + ":" + Program.Settings.ServerPort + "'";
                    connectionLabelStatus.ForeColor = Color.DarkCyan;
                }
                else if (Program.NetClient.IsConnecting)
                {
                    connectionLabelStatus.Text = "Connecting to '" + Program.Settings.ServerHostname + ":" + Program.Settings.ServerPort + "'";
                    connectionLabelStatus.ForeColor = Color.DarkCyan;
                }
                else
                {
                    switch (Program.NetClient.HandshakeResult)
                    {
                        case HandshakeResultType.InvalidVersion:
                            {
                                connectionLabelStatus.Text = "Version is incompatible, please update application.";
                                break;
                            }
                        case HandshakeResultType.MaxSeatsExceeded:
                            {
                                connectionLabelStatus.Text = "Server has run out of seat licenses.";
                                break;
                            }
                        default:
                            {
                                connectionLabelStatus.Text = "Unable to connect to server '" + Program.Settings.ServerHostname + ":" + Program.Settings.ServerPort + "'";
                                break;
                            }
                    }

                    connectionLabelStatus.ForeColor = Color.Red;
                }
            }
            else
            {
                connectionLabelStatus.Text = "Connection successful";
                connectionLabelStatus.ForeColor = Color.Green;
            }

            if (TimeLastChanged > 0 && TimeUtils.Ticks - TimeLastChanged > 300)
            {
                Program.ApplySettings();
                TimeLastChanged = 0;
            }
        }
    }
}