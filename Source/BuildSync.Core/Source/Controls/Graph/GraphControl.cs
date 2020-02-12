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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using BuildSync.Core.Utils;

namespace BuildSync.Core.Controls.Graph
{
    /// <summary>
    ///     Control that is used to display data to the user in the form of a stacked area chart (line chart with area below
    ///     the lines filled in).
    /// </summary>
    public partial class GraphControl : UserControl
    {
        /// <summary>
        ///     Determines if we are currently drawining label information.
        /// </summary>
        private bool drawLabels = true;

        /// <summary>
        ///     Gets or sets a value indicating whether if grid lines are drawn in th ebackground of the graph.
        /// </summary>
        /// <remarks>
        ///     The interval of the grid lines is defined in each value of the graphs <see cref="Series" />.
        /// </remarks>
        public bool DrawGridLines { get; set; } = true;

        /// <summary>
        ///     Gets or sets a value indicating whether we are rendering labels for the graph.
        /// </summary>
        public bool DrawLabels
        {
            get => drawLabels;
            set
            {
                if (drawLabels == false)
                {
                    throw new InvalidOperationException("DrawLabels cannot be toggled after being disabled.");
                }

                drawLabels = value;

                if (!drawLabels && mainPanel != null)
                {
                    mainPanel.Dock = DockStyle.Fill;
                    graphNameLabel.Visible = false;
                    xSeriesMaxLabel.Visible = false;
                    xSeriesMinLabel.Visible = false;
                    ySeriesMaxLabel.Visible = false;
                }
            }
        }

        /// <summary>
        ///     Gets or sets the color used to render grid lines.
        /// </summary>
        public Color GridColor { get; set; } = Color.FromArgb(255, 223, 238, 246);

