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
using System.Windows.Forms;
using BuildSync.Client.Forms;
using BuildSync.Core.Scm;

namespace BuildSync.Client.Controls.Settings
{
    /// <summary>
    ///     Control thats displayed in the <see cref="SettingsForm" /> to allow the user to configure all
    ///     server settings.
    /// </summary>
    public partial class ScmSettings : SettingsControlBase
    {
        private readonly bool SkipValidity;

        /// <summary>
        ///     Gets the title displayed over the settings when this control is displayed.
        /// </summary>
        public override string GroupName => "SCM Settings";

        /// <summary>
        ///     Initializes a new instance of the <see cref="ServerSettings" /> class.
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
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddClicked(object sender, EventArgs e)
        {
            using (AddScmWorkspaceForm Form = new AddScmWorkspaceForm())
            {
                if (Form.ShowDialog(this) == DialogResult.OK)
                {
                    Program.Settings.ScmWorkspaces.Add(Form.Settings);
                    Program.SaveSettings();

                    RefreshItems();
                }
            }
        }

        /// <summary>
        /// </summary>
        private void RefreshItems()
        {
            workspaceList.Items.Clear();
            foreach (ScmWorkspaceSettings Settings in Program.Settings.ScmWorkspaces)
            {
                ListViewItem Item = new ListViewItem(
                    new[]
                    {
                        Settings.Server,
                        Settings.Username,
                        Settings.Password.Length > 0 ? "******" : "",
                        Settings.Location
                    }
                );
                Item.Tag = Settings;
                Item.Group = workspaceList.Groups[Settings.ProviderType.ToString()];
                workspaceList.Items.Add(Item);
            }
        }

        /// <summary>
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
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StateChanged(object sender, EventArgs e)
        {
            UpdateValidityState();
        }

        /// <summary>
        /// </summary>
        private void UpdateValidityState()
        {
            if (SkipValidity)
            {
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
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WorkspaceSelectionChanged(object sender, EventArgs e)
        {
            RemoveServerButton.Enabled = workspaceList.SelectedItems.Count > 0;
        }
    }
}