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
using System.IO;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows.Forms;
using BuildSync.Client.Properties;
using Microsoft.WindowsAPICodePack.Dialogs;
using BuildSync.Core.Downloads;
using BuildSync.Core.Utils;
using BuildSync.Core.Storage;
using BuildSync.Client.Forms;

namespace BuildSync.Client.Controls.Settings
{
    /// <summary>
    ///     Control thats displayed in the <see cref="SettingsForm" /> to allow the user to configure all
    ///     storage settings.
    /// </summary>
    public partial class StorageSettings : SettingsControlBase
    {
        private readonly bool SkipValidity;

        /// <summary>
        ///     Gets the title displayed over the settings when this control is displayed.
        /// </summary>
        public override string GroupName => "Storage Settings";

        /// <summary>
        ///     Initializes a new instance of the <see cref="ServerSettings" /> class.
        /// </summary>
        public StorageSettings()
        {
            InitializeComponent();

            foreach (Enum val in Enum.GetValues(typeof(ManifestStorageHeuristic)))
            {
                HeuristicComboBox.Items.Add(val.GetAttributeOfType<DescriptionAttribute>().Description);
            }

            SkipValidity = true;
            HeuristicComboBox.SelectedIndex = (int)Program.Settings.StorageHeuristic;
            prioritizeTagsTextBox.TagIds = new List<Guid>(Program.Settings.PrioritizeKeepingBuildTagIds);
            deprioritizeTagsTextBox.TagIds = new List<Guid>(Program.Settings.PrioritizeDeletingBuildTagIds);
            SkipValidity = false;

            RefreshItems();
            UpdateValidityState();
        }

        /// <summary>
        /// </summary>
        private void RefreshItems()
        {
            storageLocationList.Items.Clear();
            foreach (StorageLocation Settings in Program.Settings.StorageLocations)
            {
                ListViewItem Item = new ListViewItem(
                    new[]
                    {
                        Settings.Path,
                        StringUtils.FormatAsSize(Program.StorageManager.GetLocationDiskUsage(Settings)),
                        StringUtils.FormatAsSize(Settings.MaxSize),
                    }
                );
                Item.ImageIndex = 4;
                Item.Tag = Settings;
                storageLocationList.Items.Add(Item);
            }
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
                return;
            }

            Program.Settings.StorageHeuristic = (ManifestStorageHeuristic)HeuristicComboBox.SelectedIndex;

            Program.Settings.PrioritizeKeepingBuildTagIds = prioritizeTagsTextBox.TagIds;
            Program.Settings.PrioritizeDeletingBuildTagIds = deprioritizeTagsTextBox.TagIds;

            deleteLocationToolStripMenuItem.Enabled = storageLocationList.SelectedItems.Count > 0;
            RemoveStorageLocationButton.Enabled = storageLocationList.SelectedItems.Count > 0;
            editLocationToolStripMenuItem.Enabled = storageLocationList.SelectedItems.Count > 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddStorageClicked(object sender, EventArgs e)
        {
            AddStorageLocationForm Form = new AddStorageLocationForm();
            if (Form.ShowDialog(this) == DialogResult.OK)
            {
                Program.Settings.StorageLocations.Add(Form.Settings);
                Program.SaveSettings();

                RefreshItems();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveStorageClicked(object sender, EventArgs e)
        {
            if (storageLocationList.SelectedItems.Count == 0)
            {
                return;
            }

            StorageLocation Settings = storageLocationList.SelectedItems[0].Tag as StorageLocation;
            Program.Settings.StorageLocations.Remove(Settings);
            Program.SaveSettings();

            RefreshItems();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StorageLocationItemChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            UpdateValidityState();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditClicked(object sender, EventArgs e)
        {
            if (storageLocationList.SelectedItems.Count == 0)
            {
                return;
            }

            AddStorageLocationForm Form = new AddStorageLocationForm();
            Form.Settings = storageLocationList.SelectedItems[0].Tag as StorageLocation;
            if (Form.ShowDialog(this) == DialogResult.OK)
            {
                Program.SaveSettings();

                RefreshItems();
            }
        }
    }
}