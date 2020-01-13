using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BuildSync.Core.Utils;
using BuildSync.Core.Scm;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace BuildSync.Client.Forms
{
    /// <summary>
    /// 
    /// </summary>
    public partial class AddScmWorkspaceForm : Form
    {
        /// <summary>
        /// 
        /// </summary>
        public ScmWorkspaceSettings Settings { get; private set; } = null;

        /// <summary>
        /// 
        /// </summary>
        public AddScmWorkspaceForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        private void ValidateState()
        {
            AddButton.Enabled = (
                StringUtils.IsValidNetAddress(ServerNameTextBox.Text) &&
                (PasswordTextBox.Text.Trim().Length == 0 || UsernameTextBox.Text.Trim().Length > 0) &&
                Directory.Exists(LocalFolderTextBox.Text)
            );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataStateChanged(object sender, EventArgs e)
        {
            ValidateState();
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
            Dialog.Title = "Select Workspace Folder";

            if (Dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                LocalFolderTextBox.Text = Dialog.FileName;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddClicked(object sender, EventArgs e)
        {
            Settings = new ScmWorkspaceSettings();
            Settings.ProviderType = (ScmProviderType)WorkspaceTypeComboBox.SelectedIndex;
            Settings.Server = ServerNameTextBox.Text.Trim();
            Settings.Username = UsernameTextBox.Text.Trim();
            Settings.Password = PasswordTextBox.Text.Trim();
            Settings.Location = LocalFolderTextBox.Text.Trim();

            // Check no other workspaces exist with same local folder.
            foreach (ScmWorkspaceSettings Workspace in Program.Settings.ScmWorkspaces)
            {
                if (FileUtils.NormalizePath(Settings.Location) == FileUtils.NormalizePath(Workspace.Location))
                {
                    MessageBox.Show("A workspace is already configured that exists at the same location.", "Duplicate Workspace", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLoaded(object sender, EventArgs e)
        {
            WorkspaceTypeComboBox.Items.Clear();
            string[] names = Enum.GetNames(typeof(ScmProviderType));
            foreach (string str in names)
            {
                WorkspaceTypeComboBox.Items.Add(str);
            }
            WorkspaceTypeComboBox.SelectedIndex = 0;
        }
    }
}
