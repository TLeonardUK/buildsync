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
using System.Diagnostics;
using BuildSync.Core.Utils;

namespace BuildSync.Client.Controls.Settings
{
    /// <summary>
    ///     Control thats displayed in the <see cref="SettingsForm" /> to allow the user to configure all
    ///     general settings.
    /// </summary>
    public partial class GeneralSettings : SettingsControlBase
    {
        private readonly bool SkipValidity;

        /// <summary>
        ///     Gets the title displayed over the settings when this control is displayed.
        /// </summary>
        public override string GroupName => "General Settings";

        /// <summary>
        ///     Initializes a new instance of the <see cref="GeneralSettings" /> class.
        /// </summary>
        public GeneralSettings()
        {
            InitializeComponent();

            SkipValidity = true;

            foreach (string key in Enum.GetNames(typeof(LogLevel)))
            {
                logLevelComboBox.Items.Add(key);
            }

            RunOnStartupCheckbox.Checked = Program.Settings.RunOnStartup;
            MinimizeToTrayCheckbox.Checked = Program.Settings.MinimizeToTrayOnClose;
            runInstallWhenLaunchingCheckbox.Checked = Program.Settings.AlwaysRunInstallBeforeLaunching;
            skipVerificationCheckBox.Checked = Program.Settings.SkipValidation;
            skipInitialization.Checked = Program.Settings.SkipDiskAllocation;
            showInternalDownloadsCheckBox.Checked = Program.Settings.ShowInternalDownloads;
            logLevelComboBox.SelectedIndex = (int)Program.Settings.LoggingLevel;
            autoFixValidationErrorsCheckBox.Checked = Program.Settings.AutoFixValidationErrors;
            allowRemoteActionsCheckBox.Checked = Program.Settings.AllowRemoteActions;

            SkipValidity = false;
            
            UpdateValidityState();
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

            Program.Settings.RunOnStartup = RunOnStartupCheckbox.Checked;
            Program.Settings.MinimizeToTrayOnClose = MinimizeToTrayCheckbox.Checked;
            Program.Settings.AlwaysRunInstallBeforeLaunching = runInstallWhenLaunchingCheckbox.Checked;
            Program.Settings.SkipValidation = skipVerificationCheckBox.Checked;
            Program.Settings.SkipDiskAllocation = skipInitialization.Checked;
            Program.Settings.ShowInternalDownloads = showInternalDownloadsCheckBox.Checked;
            Program.Settings.LoggingLevel = (LogLevel)logLevelComboBox.SelectedIndex;
            Program.Settings.AutoFixValidationErrors = autoFixValidationErrorsCheckBox.Checked;
            Program.Settings.AllowRemoteActions = allowRemoteActionsCheckBox.Checked;

            Logger.MaximumVerbosity = Program.Settings.LoggingLevel;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ViewLogsClicked(object sender, EventArgs e)
        {
            string LoggingDir = Path.Combine(Program.AppDataDir, "Logging");
            Process.Start("explorer", LoggingDir);
        }
    }
}