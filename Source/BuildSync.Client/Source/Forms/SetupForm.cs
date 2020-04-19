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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BuildSync.Client.Controls.Setup;

namespace BuildSync.Client.Forms
{
    /// <summary>
    ///     Shows the user a form that guides user through first time seutp.
    ///     Individual setting pages take the form of nested controls, see <see cref="SetupPageControlBase" /> for details.
    /// </summary>
    public partial class SetupForm : Form
    {
        /// <summary>
        /// 
        /// </summary>
        private int PageIndex = 0;

        /// <summary>
        /// 
        /// </summary>
        private bool CloseOnFinish = false;

        /// <summary>
        ///     Initializes a new instance of the <see cref="SetupForm" /> class.
        /// </summary>
        public SetupForm()
        {
            InitializeComponent();

            AddPagePanel<StartSetupPage>();
            AddPagePanel<StorageSetupPage>();
            AddPagePanel<ServerSetupPage>();
            AddPagePanel<TagsSetupPage>();            
            AddPagePanel<FinishSetupPage>();

            UpdateSettingsPanels();
        }

        /// <summary>
        ///     Adds a new page panel by type and associates it with the
        /// </summary>
        /// <typeparam name="Type">Type of settings panel to add.</typeparam>
        private void AddPagePanel<Type>()
            where Type : SetupPageControlBase
        {
            SetupPageControlBase settings = Activator.CreateInstance(typeof(Type)) as SetupPageControlBase;
            settings.Dock = DockStyle.Top;

            pagePanelContainer.Controls.Add(settings);
        }

        /// <summary>
        ///     Updates the settings panel controls. Showing the one that is associated with
        ///     the selected node and hiding the others. Also adjusts the page title text.
        /// </summary>
        private void UpdateSettingsPanels()
        {
            pageGroupNameLabel.Text = string.Empty;

            for (int i = 0; i < pagePanelContainer.Controls.Count; i++)
            {
                SetupPageControlBase panel = pagePanelContainer.Controls[i] as SetupPageControlBase;

                bool selected = (i == PageIndex);
                panel.Visible = selected;

                if (selected)
                {
                    pageGroupNameLabel.Text = panel.Title;
                }
            }

            SetupPageControlBase selectedPanel = pagePanelContainer.Controls[PageIndex] as SetupPageControlBase;
            previousButton.Enabled = selectedPanel.PreviousEnabled;
            nextButton.Enabled = selectedPanel.NextEnabled;

            nextButton.Text = (PageIndex == pagePanelContainer.Controls.Count - 1) ? "Finish" : "Next";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnFormClosed(object sender, FormClosedEventArgs e)
        {
            if (!CloseOnFinish)
            {
                Environment.Exit(0);
                return;
            }

            Program.Settings.FirstRun = false;
            Program.SaveSettings(true);

            FinishSetupPage panel = pagePanelContainer.Controls[PageIndex] as FinishSetupPage;
            if (panel != null)
            {
                if (panel.ShouldStartNewDownload)
                {
                    Program.AppForm.ShowAddDownload();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTick(object sender, EventArgs e)
        {
            UpdateSettingsPanels();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPreviousClicked(object sender, EventArgs e)
        {
            PageIndex--;
            Program.ApplySettings();
            UpdateSettingsPanels();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnNextClicked(object sender, EventArgs e)
        {
            if (PageIndex >= pagePanelContainer.Controls.Count - 1)
            {
                CloseOnFinish = true;
                Close();
                return;
            }
            else
            {
                PageIndex++;
                Program.ApplySettings();
                UpdateSettingsPanels();
            }
        }
    }
}
