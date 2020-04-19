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
    public class RollingAverageOverTime
    {
        private struct Sample
        {
            public ulong Time;
            public double Value;

            public Sample(ulong t, double v)
            {
                Time = t;
                Value = v;
            }
        }

        private readonly int MaxTimespan;
        private readonly List<Sample> Values = new List<Sample>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SampleCount"></param>
        public RollingAverageOverTime(int InMaxTimespan)
        {
            MaxTimespan = InMaxTimespan;
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
                ulong Time = TimeUtils.Ticks;

                Values.Add(new Sample(Time, Value));

                // Remove old values.
                while (Values.Count > 0)
                {
                    if (Time - Values[0].Time > (ulong)MaxTimespan)
                    {
                        Values.RemoveAt(0);
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public double Get(bool UseStdDeviation = false)
        {
            double Result = 0;
            lock (Values)
            {
                ulong TimeMinBound = TimeUtils.Ticks - (ulong)MaxTimespan;
                double Total = 0;
                int Samples = 0;

                if (UseStdDeviation)
                {
                    double StdDeviation = GetStandardDeviation(TimeMinBound) / 2;
                    double Mean = GetStdDevMean(TimeMinBound);
                    for (int i = 0; i < Values.Count; i++)
                    {
                        if (Values[i].Time >= TimeMinBound)
                        {
                            double Drift = Math.Abs(Values[i].Value - Mean);
                            if (Drift <= StdDeviation)
                            {
                                Total += Values[i].Value;
                                Samples++;
                            }
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < Values.Count; i++)
                    {
                        if (Values[i].Time >= TimeMinBound)
                        {
                            Total += Values[i].Value;
                            Samples++;
                        }
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
        private double GetStdDevMean(ulong TimeMinBound)
        {
            int Count = 0;

            double Total = 0.0f;
            for (int i = 0; i < Values.Count; i++)
            {
                if (Values[i].Time >= TimeMinBound)
                {
                    Total += Values[i].Value;
                    Count++;
                }
            }

            if (Count > 0)
            {
                return Total / Count;
            }
            else
            {
                return 0.0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="as_sample"></param>
        /// <returns></returns>
        private double GetStandardDeviation(ulong TimeMinBound, bool as_sample = false)
        {
            int Count = 0;

            double Sum = 0.0f;
            for (int i = 0; i < Values.Count; i++)
            {
                if (Values[i].Time >= TimeMinBound)
                {
                    Sum += Values[i].Value;
                    Count++;
                }
            }

            if (Count == 0)
            {
                return 0.0;
            }

            // Get the mean.
            double mean = Sum / Count;

            // Get the sum of the squares of the differences
            // between the values and the mean.
            double sum_of_squares = 0.0f;
            for (int i = 0; i < Values.Count; i++)
            {
                if (Values[i].Time >= TimeMinBound)
                {
                    sum_of_squares += (Values[i].Value - mean) * (Values[i].Value - mean);
                }
            }

            if (as_sample)
            {
                return Math.Sqrt(sum_of_squares / (Count - 1));
            }

            return Math.Sqrt(sum_of_squares / Count);
        }
    }
}