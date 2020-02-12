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
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using BuildSync.Core;

namespace BuildSync.Client.Forms
{
    /// <summary>
    /// </summary>
    public partial class AboutForm : Form
    {
        /// <summary>
        /// </summary>
        public AboutForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Loaded(object sender, EventArgs e)
        {
            VersionLabel.Text = AppVersion.VersionString;

            string ExePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            while (true)
            {
                string HelpDocs = Path.Combine(ExePath, "Docs/Licenses.rtf");
                Console.WriteLine("Trying: {0}", HelpDocs);
                if (File.Exists(HelpDocs))
                {
                    licenseTextBox.LoadFile(HelpDocs);
                    break;
                }

                ExePath = Path.GetDirectoryName(ExePath);
                if (ExePath == null || !ExePath.Contains('\\') && !ExePath.Contains('/'))
                {
                    MessageBox.Show("Failed to find about rtf file, installation may be corrupt.", "Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                }
            }
        }

        /// <summary>
        /// </summary>
        private void OkClicked(object sender, EventArgs e)
        {
            Close();
        }
    }
}