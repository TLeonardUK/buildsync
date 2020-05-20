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

namespace BuildSync.Core.Utils
{
    /// <summary>
    ///     Contains various helpful utility functions for control drawing.
    /// </summary>
    public static class Drawing
    {
        /// <summary>
        ///     List of various primary colors. Similar to the colors win10 uses to distinguish various elements (categories in
        ///     task manager etc).
        ///     Meshes well with the colors provided by PrimaryOutlinePens.
        /// </summary>
        public static Color[] PrimaryFillColors =
        {
            Color.FromArgb(255, 241, 246, 250), // Blue
            Color.FromArgb(255, 244, 242, 244), // Purple
            Color.FromArgb(255, 239, 247, 233), // Green
            Color.FromArgb(255, 252, 243, 235), // Brown
            Color.FromArgb(255, 252, 235, 237) // Red
        };

        /// <summary>
        ///     List of various primary colors. Similar to the colors win10 uses to distinguish various elements (categories in
        ///     task manager etc).
        ///     Meshes well with the colors provided by PrimaryOutlineBrushes.
        /// </summary>
        public static Color[] PrimaryOutlineColors =
        {
            Color.FromArgb(255, 17, 125, 187), // Blue
            Color.FromArgb(255, 139, 18, 174), // Purple
            Color.FromArgb(255, 77, 166, 12), // Green
            Color.FromArgb(255, 167, 79, 1), // Brown
            Color.FromArgb(255, 167, 1, 20) // Red
        };

        /// <summary>
        ///     Solid brush similar to the one win10 uses for selected-item backgrounds. Light blue color.
        /// </summary>
        public static Brush SelectedBrush = new SolidBrush(Color.FromArgb(255, 205, 232, 255));

        /// <summary>
        ///     Large size font used to drawing item sub titles.
        /// </summary>
        public static Font SubtitleFont = new Font(FontFamily.GenericSansSerif, 8.25f, FontStyle.Regular);

        /// <summary>
        ///     Large size font used to drawing item titles.
        /// </summary>
        public static Font TitleFont = new Font(FontFamily.GenericSansSerif, 11.25f, FontStyle.Regular);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="c"></param>
        /// <param name="saturation"></param>
        /// <returns></returns>
        public static Color Saturate(Color c, double saturation)
        {
            Func<double, int> clamp = i => Math.Min(255, Math.Max(0, Convert.ToInt32(i)));
            return Color.FromArgb(c.A, clamp(c.R * saturation), clamp(c.G * saturation), clamp(c.B * saturation));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static float ColorDistance(Color c1, Color c2)
        {
            int r = c1.R - c2.R;
            int g = c1.G - c2.G;
            int b = c1.B - c2.B;
            return r*r + g*g + b*b;
        }
    }
}