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
using BuildSync.Core.Utils;

namespace BuildSync.Client.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public partial class ColorSelector : UserControl
    {
        /// <summary>
        /// 
        /// </summary>
        public class ColorItem
        {
            public Color Color;
            public string Text;
        }

        /// <summary>
        /// 
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color SelectedColor
        {
            get
            {
                return InternalColors[MainComboBox.SelectedIndex].Color;
            }
            set
            {
                int ClosestIndex = 0;
                float ClosestDistance = float.MaxValue;
                for (int i = 0; i < InternalColors.Length; i++)
                {
                    float Distance = Drawing.ColorDistance(value, InternalColors[i].Color);
                    if (Distance < ClosestDistance)
                    {
                        ClosestIndex = i;
                        ClosestDistance = Distance;
                    }
                }
                MainComboBox.SelectedIndex = ClosestIndex;
            }
        }

        private ColorItem[] InternalColors;

        /// <summary>
        /// 
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ColorItem[] Colors 
        {
            get
            {
                return InternalColors;
            }
            set
            {
                InternalColors = value;

                MainComboBox.Items.Clear();
                foreach (ColorItem Item in InternalColors)
                {
                    MainComboBox.Items.Add(Item);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [Browsable(true)]
        public event EventHandler OnColorChanged;

        /// <summary>
        /// 
        /// </summary>
        public ColorSelector()
        {
            InitializeComponent();

            List<ColorItem> DefaultItems = new List<ColorItem>();
            DefaultItems.Add(new ColorItem { Color = Color.FromArgb(255, 210, 212, 220), Text = "Grey" });
            DefaultItems.Add(new ColorItem { Color = Color.FromArgb(255, 255, 179, 186), Text = "Red" });
            DefaultItems.Add(new ColorItem { Color = Color.FromArgb(255, 255, 223, 186), Text = "Orange" });
            DefaultItems.Add(new ColorItem { Color = Color.FromArgb(255, 255, 255, 186), Text = "Yellow" });
            DefaultItems.Add(new ColorItem { Color = Color.FromArgb(255, 186, 255, 201), Text = "Green" });
            DefaultItems.Add(new ColorItem { Color = Color.FromArgb(255, 186, 255, 255), Text = "Blue" });
            DefaultItems.Add(new ColorItem { Color = Color.FromArgb(255, 179, 153, 212), Text = "Purple" });
            DefaultItems.Add(new ColorItem { Color = Color.FromArgb(255, 223, 169, 149), Text = "Brown" });

            Colors = DefaultItems.ToArray();
            MainComboBox.SelectedIndex = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectedIndexChanged(object sender, EventArgs e)
        {
            OnColorChanged?.Invoke(sender, e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            if (e.Index >= 0)
            {
                ColorItem ColorItem = Colors[e.Index];

                Graphics g = e.Graphics;
                Brush brush = ((e.State & DrawItemState.Selected) == DrawItemState.Selected) ?
                               SystemBrushes.Highlight : new SolidBrush(e.BackColor);
                Brush mainBrush = new SolidBrush(ColorItem.Color);
                Brush darkBrush = new SolidBrush(Drawing.Saturate(ColorItem.Color, 0.25f));
                Pen darkPen = new Pen(Drawing.Saturate(ColorItem.Color, 0.5f));

                float padding = 3;

                g.FillRectangle(brush, e.Bounds);
                g.FillRectangle(mainBrush, e.Bounds.X + padding, e.Bounds.Y + padding, e.Bounds.Height - (padding * 2), e.Bounds.Height - (padding * 2));
                g.DrawRectangle(darkPen, e.Bounds.X + padding, e.Bounds.Y + padding, e.Bounds.Height - (padding * 2), e.Bounds.Height - (padding * 2));
                e.Graphics.DrawString(ColorItem.Text, e.Font, darkBrush, e.Bounds.X + e.Bounds.Height + (padding*1), e.Bounds.Y, StringFormat.GenericDefault);
            }
            e.DrawFocusRectangle();
        }
    }
}
