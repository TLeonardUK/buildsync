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
using System.Linq;

namespace BuildSync.Core.Utils
{
    /// <summary>
    /// 
    /// </summary>
    public class RollingAverage
    {
        private readonly int MaxSamples;
        private readonly int MaxStdDevSamples;
        private readonly List<double> Values = new List<double>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SampleCount"></param>
        public RollingAverage(int SampleCount, int StdDevSamples = 0)
        {
            MaxSamples = SampleCount;

            // We only generate stddev from most recent values, so if values change dramatically in a short period we filter old values out.
            if (StdDevSamples == 0)
            {
                MaxStdDevSamples = SampleCount;
            }
            else
            {
                MaxStdDevSamples = StdDevSamples;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Reset()
        {
            Values.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Value"></param>
        public void Add(double Value)
        {
            lock (Values)
            {
                Values.Add(Value);
                if (Values.Count > MaxSamples)
                {
                    Values.RemoveAt(0);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public double Get(bool PruneDeviations = false)
        {
            double Result = 0;
            lock (Values)
            {
                double StdDeviation = GetStandardDeviation() / 2;
                double Mean = GetStdDevMean();
                double Total = 0;
                int Samples = 0;
                for (int i = 0; i < Values.Count; i++)
                {
                    double Drift = Math.Abs(Values[i] - Mean);
                    if (!PruneDeviations || Drift <= StdDeviation)
                    {
                        Total += Values[i];
                        Samples++;
                    }
                }

                if (Samples > 0)
                {
                    Result = Total / Samples;
                }
            }

            return Result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public double GetStdDevMean()
        {
            int Count = Math.Min(Values.Count, MaxStdDevSamples);
            if (Count == 0)
            {
                return 0.0;
            }

            double Total = 0.0f;
            for (int i = Values.Count - Count; i < Values.Count; i++)
            {
                Total += Values[i];
            }

            return Total / Count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="as_sample"></param>
        /// <returns></returns>
        public double GetStandardDeviation(bool as_sample = false)
        {
            int Count = Math.Min(Values.Count, MaxStdDevSamples);
            if (Count == 0)
            {
                return 0.0;
            }

            double Sum = 0.0;
            for (int i = Values.Count - Count; i < Values.Count; i++)
            {
                Sum += Values[i];
            }

            // Get the mean.
            double mean = Sum / Count;

            // Get the sum of the squares of the differences
            // between the values and the mean.
            double sum_of_squares = 0.0f;
            for (int i = Values.Count - Count; i < Values.Count; i++)
            {
                sum_of_squares += (Values[i] - mean) * (Values[i] - mean);
            }

            if (as_sample)
            {
                return Math.Sqrt(sum_of_squares / (Count - 1));
            }

            return Math.Sqrt(sum_of_squares / Count);
        }
    }
}