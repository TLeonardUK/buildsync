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
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BuildSync.Client.Forms;

namespace BuildSync.Client.Controls.Settings
{
    /// <summary>
    ///     Control thats displayed in the <see cref="SettingsForm"/> to allow the user to configure all
    ///     server settings.
    /// </summary>
    public partial class ServerSettings : SettingsControlBase
    {
        private bool SkipValidity = false;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ServerSettings"/> class.
        /// </summary>
        public ServerSettings()
        {
            InitializeComponent();

            SkipValidity = true;
            ServerHostnameTextBox.Text = Program.Settings.ServerHostname;
            ServerPortTextBox.Value = Program.Settings.ServerPort;
            PeerPortRangeStartBox.Value = Program.Settings.ClientPortRangeMin;
            PeerPortRangeEndBox.Value = Program.Settings.ClientPortRangeMax;
            SkipValidity = false;

            UpdateValidityState();
        }

        /// <summary>
        ///     Gets the title displayed over the settings when this control is displayed.
        /// </summary>
        public override string GroupName
        {
            get
            {
                return "Server Settings";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateValidityState()
        {
            if (SkipValidity)
            {
                return;
            }

            Program.Settings.ServerHostname = ServerHostnameTextBox.Text;
            Program.Settings.ServerPort = (int)ServerPortTextBox.Value;
            Program.Settings.ClientPortRangeMin = (int)PeerPortRangeStartBox.Value;
            Program.Settings.ClientPortRangeMax = (int)PeerPortRangeEndBox.Value;

            if (Uri.CheckHostName(Program.Settings.ServerHostname) != UriHostNameType.Unknown)
            {
                ServerHostnameIcon.Image = Properties.Resources.ic_check_circle_2x;
            }
            else
            {
                ServerHostnameIcon.Image = Properties.Resources.ic_error_red_48pt;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StateChanged(object sender, EventArgs e)
        {
            UpdateValidityState();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FindServerClicked(object sender, EventArgs e)
        {
            FindServerForm form = new FindServerForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                ServerHostnameTextBox.Text = form.SelectedHostname;
                ServerPortTextBox.Value = form.SelectedPort;

                UpdateValidityState();
            }
        }
    }
}
