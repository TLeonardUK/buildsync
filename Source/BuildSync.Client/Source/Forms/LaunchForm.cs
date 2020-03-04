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
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using BuildSync.Core.Downloads;
using BuildSync.Core.Manifests;
using BuildSync.Core.Utils;

namespace BuildSync.Client.Forms
{
    /// <summary>
    /// </summary>
    public partial class LaunchForm : Form
    {
        /// <summary>
        /// 
        /// </summary>
        public ManifestDownloadState Downloader;

        /// <summary>
        /// 
        /// </summary>
        public DownloadState DownloadState;

        /// <summary>
        /// 
        /// </summary>
        private BuildSettings Settings;

        /// <summary>
        /// </summary>
        public LaunchForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormHasClosed(object sender, FormClosedEventArgs e)
        {
            Program.SaveSettings();
        }

        /// <summary>
        /// </summary>
        /// <param name=""></param>
        private bool Install(BuildLaunchMode Mode)
        {
            string ResultMessage = "";
            bool Success = true;

            Task work = Task.Run(
                () =>
                {
                    Success = Mode.Install(Downloader.LocalFolder, ref ResultMessage);
                    if (Success)
                    {
                        Downloader.Installed = true;
                    }
                }
            );

            ProgressForm form = new ProgressForm(work);
            form.ShowDialog();

            if (!Success)
            {
                MessageBox.Show("Failed to start install executable with error:\n\n" + ResultMessage, "Install Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return Success;
        }

        /// <summary>
        /// </summary>
        /// <param name=""></param>
        private void Launch(BuildLaunchMode Mode)
        {
            if (!Downloader.Installed || Program.Settings.AlwaysRunInstallBeforeLaunching)
            {
                if (!Install(Mode))
                {
                    return;
                }
            }

            string ResultMessage = "";
            if (!Mode.Launch(Downloader.LocalFolder, ref ResultMessage))
            {
                MessageBox.Show(ResultMessage, "Launch Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            Close();
        }

        /// <summary>
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
                if (StoredSettings != null && !Var.Internal)
                {
                    if (StoredSettings.Values.ContainsKey(Var.Name))
                    {
                        Var.Value = StoredSettings.Values[Var.Name];
                    }
                }

                if (Var.ConditionResult && !Var.Internal)
                {
                    switch (Var.DataType)
                    {
                        case BuildLaunchVariableDataType.String:
                        {
                            if (Var.Options.Count == 0)
                            {
                                GridObj.Add(new DynamicPropertyGridProperty(Var.Name, Var.FriendlyName, Var.FriendlyDescription, Var.FriendlyCategory, Var.Value, Var.Value, false, true));
                            }
                            else
                            {
                                GridObj.Add(new DynamicPropertyGridOptionsProperty(Var.Name, Var.FriendlyName, Var.FriendlyDescription, Var.FriendlyCategory, Var.Value, Var.Value, false, true, Var.Options));
                            }

                            break;
                        }
                        case BuildLaunchVariableDataType.Float:
                        {
                            float Value = 0.0f;
                            float.TryParse(Var.Value, out Value);

                            GridObj.Add(new DynamicPropertyGridRangedProperty(Var.Name, Var.FriendlyName, Var.FriendlyDescription, Var.FriendlyCategory, Value, Value, false, true, Var.MinValue, Var.MaxValue));

                            break;
                        }
                        case BuildLaunchVariableDataType.Int:
                        {
                            int Value = 0;
                            int.TryParse(Var.Value, out Value);

                            GridObj.Add(new DynamicPropertyGridRangedProperty(Var.Name, Var.FriendlyName, Var.FriendlyDescription, Var.FriendlyCategory, Value, Value, false, true, Var.MinValue, Var.MaxValue));

                            break;
                        }
                        case BuildLaunchVariableDataType.Bool:
                        {
                            bool Value = false;
                            bool.TryParse(Var.Value, out Value);

                            GridObj.Add(new DynamicPropertyGridProperty(Var.Name, Var.FriendlyName, Var.FriendlyDescription, Var.FriendlyCategory, Value, Value, false, true));

                            break;
                        }
                    }
                }
            }

            ModePropertyGrid.SelectedObject = GridObj;
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnShown(object sender, EventArgs e)
        {
            string ConfigFilePath = Path.Combine(Downloader.LocalFolder, "buildsync.json");
            bool IsScript = false;
            if (!File.Exists(ConfigFilePath))
            {
                ConfigFilePath = Path.Combine(Downloader.LocalFolder, "buildsync.cs");
                IsScript = true;
            }

            if (!File.Exists(ConfigFilePath))
            {
                MessageBox.Show("Build has no configured launch settings. Ensure a buildsync.json or buildsync.cs file is added to all builds.", "Not Configured", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
                return;
            }

            if (!IsScript)
            {
                if (!SettingsBase.Load(ConfigFilePath, out Settings))
                {
                    MessageBox.Show("The included buildsync.json file could not be loaded, it may be malformed. Check console output window for details.", "Malformed Config File", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Close();
                    return;
                }
            }
            else
            {
                Settings = new BuildSettings();
                try
                {
                    Settings.ScriptSource = File.ReadAllText(ConfigFilePath);
                }
                catch (Exception Ex)
                {
                    Logger.Log(LogLevel.Error, LogCategory.IO, "Failed to read file '{0}' due to exception: {1}", ConfigFilePath, Ex.Message);
                    MessageBox.Show("The included buildsync.cs file could not be loaded, it may be malformed. Check console output window for details.", "Malformed Config File", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Close();
                    return;
                }
            }

            List<BuildLaunchMode> Modes;
            try
            {
                Modes = Settings.Compile();

                // Add various internal variables to pass in bits of info.
                foreach (BuildLaunchMode Mode in Modes)
                {
                    Mode.AddStringVariable("INSTALL_LOCATION", DownloadState.InstallLocation);
                    Mode.AddStringVariable("BUILD_DIR", Downloader.LocalFolder);

                    // We show this internal var to the end user.
                    BuildLaunchVariable DeviceVar = Mode.AddStringVariable("INSTALL_DEVICE_NAME", DownloadState.InstallDeviceName);
                    DeviceVar.Internal = (DownloadState.InstallDeviceName.Length == 0);
                    DeviceVar.FriendlyName = "Target Device";
                    DeviceVar.FriendlyDescription = "Device to install and launch on.";
                    DeviceVar.FriendlyCategory = "Launch Device";
                    DeviceVar.Options = new List<string>();
                    DeviceVar.Value = "";

                    string[] Options = DownloadState.InstallDeviceName.Split(',');
                    foreach (string Option in Options)
                    {
                        string Trimmed = Option.Trim();
                        if (Trimmed.Length > 0)
                        {
                            DeviceVar.Options.Add(Trimmed);

                            if (DeviceVar.Value.Length == 0)
                            {
                                DeviceVar.Value = Trimmed;
                            }
                        }
                    }
                }
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
            }
            else if (Modes.Count == 1 && Modes[0].GetNonInternalVariableCount() == 0 && DownloadState.InstallDeviceName == "") 
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
        /// </summary>
        /// <param name="s"></param>
        /// <param name="e"></param>
        private void PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            StoreSettings();
        }

        /// <summary>
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
                    DynamicPropertyGridProperty Property = GridObj.Find(Var.Name);
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
            while (Program.Settings.LaunchSettings.Count > Client.Settings.MaxLaunchSettings)
            {
                Program.Settings.LaunchSettings.RemoveAt(0);
            }
        }
    }
}