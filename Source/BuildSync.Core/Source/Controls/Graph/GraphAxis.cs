using System;
using System.ComponentModel;

namespace BuildSync.Core.Controls.Graph
{
    /// <summary>
    ///     Describes an individual axis in a  <see cref="GraphSeries"/>.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [Serializable]
    public class GraphAxis
    {
        /// <summary>
        ///     Gets or sets minimum value this axis can hold. This will determine the bounds
        ///     of the graph axis when rendered.
        /// </summary>
        public float Min { get; set; } = 0;

        /// <summary>
        ///     Gets or sets the text label shown next to the minimum end of this axis.
        /// </summary>
        public string MinLabel { get; set; } = "0";

        /// <summary>
        ///     Gets or sets maximum value this axis can hold. This will determine the bounds
        ///     of the graph axis when rendered.
        /// </summary>
        public float Max { get; set; } = 100;

        /// <summary>
        ///     Gets or sets the text label shown next to the maximum end of this axis.
        /// </summary>
        public string MaxLabel { get; set; } = "100%";

        /// <summary>
        ///     Gets or sets the interval that grid lines should be drawn on this axis.
        /// </summary>
        /// <remarks>
        ///     Typical value is something in the range of Max / 10.
        /// </remarks>
        public float GridInterval { get; set; } = 10;

        /// <summary>
        ///     Gets or sets a value indicating if maximum values of each axis should adjust based on input recieved.
        /// </summary>
        public bool AutoAdjustMax { get; set; } = false;

        /// <summary>
        ///     Gets or sets if the max value label on the y axis should be formatted as a size if using auto-adjust.
        /// </summary>
        public bool FormatMaxLabelAsSize { get; set; } = false;
    }
}
