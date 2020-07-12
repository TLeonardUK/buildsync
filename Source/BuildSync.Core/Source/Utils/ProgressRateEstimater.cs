using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildSync.Core.Utils
{
    /// <summary>
    /// 
    /// </summary>
    public class ProgressRateEstimater
    {
        private float Progress = 0.0f;
        private RollingAverageOverTime Rate = new RollingAverageOverTime(30 * 1000);
        private double RateLastSample = 0.0;
        private ulong RateLastSampleTime = 0;

        private ulong TimeOfLastEstimateUpdate = 0;

        private double Rate2ndOrder = 0.0;

        public double UnaveragedEstimatedSeconds = 0.0;

        public double EstimatedSeconds { get; internal set; } = 0.0;
        public double EstimatedProgress { get; internal set; } = 0.0;

        public void SetProgress(float InProgress)
        {
            Progress = InProgress;
        }

        public void Poll()
        {
            ulong Time = TimeUtils.Ticks;

            double Progress = this.Progress;
            double ProgressDelta = Progress - RateLastSample;
            double Elapsed = (Time - RateLastSampleTime) / 1000.0;
            if (Math.Abs(ProgressDelta) >= 0.0001 && Elapsed >= 1.0) // Spread samples out.
            {
                if (ProgressDelta < 0.0f)
                {
                    Rate.Reset();
                }
                else
                {
                    double PercentPerSecond = ProgressDelta / Elapsed;

                    Rate.Add(PercentPerSecond);

                    //Console.WriteLine("Sample:{0} Average:{1} Elapsed:{2} Delta:{3}", PercentPerSecond.ToString("0." + new string('#', 339)), Rate.Get(false).ToString("0." + new string('#', 339)), Elapsed, ProgressDelta);
                }

                RateLastSample = Progress;
                RateLastSampleTime = Time;
            }

            //Rate2ndOrder = (Rate2ndOrder * 0.9) + (Rate.Get(false) * 0.1);

            double AvgPercentPerSecond = Rate.Get(false);// Rate2ndOrder;//.Get();// Rate.Get(true);
            if (AvgPercentPerSecond > 0.0)
            {
                double SecondsSinceLastSample = (Time - RateLastSampleTime) / 1000.0f;
                double EstimatedInstalledPercent = AvgPercentPerSecond * SecondsSinceLastSample;
                EstimatedProgress = (float)(RateLastSample + EstimatedInstalledPercent);

                double PercentRemaining = (1.0f - Math.Min(1.0f, EstimatedProgress));
                double NextEstimatedSeconds = (float)(PercentRemaining / AvgPercentPerSecond);

                // If time is jumping, only apply it its been > last estimate for a long period.
                // Stops time jittering up and down a lot.
                double NextSecondsDelta = /*Math.Abs*/(NextEstimatedSeconds - UnaveragedEstimatedSeconds);
                double ElapsedSecondsSinceLastUpdate = (Time - TimeOfLastEstimateUpdate) / 1000.0;
                double MaxNextSecondsDelta = (ElapsedSecondsSinceLastUpdate * 2.0) + 25.0f;
                if (false)//Math.Ceiling(NextEstimatedSeconds) > Math.Ceiling(UnaveragedEstimatedSeconds))// ||  // Estimate suddenly went above old value.
                    //NextSecondsDelta < -MaxNextSecondsDelta)    // Suddenly dropped by a large amount.
                {
                    //Console.WriteLine("Delta={0} Max={1} Next={2} Estimate={3} Elapsed={4}", NextSecondsDelta, MaxNextSecondsDelta, NextEstimatedSeconds, UnaveragedEstimatedSeconds, ElapsedSecondsSinceLastUpdate);
                    if (ElapsedSecondsSinceLastUpdate > 5.0)
                    {
                        UnaveragedEstimatedSeconds = NextEstimatedSeconds;
                        TimeOfLastEstimateUpdate = Time;
                    }
                }
                else
                {
                    //Console.WriteLine("New={0}", NextEstimatedSeconds);
                    UnaveragedEstimatedSeconds = NextEstimatedSeconds;
                    TimeOfLastEstimateUpdate = Time;
                }

                //Console.WriteLine("Average={0:0.0}%/s EProgress={1:0.0}% Estimate={2:0.0}s Smoothed={3:0.0}s SinceLastSample={4:0.0} InstallSinceLastSample={5:0.0} RateLastSample={6:0.0}", (AvgPercentPerSecond*100.0), (EstimatedProgress * 100.0), NextEstimatedSeconds, EstimatedSeconds, SecondsSinceLastSample, (EstimatedInstalledPercent*100), (RateLastSample*100));
            }

            /*if (UnaveragedEstimatedSeconds > EstimatedSeconds)
            {
                EstimatedSeconds = (EstimatedSeconds * 0.9) + (UnaveragedEstimatedSeconds * 0.1);
            }
            else
            {*/
            EstimatedSeconds = (EstimatedSeconds * 0.95) + (UnaveragedEstimatedSeconds * 0.05);
            //}

            //EstimatedSeconds = UnaveragedEstimatedSeconds;// (EstimatedSeconds * 0.8) + (UnaveragedEstimatedSeconds * 0.2);
        }
    }
}
