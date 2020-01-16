using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace BuildSync.Core.Utils
{
    /// <summary>
    ///     Contains various helpful utility functions for control drawing.
    /// </summary>
    public static class Drawing
    {
        /// <summary>
        ///     Solid brush similar to the one win10 uses for selected-item backgrounds. Light blue color.
        /// </summary>
        public static Brush SelectedBrush = new SolidBrush(Color.FromArgb(255, 205, 232, 255));

        /// <summary>
        ///     List of various primary colors. Similar to the colors win10 uses to distinguish various elements (categories in task manager etc).
        ///     Meshes well with the colors provided by PrimaryOutlineBrushes.
        /// </summary>
        public static Color[] PrimaryOutlineColors = new Color[]
        {
            Color.FromArgb(255, 17, 125, 187), // Blue
            Color.FromArgb(255, 139, 18, 174), // Purple
            Color.FromArgb(255, 77, 166, 12), // Green
            Color.FromArgb(255, 167, 79, 1), // Brown
            Color.FromArgb(255, 167, 1, 20), // Red
        };

        /// <summary>
        ///     List of various primary colors. Similar to the colors win10 uses to distinguish various elements (categories in task manager etc).
        ///     Meshes well with the colors provided by PrimaryOutlinePens.
        /// </summary>
        public static Color[] PrimaryFillColors = new Color[]
        {
            Color.FromArgb(255, 241, 246, 250), // Blue
            Color.FromArgb(255, 244, 242, 244), // Purple
            Color.FromArgb(255, 239, 247, 233), // Green
            Color.FromArgb(255, 252, 243, 235), // Brown
            Color.FromArgb(255, 252, 235, 237), // Red
        };

        /// <summary>
        ///     Large size font used to drawing item titles.
        /// </summary>
        public static Font TitleFont = new Font(FontFamily.GenericSansSerif, 11.25f, FontStyle.Regular);

        /// <summary>
        ///     Large size font used to drawing item sub titles.
        /// </summary>
        public static Font SubtitleFont = new Font(FontFamily.GenericSansSerif, 8.25f, FontStyle.Regular);
    }
}
