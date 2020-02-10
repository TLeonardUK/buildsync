using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BuildSync.Client.Controls.Settings
{
    /// <summary>
    ///     Control thats displayed in the <see cref="SettingsForm"/> to allow the user to configure all
    ///     general settings.
    /// </summary>
    public partial class GeneralSettings : SettingsControlBase
    {
        private bool SkipValidity = false;

        /// <summary>
        ///     Initializes a new instance of the <see cref="GeneralSettings"/> class.
        /// </summary>
        public GeneralSettings()
        {
            InitializeComponent();

            SkipValidity = true;
            RunOnStartupCheckbox.Checked = Program.Settings.RunOnStartup;
            MinimizeToTrayCheckbox.Checked = Program.Settings.MinimizeToTrayOnClose;
            runInstallWhenLaunchingCheckbox.Checked = Program.Settings.AlwaysRunInstallBeforeLaunching;
            skipVerificationCheckBox.Checked = Program.Settings.SkipValidation;
            skipInitialization.Checked = Program.Settings.SkipDiskAllocation;
            SkipValidity = false;

            UpdateValidityState();
        }

        /// <summary>
        ///     Gets the title displayed over the settings when this control is displayed.
        /// </summary>
        public override string GroupName
        {
            get
            {
                return "General Settings";
            }
        }

        /// <summary>
        /// 
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
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StateChanged(object sender, EventArgs e)
        {
            UpdateValidityState();
        }
    }
}
