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

using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Aga.Controls.Tree;
using Aga.Controls.Tree.NodeControls;

namespace BuildSync.Core.Utils
{
    public class ScaledNodeIcon : NodeIcon
    {
        public Size FixedSize;
        public Size Offset;

        public ScaledNodeIcon()
        {
            ScaleMode = ImageScaleMode.Fit;
        }

        public override Size MeasureSize(TreeNodeAdv node, DrawContext context)
        {
            return FixedSize;
        }

        public override void Draw(TreeNodeAdv node, DrawContext context)
        {
            Image icon = this.GetIcon(node);
            if (icon == null)
                return;
            Rectangle bounds = this.GetBounds(node, context);
            if (icon.Width <= 0 || icon.Height <= 0)
                return;

            context.Graphics.DrawImage(
                icon,
                (bounds.X + (bounds.Width * 0.5f)) - (FixedSize.Width * 0.5f) + Offset.Width,
                (bounds.Y + (bounds.Height * 0.5f)) - (FixedSize.Height * 0.5f) + Offset.Height,
                FixedSize.Width,
                FixedSize.Height);
        }
    }

}