using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

namespace BuildSync.Core.Controls.Graph
{
    /// <summary>
    ///     Describes an individual data series within a <see cref="Graph"/>.
    /// </summary>
    [Serializable]
    public class GraphSeries
    {
        /// <summary>
        ///     Gets or sets the name of this series. Used in legends and shown above
        ///     the graph in single-series graphs.
        /// </summary>
        public string Name { get; set; } = "Untitled Series";

        /// <summary>
        ///     Gets or sets the color used to fill the series.
        /// </summary>
        public Color Fill { get; set; } = Color.FromArgb(255, 241, 246, 250);

        /// <summary>
        ///     Gets or sets the color used to outline the series. This outline is used
        ///     for the actual graph line and the grpah border.
        /// </summary>
        public Color Outline { get; set; } = Color.FromArgb(255, 68, 153, 201);

        /// <summary>
        ///     Makes the ouline line dots and dashes, easy way to distiguish between multiple series if 
        ///     you don't wish to change color.
        /// </summary>
        public bool OutlineDotted { get; set; } = false;

        /// <summary>
        ///     Gets the X-Axis of the series.
        /// </summary>
        public GraphAxis XAxis { get; } = new GraphAxis();

        /// <summary>
        ///     Gets the Y-Axis of the series.
        /// </summary>
        public GraphAxis YAxis { get; } = new GraphAxis();

        /// <summary>
        ///     Gets or sets a value indicating whether this series is a sliding window. If it is 
        ///     then old data points will be pruned when the range of x-values is greater than the X-Axis maximum value.
        /// </summary>
        public bool SlidingWindow { get; set; } = true;

        /// <summary>
        ///     Gets or sets the minimal X axis between samples recorded on the graph. 0 will record on every call to AddDataPoint
        /// </summary>
        public float MinimumInterval { get; set; } = 0;

        /// <summary>
        ///     Gets the list of data points in the series. 
        /// </summary>
        /// <remarks>
        ///     Do not modify directly. Use the  <see cref="AddDataPoint"/> function instead.
        /// </remarks>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal List<GraphDataPoint> Data { get; } = new List<GraphDataPoint>();

        /// <summary>
        ///     Adds a new data point to the graph.
        /// </summary>
        /// <param name="x">Data points value on the x-axis.</param>
        /// <param name="y">Data points value on the y-axis.</param>
        public void AddDataPoint(float x, float y)
        {
            if (MinimumInterval != 0 && this.Data.Count > 0)
            {
                float Elapsed = (x - this.Data[this.Data.Count - 1].X);
                if (Elapsed < MinimumInterval)
                {
                    return;
                }
            }

            GraphDataPoint newPoint = new GraphDataPoint() { X = x, Y = y };
            this.Data.Add(newPoint);

            // Adjust Y axis if value added is over current max.
            if (YAxis.AutoAdjustMax)
            {
                float OriginalMax = YAxis.Max;

                YAxis.Max = Math.Max(y, YAxis.Max);
                YAxis.GridInterval = YAxis.Max / 10.0f;

                if (YAxis.Max != OriginalMax)
                {
                    if (YAxis.FormatMaxLabelAsSize)
                    {
                        YAxis.MaxLabel = Utils.StringUtils.FormatAsSize((long)YAxis.Max); // TODO: add some formatting to this.
                    }
                }
            }

            // If this is a sliding window we need to remove any points outside the window.
            if (this.SlidingWindow)
            {
                bool bWasRemoved = false;
                float startValue = 0.0f;

                for (int i = 0; i < this.Data.Count; i++)
                {
                    GraphDataPoint point = this.Data[i];
                    if ((x - point.X) > this.XAxis.Max)
                    {
                        this.Data.RemoveAt(i);
                        i--;

                        bWasRemoved = true;
                        startValue = point.Y;
                    }
                }

                // If we have removed an entry, add a dummy value back to the start to ensure our graph
                // doesn't suddenly clip at the end. Happens if the value being removed is over window and the value being 
                // added is less than the window.
                if (bWasRemoved)
                {
                    AddDataPoint(x - this.XAxis.Max, startValue);
                }
            }

            // Always sort the resulting data along the X-axis, it makes drawing simpler.
            // If this becomes an issue, do sorting at the insertion point rather than sorting
            // entire list.
//            this.Data.Sort((c1, c2) => Math.Sign(c1.X - c2.X));
        }

        /// <summary>
        ///     Gets the value at a given point on the X-axis.
        /// </summary>
        /// <param name="x">Position on x-axis to get value.</param>
        /// <param name="result">Reference to variable to store value in.</param>
        /// <returns>True if a value was retrieved. False if a value could not be retrieved because no data points are available for the given position on the x-axis.</returns>
        public bool GetValueAtPoint(float x, out float result)
        {
            result = 0.0f;

            for (int i = 0; i < this.Data.Count; i++)
            {
                GraphDataPoint point = this.Data[i];

                // Point is exactly x.
                if (point.X == x)
                {
                    result = point.Y;
                    return true;
                }
                else
                {
                    // Point is somewhere between last value and this value.
                    if (point.X > x)
                    {
                        if (i > 0)
                        {
                            GraphDataPoint prevPoint = this.Data[i - 1];

                            // Linear interpolate to get value at x-value.
                            float delta = (x - prevPoint.X) / (point.X - prevPoint.X);
                            result = prevPoint.X + (point.X - prevPoint.X) * delta;

                            return true;
                        }
                        
                        // No last value = no data for given point.
                        return false;
                    }
                }
            }

            return false;
        }
    }
}
