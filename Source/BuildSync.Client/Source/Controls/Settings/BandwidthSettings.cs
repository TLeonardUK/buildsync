﻿using System;
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
    ///     bandwidth settings.
    /// </summary>
    public partial class BandwidthSettings : SettingsControlBase
    {
        private bool SkipValidity = false;

        /// <summary>
        ///     Initializes a new instance of the <see cref="BandwidthSettings"/> class.
        /// </summary>
        public BandwidthSettings()
        {
            InitializeComponent();

            SkipValidity = true;
            MaxUploadBandwidthBox.Value = Program.Settings.BandwidthMaxUp * 1024;
            MaxDownloadBandwidthBox.Value = Program.Settings.BandwidthMaxDown * 1024;
            BandwidthTimespanStartHourBox.Value = Program.Settings.BandwidthStartTimeHour;
            BandwidthTimespanStartMinBox.Value = Program.Settings.BandwidthStartTimeMin;
            BandwidthTimespanEndHourBox.Value = Program.Settings.BandwidthEndTimeHour;
            BandwidthTimespanEndMinBox.Value = Program.Settings.BandwidthEndTimeMin;
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
                return "Bandwidth Settings";
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

            Program.Settings.BandwidthMaxUp = (long)MaxUploadBandwidthBox.Value / 1024;
            Program.Settings.BandwidthMaxDown = (long)MaxDownloadBandwidthBox.Value / 1024;
            Program.Settings.BandwidthStartTimeHour = (int)BandwidthTimespanStartHourBox.Value;
            Program.Settings.BandwidthStartTimeMin = (int)BandwidthTimespanStartMinBox.Value;
            Program.Settings.BandwidthEndTimeHour = (int)BandwidthTimespanEndHourBox.Value;
            Program.Settings.BandwidthEndTimeMin = (int)BandwidthTimespanEndMinBox.Value;
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
