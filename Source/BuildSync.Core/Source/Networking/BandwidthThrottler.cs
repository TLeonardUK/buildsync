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
using System.Text;
using System.Threading;
using BuildSync.Core.Utils;

namespace BuildSync.Core.Networking
{
    /// <summary>
    /// 
    /// </summary>
    public class BandwidthThrottler
    {
        /// <summary>
        /// 
        /// </summary>
        public long MaxRate
        {
            get;
            set;
        } = 0;

        /// <summary>
        /// 
        /// </summary>
        public long GlobalMaxRate
        {
            get;
            set;
        } = 0;

        /// <summary>
        /// 
        /// </summary>
        public long MinimumTransmissionUnit
        {
            get;
            set;
        } = 1024;

        /// <summary>
        /// 
        /// </summary>
        private double Tokens = 0;

        /// <summary>
        /// 
        /// </summary>
        private object TokenLock = new object();

        /// <summary>
        /// 
        /// </summary>
        private ulong LastRefillTime = TimeUtils.Ticks;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Pending"></param>
        /// <returns></returns>
        public int Throttle(int Pending)
        {
            long Limit = GlobalMaxRate;

            // No global rate, use local instead.
            if (Limit == 0)
            {
                Limit = MaxRate;
            }
            else
            {
                // If local rate is lower than global, use that instead.
                if (MaxRate > 0 && MaxRate < GlobalMaxRate)
                {
                    Limit = MaxRate;
                }
            }

            // No limit, allow all.
            if (Limit == 0)
            {
                return Pending;
            }

            // Is there enough tokens to send the entire thing or at leat the MTU?
            long Mtu = Math.Min(MinimumTransmissionUnit, Limit);
            double MinimumToSend = Math.Min((double)Mtu, (double)Pending);
            while (true)
            {
                // Refill the tokens.
                lock (TokenLock)
                {
                    ulong Time = TimeUtils.Ticks;
                    double ElapsedSeconds = (Time - LastRefillTime) / 1000.0;
                    double RefillTokens = ElapsedSeconds * (double)Limit;

                    while (true)
                    {
                        double OriginalTokens = Tokens;
                        double NewValue = Math.Min(Limit, OriginalTokens + RefillTokens);
                        if (OriginalTokens == NewValue)
                        {
                            break;
                        }

                        if (Interlocked.CompareExchange(ref Tokens, NewValue, OriginalTokens) == OriginalTokens)
                        {
                            break;
                        }
                    }

                    LastRefillTime = Time;
                }

                // See if we can take enough tokens.
                double OriginalValue = Tokens;
                double AmountToTake = Math.Min((double)OriginalValue, (double)Pending);
                if (AmountToTake >= MinimumToSend)
                {
                    if (Interlocked.CompareExchange(ref Tokens, OriginalValue - AmountToTake, OriginalValue) == OriginalValue)
                    {
                        return (int)AmountToTake;
                    }
                }
                // Otherwise sleep.
                else
                {
                    double RefillRequired = MinimumToSend - AmountToTake;
                    double TimeToFillMs = ((RefillRequired / Limit) * 1000.0);
                    Thread.Sleep((int)TimeToFillMs);
                }
            }
        }
    }
}
