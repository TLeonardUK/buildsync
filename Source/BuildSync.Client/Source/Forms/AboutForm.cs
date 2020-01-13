using System;
using System.IO;
using System.Deployment.Application;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Windows.Forms;
using BuildSync.Core;

namespace BuildSync.Client.Forms
{
    /// <summary>
    /// 
    /// </summary>
    public partial class AboutForm : Form
    {
        /// <summary>
        /// 
        /// </summary>
        public AboutForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        private void OkClicked(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Loaded(object sender, EventArgs e)
        {
            VersionLabel.Text = AppVersion.VersionString;

            string ExePath = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            while (true)
            {
                string HelpDocs = Path.Combine(ExePath, "Docs/Licenses.rtf");
                Console.WriteLine("Trying: {0}", HelpDocs);
                if (File.Exists(HelpDocs))
                {
                    licenseTextBox.LoadFile(HelpDocs);
                    break;
                }
                else
                {
                    ExePath = Path.GetDirectoryName(ExePath);
                    if (ExePath == null || !ExePath.Contains('\\') && !ExePath.Contains('/'))
                    {
                        MessageBox.Show("Failed to find about rtf file, installation may be corrupt.", "Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    }
                }
            }
        }
    }
}
