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
        public ulong QueueTime;
    }

    /// <summary>
    /// </summary>
    public class Peer
    {
        /// <summary>
        /// </summary>
        public const int BlockDownloadTimeout = 60 * 1000;

        /// <summary>
        /// </summary>
        public const int MaxRequestQueueSize = 500;

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
        /// 
        /// </summary>
        public int MaxInFlightRequests = 1;

        /// <summary>
        /// 
        /// </summary>
        public int OutstandingReads = 0;

        /// <summary>
        /// 
        /// </summary>
        public int OutstandingSends = 0;

        /// <summary>
        /// 
        /// </summary>
        public float QueueDepthMs = 0;

        /// <summary>
        /// 
        /// </summary>
        public long LastBlockQueueSequence = 0;

        /// <summary>
        /// </summary>
        public BlockListState BlockState = new BlockListState();

        /// <summary>
        /// </summary>
        public NetConnection Connection = new NetConnection();

        /// <summary>
        /// 
        /// </summary>
        public int RequestQueueDepth = 0;

        /// <summary>
        /// 
        /// </summary>
        public RollingAverage AverageRequestFulfillTime = new RollingAverage(20);

        /// <summary>
        /// 
        /// </summary>
        public RollingAverage AverageRequestConcurrency = new RollingAverage(20);       

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
        /// 
        /// </summary>
        public bool PendingQueueUpdate = false;

        /// <summary>
        /// 
        /// </summary>
        public long QueueSequence = 0;

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
        public long GetAvailableInFlightData()
        {
            return Math.Max(0, GetMaxInFlightData() - ActiveBlockDownloadSize);// Connection.TcpInfo.BytesInFlight);
        }

        /// <summary>
        /// </summary>
        /// <param name="TargetMsOfData"></param>
        /// <returns></returns>
        public long GetMaxInFlightData()
        {
            return BuildManifest.DefaultBlockSize * MaxInFlightRequests;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Depth"></param>
        public void RecievedNewQueueDepth(float DepthMs, long Sequence)
        {
            // When we start requesting data we request a single block at a time.
            // Each time we recieve a block:
            //      Get included queue depth value.
            //          > 1 = We reduce blocks requested by 1.
            //          < 1 = We increase blocks requested by 1.

            QueueDepthMs = DepthMs;
            LastBlockQueueSequence = Sequence;
            PendingQueueUpdate = true;

            /*if (DepthMs > Client.TargetMillisecondsOfDataInFlight)
            {
                MaxInFlightRequests = Math.Max(1, MaxInFlightRequests - 1);
            }
            else if (DepthMs < Client.TargetMillisecondsOfDataInFlight * 0.8f)
            {
                MaxInFlightRequests = Math.Min(MaxRequestQueueSize, MaxInFlightRequests + 1);
            }*/
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public float GetRequestQueueDepthTime()
        {
            return ((float)RequestQueueDepth * (float)AverageRequestFulfillTime.Get()) / (float)Math.Max(AverageRequestConcurrency.Get(), 1); 
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
        /// <returns></returns>
        public bool HasBlock(Guid ManifestId, int BlockIndex)
        {
            foreach (ManifestBlockListState ManifestState in BlockState.States)
            {
                if (ManifestState.Id == ManifestId)
                {
                    return ManifestState.BlockState.Get(BlockIndex);
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

                            Logger.Log(LogLevel.Warning, LogCategory.Manifest, "Pruned active download as it timed out: manifest={0} block={1}", Download.ManifestId.ToString(), Download.BlockIndex);
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