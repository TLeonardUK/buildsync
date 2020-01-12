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
using BuildSync.Core.Downloads;
using BuildSync.Core.Utils;
using Microsoft.WindowsAPICodePack.Dialogs;

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
                EditState.SelectionRule = (BuildSelectionRule)selectionRuleComboBox.SelectedIndex;
                EditState.SelectionFilter = (BuildSelectionFilter)selectionFilterComboBox.SelectedIndex;
                EditState.SelectionFilterFilePath = scmFileTextBox.Text;
                EditState.ScmWorkspaceLocation = workspaceComboBox.Text;
            }
            else
            {
                Program.DownloadManager.AddDownload(
                    nameTextBox.Text,
                    downloadFileSystemTree.SelectedPath,
                    priorityComboBox.SelectedIndex,
                    (BuildSelectionRule)selectionRuleComboBox.SelectedIndex,
                    (BuildSelectionFilter)selectionFilterComboBox.SelectedIndex,
                    scmFileTextBox.Text,
                    workspaceComboBox.Text,
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
            BuildSelectionRule SelectionRule = (BuildSelectionRule)selectionRuleComboBox.SelectedIndex;
            BuildSelectionFilter SelectionFilter = (BuildSelectionFilter)selectionFilterComboBox.SelectedIndex;

            bool IsScmSelectionRule = (
                SelectionFilter == BuildSelectionFilter.BuildTimeBeforeScmSyncTime ||
                SelectionFilter == BuildSelectionFilter.BuildTimeAfterScmSyncTime ||
                SelectionFilter == BuildSelectionFilter.BuildNameBelowFileContents ||
                SelectionFilter == BuildSelectionFilter.BuildNameAboveFileContents ||
                SelectionFilter == BuildSelectionFilter.BuildNameEqualsFileContents
            );

            bool IsScmFileRule = (
                SelectionFilter == BuildSelectionFilter.BuildNameBelowFileContents ||
                SelectionFilter == BuildSelectionFilter.BuildNameAboveFileContents ||
                SelectionFilter == BuildSelectionFilter.BuildNameEqualsFileContents
            );

            addDownloadButton.Enabled = (
                downloadFileSystemTree.SelectedPath.Length > 0 &&
                nameTextBox.Text.Trim().Length > 3 &&
                priorityComboBox.SelectedIndex >= 0 &&
                (
                    !IsScmSelectionRule ||
                    (workspaceComboBox.Enabled && workspaceComboBox.SelectedIndex >= 0)
                ) 
                &&
                (
                    !IsScmFileRule ||
                    (scmFileTextBox.Enabled && scmFileTextBox.Text.Trim().Length >= 3)
                )
            );

            bool HasSelectedBuild = (downloadFileSystemTree.SelectedManifestId != Guid.Empty);

            autoUpdateCheckBox.Visible = !HasSelectedBuild;
            buildSelectionRulePanel.Visible = !HasSelectedBuild;
            scmSettingsPanel.Visible = !HasSelectedBuild && IsScmSelectionRule;
            scmFilePanel.Visible = !HasSelectedBuild && IsScmFileRule;
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

            selectionRuleComboBox.Items.Clear();
            foreach (Enum val in Enum.GetValues(typeof(BuildSelectionRule)))
            {
                selectionRuleComboBox.Items.Add(EnumUtils.GetAttributeOfType<DescriptionAttribute>(val).Description);
            }
            selectionRuleComboBox.SelectedIndex = (int)BuildSelectionRule.Newest;

            selectionFilterComboBox.Items.Clear();
            foreach (Enum val in Enum.GetValues(typeof(BuildSelectionFilter)))
            {
                selectionFilterComboBox.Items.Add(EnumUtils.GetAttributeOfType<DescriptionAttribute>(val).Description);
            }
            selectionFilterComboBox.SelectedIndex = (int)BuildSelectionFilter.None;

            workspaceComboBox.Items.Clear();
            foreach (ScmWorkspaceSettings Settings in Program.Settings.ScmWorkspaces)
            {
                workspaceComboBox.Items.Add(Settings.Location);
            }

            if (EditState != null)
            {
                Text = "Edit Download";
                nameTextBox.Text = EditState.Name;
                priorityComboBox.SelectedIndex = EditState.Priority;
                scmFileTextBox.Text = EditState.SelectionFilterFilePath;
                selectionRuleComboBox.SelectedIndex = (int)EditState.SelectionRule;
                selectionFilterComboBox.SelectedIndex = (int)EditState.SelectionFilter;

                if (workspaceComboBox.Items.Count > 0)
                {
                    workspaceComboBox.SelectedIndex = 0;
                    for (int i = 0; i < workspaceComboBox.Items.Count; i++)
                    {
                        if (FileUtils.NormalizePath(workspaceComboBox.Items[i] as string) == FileUtils.NormalizePath(EditState.ScmWorkspaceLocation))
                        {
                            workspaceComboBox.SelectedIndex = i;
                            break;
                        }
                    }
                }

                autoUpdateCheckBox.Checked = EditState.UpdateAutomatically;
                autoInstallCheckBox.Checked = EditState.InstallAutomatically;
                downloadFileSystemTree.SelectedPath = EditState.VirtualPath;

                deviceTextBox.Text = EditState.InstallDeviceName;
                downloadFileSystemTree.SelectedPath = EditState.VirtualPath;
                addDownloadButton.Text = "Save Changes";
            }

            if (workspaceComboBox.Items.Count == 0)
            {
                workspaceComboBox.Items.Add("No workspaces defined in preferences");
                workspaceComboBox.SelectedIndex = 0;
                workspaceComboBox.Enabled = false;
            }

            ValidateState();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnShown(object sender, EventArgs e)
        {
            ValidateState();
        }
    }
}
