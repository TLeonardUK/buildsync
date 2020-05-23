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
using System.Drawing.Drawing2D;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BuildSync.Core.Utils;

namespace BuildSync.Core.Controls
{
    // TODO: Make this dirtyable, so we don't refresh unneccessarily.

    /// <summary>
    /// 
    /// </summary>
    public partial class SegmentedProgressBar : UserControl
    {
        /// <summary>
        /// 
        /// </summary>
        public class Segment
        {
            /// <summary>
            /// 
            /// </summary>
            public float Proportion = 1.0f;

            /// <summary>
            /// 
            /// </summary>
            public string Text = "";

            /// <summary>
            /// 
            /// </summary>
            public Color Color = Color.LightGreen;

            /// <summary>
            /// 
            /// </summary>
            public bool Marquee = false;

            /// <summary>
            /// 
            /// </summary>
            public float Progress = 0.0f;

            /// <summary>
            /// 
            /// </summary>
            internal float LerpedProgress = 0.0f;

            /// <summary>
            /// 
            /// </summary>
            internal float MarqueeProgress = 0.0f;

            /// <summary>
            /// 
            /// </summary>
            internal Color CachedColor = Color.White;

            /// <summary>
            /// 
            /// </summary>
            internal Color CachedLightColor = Color.White;

            /// <summary>
            /// 
            /// </summary>
            internal Color CachedDarkColor = Color.White;

            /// <summary>
            /// 
            /// </summary>
            internal Font CachedFont = SystemFonts.DefaultFont;

            /// <summary>
            /// 
            /// </summary>
            internal LinearGradientBrush CachedFillBrush = null;
        }

        /// <summary>
        /// 
        /// </summary>
        public List<Segment> Segments = new List<Segment>();

        /// <summary>
        /// 
        /// </summary>
        public SegmentedProgressBar()
        {
            InitializeComponent();

            WindowUtils.EnableDoubleBuffering(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="segment"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        private void DrawSegment(Graphics graphics, Segment segment, float x, float y, float width, float height)
        {
            if (segment.Color != segment.CachedColor)
            {
                segment.CachedLightColor = Color.FromArgb(255, Math.Min(255, (int)(segment.Color.R * 1.1f)), Math.Min(255, (int)(segment.Color.G * 1.1f)), Math.Min(255, (int)(segment.Color.B * 1.1f)));
                segment.CachedDarkColor = Color.FromArgb(255, (int)(segment.Color.R * 0.98f), (int)(segment.Color.G * 0.98f), (int)(segment.Color.B * 0.98f));
                segment.CachedFillBrush = new LinearGradientBrush(new PointF(x, y), new PointF(x, y + height), segment.CachedLightColor, segment.CachedDarkColor);
                segment.CachedColor = segment.Color;
                segment.CachedFont = SystemFonts.DefaultFont;
            }

            segment.LerpedProgress = (segment.Progress * 0.5f) + (segment.LerpedProgress * 0.5f);

            graphics.FillRectangle(SystemBrushes.Control, x, y, width, height);

            if (segment.Marquee)
            {
                segment.MarqueeProgress += 5.0f;
                if (segment.MarqueeProgress >= width - 20)
                {
                    segment.MarqueeProgress = 0.0f;
                }
                graphics.FillRectangle(segment.CachedFillBrush, x + segment.MarqueeProgress, y, 20, height);
            }
            else
            {
                graphics.FillRectangle(segment.CachedFillBrush, x, y, width * segment.LerpedProgress, height);
            }
            graphics.DrawRectangle(SystemPens.ActiveBorder, x, y, width, height);

            SizeF TextSize = graphics.MeasureString(segment.Text, segment.CachedFont);
            graphics.DrawString(segment.Text, segment.CachedFont, SystemBrushes.GrayText, x + (width * 0.5f) - (TextSize.Width * 0.5f), y + (height * 0.5f) - (TextSize.Height * 0.5f));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPaint(object sender, PaintEventArgs e)
        {
            const float Padding = 2;

            float TotalProportion = 0.0f;
            foreach (Segment segment in Segments)
            {
                TotalProportion += segment.Proportion;
            }

            float TotalPadding = (Segments.Count - 1) * Padding;

            float TotalWidth = (e.ClipRectangle.Width - 1) - TotalPadding;

            float X = e.ClipRectangle.X;
            float Y = e.ClipRectangle.Y;
            float Height = e.ClipRectangle.Height - 1;
            foreach (Segment segment in Segments)
            {
                float Width = TotalWidth * (segment.Proportion / TotalProportion);
                DrawSegment(e.Graphics, segment, X, Y, Width, Height);
                X += Width + Padding;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LayoutChanged(object sender, EventArgs e)
        {
            Invalidate();
            Refresh();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnRefresh(object sender, EventArgs e)
        {
            Invalidate();
            Refresh();
        }
    }
}
