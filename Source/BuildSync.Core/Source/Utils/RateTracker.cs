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
                    return TotalBytesRecieved;
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
                    return TotalBytesSent;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public RateTracker()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="MaxSamples"></param>
        public RateTracker(int MaxSamples)
        {
            AverageSent = new RollingAverage(MaxSamples);
            AverageRecieved = new RollingAverage(MaxSamples);
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
        public void Reset()
        {
            lock (LockObject)
            {
                TotalBytesSent = 0;
                TotalBytesRecieved = 0;

                BandwidthTimeStart = 0;
                BandwidthTimeStartBytesSent = 0;
                BandwidthTimeStartBytesRecieved = 0;

                BandwidthSent = 0;
                BandwidthRecieved = 0;

                PeakBandwidthSent = 0;
                PeakBandwidthRecieved = 0;

                RollingAverage AverageSent = new RollingAverage(10);
                RollingAverage AverageRecieved = new RollingAverage(10);
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

                double Delta = 1000.0 / (Elapsed == 0 ? 1 : Elapsed);

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
