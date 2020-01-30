using System;
using System.Collections.Generic;
using System.Text;

namespace BuildSync.Core.Utils
{

    /// <summary>
    /// 
    /// </summary>
    public class RateTracker
    {
        private object LockObject = new object();

        private long TotalBytesSent = 0;
        private long TotalBytesRecieved = 0;

        private ulong BandwidthTimeStart = 0;
        private long BandwidthTimeStartBytesSent = 0;
        private long BandwidthTimeStartBytesRecieved = 0;

        private double BandwidthSent = 0;
        private double BandwidthRecieved = 0;

        private double PeakBandwidthSent = 0;
        private double PeakBandwidthRecieved = 0;

        private RollingAverage AverageSent = new RollingAverage(10);
        private RollingAverage AverageRecieved = new RollingAverage(10);

        /// <summary>
        /// 
        /// </summary>
        public long RateIn
        {
            get
            {
                lock (LockObject)
                {
                    Update();
                    return (long)BandwidthRecieved;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public long RateOut
        {
            get
            {
                lock (LockObject)
                {
                    Update();
                    return (long)BandwidthSent;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public long PeakRateIn
        {
            get
            {
                lock (LockObject)
                {
                    Update();
                    return (long)PeakBandwidthRecieved;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public long PeakRateOut
        {
            get
            {
                lock (LockObject)
                {
                    Update();
                    return (long)PeakBandwidthSent;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public long TotalIn
        {
            get
            {
                lock (LockObject)
                {
                    Update();
                    return (long)TotalBytesRecieved;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public long TotalOut
        {
            get
            {
                lock (LockObject)
                {
                    Update();
                    return (long)TotalBytesSent;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Sent"></param>
        /// <returns></returns>
        public void Out(long Bytes)
        {
            lock (LockObject)
            {
                TotalBytesSent += Bytes;
                Update();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Sent"></param>
        /// <returns></returns>
        public void In(long Bytes)
        {
            lock (LockObject)
            {
                TotalBytesRecieved += Bytes;
                Update();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void Update()
        { 
            // Calculate bandwidth.
            ulong Elapsed = TimeUtils.Ticks - BandwidthTimeStart;
            if (Elapsed > 250)
            {
                if (BandwidthTimeStart == 0)
                {
                    Elapsed = 0;
                }

                long Sent = TotalBytesSent - BandwidthTimeStartBytesSent;
                long Recieved = TotalBytesRecieved - BandwidthTimeStartBytesRecieved;

                double Delta = 1000.0  / (Elapsed == 0 ? 1 : Elapsed);

                double SentPs = Sent * Delta;
                double RecievedPs = Recieved * Delta;

                AverageSent.Add(SentPs);
                AverageRecieved.Add(RecievedPs);

                BandwidthSent = AverageSent.Get();// (BandwidthSent * 0.5) + ((Sent * Delta) * 0.5);
                BandwidthRecieved = AverageRecieved.Get();// (BandwidthRecieved * 0.5) + ((Recieved * Delta) * 0.5);

                if (SentPs > PeakBandwidthSent)
                {
                    PeakBandwidthSent = SentPs;
                }
                if (RecievedPs > PeakBandwidthRecieved)
                {
                    PeakBandwidthRecieved = RecievedPs;
                }

                BandwidthTimeStartBytesSent = TotalBytesSent;
                BandwidthTimeStartBytesRecieved = TotalBytesRecieved;
                BandwidthTimeStart = TimeUtils.Ticks;
            }
        }
    }
}
