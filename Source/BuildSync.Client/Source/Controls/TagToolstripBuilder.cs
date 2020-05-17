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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BuildSync.Core.Utils;
using BuildSync.Core.Tags;

namespace BuildSync.Client.Controls
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="tag"></param>
    public delegate void TagClickedEventHandler(Tag Tag, bool Checked);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="tag"></param>
    public delegate void TagsRefreshedEventHandler();

    /// <summary>
    /// 
    /// </summary>
    public class TagToolstripBuilder
    {
        /// <summary>
        /// 
        /// </summary>
        private class ItemContext
        {
            public Tag Tag;
            public ToolStripMenuItem Parent;
            public string Path;
        }

        /// <summary>
        /// 
        /// </summary>
        private List<ToolStripMenuItem> MenuItems = new List<ToolStripMenuItem>();

        /// <summary>
        /// 
        /// </summary>
        private ToolStripMenuItem TagContextMenuItem = null;

        /// <summary>
        /// 
        /// </summary>
        private ContextMenuStrip TagContextMenuStrip = null;

        /// <summary>
        /// 
        /// </summary>
        public List<Tag> Tags = new List<Tag>();

        /// <summary>
        /// 
        /// </summary>
        public bool HasTags = false;

        /// <summary>
        /// 
        /// </summary>
        public event TagClickedEventHandler OnTagClicked;

        /// <summary>
        /// 
        /// </summary>
        public event TagsRefreshedEventHandler OnTagsRefreshed;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Item"></param>
        public void Attach(ToolStripMenuItem Item)
        {
            TagContextMenuItem = Item;
            if (Program.NetClient != null)
            {
                Program.NetClient.OnTagListRecieved += TagsRecieved;
                Program.NetClient.RequestTagList();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Item"></param>
        public void Attach(ContextMenuStrip Item)
        {
            TagContextMenuStrip = Item;
            if (Program.NetClient != null)
            {
                Program.NetClient.OnTagListRecieved += TagsRecieved;
                Program.NetClient.RequestTagList();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Detach()
        {
            TagContextMenuItem = null;
            TagContextMenuStrip = null;
            if (Program.NetClient != null)
            {
                Program.NetClient.OnTagListRecieved -= TagsRecieved;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void SetCheckedTags(List<Guid> TagIds)
        {
            foreach (ToolStripMenuItem ExistingItem in MenuItems)
            {
                ItemContext Context = (ExistingItem.Tag as ItemContext);
                ExistingItem.Checked = Context.Tag == null ? false : TagIds.Contains(Context.Tag.Id);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Refresh()
        {
            Program.NetClient.RequestTagList();
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

            if (Parent == null)
            {
                if (TagContextMenuStrip != null)
                {
                    TagContextMenuStrip.Items.Add(Item);
                }
                else
                {
                    TagContextMenuItem.DropDownItems.Add(Item);
                }
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
            Tags = InTags;
            HasTags = true;

            List<ToolStripMenuItem> ValidMenuItems = new List<ToolStripMenuItem>();

            InTags.Sort((Item1, Item2) => -Item1.Name.CompareTo(Item2.Name));

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
                        if (TagContextMenuStrip != null)
                        {
                            TagContextMenuStrip.Items.Remove(Menu);
                        }
                        else
                        {
                            TagContextMenuItem.DropDownItems.Remove(Menu);
                        }
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

            OnTagsRefreshed?.Invoke();
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

            OnTagClicked?.Invoke(Context.Tag, TagItem.Checked);
        }
    }
}
