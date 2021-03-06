﻿/*
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
using System.Windows.Forms;
using BuildSync.Client.Controls.Settings;

namespace BuildSync.Client.Forms
{
    /// <summary>
    ///     Shows the user a form that allows them to change general application settings.
    ///     Individual setting pages take the form of nested controls, see <see cref="SettingsControlBase" /> for details.
    /// </summary>
    public partial class PreferencesForm : Form
    {
        /// <summary>
        ///     Dictionary associating tree node with settings panel control.
        /// </summary>
        private readonly Dictionary<string, SettingsControlBase> settingsPanels = new Dictionary<string, SettingsControlBase>();

        /// <summary>
        ///     Initializes a new instance of the <see cref="PreferencesForm" /> class.
        /// </summary>
        public PreferencesForm()
        {
            InitializeComponent();

            AddSettingsPanel<ServerSettings>("serverSettingsNode");
            AddSettingsPanel<GeneralSettings>("generalSettingsNode");
            AddSettingsPanel<StorageSettings>("storageSettingsNode");
            AddSettingsPanel<BandwidthSettings>("bandwidthSettingsNode");
            AddSettingsPanel<ScmSettings>("scmSettingsNode");
            AddSettingsPanel<ReplicationSettings>("replicationSettingsNode");

            groupTreeView.ExpandAll();
            groupTreeView.SelectedNode = groupTreeView.Nodes[0];

            UpdateSettingsPanels();
        }

        /// <summary>
        ///     Adds a new settings panel by type and associates it with the
        /// </summary>
        /// <typeparam name="Type">Type of settings panel to add.</typeparam>
        /// <param name="nodeName">Name of node in tree view to associate panel with.</param>
        private void AddSettingsPanel<Type>(string nodeName)
            where Type : SettingsControlBase
        {
            SettingsControlBase settings = Activator.CreateInstance(typeof(Type)) as SettingsControlBase;
            settings.Dock = DockStyle.Top;

            settingsPanelContainer.Controls.Add(settings);
            settingsPanels.Add(nodeName, settings);
        }

        /// <summary>
        /// </summary>
        private void CloseClicked(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormHasClosed(object sender, FormClosedEventArgs e)
        {
            Program.SaveSettings();
            Program.ApplySettings();
        }

        /// <summary>
        ///     Invoked when the settings node in the tree view is changed.
        /// </summary>
        /// <param name="sender">Object that invoked this event.</param>
        /// <param name="e">Event specific arguments.</param>
        private void SelectedSettingsNodeChanged(object sender, TreeViewEventArgs e)
        {
            UpdateSettingsPanels();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
        {
        }

        /// <summary>
        ///     Updates the settings panel controls. Showing the one that is associated with
        ///     the selected node and hiding the others. Also adjusts the page title text.
        /// </summary>
        private void UpdateSettingsPanels()
        {
            settingsGroupNameLabel.Text = string.Empty;

            foreach (string nodeName in settingsPanels.Keys)
            {
                SettingsControlBase panel = settingsPanels[nodeName];

                bool selected = false;
                if (groupTreeView.SelectedNode != null)
                {
                    selected = groupTreeView.SelectedNode.Name == nodeName;
                }

                panel.Visible = selected;

                if (selected)
                {
                    settingsGroupNameLabel.Text = panel.GroupName;
                }
            }
        }
    }
}