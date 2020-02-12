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
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace BuildSync.Client.Controls.Settings
{
    /// <summary>
    ///     Control thats displayed in the <see cref="SettingsForm"/> to allow the user to configure all
    ///     storage settings.
    /// </summary>
    public partial class StorageSettings : SettingsControlBase
    {
        private bool SkipValidity = false;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ServerSettings"/> class.
        /// </summary>
        public StorageSettings()
        {
            InitializeComponent();

            SkipValidity = true;
            StoragePathTextBox.Text = Program.Settings.StoragePath;
            StorageMaxSizeTextBox.Value = Program.Settings.StorageMaxSize / 1024 / 1024 / 1024;
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
                return "Storage Settings";
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

            Program.Settings.StoragePath = StoragePathTextBox.Text;
            Program.Settings.StorageMaxSize = (long)StorageMaxSizeTextBox.Value * 1024 * 1024 * 1024;

            if (Directory.Exists(Program.Settings.StoragePath))
            {
                StoragePathIcon.Image = Properties.Resources.ic_check_circle_2x;
            }
            else
            {
                StoragePathIcon.Image = Properties.Resources.ic_error_red_48pt;
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
        private void BrowseClicked(object sender, EventArgs e)
        {
            CommonOpenFileDialog Dialog = new CommonOpenFileDialog();
            Dialog.AllowNonFileSystemItems = true;
            Dialog.Multiselect = true;
            Dialog.IsFolderPicker = true;
            Dialog.Title = "Select Storage Folder";
            Dialog.InitialDirectory = Program.Settings.StoragePath;

            if (Dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                StoragePathTextBox.Text = Dialog.FileName;
                Program.Settings.StoragePath = Dialog.FileName;

                UpdateValidityState();
            }
        }
    }
}
