using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;
using BuildSync.Core.Downloads;
using BuildSync.Core.Manifests;
using BuildSync.Core.Utils;
using BuildSync.Client;

namespace BuildSync.Client.Forms
{

    /// <summary>
    /// 
    /// </summary>
    public partial class LaunchForm : Form
    {
        public ManifestDownloadState Downloader;

        private BuildSettings Settings;

        /// <summary>
        /// 
        /// </summary>
        public LaunchForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name=""></param>
        private void Launch(BuildLaunchMode Mode)
        {
            string ExePath = Mode.Executable;
            if (!Path.IsPathRooted(ExePath))
            {
                ExePath = Path.Combine(Downloader.LocalFolder, ExePath);
            }

            string WorkingDir = Mode.WorkingDirectory;
            if (WorkingDir.Length == 0)
            {
                WorkingDir = Path.GetDirectoryName(ExePath);
            }
            else
            {
                if (!Path.IsPathRooted(WorkingDir))
                {
                    WorkingDir = Path.Combine(Downloader.LocalFolder, WorkingDir);
                }
            }

#if SHIPPING
            if (!File.Exists(ExePath))
            {
                MessageBox.Show("Could not find executable, expected to be located at: " + ExePath, "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
                return;
            }

            if (!File.Exists(WorkingDir))
            {
                MessageBox.Show("Could not find working directory, expected at: " + WorkingDir, "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
                return;
            }
#endif

            string CompiledArguments = "";
            try
            { 
                CompiledArguments = Mode.CompileArguments();
            }
            catch (InvalidOperationException Ex)
            {
                MessageBox.Show("Error encountered while evaluating launch settings:\n\n" + Ex.Message, "Malformed Config File", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
                return;
            }

            try
            {
                ProcessStartInfo StartInfo = new ProcessStartInfo();
                StartInfo.FileName = ExePath;
                StartInfo.WorkingDirectory = WorkingDir;
                StartInfo.Arguments = CompiledArguments;
                Process.Start(StartInfo);
            }
            catch (Exception Ex)
            {
                MessageBox.Show("Failed to start executable with error:\n\n" + Ex.Message, "Launch Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
                return;
            }

            Close();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnShown(object sender, EventArgs e)
        {
            string ConfigFilePath = Path.Combine(Downloader.LocalFolder, "buildsync.json");
            if (!File.Exists(ConfigFilePath))
            {
                MessageBox.Show("Build has no configured launch settings. Ensure a buildsync.json file is added to all builds.", "Not Configured", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
                return;
            }

            if (!BuildSettings.Load<BuildSettings>(ConfigFilePath, out Settings))
            {
                MessageBox.Show("The included buildsync.json file could not be loaded, it may be malformed.", "Malformed Config File", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
                return;
            }

            List<BuildLaunchMode> Modes;
            try
            {
                Modes = Settings.Compile();
            }
            catch (InvalidOperationException Ex)
            {
                MessageBox.Show("Error encountered while evaluating launch settings:\n\n" + Ex.Message, "Malformed Config File", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
                return;
            }

            if (Modes.Count == 0)
            {
                MessageBox.Show("None of the launch modes are usable.", "Not Configured", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
                return;
            }
            else if (Modes.Count == 1 && Modes[0].Variables.Count == 0)
            {
                // Instant launch, nothing for us to select really.
                Launch(Modes[0]);
            }
            else
            {
                // Show options to launch.
                foreach (BuildLaunchMode Mode in Modes)
                {
                    TreeNode Node = new TreeNode();
                    Node.Text = Mode.Name;
                    Node.Tag = Mode;
                    ModesTreeView.Nodes.Add(Node);
                }

                // Select first mode.
                ModesTreeView.SelectedNode = ModesTreeView.Nodes[0];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LaunchModeNodeSelected(object sender, TreeViewEventArgs e)
        {
            if (e.Node == null)
            {
                ModePropertyGrid.SelectedObject = null;
                return;
            }

            BuildLaunchMode Mode = e.Node.Tag as BuildLaunchMode;
            if (Mode == null)
            {
                ModePropertyGrid.SelectedObject = null;
                return;
            }


            // Have we got a stored value for this variable?
            StoredLaunchSettings StoredSettings = null;
            foreach (StoredLaunchSettings OldSettings in Program.Settings.LaunchSettings)
            {
                if (OldSettings.ManifestId == Downloader.ManifestId)
                {
                    StoredSettings = OldSettings;
                    break;
                }
            }

            DynamicPropertyGridObject GridObj = new DynamicPropertyGridObject();
            foreach (BuildLaunchVariable Var in Mode.Variables)
            {
                if (StoredSettings != null)
                {
                    if (StoredSettings.Values.ContainsKey(Var.Name))
                    {
                        Var.Value = StoredSettings.Values[Var.Name];
                    }
                }

                if (Var.ConditionResult)
                {
                    switch (Var.DataType)
                    {
                        case BuildLaunchVariableDataType.String:
                            {
                                if (Var.Options.Count == 0)
                                {
                                    GridObj.Add(new DynamicPropertyGridProperty(Var.FriendlyName, Var.FriendlyDescription, Var.FriendlyCategory, Var.Value, Var.Value, false, true));
                                }
                                else
                                {
                                    GridObj.Add(new DynamicPropertyGridOptionsProperty(Var.FriendlyName, Var.FriendlyDescription, Var.FriendlyCategory, Var.Value, Var.Value, false, true, Var.Options));
                                }

                                break;
                            }
                        case BuildLaunchVariableDataType.Float:
                            {
                                float Value = 0.0f;
                                float.TryParse(Var.Value, out Value);

                                GridObj.Add(new DynamicPropertyGridRangedProperty(Var.FriendlyName, Var.FriendlyDescription, Var.FriendlyCategory, Value, Value, false, true, Var.MinValue, Var.MaxValue));

                                break;
                            }
                        case BuildLaunchVariableDataType.Int:
                            {
                                int Value = 0;
                                int.TryParse(Var.Value, out Value);

                                GridObj.Add(new DynamicPropertyGridRangedProperty(Var.FriendlyName, Var.FriendlyDescription, Var.FriendlyCategory, Value, Value, false, true, Var.MinValue, Var.MaxValue));

                                break;
                            }
                        case BuildLaunchVariableDataType.Bool:
                            {
                                bool Value = false;
                                bool.TryParse(Var.Value, out Value);

                                GridObj.Add(new DynamicPropertyGridProperty(Var.FriendlyName, Var.FriendlyDescription, Var.FriendlyCategory, Value, Value, false, true));

                                break;
                            }
                    }
                }
            }

            ModePropertyGrid.SelectedObject = GridObj;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LaunchClicked(object sender, EventArgs e)
        {
            if (ModesTreeView.SelectedNode == null)
            {
                return;
            }

            BuildLaunchMode Mode = ModesTreeView.SelectedNode.Tag as BuildLaunchMode;
            Launch(Mode);
        }

        /// <summary>
        /// 
        /// </summary>
        private void StoreSettings()
        {
            if (ModesTreeView.SelectedNode == null || ModePropertyGrid.SelectedObject == null)
            {
                return;
            }

            BuildLaunchMode Mode = ModesTreeView.SelectedNode.Tag as BuildLaunchMode;
            DynamicPropertyGridObject GridObj = ModePropertyGrid.SelectedObject as DynamicPropertyGridObject;

            foreach (BuildLaunchVariable Var in Mode.Variables)
            {
                if (Var.ConditionResult)
                {
                    DynamicPropertyGridProperty Property = GridObj.Find(Var.FriendlyName);
                    if (Property == null)
                    {
                        continue;
                    }

                    if (Var.DataType == BuildLaunchVariableDataType.Bool)
                    {
                        Var.Value = Property.Value.ToString().ToLower();
                    }
                    else
                    {
                        Var.Value = Property.Value.ToString();
                    }
                }
            }

            // Remove old stored settings value.
            foreach (StoredLaunchSettings OldSettings in Program.Settings.LaunchSettings)
            {
                if (OldSettings.ManifestId == Downloader.ManifestId)
                {
                    Program.Settings.LaunchSettings.Remove(OldSettings);
                    break;
                }
            }

            // Add new stored settings value.
            StoredLaunchSettings LaunchSettings = new StoredLaunchSettings();
            LaunchSettings.ManifestId = Downloader.ManifestId;
            foreach (BuildLaunchVariable Var in Mode.Variables)
            {
                LaunchSettings.Values.Add(Var.Name, Var.Value);
            }
            Program.Settings.LaunchSettings.Add(LaunchSettings);

            // If over max launch settings prune.
            while (Program.Settings.LaunchSettings.Count > BuildSync.Client.Settings.MaxLaunchSettings)
            {
                Program.Settings.LaunchSettings.RemoveAt(0);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <param name="e"></param>
        private void PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            StoreSettings();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormHasClosed(object sender, FormClosedEventArgs e)
        {
            Program.SaveSettings();
        }
    }
}
