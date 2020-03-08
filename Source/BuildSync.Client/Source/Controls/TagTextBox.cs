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
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BuildSync.Core.Manifests;
using BuildSync.Core.Utils;

namespace BuildSync.Client.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public partial class TagTextBox : UserControl
    {
        /// <summary>
        /// 
        /// </summary>
        private List<BuildManifestTag> BuildTags = new List<BuildManifestTag>();

        /// <summary>
        /// 
        /// </summary>
        private List<Guid> TagIdsInternal = new List<Guid>();

        /// <summary>
        /// 
        /// </summary>
        private List<string> AddTagNames = new List<string>();

        /// <summary>
        /// 
        /// </summary>
        private bool GotTags = false;

        /// <summary>
        /// 
        /// </summary>
        public List<Guid> TagIds
        {
            get
            {
                return TagIdsInternal;
            }
            set
            {
                TagIdsInternal = value;
                UpdateState();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public TagTextBox()
        {
            InitializeComponent();

            if (Program.NetClient != null)
            {
                Program.NetClient.OnTagListRecieved += TagsRecieved;
                Program.NetClient.RequestTagList();
            }
        }

        ~TagTextBox()
        {
            if (Program.NetClient != null)
            {
                Program.NetClient.OnTagListRecieved -= TagsRecieved;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public void AddTagByName(string Name)
        {
            AddTagNames.Add(Name);

            UpdateState();
        }

        /// <summary>
        /// </summary>
        /// <param name="Users"></param>
        private void TagsRecieved(List<BuildManifestTag> InTags)
        {
            BuildTags = InTags;
            GotTags = true;

            // Add new tags.
            foreach (BuildManifestTag Tag in InTags)
            {
                bool Found = false;
                foreach (ToolStripMenuItem Item in TagContextMenuStrip.Items)
                {
                    if ((Item.Tag as BuildManifestTag).Id == Tag.Id)
                    {
                        Found = true;
                        break;
                    }
                }

                if (!Found)
                {
                    ToolStripMenuItem TagItem = new ToolStripMenuItem();
                    TagItem.Text = Tag.Name;
                    TagItem.Tag = Tag;
                    TagItem.ImageScaling = ToolStripItemImageScaling.None;
                    TagItem.Click += TagItemClicked;
                    TagContextMenuStrip.Items.Add(TagItem);

                    Logger.Log(LogLevel.Info, LogCategory.Main, "Added tag menu: {0}", Tag.Name);
                }
            }

            // Remove old tags.
            List<ToolStripMenuItem> RemovedItems = new List<ToolStripMenuItem>();
            foreach (ToolStripMenuItem Item in TagContextMenuStrip.Items)
            {
                bool Found = false;

                foreach (BuildManifestTag Tag in InTags)
                {
                    if ((Item.Tag as BuildManifestTag).Id == Tag.Id)
                    {
                        Found = true;
                        break;
                    }
                }

                if (!Found)
                {
                    RemovedItems.Add(Item);
                }
            }

            foreach (ToolStripMenuItem Item in RemovedItems)
            {
                Logger.Log(LogLevel.Info, LogCategory.Main, "Removing tag menu: {0}", Item.Text);
                TagContextMenuStrip.Items.Remove(Item);
            }

            UpdateState();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TagItemClicked(object sender, EventArgs e)
        {
            ToolStripMenuItem TagItem = sender as ToolStripMenuItem;
            if (TagItem == null)
            {
                return;
            }

            Guid TagId = (TagItem.Tag as BuildManifestTag).Id;
            if (TagIdsInternal.Contains(TagId))
            {
                TagIdsInternal.Remove(TagId);
            }
            else
            {
                TagIdsInternal.Add(TagId);
            }

            UpdateState();
        }

        /// <summary>
        /// 
        /// </summary>
        public void UpdateState()
        {
            // Add any pending tags.
            if (GotTags)
            {
                foreach (string name in AddTagNames)
                {
                    foreach (BuildManifestTag Tag in BuildTags)
                    {
                        if (Tag.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                        {
                            TagIdsInternal.Add(Tag.Id);
                            break;
                        }
                    }
                }

                AddTagNames.Clear();
            }

            // Update the text.
            string Result = "";
            foreach (BuildManifestTag Tag in BuildTags)
            {
                if (TagIdsInternal.Contains(Tag.Id))
                {
                    if (Result.Length > 0)
                    {
                        Result += ", ";
                    }
                    Result += Tag.Name;
                }
            }
            MainTextBox.Text = Result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClicked(object sender, MouseEventArgs e)
        {
            TagContextMenuStrip.Show(Cursor.Position);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ContextMenuStripOpening(object sender, CancelEventArgs e)
        {
            foreach (ToolStripMenuItem Item in TagContextMenuStrip.Items)
            {
                BuildManifestTag MenuTag = (Item.Tag as BuildManifestTag);
                Item.Checked = TagIdsInternal.Contains(MenuTag.Id);
            }
        }
    }
}
