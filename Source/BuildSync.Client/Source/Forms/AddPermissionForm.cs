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

using BuildSync.Core.Users;
using System;
using System.Windows.Forms;

namespace BuildSync.Client.Forms
{
    /// <summary>
    /// 
    /// </summary>
    public partial class AddPermissionForm : Form
    {
        public UserPermission Permission = new UserPermission();

        /// <summary>
        /// 
        /// </summary>
        public AddPermissionForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addDownloadButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VirtualPathChanged(object sender, EventArgs e)
        {
            Permission.VirtualPath = virtualPathTextBox.Text;
            UpdateState();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PermissionChanged(object sender, EventArgs e)
        {
            Permission.Type = (UserPermissionType)permissionTypeComboBox.SelectedIndex;
            UpdateState();
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateState()
        {
            addDownloadButton.Enabled = Permission.Type != UserPermissionType.Unknown;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnShown(object sender, EventArgs e)
        {
            foreach (string value in Enum.GetNames(typeof(UserPermissionType)))
            {
                permissionTypeComboBox.Items.Add(value);
            }
            permissionTypeComboBox.SelectedIndex = (int)UserPermissionType.Unknown;

            UpdateState();
        }
    }
}
