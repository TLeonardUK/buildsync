using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
    }
}
