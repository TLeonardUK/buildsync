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
using BuildSync.Client.Controls;
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
        private TagToolstripBuilder TagBuilder = new TagToolstripBuilder();

        /// <summary>
        /// 
        /// </summary>
        private TagRenderer TagRenderer = new TagRenderer();

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

            TagBuilder.OnTagClicked += TagItemClicked;
            TagBuilder.OnTagsRefreshed += UpdateState;
            TagBuilder.Attach(TagContextMenuStrip);

            WindowUtils.EnableDoubleBuffering(this);
        }

        ~TagTextBox()
        {
            TagBuilder.OnTagClicked -= TagItemClicked;
            TagBuilder.OnTagsRefreshed -= UpdateState;
            TagBuilder.Detach();
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
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TagItemClicked(Tag Tag, bool Checked)
        {
            Guid TagId = Tag.Id;
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
            if (TagBuilder.HasTags)
            {
                foreach (string name in AddTagNames)
                {
                    foreach (Tag Tag in TagBuilder.Tags)
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

                // Remove any tags that no longer exist.
                for (int i = 0; i < TagIdsInternal.Count; i++)
                {
                    Guid id = TagIdsInternal[i];
                    bool exists = false;

                    foreach (Tag tag in TagBuilder.Tags)
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
            }

            // Update the text.
            /*
            string Result = "";
            foreach (Tag Tag in TagBuilder.Tags)
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
            */

            Invalidate();

            // Set what is checked.
            TagBuilder.SetCheckedTags(TagIdsInternal);
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
            TagBuilder.Refresh();
            UpdateState();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPaint(object sender, PaintEventArgs e)
        {
            List<Tag> ActiveTags = new List<Tag>();

            foreach (Tag Tag in TagBuilder.Tags)
            {
                if (TagIdsInternal.Contains(Tag.Id))
                {
                    ActiveTags.Add(Tag);
                }
            }

            int Padding = 3;

            Rectangle RealBounds = new Rectangle(e.ClipRectangle.X + Padding, e.ClipRectangle.Y + Padding, e.ClipRectangle.Width - (Padding * 2) - 1, e.ClipRectangle.Height - (Padding * 2) - 1);

            TagRenderer.Tags = ActiveTags.ToArray();
            TagRenderer.ShowFullName = false;
            TagRenderer.Draw(RealBounds, e.Graphics);

            e.Graphics.DrawRectangle(SystemPens.ControlDark, e.ClipRectangle.X, e.ClipRectangle.Y, e.ClipRectangle.Width - 1, e.ClipRectangle.Height - 1);
        }
    }
}
