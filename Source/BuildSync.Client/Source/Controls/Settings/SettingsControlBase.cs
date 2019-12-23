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
    ///     Base control for all controls shown as pages in the <see cref="SettingsForm"/> form.
    /// </summary>
    public partial class SettingsControlBase : UserControl
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="SettingsControlBase"/> class.
        /// </summary>
        public SettingsControlBase()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Gets the title displayed over the settings when this control is displayed.
        /// </summary>
        public virtual string GroupName
        {
            get
            {
                return "Untitled";
            }
        }
    }
}
