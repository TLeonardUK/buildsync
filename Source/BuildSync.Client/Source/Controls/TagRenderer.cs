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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuildSync.Core.Utils;
using BuildSync.Core.Tags;
using Aga.Controls.Tree;
using Aga.Controls.Tree.NodeControls;

namespace BuildSync.Client.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public class TagListTreeNode : BindableControl
    {
        private TagRenderer Renderer = new TagRenderer();

        public bool ShowFullName = false;

        public TagListTreeNode()
        {
        }

        public override Size MeasureSize(TreeNodeAdv node, DrawContext context)
        {
            return new Size(16, 16);
        }

        public override void Draw(TreeNodeAdv node, DrawContext context)
        {
            Rectangle bounds = this.GetBounds(node, context);

            Renderer.Tags = GetTags(node);
            Renderer.ShowFullName = ShowFullName;
            Renderer.Draw(bounds, context.Graphics);
        }

        public Tag[] GetTags(TreeNodeAdv node)
        {
            return GetValue(node) as Tag[];
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public struct TagRendererResources
    {
        public Brush BackgroundBrush;
        public Pen BorderPen;
        public Brush TextBrush;
        public string TagName;
        public SizeF TextSize;
    }

    /// <summary>
    /// 
    /// </summary>
    public class TagRenderer
    {
        public static Dictionary<string, TagRendererResources> Resources = new Dictionary<string, TagRendererResources>();
        public Tag[] Tags = null;
        public bool ShowFullName = false;

        public void Draw(Rectangle bounds, Graphics graphics)
        {
            if (Tags == null)
            {
                return;
            }

            RectangleF realBounds = bounds;
            for (int i = 0; i < Tags.Length; i++)
            {
                float width = DrawTag(Tags[i], realBounds, graphics);
                realBounds.X += width;
            }
        }

        public static void InvalidateResources()
        {
            Resources.Clear();
        }

        public static TagRendererResources GetTagResources(Tag tag, Graphics graphics, bool ShowFullNames)
        {
            string Key = tag.Id.ToString();
            if (ShowFullNames)
            {
                Key = Key + "_FullName";
            }

            if (Resources.ContainsKey(Key))
            {
                return Resources[Key];
            }

            string tagName = tag.Name;
            if (!ShowFullNames)
            {
                int splitIndex = tagName.LastIndexOf('/');
                if (splitIndex > 0)
                {
                    tagName = tagName.Substring(splitIndex + 1);
                }
            }

            Color DesaturatedColor = Drawing.Saturate(tag.Color, 0.5f);
            Color VerDesaturatedColor = Drawing.Saturate(tag.Color, 0.25f);

            TagRendererResources Res = new TagRendererResources();
            Res.BackgroundBrush = new SolidBrush(tag.Color);// SystemBrushes.Control;
            Res.BorderPen = new Pen(DesaturatedColor);
            Res.TextBrush = new SolidBrush(VerDesaturatedColor);
            Res.TagName = tagName;
            Res.TextSize = graphics.MeasureString(tagName, SystemFonts.DefaultFont);

            Resources.Add(Key, Res);

            return Res;
        }

        public float DrawTag(Tag tag, RectangleF bounds, Graphics graphics)
        {
            float padding = 2;
            float tagPadding = 10;

            TagRendererResources resources = GetTagResources(tag, graphics, ShowFullName);

            float tagHeight = bounds.Height;
            float tagPipWidth = tagHeight * 0.5f;
            float tagWidth = resources.TextSize.Width + (padding * 2);
            float tagX = bounds.X + tagPipWidth;
            float tagY = bounds.Y;

            graphics.FillEllipse(resources.BackgroundBrush, tagX - tagPipWidth, tagY, tagPipWidth * 2, tagHeight);
            graphics.DrawEllipse(resources.BorderPen, tagX - tagPipWidth, tagY, tagPipWidth * 2, tagHeight);
            graphics.FillRectangle(resources.BackgroundBrush, tagX, tagY, tagWidth, tagHeight);
            graphics.DrawRectangle(resources.BorderPen, tagX, tagY, tagWidth, tagHeight);
            graphics.FillRectangle(resources.BackgroundBrush, tagX -1, tagY + 1, 2, tagHeight - 1);
            graphics.DrawString(resources.TagName, SystemFonts.DefaultFont, resources.TextBrush, tagX + padding, tagY + (tagHeight*0.5f) - (resources.TextSize.Height*0.5f));

            return tagPipWidth + tagWidth + tagPadding;
        }
    }
}
