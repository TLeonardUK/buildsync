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
    public class InstallRateEstimater
    {
        private float InstallProgress = 0.0f;
        private RollingAverage InstallRate = new RollingAverage(20);
        private double InstallRateLastSample = 0.0;
        private ulong InstallRateLastSampleTime = 0;

        public float EstimatedSeconds { get; internal set; } = 0.0f;
        public float EstimatedProgress { get; internal set; } = 0.0f;

        public void SetProgress(float InProgress)
        {
            InstallProgress = InProgress;
        }

        public void Poll()
        {
            double Progress = InstallProgress;
            double ProgressDelta = Progress - InstallRateLastSample;
            if (Math.Abs(ProgressDelta) > 0.001f)
            {
                if (ProgressDelta < 0.0f)
                {
                    InstallRate.Reset();
                }
                else
                {
                    double Elapsed = (TimeUtils.Ticks - InstallRateLastSampleTime) / 1000.0f;
                    double PercentPerSecond = ProgressDelta / Elapsed;

                    InstallRate.Add(PercentPerSecond);
                }

                InstallRateLastSample = Progress;
                InstallRateLastSampleTime = TimeUtils.Ticks;
            }

            double AvgPercentPerSecond = InstallRate.Get();
            if (AvgPercentPerSecond > 0.0f)
            {
                double SecondsSinceLastSample = (TimeUtils.Ticks - InstallRateLastSampleTime) / 1000.0f;
                double EstimatedInstalledPercent = AvgPercentPerSecond * SecondsSinceLastSample;
                EstimatedProgress = (float)(InstallProgress + EstimatedInstalledPercent);

                double PercentRemaining = (1.0f - Math.Min(1.0f, EstimatedProgress));
                EstimatedSeconds = (float)(PercentRemaining / AvgPercentPerSecond);
            }
        }
    }
}
