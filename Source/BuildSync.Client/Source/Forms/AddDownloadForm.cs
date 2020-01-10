using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BuildSync.Core.Downloads;

namespace BuildSync.Client.Forms
{
    /// <summary>
    /// 
    /// </summary>
    public partial class AddDownloadForm : Form
    {
        /// <summary>
        /// 
        /// </summary>
        public DownloadState EditState;

        /// <summary>
        /// 
        /// </summary>
        public AddDownloadForm()
        {
            InitializeComponent();

            DialogResult = DialogResult.Abort;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddDownloadClicked(object sender, EventArgs e)
        {
            if (EditState != null)
            {
                EditState.Name = nameTextBox.Text;
                EditState.Priority = priorityComboBox.SelectedIndex;
                EditState.UpdateAutomatically = autoUpdateCheckBox.Checked;
                EditState.InstallAutomatically = autoInstallCheckBox.Checked;
                EditState.InstallDeviceName = deviceTextBox.Text;
                EditState.VirtualPath = downloadFileSystemTree.SelectedPath;
            }
            else
            {
                Program.DownloadManager.AddDownload(
                    nameTextBox.Text,
                    downloadFileSystemTree.SelectedPath,
                    priorityComboBox.SelectedIndex,
                    autoUpdateCheckBox.Checked,
                    autoInstallCheckBox.Checked,
                    deviceTextBox.Text
                );
            }

            Program.SaveSettings();

            DialogResult = DialogResult.OK;
            Close();
        }

        /// <summary>
        /// 
        /// </summary>
        private void ValidateState()
        {
            addDownloadButton.Enabled = (
                downloadFileSystemTree.SelectedPath.Length > 0 &&
                nameTextBox.Text.Trim().Length > 3 &&
                priorityComboBox.SelectedIndex >= 0
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
        private void OnLoaded(object sender, EventArgs e)
        {
            priorityComboBox.Items.Clear();
            priorityComboBox.Items.Add("Lowest");
            priorityComboBox.Items.Add("Low");
            priorityComboBox.Items.Add("Normal");
            priorityComboBox.Items.Add("High");
            priorityComboBox.Items.Add("Highest");
            priorityComboBox.SelectedIndex = 2;

            if (EditState != null)
            {
                Text = "Edit Download";
                nameTextBox.Text = EditState.Name;
                priorityComboBox.SelectedIndex = EditState.Priority;
                autoUpdateCheckBox.Checked = EditState.UpdateAutomatically;                
                autoInstallCheckBox.Checked = EditState.InstallAutomatically;
                deviceTextBox.Text = EditState.InstallDeviceName;
                downloadFileSystemTree.SelectedPath = EditState.VirtualPath;
                addDownloadButton.Text = "Save Changes";
            }
        }
    }
}
