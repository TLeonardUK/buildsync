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
using System.Diagnostics;
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
    /// <param name="Connection"></param>
    /// <param name="Message"></param>
    public delegate void BuildsRecievedHandler(string RootPath, NetMessage_GetBuildsResponse.BuildInfo[] Builds);

    /// <summary>
    /// </summary>
    /// <param name="Connection"></param>
    /// <param name="Message"></param>
    public delegate void ServerStateRecievedHandler(NetMessage_GetServerStateResponse Response);

    /// <summary>
    /// </summary>
    /// <param name="Connection"></param>
    /// <param name="Message"></param>
    public delegate void PermissionsUpdatedHandler();

    /// <summary>
    /// </summary>
    public delegate void ConenctedToServerHandler();

    /// <summary>
    /// </summary>
    public delegate void LostConnectionToServerHandler();

    /// <summary>
    /// </summary>
    public delegate void FailedToConnectToServerHandler();

    /// <summary>
    /// </summary>
    public delegate void UserListRecievedHandler(List<User> Users, List<UserGroup> UserGroups);

    /// <summary>
    /// </summary>
    public delegate void LicenseInfoRecievedHandler(License LicenseInfo);

    /// <summary>
    /// </summary>
    public delegate void ManifestPublishResultRecievedHandler(Guid ManifestId, PublishManifestResult Result);

    /// <summary>
    /// </summary>
    public delegate void ManifestDeleteResultRecievedHandler(Guid ManifestId);

    /// <summary>
    /// </summary>
    public class Statistic_CpuUsage : Statistic
    {
        public Statistic_CpuUsage()
        {
            Name = @"Process\CPU Usage";
            MaxLabel = "100";
            MaxValue = 100;
            DefaultShown = false;

            Series.YAxis.AutoAdjustMax = false;
            Series.YAxis.FormatMaxLabelAsInteger = true;
        }
    }

    /// <summary>
    /// </summary>
    public class Statistic_MemoryUsage : Statistic
    {
        public Statistic_MemoryUsage()
        {
            Name = @"Process\Memory Usage";
            MaxLabel = "128 mb";
            MaxValue = 128 * 1024 * 1024;
            DefaultShown = false;

            Series.YAxis.AutoAdjustMax = true;
            Series.YAxis.FormatMaxLabelAsSize = true;
        }
    }

    /// <summary>
    /// </summary>
    public class Statistic_PeerCount : Statistic
    {
        public Statistic_PeerCount()
        {
            Name = @"Peers\Peer Count";
            MaxLabel = "8";
            MaxValue = 8;
            DefaultShown = false;

            Series.YAxis.AutoAdjustMax = true;
            Series.YAxis.FormatMaxLabelAsInteger = true;
        }
    }

    /// <summary>
    /// </summary>
    public class Statistic_DataInFlight : Statistic
    {
        public Statistic_DataInFlight()
        {
            Name = @"Peers\Data In Flight";
            MaxLabel = "32 MB";
            MaxValue = 32;
            DefaultShown = false;

            Series.YAxis.AutoAdjustMax = true;
            Series.YAxis.FormatMaxLabelAsSize = true;
        }
    }

    /// <summary>
    /// </summary>
    public class Statistic_BlocksInFlight : Statistic
    {
        public Statistic_BlocksInFlight()
        {
            Name = @"Peers\Blocks In Flight";
            MaxLabel = "32";
            MaxValue = 32;
            DefaultShown = false;

            Series.YAxis.AutoAdjustMax = true;
            Series.YAxis.FormatMaxLabelAsInteger = true;
        }
    }

    /// <summary>
    /// </summary>
    public class Statistic_AverageBlockLatency : Statistic
    {
        public Statistic_AverageBlockLatency()
        {
            Name = @"Peers\Average Block Latency (ms)";
            MaxLabel = "1000 ms";
            MaxValue = 1000;
            DefaultShown = false;

            Series.YAxis.AutoAdjustMax = true;
            Series.YAxis.FormatMaxLabelAsInteger = true;
        }
    }

    /// <summary>
    /// </summary>
    public class Statistic_AverageBlockSize : Statistic
    {
        public Statistic_AverageBlockSize()
        {
            Name = @"Peers\Average Block Size (MB)";
            MaxLabel = "2 mb";
            MaxValue = 2;
            DefaultShown = false;

            Series.YAxis.AutoAdjustMax = true;
            Series.YAxis.FormatMaxLabelAsSize = true;
        }
    }

    /// <summary>
    /// </summary>
    public class Statistic_RequestFailures : Statistic
    {
        public Statistic_RequestFailures()
        {
            Name = @"Peers\Request Failures";
            MaxLabel = "10";
            MaxValue = 10;
            DefaultShown = false;

            Series.Outline = Drawing.PrimaryOutlineColors[4];
            Series.Fill = Drawing.PrimaryFillColors[4];

            Series.YAxis.AutoAdjustMax = true;
            Series.YAxis.FormatMaxLabelAsInteger = true;
        }
    }

    /// <summary>
    /// </summary>
    public class Statistic_BlockListUpdates : Statistic
    {
        public Statistic_BlockListUpdates()
        {
            Name = @"Peers\Block List Updates";
            MaxLabel = "8";
            MaxValue = 8;
            DefaultShown = false;

            Series.YAxis.AutoAdjustMax = true;
            Series.YAxis.FormatMaxLabelAsInteger = true;
        }
    }

    /// <summary>
    /// </summary>
    public class Statistic_PendingBlockRequests : Statistic
    {
        public Statistic_PendingBlockRequests()
        {
            Name = @"Peers\Pending Block Requests";
            MaxLabel = "32";
            MaxValue = 32;
            DefaultShown = false;

            Series.YAxis.AutoAdjustMax = true;
            Series.YAxis.FormatMaxLabelAsInteger = true;
        }
    }

    /// <summary>
    /// </summary>
    public class Statistic_ActiveBlockRequests : Statistic
    {
        public Statistic_ActiveBlockRequests()
        {
            Name = @"Peers\Active Block Requests";
            MaxLabel = "32";
            MaxValue = 32;
            DefaultShown = false;

            Series.YAxis.AutoAdjustMax = true;
            Series.YAxis.FormatMaxLabelAsInteger = true;
        }
    }

    /// <summary>
    /// </summary>
    public class Client
    {
        /// <summary>
        /// </summary>
        public const int MaxPeerConnections = 30;

        /// <summary>
        /// </summary>
        public const int TargetMillisecondsOfDataInFlight = 200;

        /// <summary>
        /// </summary>
        private const int BlockListUpdateInterval = 10 * 1000;

        /// <summary>
        /// </summary>
        private const int ClientStateUpdateInterval = 3 * 1000;

        /// <summary>
        /// </summary>
        private const int ConnectionAttemptInterval = 30 * 1000;

        /// <summary>
        /// </summary>
        private const int PeerConnectionAttemptInterval = 10 * 1000;

        /// <summary>
        /// </summary>
        private const int ListenAttemptInterval = 2 * 1000;

        /// <summary>
        /// </summary>
        private const int MaxConcurrentPeerRequests = 32;

        /// <summary>
        /// </summary>
        public long BlockListUpdateRate;

        /// <summary>
        /// </summary>
        public long BlockRequestFailureRate;

        /// <summary>
        /// </summary>
        public Queue<Action> DeferredActions = new Queue<Action>();

        /// <summary>
        /// </summary>
        public bool InternalConnectionsDisabled;

        /// <summary>
        /// </summary>
        public int PeerListenPortRangeMax;

        /// <summary>
        /// </summary>
        public int PeerListenPortRangeMin;

        /// <summary>
        /// </summary>
        public UserPermissionCollection Permissions = new UserPermissionCollection();

        /// <summary>
        /// </summary>
        public string ServerHostname;

        /// <summary>
        /// </summary>
        public int ServerPort;

        /// <summary>
        /// </summary>
        private int ActivePeerRequests = 0;

        /// <summary>
        /// </summary>
        private int OutstandingBlockSends = 0;

        /// <summary>
        /// </summary>
        private bool BlockListUpdatePending;

        /// <summary>
        /// </summary>
        private readonly NetConnection Connection = new NetConnection();

        /// <summary>
        /// </summary>
        private bool ConnectionInfoUpdateRequired;

        /// <summary>
        /// </summary>
        private bool DisableReconnect;

        /// <summary>
        /// </summary>
        private bool ForceBlockListUpdate;

        /// <summary>
        /// </summary>
        private bool InternalTrafficEnabled = true;

        /// <summary>
        /// </summary>
        private ulong LastBlockListUpdateTime;

        /// <summary>
        /// </summary>
        private ulong LastClientStateUpdateTime;

        /// <summary>
        /// </summary>
        private ulong LastConnectionAttempt;

        /// <summary>
        /// </summary>
        private ulong LastListenAttempt;

        /// <summary>
        /// </summary>
        private int LastManifestStateDirtyCounter;

        /// <summary>
        /// </summary>
        private readonly NetConnection ListenConnection = new NetConnection();

        /// <summary>
        /// </summary>
        private ManifestDownloadManager ManifestDownloadManager;

        /// <summary>
        /// </summary>
        private BuildManifestRegistry ManifestRegistry;

        /// <summary>
        /// </summary>
        private int PeerBlockRequestShuffleIndex;

        /// <summary>
        /// </summary>
        private int PeerCycleIndex;

        /// <summary>
        /// </summary>
        private readonly List<Peer> Peers = new List<Peer>();

        /// <summary>
        /// </summary>
        private int PortIndex;

        /// <summary>
        /// </summary>
        private List<IPEndPoint> RelevantPeerAddresses;

        /// <summary>
        /// </summary>
        private bool Started;

        /// <summary>
        /// </summary>
        public event BuildsRecievedHandler OnBuildsRecieved;

        /// <summary>
        /// </summary>
        public event ConenctedToServerHandler OnConnectedToServer;

        /// <summary>
        /// </summary>
        public event FailedToConnectToServerHandler OnFailedToConnectToServer;

        /// <summary>
        /// </summary>
        public event LicenseInfoRecievedHandler OnLicenseInfoRecieved;

        /// <summary>
        /// </summary>
        public event LostConnectionToServerHandler OnLostConnectionToServer;

        /// <summary>
        /// </summary>
        public event ManifestDeleteResultRecievedHandler OnManifestDeleteResultRecieved;

        /// <summary>
        /// </summary>
        public event ManifestPublishResultRecievedHandler OnManifestPublishResultRecieved;

        /// <summary>
        /// </summary>
        public event PermissionsUpdatedHandler OnPermissionsUpdated;

        /// <summary>
        /// </summary>
        public event ServerStateRecievedHandler OnServerStateRecieved;

        /// <summary>
        /// </summary>
        public event UserListRecievedHandler OnUserListRecieved;

        /// <summary>
        /// </summary>
        public Peer[] AllPeers
        {
            get
            {
                lock (Peers)
                {
                    return Peers.ToArray();
                }
            }
        }

        /// <summary>
        /// </summary>
        public bool ConnectionsDisabled
        {
            get => InternalConnectionsDisabled;
            set
            {
                bool Changed = InternalConnectionsDisabled != value;
                InternalConnectionsDisabled = value;
                if (InternalConnectionsDisabled && Changed)
                {
                    RestartConnections();
                }
            }
        }

        /// <summary>
        /// </summary>
        public HandshakeResultType HandshakeResult { get; private set; }

        /// <summary>
        /// </summary>
        public bool IsConnected => Connection != null ? Connection.IsConnected : false;

        /// <summary>
        /// </summary>
        public bool IsConnecting => Connection != null ? Connection.IsConnecting : false;

        /// <summary>
        /// </summary>
        public bool IsReadyForData => Connection != null ? Connection.IsReadyForData : false;

        /// <summary>
        /// </summary>
        public int PeerCount
        {
            get
            {
                lock (Peers)
                {
                    return Peers.Count;
                }
            }
        }

        /// <summary>
        /// </summary>
        public int ConnectedPeerCount
        {
            get
            {
                lock (Peers)
                {
                    int Count = 0;
                    foreach (Peer peer in Peers)
                    {
                        if (peer.Connection.IsReadyForData)
                        {
                            Count++;
                        }
                    }
                    return Count;
                }
            }
        }

        /// <summary>
        /// </summary>
        public bool TrafficEnabled
        {
            get => InternalTrafficEnabled;
            set
            {
                if (InternalTrafficEnabled != value)
                {
                    ForceBlockListUpdate = true;
                    InternalTrafficEnabled = value;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private PerformanceCounter ProcessCpuCounter;

        /// <summary>
        /// 
        /// </summary>
        private PerformanceCounter ProcessMemoryCounter;

        /// <summary>
        /// 
        /// </summary>
        private ulong PerfCounterTimer = 0;

        /// <summary>
        /// </summary>
        public Client()
        {
            Connection.OnMessageRecieved += HandleMessage;
            Connection.OnConnect += Connection => { OnConnectedToServer?.Invoke(); };
            Connection.OnDisconnect += Connection => { OnLostConnectionToServer?.Invoke(); };
            Connection.OnConnectFailed += Connection =>
            {
                OnFailedToConnectToServer?.Invoke();
                HandshakeResult = HandshakeResultType.Unknown;
            };
            Connection.OnHandshakeResult += (Connection, ResultType) =>
            {
                HandshakeResult = ResultType;
                if (ResultType == HandshakeResultType.InvalidVersion)
                {
                    DisableReconnect = true;
                }
            };

            ListenConnection.OnClientConnect += PeerConnected;

            MemoryPool.PreallocateBuffers((int) BuildManifest.BlockSize, 128);
            MemoryPool.PreallocateBuffers(Crc32.BufferSize, 16);
            NetConnection.PreallocateBuffers(NetConnection.MaxRecieveMessageBuffers, NetConnection.MaxSendMessageBuffers, NetConnection.MaxGenericMessageBuffers, NetConnection.MaxSmallMessageBuffers);

            try
            {
                ProcessCpuCounter = new PerformanceCounter("Process", "% Processor Time", Process.GetCurrentProcess().ProcessName, true);
                ProcessMemoryCounter = new PerformanceCounter("Process", "Working Set", Process.GetCurrentProcess().ProcessName, true);
            }
            catch (Exception Ex)
            {
                Logger.Log(LogLevel.Warning, LogCategory.Main, "Failed to load performance counters for process: {0}", Process.GetCurrentProcess().ProcessName);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="guid"></param>
        public bool DeleteManifest(Guid ManifestId)
        {
            if (!Connection.IsReadyForData)
            {
                Logger.Log(LogLevel.Warning, LogCategory.Main, "Failed to delete, no connection to server?");
                return false;
            }

            NetMessage_DeleteManifest Msg = new NetMessage_DeleteManifest();
            Msg.ManifestId = ManifestId;
            Connection.Send(Msg);

            return true;
        }

        /// <summary>
        /// </summary>
        /// <param name="Path"></param>
        public bool DeleteUser(string Username)
        {
            if (!Connection.IsReadyForData)
            {
                Logger.Log(LogLevel.Warning, LogCategory.Main, "Failed to delete user, no connection to server?");
                return false;
            }

            NetMessage_DeleteUser Msg = new NetMessage_DeleteUser();
            Msg.Username = Username;
            Connection.Send(Msg);

            return true;
        }

        /// <summary>
        /// </summary>
        /// <param name="Path"></param>
        public bool DeleteUserGroup(string Name)
        {
            if (!Connection.IsReadyForData)
            {
                Logger.Log(LogLevel.Warning, LogCategory.Main, "Failed to delete user group, no connection to server?");
                return false;
            }

            NetMessage_DeleteUserGroup Msg = new NetMessage_DeleteUserGroup();
            Msg.Name = Name;
            Connection.Send(Msg);

            return true;
        }

        /// <summary>
        /// </summary>
        /// <param name="Path"></param>
        public bool CreateUserGroup(string Name)
        {
            if (!Connection.IsReadyForData)
            {
                Logger.Log(LogLevel.Warning, LogCategory.Main, "Failed to create user group, no connection to server?");
                return false;
            }

            NetMessage_CreateUserGroup Msg = new NetMessage_CreateUserGroup();
            Msg.Name = Name;
            Connection.Send(Msg);

            return true;
        }

        /// <summary>
        /// </summary>
        /// <param name="Path"></param>
        public bool RemoveUserGroupPermission(string GroupName, UserPermissionType Type, string Path)
        {
            if (!Connection.IsReadyForData)
            {
                Logger.Log(LogLevel.Warning, LogCategory.Main, "Failed to remove user group permission, no connection to server?");
                return false;
            }

            NetMessage_RemoveUserGroupPermission Msg = new NetMessage_RemoveUserGroupPermission();
            Msg.GroupName = GroupName;
            Msg.PermissionType = Type;
            Msg.PermissionPath = Path;
            Connection.Send(Msg);

            return true;
        }

        /// <summary>
        /// </summary>
        /// <param name="Path"></param>
        public bool AddUserGroupPermission(string GroupName, UserPermissionType Type, string Path)
        {
            if (!Connection.IsReadyForData)
            {
                Logger.Log(LogLevel.Warning, LogCategory.Main, "Failed to add user group permission, no connection to server?");
                return false;
            }

            NetMessage_AddUserGroupPermission Msg = new NetMessage_AddUserGroupPermission();
            Msg.GroupName = GroupName;
            Msg.PermissionType = Type;
            Msg.PermissionPath = Path;
            Connection.Send(Msg);

            return true;
        }

        /// <summary>
        /// </summary>
        /// <param name="Path"></param>
        public bool RemoveUserFromUserGroup(string GroupName, string Username)
        {
            if (!Connection.IsReadyForData)
            {
                Logger.Log(LogLevel.Warning, LogCategory.Main, "Failed to remove user from user group, no connection to server?");
                return false;
            }

            NetMessage_RemoveUserFromUserGroup Msg = new NetMessage_RemoveUserFromUserGroup();
            Msg.GroupName = GroupName;
            Msg.Username = Username;
            Connection.Send(Msg);

            return true;
        }

        /// <summary>
        /// </summary>
        /// <param name="Path"></param>
        public bool AddUserToUserGroup(string GroupName, string Username)
        {
            if (!Connection.IsReadyForData)
            {
                Logger.Log(LogLevel.Warning, LogCategory.Main, "Failed to add user to user group, no connection to server?");
                return false;
            }

            NetMessage_AddUserToUserGroup Msg = new NetMessage_AddUserToUserGroup();
            Msg.GroupName = GroupName;
            Msg.Username = Username;
            Connection.Send(Msg);

            return true;
        }

        /// <summary>
        /// </summary>
        public void Disconnect()
        {
            Started = false;

            RestartConnections();
        }

        /// <summary>
        /// </summary>
        public void Poll()
        {
            if (Started)
            {
                // Reconnect?
                if (!Connection.IsConnected && !Connection.IsConnecting && !InternalConnectionsDisabled && !DisableReconnect)
                {
                    ulong ElapsedTime = TimeUtils.Ticks - LastConnectionAttempt;
                    if (ElapsedTime > ConnectionAttemptInterval)
                    {
                        ConnectionInfoUpdateRequired = true;
                        ConnectToServer();
                    }
                }

                // Reattempt to create listen server?
                if (!ListenConnection.IsListening && !InternalConnectionsDisabled && !DisableReconnect)
                {
                    ulong ElapsedTime = TimeUtils.Ticks - LastListenAttempt;
                    if (ElapsedTime > ListenAttemptInterval)
                    {
                        ConnectionInfoUpdateRequired = true;
                        ListenForPeers();
                    }
                }
            }

            // Periodically send our manifest block state to the server.
            if (Connection.IsReadyForData)
            {
                if (ManifestDownloadManager.StateDirtyCount > LastManifestStateDirtyCounter)
                {
                    BlockListUpdatePending = true;
                    LastManifestStateDirtyCounter = ManifestDownloadManager.StateDirtyCount;
                }

                if (ConnectionInfoUpdateRequired && ListenConnection.IsListening)
                {
                    SendConnectionInfo();
                    ConnectionInfoUpdateRequired = false;
                }

                ulong ElapsedTime = TimeUtils.Ticks - LastBlockListUpdateTime;
                if (ElapsedTime > BlockListUpdateInterval && BlockListUpdatePending || ForceBlockListUpdate)
                {
                    SendBlockListUpdate();

                    LastBlockListUpdateTime = TimeUtils.Ticks;
                    BlockListUpdatePending = false;
                    ForceBlockListUpdate = false;
                }

                ElapsedTime = TimeUtils.Ticks - LastClientStateUpdateTime;
                if (ElapsedTime > ClientStateUpdateInterval)
                {
                    SendClientUpdate();

                    LastClientStateUpdateTime = TimeUtils.Ticks;
                }
            }
            else
            {
                BlockListUpdatePending = true;
                LastBlockListUpdateTime = 0;
            }

            if (!InternalConnectionsDisabled)
            {
                ConnectToPeers();
            }

            Connection.Poll();
            ListenConnection.Poll();

            // Poll local initiated peer connections.
            lock (Peers)
            {
                foreach (Peer peer in Peers)
                {
                    if (!peer.RemoteInitiated)
                    {
                        peer.Connection.Poll();
                    }
                }
            }

            // Execute all deferred actions.
            lock (DeferredActions)
            {
                while (DeferredActions.Count > 0)
                {
                    DeferredActions.Dequeue().Invoke();
                }
            }

            UpdateBlockDownloads();
            UpdatePeerRequests();

            //ulong elapsed = TimeUtils.Ticks - time;
            //time = TimeUtils.Ticks;
            //Console.WriteLine("Elapsed: {0}", elapsed);

            // Update some stats.
            lock (Peers)
            {
                Statistic.Get<Statistic_PeerCount>().AddSample(Peers.Count);

                long DataInFlight = 0;
                long BlocksInFlight = 0;
                double AverageBlockLatency = 0;
                double AverageBlockSize = 0;
                long PendingBlockRequests = 0;
                foreach (Peer peer in Peers)
                {
                    DataInFlight += peer.ActiveBlockDownloadSize;
                    BlocksInFlight += peer.ActiveBlockDownloads.Count;
                    AverageBlockSize += peer.AverageBlockSize.Get();
                    AverageBlockLatency += peer.BlockRecieveLatency.Get();
                    PendingBlockRequests += peer.BlockRequestQueue.Count;
                }

                if (Peers.Count > 0)
                {
                    AverageBlockSize /= Peers.Count;
                    AverageBlockLatency /= Peers.Count;
                }

                Statistic.Get<Statistic_RequestFailures>().AddSample(BlockRequestFailureRate);
                Statistic.Get<Statistic_BlockListUpdates>().AddSample(BlockListUpdateRate);
                Statistic.Get<Statistic_DataInFlight>().AddSample(DataInFlight / 1024 / 1024);
                Statistic.Get<Statistic_BlocksInFlight>().AddSample(BlocksInFlight);
                Statistic.Get<Statistic_AverageBlockLatency>().AddSample((float) AverageBlockLatency);
                Statistic.Get<Statistic_AverageBlockSize>().AddSample((float) AverageBlockSize / 1024 / 1024);

                Statistic.Get<Statistic_ActiveBlockRequests>().AddSample(ActivePeerRequests);
                Statistic.Get<Statistic_PendingBlockRequests>().AddSample(PendingBlockRequests);

                // Querying perf counter is super expensive, don't do it often.
                if (TimeUtils.Ticks - PerfCounterTimer > 1000)
                {
                    if (ProcessCpuCounter != null)
                    {
                        Statistic.Get<Statistic_CpuUsage>().AddSample(ProcessCpuCounter.NextValue() / Environment.ProcessorCount);
                    }
                    if (ProcessMemoryCounter != null)
                    {
                        Statistic.Get<Statistic_MemoryUsage>().AddSample(ProcessMemoryCounter.NextValue());
                    }
                    PerfCounterTimer = TimeUtils.Ticks;
                }

                BlockRequestFailureRate = 0;
                BlockListUpdateRate = 0;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="guid"></param>
        public bool PublishManifest(BuildManifest Manifest)
        {
            if (!Connection.IsReadyForData)
            {
                Logger.Log(LogLevel.Warning, LogCategory.Main, "Failed to publish, no connection to server?");
                return false;
            }

            byte[] Data = Manifest.ToByteArray();

            NetMessage_PublishManifest Msg = new NetMessage_PublishManifest();
            Msg.Data = Data;
            Msg.ManifestId = Manifest.Guid;
            Connection.Send(Msg);

            return true;
        }

        /// <summary>
        /// </summary>
        /// <param name="Path"></param>
        public bool RequestApplyLicense(License license)
        {
            if (!Connection.IsReadyForData)
            {
                Logger.Log(LogLevel.Warning, LogCategory.Main, "Failed to apply license, no connection to server?");
                return false;
            }

            NetMessage_ApplyLicense Msg = new NetMessage_ApplyLicense();
            Msg.License = license;
            Connection.Send(Msg);

            return true;
        }

        /// <summary>
        /// </summary>
        /// <param name="Path"></param>
        public bool RequestBuilds(string Path)
        {
            if (!Connection.IsReadyForData)
            {
                Logger.Log(LogLevel.Warning, LogCategory.Main, "Failed to request builds, no connection to server?");
                return false;
            }

            NetMessage_GetBuilds Msg = new NetMessage_GetBuilds();
            Msg.RootPath = Path;
            Connection.Send(Msg);

            return true;
        }

        /// <summary>
        /// </summary>
        /// <param name="Path"></param>
        public bool RequestLicenseInfo()
        {
            if (!Connection.IsReadyForData)
            {
                Logger.Log(LogLevel.Warning, LogCategory.Main, "Failed to request license info, no connection to server?");
                return false;
            }

            NetMessage_GetLicenseInfo Msg = new NetMessage_GetLicenseInfo();
            Connection.Send(Msg);

            return true;
        }

        /// <summary>
        /// </summary>
        public bool RequestManifest(Guid Id)
        {
            if (!Connection.IsReadyForData)
            {
                Logger.Log(LogLevel.Warning, LogCategory.Main, "Failed to request manifests, no connection to server?");
                return false;
            }

            NetMessage_GetManifest Msg = new NetMessage_GetManifest();
            Msg.ManifestId = Id;
            Connection.Send(Msg);

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool RequestChooseDeletionCandidate(List<Guid> Candidates)
        {
            if (!Connection.IsReadyForData)
            {
                Logger.Log(LogLevel.Warning, LogCategory.Main, "Failed to request deletion candidate, no connection to server?");
                return false;
            }

            NetMessage_ChooseDeletionCandidate Msg = new NetMessage_ChooseDeletionCandidate();
            Msg.CandidateManifestIds = Candidates;
            Connection.Send(Msg);

            return true;
        }
    
        /// <summary>
        /// </summary>
        /// <param name="Path"></param>
        public bool RequestServerState()
        {
            if (!Connection.IsReadyForData)
            {
                Logger.Log(LogLevel.Warning, LogCategory.Main, "Failed to request server state, no connection to server?");
                return false;
            }

            NetMessage_GetServerState Msg = new NetMessage_GetServerState();
            Connection.Send(Msg);

            return true;
        }

        /// <summary>
        /// </summary>
        /// <param name="Path"></param>
        public bool RequestUserList()
        {
            if (!Connection.IsReadyForData)
            {
                Logger.Log(LogLevel.Warning, LogCategory.Main, "Failed to request users, no connection to server?");
                return false;
            }

            NetMessage_GetUsers Msg = new NetMessage_GetUsers();
            Connection.Send(Msg);

            return true;
        }

        /// <summary>
        /// </summary>
        public void RestartConnections()
        {
            lock (Peers)
            {
                foreach (Peer peer in Peers)
                {
                    peer.Connection.Disconnect();
                }

                Connection.Disconnect();
                ListenConnection.Disconnect();

                // Deferred actions contain responses to connection events, so purge them. 
                lock (DeferredActions)
                {
                    DeferredActions.Clear();
                }

                LastConnectionAttempt = 0;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="Path"></param>
        public bool SetServerMaxBandwidth(long MaxBandwidth)
        {
            if (!Connection.IsReadyForData)
            {
                Logger.Log(LogLevel.Warning, LogCategory.Main, "Failed to set max bandwidth, no connection to server?");
                return false;
            }

            NetMessage_SetServerMaxBandwidth Msg = new NetMessage_SetServerMaxBandwidth();
            Msg.BandwidthLimit = MaxBandwidth;
            Connection.Send(Msg);

            return true;
        }

        /// <summary>
        /// </summary>
        /// <param name="Hostname"></param>
        /// <param name="Port"></param>
        public void Start(string Hostname, int Port, int ListenPortRangeMin, int ListenPortRangeMax, BuildManifestRegistry BuildManifest, ManifestDownloadManager DownloadManager)
        {
            ServerHostname = Hostname;
            ServerPort = Port;
            PeerListenPortRangeMin = ListenPortRangeMin;
            PeerListenPortRangeMax = ListenPortRangeMax;
            ManifestRegistry = BuildManifest;
            ManifestDownloadManager = DownloadManager;
            ManifestDownloadManager.OnManifestRequested += Id => { RequestManifest(Id); };
            ManifestDownloadManager.OnRequestChooseDeletionCandidate += Candidates => { RequestChooseDeletionCandidate(Candidates); };

            Started = true;
        }

        /// <summary>
        /// </summary>
        private void ConnectToPeers()
        {
            if (RelevantPeerAddresses != null)
            {
                // Connect to relevant peers we are not already connected to.
                foreach (IPEndPoint EndPoint in RelevantPeerAddresses)
                {
                    Peer peer = null;
                    lock (Peers)
                    {
                        peer = GetPeerByAddress(EndPoint);
                        if (peer == null && Peers.Count < MaxPeerConnections)
                        {
                            peer = new Peer();
                            peer.Address = EndPoint;
                            peer.RemoteInitiated = false;
                            peer.LastConnectionAttemptTime = 0;
                            peer.Connection.OnMessageRecieved += HandleMessage;
                            Peers.Add(peer);
                        }
                    }

                    if (peer == null)
                    {
                        continue;
                    }

                    // Attempt connection if time has elapsed.
                    if (!peer.Connection.IsConnected && !peer.Connection.IsConnecting && !peer.RemoteInitiated)
                    {
                        if (peer.WasConnected)
                        {
                            Logger.Log(LogLevel.Info, LogCategory.Peers, "Disconnected from peer: {0}", peer.Address.ToString());
                        }

                        ulong Elapsed = TimeUtils.Ticks - peer.LastConnectionAttemptTime;
                        if (peer.LastConnectionAttemptTime == 0 || Elapsed > PeerConnectionAttemptInterval)
                        {
                            Logger.Log(LogLevel.Info, LogCategory.Peers, "Connecting to peer: {0}", peer.Address.ToString());
                            peer.Connection.BeginConnect(peer.Address.Address.ToString(), peer.Address.Port);

                            peer.LastConnectionAttemptTime = TimeUtils.Ticks;
                        }
                    }
                    else if (peer.Connection.IsConnected)
                    {
                        peer.LastConnectionAttemptTime = TimeUtils.Ticks;

                        if (!peer.WasConnected)
                        {
                            // Exchange block list with remote.
                            BlockListUpdatePending = true;
                        }
                    }

                    peer.WasConnected = peer.Connection.IsConnected;
                }
            }

            // Disconnect from no longer relevant peers if we are above ideal number.
            lock (Peers)
            {
                for (int i = 0; i < Peers.Count; i++)
                {
                    Peer peer = Peers[i];

                    bool ShouldRemove = false;

                    if (RelevantPeerAddresses != null && !peer.RemoteInitiated)
                    {
                        bool Exists = false;

                        foreach (IPEndPoint EndPoint in RelevantPeerAddresses)
                        {
                            if (EndPoint.Equals(peer.Address))
                            {
                                Exists = true;
                                break;
                            }
                        }

                        if (!Exists)
                        {
                            ShouldRemove = true;
                        }
                    }

                    // Always remove peers that were remote initiated and are now closed as we won't be
                    // reinitiating their connection anytime soon.
                    if (peer.RemoteInitiated && !peer.Connection.IsConnected)
                    {
                        ShouldRemove = true;
                    }
                    /*else if (Peers.Count < IdealPeerCount)
                    {
                        // Don't bother removing peers if below ideal peer count, they may be useful later.
                        ShouldRemove = false;
                    }*/

                    if (ShouldRemove)
                    {
                        Logger.Log(LogLevel.Info, LogCategory.Peers, "Disconnecting from peer: {0}", peer.Address.ToString());

                        peer.Connection.OnMessageRecieved -= HandleMessage;

                        if (peer.Connection.IsConnected)
                        {
                            peer.Connection.Disconnect();
                        }

                        Peers.RemoveAt(i);
                        i--;
                    }
                }
            }
        }

        /// <summary>
        /// </summary>
        private void ConnectToServer()
        {
            Connection.BeginConnect(ServerHostname, ServerPort);

            LastConnectionAttempt = TimeUtils.Ticks;
        }

        /// <summary>
        /// </summary>
        /// <param name="EndPoint"></param>
        /// <returns></returns>
        private Peer GetPeerByAddress(IPEndPoint EndPoint)
        {
            lock (Peers)
            {
                foreach (Peer peer in Peers)
                {
#if SHIPPING
                    if (peer.Address.Address.Equals(EndPoint.Address))
#else
                    if (peer.Address.Equals(EndPoint))
#endif
                    {
                        return peer;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// </summary>
        /// <param name="EndPoint"></param>
        /// <returns></returns>
        private Peer GetPeerByConnection(NetConnection Connection)
        {
            lock (Peers)
            {
                foreach (Peer peer in Peers)
                {
                    if (peer.Connection == Connection)
                    {
                        return peer;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// </summary>
        /// <param name="Connection"></param>
        /// <param name="Message"></param>
        private void HandleMessage(NetConnection Connection, NetMessage BaseMessage)
        {
            // Server has sent us back a list of builds from a previous request.
            if (BaseMessage is NetMessage_GetBuildsResponse)
            {
                NetMessage_GetBuildsResponse Msg = BaseMessage as NetMessage_GetBuildsResponse;

                Logger.Log(LogLevel.Verbose, LogCategory.Main, "Recieved builds in folder: {0}", Msg.RootPath);

                OnBuildsRecieved?.Invoke(Msg.RootPath, Msg.Builds);
            }

            // Server is giving us response to manifest publishing.
            else if (BaseMessage is NetMessage_PublishManifestResponse)
            {
                NetMessage_PublishManifestResponse Msg = BaseMessage as NetMessage_PublishManifestResponse;

                Logger.Log(LogLevel.Info, LogCategory.Main, "Recieved response for manifest publish: {0}", Msg.Result.ToString());

                OnManifestPublishResultRecieved?.Invoke(Msg.ManifestId, Msg.Result);
            }

            // Server is giving us response to manifest deleting.
            else if (BaseMessage is NetMessage_DeleteManifestResponse)
            {
                NetMessage_DeleteManifestResponse Msg = BaseMessage as NetMessage_DeleteManifestResponse;

                Logger.Log(LogLevel.Info, LogCategory.Main, "Recieved response for manifest delete: {0}", Msg.ManifestId);

                OnManifestDeleteResultRecieved?.Invoke(Msg.ManifestId);
            }

            // Server is sending us a list of peers that are relevant to our active downloads.
            else if (BaseMessage is NetMessage_RelevantPeerListUpdate)
            {
                NetMessage_RelevantPeerListUpdate Msg = BaseMessage as NetMessage_RelevantPeerListUpdate;

                Logger.Log(LogLevel.Info, LogCategory.Main, "Recieved relevant peer list update.");

                RelevantPeerAddresses = Msg.PeerAddresses;
            }

            // Server is sending us our permissions
            else if (BaseMessage is NetMessage_PermissionUpdate)
            {
                NetMessage_PermissionUpdate Msg = BaseMessage as NetMessage_PermissionUpdate;

                Logger.Log(LogLevel.Info, LogCategory.Main, "Recieved permissions update.");

                Logger.Log(LogLevel.Info, LogCategory.Main, "My Permissions:");
                foreach (UserPermission Permission in Msg.Permissions.Permissions)
                {
                    Logger.Log(LogLevel.Info, LogCategory.Main, "\tType={0} Path={1}", Permission.Type.ToString(), Permission.VirtualPath);
                }

                Permissions = Msg.Permissions;
                OnPermissionsUpdated?.Invoke();
            }

            // Server has chosen which manifest we should delete.
            else if (BaseMessage is NetMessage_ChooseDeletionCandidateResponse)
            {
                NetMessage_ChooseDeletionCandidateResponse Msg = BaseMessage as NetMessage_ChooseDeletionCandidateResponse;

                Logger.Log(LogLevel.Info, LogCategory.Main, "Recieved candidate deletion response.");

                ManifestDownloadManager.PruneManifest(Msg.ManifestId);
            }            

            // Server is sending us a manifest we previously requested.
            else if (BaseMessage is NetMessage_GetManifestResponse)
            {
                NetMessage_GetManifestResponse Msg = BaseMessage as NetMessage_GetManifestResponse;

                Logger.Log(LogLevel.Info, LogCategory.Main, "Recieved manifest: {0}", Msg.ManifestId.ToString());
                try
                {
                    BuildManifest Manifest = BuildManifest.FromByteArray(Msg.Data);
                    ManifestRegistry.RegisterManifest(Manifest);
                }
                catch (Exception ex)
                {
                    Logger.Log(LogLevel.Error, LogCategory.Main, "Failed to process manifest response due to error: {0}", ex.Message);
                }
            }

            // Recieved block list update from peer.
            else if (BaseMessage is NetMessage_BlockListUpdate)
            {
                NetMessage_BlockListUpdate Msg = BaseMessage as NetMessage_BlockListUpdate;

                Logger.Log(LogLevel.Verbose, LogCategory.Main, "Recieved block list update from: {0}", Connection.Address.ToString());

                Peer peer = GetPeerByConnection(Connection);
                if (peer != null)
                {
                    lock (Peers)
                    {
                        peer.BlockState = Msg.BlockState;
                    }
                }

                Interlocked.Increment(ref BlockListUpdateRate);

                UpdateAvailableBlocks();
            }

            // Peer requested a block of data from us.
            else if (BaseMessage is NetMessage_GetBlock)
            {
                NetMessage_GetBlock Msg = BaseMessage as NetMessage_GetBlock;

                Peer peer = GetPeerByConnection(Connection);
                if (peer != null)
                {
                    PendingBlockRequest Request;
                    Request.Message = Msg;
                    Request.Requester = Connection;
                    peer.BlockRequestQueue.Enqueue(Request);
                    //Console.WriteLine("[Enqueing] BlockIndex={0} Manifest={1}", Request.Message.BlockIndex, Request.Message.ManifestId.ToString());
                }

                //PendingBlockRequestQueue.Add(new PendingBlockRequest { Msg, Connection });
            }

            // Peer provided block of data we requested.
            else if (BaseMessage is NetMessage_GetBlockResponse)
            {
                NetMessage_GetBlockResponse Msg = BaseMessage as NetMessage_GetBlockResponse;

                if (Msg.Data.Data == null)
                {
                    Logger.Log(LogLevel.Info, LogCategory.Main, "Recieved null block {0} in manifest {1} from {2}, looks like peer no longer has it.", Msg.BlockIndex, Msg.ManifestId.ToString(), Connection.Address.ToString());

                    lock (DeferredActions)
                    {
                        DeferredActions.Enqueue(
                            () =>
                            {
                                Peer peer = GetPeerByConnection(Connection);
                                if (peer != null)
                                {
                                    peer.SetBlockState(Msg.ManifestId, Msg.BlockIndex, false);
                                }
                            }
                        );
                    }

                    Interlocked.Increment(ref BlockRequestFailureRate);
                    Msg.Cleanup();
                }
                else
                {
                    //Logger.Log(LogLevel.Info, LogCategory.Main, "Recieved block {0} in manifest {1} from {2}", Msg.BlockIndex, Msg.ManifestId.ToString(), Connection.Address.ToString());
                    Peer peer = GetPeerByConnection(Connection);
                    if (peer != null)
                    {
                        //peer.MarkActiveBlockDownloadAsRecieved(Msg.ManifestId, Msg.BlockIndex);
                    }

                    if (!peer.HasActiveBlockDownload(Msg.ManifestId, Msg.BlockIndex))
                    {
                        Logger.Log(LogLevel.Warning, LogCategory.Main, "Recieved unexpected block {0} in manifest {1} from {2}", Msg.BlockIndex, Msg.ManifestId.ToString(), Connection.Address.ToString());

                        Interlocked.Increment(ref BlockRequestFailureRate);
                        Msg.Cleanup();

                        return;
                    }

                    BlockAccessCompleteHandler Callback = bSuccess =>
                    {
                        Msg.Cleanup();

                        //Console.WriteLine("[Finished] BlockIndex={0} Manifest={1} bSuccess={2} From={3}", Msg.BlockIndex, Msg.ManifestId.ToString(), bSuccess, HostnameCache.GetHostname(Connection.Address.ToString()));

                        lock (DeferredActions)
                        {
                            DeferredActions.Enqueue(
                                () =>
                                {
                                    // Mark block as complete.
                                    if (bSuccess)
                                    {
                                        ManifestDownloadManager.MarkBlockAsComplete(Msg.ManifestId, Msg.BlockIndex, true);
                                    }

                                    // Remove active download marker for this block.
                                    peer.RemoveActiveBlockDownload(Msg.ManifestId, Msg.BlockIndex, bSuccess);
                                }
                            );
                        }
                    };

                    //Console.WriteLine("[Recieved] BlockIndex={0} Manifest={1}", Msg.BlockIndex, Msg.ManifestId.ToString());
                    if (!ManifestDownloadManager.SetBlockData(Msg.ManifestId, Msg.BlockIndex, Msg.Data, Callback))
                    {
                        Logger.Log(LogLevel.Warning, LogCategory.Main, "Recieved invalid block data from: {0}", HostnameCache.GetHostname(Connection.Address.Address.ToString()));
                    }
                }
            }

            // Recieved user list.
            else if (BaseMessage is NetMessage_GetUsersResponse)
            {
                NetMessage_GetUsersResponse Msg = BaseMessage as NetMessage_GetUsersResponse;

                Logger.Log(LogLevel.Verbose, LogCategory.Main, "Recieved users list with {0} users.", Msg.Users.Count);

                OnUserListRecieved?.Invoke(Msg.Users, Msg.UserGroups);
            }

            // Receive license info,
            else if (BaseMessage is NetMessage_GetLicenseInfoResponse)
            {
                NetMessage_GetLicenseInfoResponse Msg = BaseMessage as NetMessage_GetLicenseInfoResponse;

                Logger.Log(LogLevel.Info, LogCategory.Main, "Recieved license info.");

                OnLicenseInfoRecieved?.Invoke(Msg.License);
            }

            // Receive license info,
            else if (BaseMessage is NetMessage_GetServerStateResponse)
            {
                NetMessage_GetServerStateResponse Msg = BaseMessage as NetMessage_GetServerStateResponse;

                OnServerStateRecieved?.Invoke(Msg);
            }

            // Enforcing bandwidth limitations
            else if (BaseMessage is NetMessage_EnforceBandwidthLimit)
            {
                NetMessage_EnforceBandwidthLimit Msg = BaseMessage as NetMessage_EnforceBandwidthLimit;

                Logger.Log(LogLevel.Info, LogCategory.Main, "Server has enforced bandwidth limit of: {0}", StringUtils.FormatAsTransferRate(Msg.BandwidthLimit));

                // Only limit in rate, out rate is limited implicitly.
                NetConnection.GlobalBandwidthThrottleIn.GlobalMaxRate = Msg.BandwidthLimit;
            }
        }

        /// <summary>
        /// </summary>
        private void ListenForPeers()
        {
            int PortCount = PeerListenPortRangeMax - PeerListenPortRangeMin + 1;
            int Port = PeerListenPortRangeMin + PortIndex++ % PortCount;
            ListenConnection.BeginListen(Port, false);

            LastListenAttempt = TimeUtils.Ticks;
        }

        /// <summary>
        /// </summary>
        private void MarkBlockDownloadsForUpdate()
        {
            //DownloadQueueIsDirty = 1;
        }

        /// <summary>
        /// </summary>
        /// <param name="Connection"></param>
        /// <param name="ClientConnection"></param>
        private void PeerConnected(NetConnection Connection, NetConnection ClientConnection)
        {
            lock (Peers)
            {
                Peer existingPeer = GetPeerByAddress(ClientConnection.Address);
                
                Logger.Log(LogLevel.Info, LogCategory.Peers, "Peer connected from {0}.", ClientConnection.Address.ToString());

                Peer newPeer = new Peer();
                newPeer.Address = ClientConnection.Address;
                newPeer.Connection = ClientConnection;
                newPeer.Connection.OnMessageRecieved += HandleMessage;
                newPeer.RemoteInitiated = true;
                newPeer.LastConnectionAttemptTime = 0;
                Peers.Add(newPeer);

                if (false)//existingPeer != null)
                { 
                    // If we have multiple connections (typically caused because we both try and open a connection at the same time), we need to close one of them.
                    bool bRemoteHasPriority = (newPeer.Connection.Address.Address.Address < existingPeer.Connection.Address.Address.Address);

                    Logger.Log(LogLevel.Info, LogCategory.Peers, "Peer connected from {0}, but we already have a local connection, {1} takes priority.", newPeer.Connection.Address.ToString(), bRemoteHasPriority ? "REMOTE" : "LOCAL");

                    // The peer with the lowest ip address takes priority.
                    if (bRemoteHasPriority)
                    {
                        // Their connection takes priority.
                        existingPeer.Connection.QueueDisconnect();
                    }
                    else
                    {
                        // Our connection takes priority.                        
                        ClientConnection.QueueDisconnect();

                        existingPeer.LastConnectionAttemptTime = 0;
                    }
                }
            }

            // Exchange block list with remote.
            BlockListUpdatePending = true;
        }

        /// <summary>
        /// </summary>
        private void SendBlockListUpdate()
        {
            Logger.Log(LogLevel.Verbose, LogCategory.Main, "Sending block list update.");

            BlockListState State = ManifestDownloadManager.GetBlockListState();

            NetMessage_BlockListUpdate Msg = new NetMessage_BlockListUpdate();
            Msg.BlockState = State;

            // Send to server and each peer.
            foreach (Peer peer in Peers)
            {
                if (peer.Connection.IsConnected)
                {
                    Logger.Log(LogLevel.Verbose, LogCategory.Main, "\tSent to peer: " + peer.Connection.Address);
                    peer.Connection.Send(Msg);
                }
            }

            Logger.Log(LogLevel.Verbose, LogCategory.Main, "\tSent to server.");
            Connection.Send(Msg);

            /*
            BlockListState DeltaEncoded = State;
            if (LastBlockListState != null)
            {
                DeltaEncoded = State.GetDelta(LastBlockListState);
            }

            NetMessage_BlockListUpdate Msg = new NetMessage_BlockListUpdate();
            Msg.BlockState = DeltaEncoded;

            LastBlockListState = DeltaEncoded;

            Connection.Send(Msg);
            */
        }

        /// <summary>
        /// </summary>
        private void SendClientUpdate()
        {
            NetMessage_ClientStateUpdate Msg = new NetMessage_ClientStateUpdate();
            Msg.DownloadRate = NetConnection.GlobalBandwidthStats.RateIn;
            Msg.UploadRate = NetConnection.GlobalBandwidthStats.RateOut;
            Msg.TotalDownloaded = NetConnection.GlobalBandwidthStats.TotalIn;
            Msg.TotalUploaded = NetConnection.GlobalBandwidthStats.TotalOut;
            Msg.DiskUsage = 0;
            Msg.ConnectedPeerCount = 0;
            Msg.Version = AppVersion.VersionString;

            lock (Peers)
            {
                for (int i = 0; i < Peers.Count; i++)
                {
                    Peer peer = Peers[i];
                    if (peer.Connection.IsConnected)
                    {
                        Msg.ConnectedPeerCount++;
                    }
                }
            }


            foreach (ManifestDownloadState Manifest in ManifestDownloadManager.States.States)
            {
                Msg.DiskUsage += Manifest.Manifest != null ? Manifest.Manifest.GetTotalSize() : 0;
            }

            Connection.Send(Msg);
        }

        /// <summary>
        /// </summary>
        private void SendConnectionInfo()
        {
            if (!Connection.IsReadyForData)
            {
                return;
            }

            NetMessage_ConnectionInfo Msg = new NetMessage_ConnectionInfo();
            Msg.Username = Environment.UserDomainName + "\\" + Environment.UserName;
            Msg.PeerConnectionAddress = ListenConnection.ListenAddress;
            Connection.Send(Msg);
        }

        /// <summary>
        /// </summary>
        private void UpdateAvailableBlocks()
        {
            // Inform the download manager what blocks are available from peers.
            BlockListState State = new BlockListState();

            lock (Peers)
            {
                foreach (Peer peer in Peers)
                {
                    if (peer.BlockState != null)
                    {
                        State.Union(peer.BlockState);
                    }
                }
            }

            ManifestDownloadManager.SetAvailableToDownloadBlocks(State);
        }

        /// <summary>
        /// </summary>
        private void UpdateBlockDownloads()
        {
            // TODO: Make this whole function less crap and less cpu consuming.
            //if (Interlocked.CompareExchange(ref DownloadQueueIsDirty, 0, 1) == 1)
            //{
            //    return;
            //}

            ManifestDownloadManager.UpdateBlockQueue();

            if (ManifestDownloadManager.DownloadQueue == null)
            {
                return;
            }

            lock (Peers)
            {
                // Remove any downloads that have timed out.
                foreach (Peer peer in Peers)
                {
                    peer.PruneTimeoutDownloads();
                }

                for (int i = 0; i < ManifestDownloadManager.DownloadQueue.Count; i++)
                {
                    ManifestPendingDownloadBlock Item = ManifestDownloadManager.DownloadQueue[i];

                    // Make sure block is not already being downloaded.
                    // TODO: Change to a set lookup, this is slow.
                    bool AlreadyDownloading = false;
                    foreach (Peer peer in Peers)
                    {
                        if (peer.IsDownloadingBlock(Item))
                        {
                            AlreadyDownloading = true;
                        }
                    }

                    if (AlreadyDownloading)
                    {
                        continue;
                    }

                    // Get block information so we know if its small enough to add to any peers queue.
                    BuildManifestBlockInfo BlockInfo = new BuildManifestBlockInfo();
                    if (!ManifestDownloadManager.GetBlockInfo(Item.ManifestId, Item.BlockIndex, ref BlockInfo))
                    {
                        continue;
                    }

                    // Find all peers that have this block.
                    List<Peer> LeastLoadedPeers = new List<Peer>();
                    long LeastLoadedPeerAvailableBandwidth = 0;
                    for (int j = 0; j < Peers.Count; j++)
                    {
                        Peer Peer = Peers[j];
                        if (!Peer.Connection.IsReadyForData)
                        {
                            continue;
                        }

                        if (!Peer.HasBlock(Item))
                        {
                            continue;
                        }

                        long MaxBandwidth = Peer.GetMaxInFlightData(TargetMillisecondsOfDataInFlight);
                        long BandwidthAvailable = Peer.GetAvailableInFlightData(TargetMillisecondsOfDataInFlight);

                        if ((BandwidthAvailable >= BlockInfo.TotalSize || BlockInfo.TotalSize > MaxBandwidth) && (LeastLoadedPeers.Count == 0 || Peer.ActiveBlockDownloadSize <= LeastLoadedPeerAvailableBandwidth))
                        {
                            if (LeastLoadedPeers.Count == 0 || Peer.ActiveBlockDownloadSize < LeastLoadedPeerAvailableBandwidth)
                            {
                                LeastLoadedPeers.Clear();
                                LeastLoadedPeerAvailableBandwidth = Peer.ActiveBlockDownloadSize;
                            }

                            LeastLoadedPeers.Add(Peer);
                        }
                    }

                    if (LeastLoadedPeers.Count > 0)
                    {
                        Peer LeastLoadedPeer = LeastLoadedPeers[PeerBlockRequestShuffleIndex % LeastLoadedPeers.Count];
                        PeerBlockRequestShuffleIndex++;
                        if (PeerBlockRequestShuffleIndex > Peers.Count)
                        {
                            PeerBlockRequestShuffleIndex = 0;
                        }

                        NetMessage_GetBlock Msg = new NetMessage_GetBlock();
                        Msg.ManifestId = Item.ManifestId;
                        Msg.BlockIndex = Item.BlockIndex;
                        LeastLoadedPeer.Connection.Send(Msg);

                        Item.TimeStarted = TimeUtils.Ticks;
                        Item.Size = BlockInfo.TotalSize;

                        LeastLoadedPeer.AddActiveBlockDownload(Item);

                        long MaxBandwidth = LeastLoadedPeer.GetMaxInFlightData(TargetMillisecondsOfDataInFlight);
                        long BandwidthAvailable = LeastLoadedPeer.GetAvailableInFlightData(TargetMillisecondsOfDataInFlight);
                    }
                }

                if (Peers.Count > 0)
                {
                    PeerCycleIndex = ++PeerCycleIndex % Peers.Count;
                }
            }
        }

        /// <summary>
        /// </summary>
        private void UpdatePeerRequests()
        {
            int ActiveRequests = ActivePeerRequests + OutstandingBlockSends;
            while (ActiveRequests < MaxConcurrentPeerRequests)
            {
                Peer BestPeer = null;
                ulong BestPeerElapsed = ulong.MaxValue;

                lock (Peers)
                {
                    // Find peer with the longest time since one if its requests were fulfilled.
                    foreach (Peer peer in Peers)
                    {
                        if (peer.BlockRequestQueue.Count > 0 && peer.Connection.IsReadyForData)
                        {
                            ulong Elapsed = TimeUtils.Ticks - peer.LastBlockRequestFulfillTime;
                            if (Elapsed < BestPeerElapsed)
                            {
                                BestPeer = peer;
                                BestPeerElapsed = Elapsed;
                            }
                        }
                    }
                }

                if (BestPeer != null)
                {
                    PendingBlockRequest Request;
                    if (BestPeer.BlockRequestQueue.TryDequeue(out Request))
                    {
                        //Console.WriteLine("[Processing] BlockIndex={0} Manifest={1}", Request.Message.BlockIndex, Request.Message.ManifestId.ToString());

                        BestPeer.LastBlockRequestFulfillTime = TimeUtils.Ticks;
                        Interlocked.Increment(ref ActivePeerRequests);

                        NetMessage_GetBlockResponse Response = new NetMessage_GetBlockResponse();
                        Response.ManifestId = Request.Message.ManifestId;
                        Response.BlockIndex = Request.Message.BlockIndex;

                        BlockAccessCompleteHandler Callback = bSuccess =>
                        {
                            Interlocked.Decrement(ref ActivePeerRequests);

                            lock (DeferredActions)
                            {
                                DeferredActions.Enqueue(
                                    () =>
                                    {
                                        if (!bSuccess)
                                        {
                                            Logger.Log(LogLevel.Warning, LogCategory.Main, "Failed to retrieve requested block {0} in manifest {1} for peer {2}.", Request.Message.BlockIndex, Request.Message.ManifestId.ToString(), Request.Requester.Address.ToString());

                                            ManifestDownloadManager.MarkAllBlockFilesAsUnavailable(Request.Message.ManifestId, Request.Message.BlockIndex);
                                            //ManifestDownloadManager.MarkBlockAsUnavailable(Msg.ManifestId, Msg.BlockIndex);
                                            Response.Data.SetNull();
                                        }

                                        if (Request.Requester.IsReadyForData)
                                        {
                                            Interlocked.Increment(ref OutstandingBlockSends);
                                            Request.Requester.Send(Response, (bool success) => { 
                                                Interlocked.Decrement(ref OutstandingBlockSends); 
                                            });
                                        }

                                        Response.Data.SetNull(); // Free data it's been serialized by this point.
                                    }
                                );
                            }
                        };

                        bool FailedOutOfMemory = false;
                        bool Ret = ManifestDownloadManager.GetBlockData(Request.Message.ManifestId, Request.Message.BlockIndex, ref Response.Data, Callback, out FailedOutOfMemory);

                        // If we don't have enough memory available to store this block in memory right now then requeue and stop trying to fulfill peer requests, none will has memory yet.
                        if (!Ret && FailedOutOfMemory)
                        {
                            //Console.WriteLine("[Requeing] BlockIndex={0} Manifest={1}", Request.Message.BlockIndex, Request.Message.ManifestId.ToString());
                            Interlocked.Decrement(ref ActivePeerRequests);
                            BestPeer.BlockRequestQueue.Enqueue(Request);
                            return;
                        }
                    }
                }
                else
                {
                    break;
                }
            }
        }
    }
}