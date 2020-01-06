﻿using System;
using System.Collections.Generic;
using System.Text;

namespace BuildSync.Core.Utils
{

    /// <summary>
    /// 
    /// </summary>
    public class BandwidthTracker
    {
        private object LockObject = new object();

        private long TotalBytesSent = 0;
        private long TotalBytesRecieved = 0;

        private ulong BandwidthTimeStart = 0;
        private long BandwidthTimeStartBytesSent = 0;
        private long BandwidthTimeStartBytesRecieved = 0;

        private double BandwidthSent = 0;
        private double BandwidthRecieved = 0;

        private RollingAverage AverageSent = new RollingAverage(30);
        private RollingAverage AverageRecieved = new RollingAverage(30);

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
        public void BytesOut(long Bytes)
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
        public void BytesIn(long Bytes)
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
            if (Elapsed > 1000)
            {
                if (BandwidthTimeStart == 0)
                {
                    Elapsed = 0;
                }

                long Sent = TotalBytesSent - BandwidthTimeStartBytesSent;
                long Recieved = TotalBytesRecieved - BandwidthTimeStartBytesRecieved;

                double Delta = Elapsed / 1000.0;

                AverageSent.Add(Sent * Delta);
                AverageRecieved.Add(Recieved* Delta);

                BandwidthSent = AverageSent.Get();// (BandwidthSent * 0.5) + ((Sent * Delta) * 0.5);
                BandwidthRecieved = AverageRecieved.Get();// (BandwidthRecieved * 0.5) + ((Recieved * Delta) * 0.5);

                BandwidthTimeStartBytesSent = TotalBytesSent;
                BandwidthTimeStartBytesRecieved = TotalBytesRecieved;
                BandwidthTimeStart = TimeUtils.Ticks;
            }
        }
    }
}