        /// <summary>
        ///     Gets or sets the set of data series that are drawn on the graph.
        /// </summary>
        /// <remarks>
        ///     Series are drawn on top of each other, from start to end of the array.
        /// </remarks>
        public GraphSeries[] Series { get; set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Graph" /> class.
        /// </summary>
        public GraphControl()
        {
            InitializeComponent();

            WindowUtils.EnableDoubleBuffering(mainPanel);
        }

        /// <summary>
        ///     Paints the graph onto the given graphics context in the given clip bounds..
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="clipRactangle"></param>
        public void PaintControl(Graphics graphics, Rectangle clipRactangle)
        {
            Brush backgroundBrush = Brushes.White;
            Pen gridPen = Pens.LightGray;

            Rectangle clipArea = new Rectangle(
                clipRactangle.X,
                clipRactangle.Y,
                clipRactangle.Width - 1,
                clipRactangle.Height - 1
            );

            // Default mode looks like ass with a high-noise graph.
            graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // Fill background with a white canvas.
            graphics.FillRectangle(backgroundBrush, clipArea);

            // Can't do anything else without some graph series!
            Pen primaryOutlinePen = Pens.Black;
            if (Series != null && Series.Length != 0)
            {
                // Draw each series.
                foreach (GraphSeries series in Series)
                {
                    if (series != null)
                    {
                        PaintSeries(series, graphics, clipArea);
                    }
                }

                GraphSeries primarySeries = Series[0];
                if (primarySeries == null)
                {
                    return;
                }

                primaryOutlinePen = new Pen(primarySeries.Outline);

                graphNameLabel.Text = primarySeries.Name;
                xSeriesMaxLabel.Text = primarySeries.XAxis.MaxLabel;
                xSeriesMinLabel.Text = primarySeries.XAxis.MinLabel;
                ySeriesMaxLabel.Text = primarySeries.YAxis.MaxLabel;

                if (DrawGridLines)
                {
                    List<GraphDataPoint> primarySeriesData = primarySeries.Data;

                    // Draw a grid overlaid and following series 1 along the x-axis.
                    float xRange = primarySeries.XAxis.Max - primarySeries.XAxis.Min;
                    float xGridLineSteps = clipArea.Width / (xRange / primarySeries.XAxis.GridInterval);
                    float xGridLinePosition = clipArea.Width;

                    if (primarySeriesData.Count > 0)
                    {
                        float gridIntervalRemainder = primarySeriesData.Last().X % primarySeries.XAxis.GridInterval;
                        xGridLinePosition = clipArea.Width - gridIntervalRemainder / xRange * clipArea.Width;
                    }

                    for (int steps = 0; steps < 1000 && xGridLinePosition >= 0.0f; steps++)
                    {
                        graphics.DrawLine(
                            gridPen,
                            clipArea.X + xGridLinePosition,
                            clipArea.Y,
                            clipArea.X + xGridLinePosition,
                            clipArea.Y + clipArea.Height
                        );

                        xGridLinePosition -= xGridLineSteps;
                    }

                    // Draw a grid overlaid and following series 1 along the y-axis.
                    float yRange = primarySeries.YAxis.Max - primarySeries.YAxis.Min;
                    float yGridLineSteps = clipArea.Height / (yRange / primarySeries.YAxis.GridInterval);
                    float yGridLinePosition = 0;
                    for (int steps = 0; steps < 1000 && yGridLinePosition < clipArea.Height; steps++)
                    {
                        graphics.DrawLine(
                            gridPen,
                            clipArea.X,
                            clipArea.Y + yGridLinePosition,
                            clipArea.X + clipArea.Width,
                            clipArea.Y + yGridLinePosition
                        );

                        yGridLinePosition += yGridLineSteps;
                    }
                }
            }

            // Draw the outline of the graph in the primary series color.
            graphics.DrawRectangle(primaryOutlinePen, clipArea);
        }

        /// <summary>
        ///     Event handler for the Paint event of the main graph panel. Renders
        ///     the actual graph to the control.
        /// </summary>
        /// <param name="sender">Object that invoked this event.</param>
        /// <param name="e">Event specific arguments.</param>
        private void OnPanelPaint(object sender, PaintEventArgs e)
        {
            PaintControl(e.Graphics, e.ClipRectangle);
        }

        /// <summary>
        ///     Event handler for the refresh timer tick. Causes the control to be invalidated
        ///     and repainted at the next opportunity. Used to control refresh rate of the control.
        /// </summary>
        /// <param name="sender">Object that invoked this event.</param>
        /// <param name="e">Event specific arguments.</param>
        private void OnTimerTicked(object sender, EventArgs e)
        {
            mainPanel.Invalidate();
            mainPanel.Refresh();
        }

        /// <summary>
        ///     Paints a data series onto the control.
        /// </summary>
        /// <param name="series">Data series to paint.</param>
        /// <param name="graphics">Graphics objects to paint using.</param>
        /// <param name="area">Area within the graphics viewport to render the data series.</param>
        private void PaintSeries(GraphSeries series, Graphics graphics, Rectangle area)
        {
            // Figure out general ranges and shape of graph series.
            List<GraphDataPoint> data = series.Data;
            if (data.Count == 0)
            {
                return;
            }

            float xMinValue = data[0].X;
            float xMaxValue = data[data.Count - 1].X;

            float xRange = xMaxValue - xMinValue;
            float xFullRange = series.XAxis.Max - series.XAxis.Min;
            float yFullRange = series.YAxis.Max - series.YAxis.Min;

            float polygonWidth = area.Width * ((xMaxValue - xMinValue) / xFullRange);
            float polygonX = area.X + area.Width - polygonWidth;

            // Build polygon that represents graph outline.
            PointF[] polygon = new PointF[data.Count + 2];

            int vertIndex = 0;
            polygon[vertIndex++] = new PointF(polygonX, area.Y + area.Height);

            foreach (GraphDataPoint point in data)
            {
                float xOffset = (point.X - xMinValue - series.XAxis.Min) / xFullRange;
                float yScalar = Math.Min(1.0f, Math.Max(0.0f, (point.Y - series.YAxis.Min) / yFullRange));
                float vertX = polygonX + area.Width * xOffset;
                float vertY = area.Y + area.Height - area.Height * yScalar;

                polygon[vertIndex++] = new PointF(vertX, vertY);
            }

            polygon[vertIndex++] = new PointF(polygonX + polygonWidth, area.Y + area.Height);

            // Create pen to draw outline in.
            Pen outlinePen = new Pen(series.Outline, 1.0f);
            if (series.OutlineDotted)
            {
                outlinePen.DashStyle = DashStyle.Dot;
            }

            // Draw our fancy new polygon.
            graphics.FillPolygon(new SolidBrush(series.Fill), polygon);
            graphics.DrawPolygon(outlinePen, polygon);
        }
    }
}