using System;

namespace BuildSync.Core.Controls.Graph
{
    /// <summary>
    ///     Describes an individual data point within a <see cref="GraphSeries"/>.
    /// </summary>
    [Serializable]
    public struct GraphDataPoint
    {
        /// <summary>
        ///     The value on the x-axis of this data point.
        /// </summary>
        public float X;

        /// <summary>
        ///     The value on the y-axis of this data point.
        /// </summary>
        public float Y;
    }
}
