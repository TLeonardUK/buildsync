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
using System.Text;
using System.Threading.Tasks;

namespace BuildSync.Core.Utils
{
    public class RollingAverage
    {
        private int MaxSamples = 0;
        private List<double> Values = new List<double>();

        public RollingAverage(int SampleCount)
        {
            MaxSamples = SampleCount;
        }

        public double GetStandardDeviation(bool as_sample = false)
        {
            // Get the mean.
            double mean = Values.Sum() / Values.Count();

            // Get the sum of the squares of the differences
            // between the values and the mean.
            var squares_query =
                from double value in Values
                select (value - mean) * (value - mean);
            double sum_of_squares = squares_query.Sum();

            if (as_sample)
            {
                return Math.Sqrt(sum_of_squares / (Values.Count() - 1));
            }
            else
            {
                return Math.Sqrt(sum_of_squares / Values.Count());
            }
        }

        public double GetMean()
        {
            double Total = 0.0f;
            for (int i = 0; i < Values.Count; i++)
            {
                Total += Values[i];
            }
            return Total / Values.Count;
        }

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

        public double Get()
        {
            double Result = 0;
            lock (Values)
            {
                double StdDeviation = GetStandardDeviation();
                double Mean = GetMean();
                double Total = 0;
                int Samples = 0;
                for (int i = 0; i < Values.Count; i++)
                {
                    double Drift = Math.Abs(Values[i] - Mean);
                    if (Drift <= StdDeviation)
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
    }
}
