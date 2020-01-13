using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BuildSync.Core.Scm;
using BuildSync.Client.Forms;

namespace BuildSync.Client.Controls.Settings
{
    /// <summary>
    ///     Control thats displayed in the <see cref="SettingsForm"/> to allow the user to configure all
    ///     server settings.
    /// </summary>
    public partial class ScmSettings : SettingsControlBase
    {
        private bool SkipValidity = false;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ServerSettings"/> class.
        /// </summary>
        public ScmSettings()
        {
            InitializeComponent();

            SkipValidity = true;
            /*ServerTextBox.Text = Program.Settings.PerforceServer;
            UsernameTextBox.Text = Program.Settings.PerforceUsername;
            PasswordTextBox.Text = Program.Settings.PerforcePassword;*/
            SkipValidity = false;

            foreach (string str in Enum.GetNames(typeof(ScmProviderType)))
            {
                workspaceList.Groups.Add(str, str);
            }

            RefreshItems();

            UpdateValidityState();
        }

        /// <summary>
        ///     Gets the title displayed over the settings when this control is displayed.
        /// </summary>
        public override string GroupName
        {
            get
            {
                return "SCM Settings";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void RefreshItems()
        {
            workspaceList.Items.Clear();
            foreach (ScmWorkspaceSettings Settings in Program.Settings.ScmWorkspaces)
            {
                ListViewItem Item = new ListViewItem(new string[] { 
                    Settings.Server,
                    Settings.Username,
                    Settings.Password.Length > 0 ? "******" : "",
                    Settings.Location
                });
                Item.Tag = Settings;
                Item.Group = workspaceList.Groups[Settings.ProviderType.ToString()];
                workspaceList.Items.Add(Item);
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

            /*Program.Settings.PerforceServer = ServerTextBox.Text;
            Program.Settings.PerforceUsername = UsernameTextBox.Text;
            Program.Settings.PerforcePassword = PasswordTextBox.Text;

            int Port = 0;
            string[] Split = Program.Settings.PerforceServer.Split(':');

            if ((Split.Length == 1 && Uri.CheckHostName(Split[0]) != UriHostNameType.Unknown) ||
                (Split.Length == 2 && Uri.CheckHostName(Split[0]) != UriHostNameType.Unknown && int.TryParse(Split[1], out Port)))
            {
                ServerIcon.Image = Properties.Resources.ic_check_circle_2x;
            }
            else
            {
                ServerIcon.Image = Properties.Resources.ic_error_red_48pt;
            }

            if (Program.Settings.PerforceUsername.Trim().Length >= 3)
            {
                UsernameIcon.Image = Properties.Resources.ic_check_circle_2x;
            }
            else
            {
                UsernameIcon.Image = Properties.Resources.ic_error_red_48pt;
            }*/
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
        private void AddClicked(object sender, EventArgs e)
        {
            AddScmWorkspaceForm Form = new AddScmWorkspaceForm();
            if (Form.ShowDialog(this) == DialogResult.OK)
            {
                Program.Settings.ScmWorkspaces.Add(Form.Settings);
                Program.SaveSettings();

                RefreshItems();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveClicked(object sender, EventArgs e)
        {
            if (workspaceList.SelectedItems.Count == 0)
            {
                return;
            }

            ScmWorkspaceSettings Settings = workspaceList.SelectedItems[0].Tag as ScmWorkspaceSettings;
            Program.Settings.ScmWorkspaces.Remove(Settings);
            Program.SaveSettings();

            RefreshItems();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WorkspaceSelectionChanged(object sender, EventArgs e)
        {
            RemoveServerButton.Enabled = workspaceList.SelectedItems.Count > 0;
        }
    }
}
