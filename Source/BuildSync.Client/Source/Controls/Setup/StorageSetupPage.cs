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
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows.Forms;
using BuildSync.Client.Properties;
using Microsoft.WindowsAPICodePack.Dialogs;
using BuildSync.Core.Downloads;
using BuildSync.Core.Utils;
using BuildSync.Core.Storage;

namespace BuildSync.Client.Controls.Setup
{
    /// <summary>
    ///     Control thats displayed in the <see cref="SettingsForm" /> to allow the user to configure all
    ///     server settings.
    /// </summary>
    public partial class StorageSetupPage : SetupPageControlBase
    {
        private readonly bool SkipValidity;

        /// <summary>
        ///     Gets the title displayed over the settings when this control is displayed.
        /// </summary>
        public override string Title => "Storage";

        /// <summary>
        /// 
        /// </summary>
        public override bool PreviousEnabled => true;

        /// <summary>
        /// 
        /// </summary>
        public override bool NextEnabled
        {
            get
            {
                return StorageMaxSizeTextBox.Value > 0 && Directory.Exists(StoragePathTextBox.Text);
            }       
        }


        /// <summary>
        ///     Initializes a new instance of the <see cref="ServerSettings" /> class.
        /// </summary>
        public StorageSetupPage()
        {
            InitializeComponent();

            SkipValidity = true;
            StoragePathTextBox.Text = Program.Settings.StorageLocations.Count > 0 ? Program.Settings.StorageLocations[0].Path : "";
            StorageMaxSizeTextBox.Value = Program.Settings.StorageLocations.Count > 0 ? Program.Settings.StorageLocations[0].MaxSize : 0;
            SkipValidity = false;

            UpdateValidityState();
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BrowseClicked(object sender, EventArgs e)
        {
            using (CommonOpenFileDialog Dialog = new CommonOpenFileDialog())
            {
                Dialog.AllowNonFileSystemItems = true;
                Dialog.Multiselect = true;
                Dialog.IsFolderPicker = true;
                Dialog.Title = "Select Storage Folder";
                Dialog.InitialDirectory = StoragePathTextBox.Text;

                if (Dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    StoragePathTextBox.Text = Dialog.FileName;

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

            if (Directory.Exists(StoragePathTextBox.Text))
            {
                StoragePathIcon.Image = Resources.ic_check_circle_2x;

                if (Program.Settings.StorageLocations.Count == 0)
                {
                    Program.Settings.StorageLocations.Add(new StorageLocation());
                }

                Program.Settings.StorageLocations[0].Path = StoragePathTextBox.Text;
                Program.Settings.StorageLocations[0].MaxSize = StorageMaxSizeTextBox.Value;
            }
            else
            {
                StoragePathIcon.Image = Resources.ic_error_red_48pt;
            }
        }
    }
}