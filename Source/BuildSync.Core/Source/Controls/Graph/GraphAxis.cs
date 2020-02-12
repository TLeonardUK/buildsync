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
using System.ComponentModel;

namespace BuildSync.Core.Controls.Graph
{
    /// <summary>
    ///     Describes an individual axis in a  <see cref="GraphSeries" />.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [Serializable]
    public class GraphAxis
    {
        /// <summary>
        ///     Gets or sets a value indicating if maximum values of each axis should adjust based on input recieved.
        /// </summary>
        public bool AutoAdjustMax { get; set; } = false;

        /// <summary>
        ///     Gets or sets if the max value label on the y axis should be formatted as a size if using auto-adjust.
        /// </summary>
        public bool FormatMaxLabelAsSize { get; set; } = false;

        /// <summary>
        ///     Gets or sets if the max value label on the y axis should be formatted as a transfer rate if using auto-adjust.
        /// </summary>
        public bool FormatMaxLabelAsTransferRate { get; set; } = false;

        /// <summary>
        ///     Gets or sets the interval that grid lines should be drawn on this axis.
        /// </summary>
        /// <remarks>
        ///     Typical value is something in the range of Max / 10.
        /// </remarks>
        public float GridInterval { get; set; } = 10;

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
        ///     Gets or sets minimum value this axis can hold. This will determine the bounds
        ///     of the graph axis when rendered.
        /// </summary>
        public float Min { get; set; } = 0;

        /// <summary>
        ///     Gets or sets the text label shown next to the minimum end of this axis.
        /// </summary>
        public string MinLabel { get; set; } = "0";
    }
}