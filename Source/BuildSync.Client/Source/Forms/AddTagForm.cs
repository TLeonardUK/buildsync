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
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using BuildSync.Core.Users;

namespace BuildSync.Client.Forms
{
    /// <summary>
    /// </summary>
    public partial class AddTagForm : Form
    {
        /// <summary>
        /// 
        /// </summary>
        public string TagName = "";

        /// <summary>
        /// 
        /// </summary>
        public Color TagColor = Color.Red;

        /// <summary>
        /// 
        /// </summary>
        public bool TagUnique = false;

        /// <summary>
        /// 
        /// </summary>
        public Guid TagDecayTagId = Guid.Empty;

        /// <summary>
        /// 
        /// </summary>
        public bool InternalSetting = false;

        /// <summary>
        /// </summary>
        public AddTagForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OkClicked(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnShown(object sender, EventArgs e)
        {
            InternalSetting = true;

            nameTextBox.Text = TagName;
            colorSelector.SelectedColor = TagColor;
            uniqueCheckBox.Checked = TagUnique;
            decayTagTextBox.MultipleTags = false;
            decayTagTextBox.TagIds = new List<Guid>();

            if (TagDecayTagId != Guid.Empty)
            {
                decayTagTextBox.TagIds.Add(TagDecayTagId);
            }

            InternalSetting = false;

            UpdateState();
        }

        /// <summary>
        /// </summary>
        private void UpdateState()
        {
            if (InternalSetting)
            {
                return;
            }

            addGroupButton.Enabled = (nameTextBox.Text.Trim().Length > 0);
            TagName = nameTextBox.Text;
            TagColor = colorSelector.SelectedColor;
            TagUnique = uniqueCheckBox.Checked;
            TagDecayTagId = decayTagTextBox.TagIds.Count > 0 ? decayTagTextBox.TagIds[0] : Guid.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StateChanged(object sender, EventArgs e)
        {
            UpdateState();

            decayLabel.Visible = TagUnique;
            decayTagTextBox.Visible = TagUnique;
        }
    }
}