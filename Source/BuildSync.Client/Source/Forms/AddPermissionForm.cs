using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BuildSync.Core.Users;

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
