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
using BuildSync.Core.Tags;

namespace BuildSync.Client.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public partial class TagTextBox : UserControl
    {
        private class ItemContext
        {
            public Tag Tag;
            public ToolStripMenuItem Parent;
            public string Path;
        }

        /// <summary>
        /// 
        /// </summary>
        private List<Tag> BuildTags = new List<Tag>();

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
        [Browsable(true)]
        public event EventHandler OnTagsChanged;

        /// <summary>
        /// 
        /// </summary>
        [Browsable(true)]
        public bool MultipleTags = true;

        /// <summary>
        /// 
        /// </summary>
        private bool GotTags = false;

        /// <summary>
        /// 
        /// </summary>
        private List<ToolStripMenuItem> MenuItems = new List<ToolStripMenuItem>();

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
        /// 
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        private ToolStripMenuItem CreateMenuItem(string Name, Tag tag, List<ToolStripMenuItem> TouchedItems)
        {
            ToolStripMenuItem Parent = null;

            string[] Split = Name.Split('/');
            if (Split.Length > 1)
            {
                string ParentName = "";
                for (int i = 0; i < Split.Length - 1; i++)
                {
                    if (ParentName.Length > 0)
                    {
                        ParentName += "/";
                    }
                    ParentName += Split[i];
                }

                Parent = CreateMenuItem(ParentName, null, TouchedItems);
            }

            foreach (ToolStripMenuItem ExistingItem in MenuItems)
            {
                if ((ExistingItem.Tag as ItemContext).Path == Name)
                {
                    if (tag != null)
                    {
                        (ExistingItem.Tag as ItemContext).Tag = tag;
                    }
                    TouchedItems.Add(ExistingItem);
                    return ExistingItem;
                }
            }

            ItemContext Context = new ItemContext();
            Context.Parent = Parent;
            Context.Path = Name;
            Context.Tag = tag;

            ToolStripMenuItem Item = new ToolStripMenuItem();
            Item.Text = Split[Split.Length - 1];
            Item.Tag = Context;
            Item.ImageScaling = ToolStripItemImageScaling.None;
            Item.Click += TagItemClicked;
            TagContextMenuStrip.Items.Add(Item);

            if (Parent == null)
            {
                TagContextMenuStrip.Items.Add(Item);
            }
            else
            {
                Parent.DropDownItems.Add(Item);
            }

            MenuItems.Add(Item);
            TouchedItems.Add(Item);

            return Item;
        }

        /// <summary>
        /// </summary>
        /// <param name="Users"></param>
        private void TagsRecieved(List<Tag> InTags)
        {
            BuildTags = InTags;
            GotTags = true;

            List<ToolStripMenuItem> ValidMenuItems = new List<ToolStripMenuItem>();

            // Add each tag.
            foreach (Tag tag in InTags)
            {
                tag.Name = tag.Name.Replace("\\", "/");
                CreateMenuItem(tag.Name.Replace("\\", "/"), tag, ValidMenuItems);
            }

            // Remove tags that no longer exist.
            for (int i = 0; i < MenuItems.Count; i++)
            {
                ToolStripMenuItem Menu = MenuItems[i];
                if (!ValidMenuItems.Contains(Menu))
                {
                    ItemContext Context = Menu.Tag as ItemContext;

                    if (Context.Parent == null)
                    {
                        TagContextMenuStrip.Items.Remove(Menu);
                    }
                    else
                    {
                        Context.Parent.DropDownItems.Remove(Menu);
                    }

                    Logger.Log(LogLevel.Info, LogCategory.Main, "Removing tag menu: {0}", Menu.Text);
                    MenuItems.RemoveAt(i);
                    i--;
                }
            }

            // Remove tags that are no longer valid.
            for (int i = 0; i < TagIdsInternal.Count; i++)
            {
                Guid id = TagIdsInternal[i];
                bool exists = false;

                foreach (Tag tag in InTags)
                {
                    if (tag.Id == id)
                    {
                        exists = true;
                        break;
                    }
                }

                if (!exists)
                {
                    TagIdsInternal.RemoveAt(i);
                    i--;
                }
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

            ItemContext Context = TagItem.Tag as ItemContext;
            if (Context == null || Context.Tag == null)
            {
                return;
            }

            Guid TagId = Context.Tag.Id;
            if (TagIdsInternal.Contains(TagId))
            {
                TagIdsInternal.Remove(TagId);
            }
            else
            {
                if (!MultipleTags)
                {
                    TagIdsInternal.Clear();
                }
                TagIdsInternal.Add(TagId);
            }

            OnTagsChanged?.Invoke(this, new EventArgs());

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
                    foreach (Tag Tag in BuildTags)
                    {
                        if (Tag.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                        {
                            TagIdsInternal.Add(Tag.Id);

                            OnTagsChanged?.Invoke(this, new EventArgs());
                            break;
                        }
                    }
                }

                AddTagNames.Clear();
            }

            // Update the text.
            string Result = "";
            foreach (Tag Tag in BuildTags)
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
            foreach (ToolStripMenuItem Item in MenuItems)
            {
                ItemContext Context = Item.Tag as ItemContext;
                if (Context != null && Context.Tag != null)
                {
                    Item.Checked = TagIdsInternal.Contains(Context.Tag.Id);
                }
            }
        }
    }
}
