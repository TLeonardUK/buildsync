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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using BuildSync.Core.Downloads;
using BuildSync.Core.Licensing;
using BuildSync.Core.Manifests;
using BuildSync.Core.Networking;
using BuildSync.Core.Networking.Messages;
using BuildSync.Core.Users;
using BuildSync.Core.Utils;

namespace BuildSync.Core.Client
{
    /// <summary>
    /// </summary>
    public struct PendingBlockRequest
    {
        public NetMessage_GetBlock Message;
        public NetConnection Requester;
    }

    /// <summary>
    /// </summary>
    public class Peer
    {
        /// <summary>
        /// </summary>
        public const int BlockDownloadTimeout = 30 * 1000;

        /// <summary>
        /// </summary>
        public List<ManifestPendingDownloadBlock> ActiveBlockDownloads = new List<ManifestPendingDownloadBlock>();

        /// <summary>
        /// </summary>
        public long ActiveBlockDownloadSize;

        /// <summary>
        /// </summary>
        public IPEndPoint Address;

        /// <summary>
        /// </summary>
        public RollingAverage AverageBlockSize = new RollingAverage(20);

        /// <summary>
        /// </summary>
        public RollingAverage BlockRecieveLatency = new RollingAverage(20);

        /// <summary>
        /// </summary>
        public ConcurrentQueue<PendingBlockRequest> BlockRequestQueue = new ConcurrentQueue<PendingBlockRequest>();

        /// <summary>
        /// </summary>
        public BlockListState BlockState = new BlockListState();

        /// <summary>
        /// </summary>
        public NetConnection Connection = new NetConnection();

        /// <summary>
        /// </summary>
        public ulong LastBlockRequestFulfillTime = TimeUtils.Ticks;

        /// <summary>
        /// </summary>
        public ulong LastConnectionAttemptTime;

        /// <summary>
        /// </summary>
        public ulong LastPrintTime = TimeUtils.Ticks;

        /// <summary>
        /// </summary>
        public bool RemoteInitiated;

        /// <summary>
        /// </summary>
        public bool WasConnected;

