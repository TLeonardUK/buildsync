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
using System.Windows.Forms;
using BuildSync.Core.Scm;
using BuildSync.Core.Utils;
using BuildSync.Core.Storage;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace BuildSync.Client.Forms
{
    /// <summary>
    /// </summary>
    public partial class AddStorageLocationForm : Form
    {
        /// <summary>
        /// 
        /// </summary>
        private StorageLocation InternalSettings;

        /// <summary>
        /// 
        /// </summary>
        private bool Editing = false;

        /// <summary>
        /// </summary>
        public StorageLocation Settings 
        {
            get
            {
                return InternalSettings;
            }
            set
            {
                LocalFolderTextBox.Text = value.Path;
                MaxSizeTextBox.Value = value.MaxSize;
                InternalSettings = value;
                Editing = true;
            }
        }

        /// <summary>
        /// </summary>
        public AddStorageLocationForm()
        {
            InitializeComponent();

            Settings = new StorageLocation();
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddClicked(object sender, EventArgs e)
        {
            Settings.Path = LocalFolderTextBox.Text.Trim();
            Settings.MaxSize = MaxSizeTextBox.Value;

            // Check no other workspaces exist with same local folder.
            if (!Editing)
            {
                foreach (StorageLocation Workspace in Program.Settings.StorageLocations)
                {
                    if (FileUtils.NormalizePath(Settings.Path) == FileUtils.NormalizePath(Workspace.Path))
                    {
                        MessageBox.Show("Storage location already exists at the same path.", "Duplicate Location", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BrowseClicked(object sender, EventArgs e)
        {
            CommonOpenFileDialog Dialog = new CommonOpenFileDialog();
            Dialog.AllowNonFileSystemItems = true;
            Dialog.Multiselect = true;
            Dialog.IsFolderPicker = true;
            Dialog.Title = "Select Location";

            if (Dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                LocalFolderTextBox.Text = Dialog.FileName;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataStateChanged(object sender, EventArgs e)
        {
            ValidateState();
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLoaded(object sender, EventArgs e)
        {
            ValidateState();
        }

        /// <summary>
        /// </summary>
        private void ValidateState()
        {
            AddButton.Enabled = MaxSizeTextBox.Value > 0 &&
                                Directory.Exists(LocalFolderTextBox.Text);
        }
    }
}