        /// <summary>
        /// </summary>
        /// <param name="Block"></param>
        public void AddActiveBlockDownload(ManifestPendingDownloadBlock Block)
        {
            lock (ActiveBlockDownloads)
            {
                ActiveBlockDownloadSize += Block.Size;
                ActiveBlockDownloads.Add(Block);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="TargetMsOfData"></param>
        /// <returns></returns>
        public long GetAvailableInFlightData(int TargetMsOfData)
        {
            return Math.Max(0, GetMaxInFlightData(TargetMsOfData) - ActiveBlockDownloadSize);
        }

        /// <summary>
        /// </summary>
        /// <param name="TargetMsOfData"></param>
        /// <returns></returns>
        public long GetMaxInFlightData(int TargetMsOfData)
        {
            // Calculate a rough idea for how many bytes we should have in flight at a given time.
#if false
            double InBlockRecieveLatency = BlockRecieveLatency.Get();
            double InAverageBlockSize = AverageBlockSize.Get();

            long MaxInFlightBytes = 0;
            if (InBlockRecieveLatency < 5 || InAverageBlockSize < 1024)
            {
                MaxInFlightBytes = BuildManifest.BlockSize * 1;
            }
            else
            {
                MaxInFlightBytes = Math.Max(BuildManifest.BlockSize, (long)((TargetMsOfData / InBlockRecieveLatency) * InAverageBlockSize));
            }

            return MaxInFlightBytes;// * 5;
#elif false
            // Always try to trent towards a higher capacity until we stabalize at our available bandwidth.
            const double TrendUpwardsFactor = 2;

            double LinkCapacityBytesPerSecond = Connection.BandwidthStats.RateIn / TrendUpwardsFactor; 
            double LatencySeconds = Connection.BestPing / 1000.0f;
            if (LatencySeconds == 0)
            {
                LatencySeconds = 0.001f;
            }

            double BandwidthDelayProductBytes = LinkCapacityBytesPerSecond * LatencySeconds;
            long MaxInFlightBytes = (long)BandwidthDelayProductBytes;

            // Cap to minimum of a couple ofs, so we always have one being sent out and one before processed.
            MaxInFlightBytes = Math.Max(BuildManifest.BlockSize * 2, MaxInFlightBytes);

            // Always try to trent towards a higher capacity until we stabalize at our available bandwidth.
            MaxInFlightBytes = (long)(MaxInFlightBytes * TrendUpwardsFactor);

            if (TimeUtils.Ticks - LastPrintTime > 1000)
            {
                Console.WriteLine("a={0} b={1} bytes={2}", StringUtils.FormatAsTransferRate((long)LinkCapacityBytesPerSecond), LatencySeconds, StringUtils.FormatAsSize(MaxInFlightBytes));
                LastPrintTime = TimeUtils.Ticks;
            }

            return MaxInFlightBytes;
#elif true
            // Keep at least 1 second of data in flight.
            double RealTargetMsOfData = Math.Max(TargetMsOfData, Connection.BestPing * 2.0f);

            const double TrendUpwardsFactor = 2.0f;
            double TargetSecondsOfData = RealTargetMsOfData / 1000.0;
            long TargetInFlight = (long) (Connection.BandwidthStats.PeakRateIn * TargetSecondsOfData);

            long MaxInFlightBytes = TargetInFlight;

            // Cap to minimum of a couple of 2 blocks, so we always have one being sent out and one before processed.
            MaxInFlightBytes = Math.Max(BuildManifest.BlockSize * 2, MaxInFlightBytes);

            // Always try to trent towards a higher capacity until we stabalize at our available bandwidth.
            MaxInFlightBytes = (long) (MaxInFlightBytes * TrendUpwardsFactor);

            // Round to next largest block size.
            MaxInFlightBytes = (MaxInFlightBytes + (BuildManifest.BlockSize - 1)) / BuildManifest.BlockSize * BuildManifest.BlockSize;

            return MaxInFlightBytes;
#elif false
            // Fuck it, this is simpler and causes less edge cases. Worst case we are waiting for a slow peer to return
            // like 4 * worst case of the above one.
            return 40 * 1024 * 1024;
#else
            return 128 * 1024 * 1024; // Gigabit of data. We limit the actual amount based on our local link speed.
#endif
        }

        /// <summary>
        /// </summary>
        /// <param name="ManifestId"></param>
        /// <param name="BlockIndex"></param>
        /// <returns></returns>
        public bool HasActiveBlockDownload(Guid ManifestId, int BlockIndex)
        {
            lock (ActiveBlockDownloads)
            {
                for (int i = 0; i < ActiveBlockDownloads.Count; i++)
                {
                    ManifestPendingDownloadBlock Download = ActiveBlockDownloads[i];

                    if (Download.BlockIndex == BlockIndex &&
                        Download.ManifestId == ManifestId)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public bool HasBlock(ManifestPendingDownloadBlock Block)
        {
            foreach (ManifestBlockListState ManifestState in BlockState.States)
            {
                if (ManifestState.Id == Block.ManifestId)
                {
                    return ManifestState.BlockState.Get(Block.BlockIndex);
                }
            }

            return false;
        }

        /// <summary>
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public bool IsDownloadingBlock(ManifestPendingDownloadBlock Block)
        {
            lock (ActiveBlockDownloads)
            {
                foreach (ManifestPendingDownloadBlock Download in ActiveBlockDownloads)
                {
                    if (Download.BlockIndex == Block.BlockIndex && Download.ManifestId == Block.ManifestId)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// </summary>
        /// <param name="ManifestId"></param>
        /// <param name="BlockIndex"></param>
        public void MarkActiveBlockDownloadAsRecieved(Guid ManifestId, int BlockIndex)
        {
            lock (ActiveBlockDownloads)
            {
                for (int i = 0; i < ActiveBlockDownloads.Count; i++)
                {
                    ManifestPendingDownloadBlock Download = ActiveBlockDownloads[i];

                    if (Download.BlockIndex == BlockIndex &&
                        Download.ManifestId == ManifestId)
                    {
                        if (!Download.Recieved)
                        {
                            UpdateLatency(Download);
                            ActiveBlockDownloadSize -= Download.Size;
                            Download.Recieved = true;
                            ActiveBlockDownloads[i] = Download;
                        }

                        break;
                    }
                }
            }
        }

        /// <summary>
        /// </summary>
        public void PruneTimeoutDownloads()
        {
            lock (ActiveBlockDownloads)
            {
                for (int i = 0; i < ActiveBlockDownloads.Count; i++)
                {
                    ManifestPendingDownloadBlock Download = ActiveBlockDownloads[i];

                    ulong Elapsed = TimeUtils.Ticks - Download.TimeStarted;
                    if (Elapsed > BlockDownloadTimeout)
                    {
                        if (!Download.Recieved)
                        {
                            ActiveBlockDownloadSize -= Download.Size;

                            Download.Recieved = true;
                            ActiveBlockDownloads[i] = Download;
                        }

                        ActiveBlockDownloads.RemoveAt(i);
                        i--;
                    }
                }
            }
        }

        /// <summary>
        /// </summary>
        public void RemoveActiveBlockDownload(Guid ManifestId, int BlockIndex, bool WasSuccess)
        {
            lock (ActiveBlockDownloads)
            {
                for (int i = 0; i < ActiveBlockDownloads.Count; i++)
                {
                    ManifestPendingDownloadBlock Download = ActiveBlockDownloads[i];

                    if (Download.BlockIndex == BlockIndex &&
                        Download.ManifestId == ManifestId)
                    {
                        if (!Download.Recieved)
                        {
                            if (WasSuccess)
                            {
                                UpdateLatency(Download);
                            }

                            ActiveBlockDownloadSize -= Download.Size;
                            Download.Recieved = true;
                            ActiveBlockDownloads[i] = Download;
                        }

                        //Console.WriteLine("Removing (for block {0} in manifest {1}) of size {2} total queued {3}.", Download.ManifestId, Download.BlockIndex, Download.Size, ActiveBlockDownloadSize);

                        ActiveBlockDownloads.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public bool SetBlockState(Guid ManifestId, int BlockIndex, bool State)
        {
            foreach (ManifestBlockListState ManifestState in BlockState.States)
            {
                if (ManifestState.Id == ManifestId)
                {
                    ManifestState.BlockState.Set(BlockIndex, State);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// </summary>
        /// <param name="Block"></param>
        private void UpdateLatency(ManifestPendingDownloadBlock Download)
        {
            ulong Elapsed = TimeUtils.Ticks - Download.TimeStarted;
            //Console.WriteLine("Recieved block {0} in {1} ms", Download.BlockIndex, Elapsed);
            BlockRecieveLatency.Add(Elapsed);
            AverageBlockSize.Add(Download.Size);
        }
    }
}