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
using System.Drawing;
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
using BuildSync.Core.Networking.Messages.RemoteActions;
using BuildSync.Core.Users;
using BuildSync.Core.Utils;
using BuildSync.Core.Tags;
using BuildSync.Core.Routes;
using BuildSync.Core.Storage;

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
    public delegate void FilteredBuildsRecievedHandler(NetMessage_GetFilteredBuildsResponse.BuildInfo[] Builds);

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
    public delegate void TagListRecievedHandler(List<Tag> Tag);

    /// <summary>
    /// </summary>
    public delegate void RouteListRecievedHandler(List<Route> Routes);

    /// <summary>
    /// </summary>
    public delegate void BuildPublishedHandler(string Path, Guid ManifestId);

    /// <summary>
    /// </summary>
    public delegate void BuildUpdatedHandler(string Path, Guid ManifestId);

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
    /// <param name="Connection"></param>
    /// <param name="Message"></param>
    public delegate void ClientTagsUpdatedByServerHandler();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="Msg"></param>
    public delegate void RemoteActionProgressRecievedHandler(NetMessage_RemoteActionProgress Msg);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="Msg"></param>
    public delegate void SolicitRemoteActionRecievedHandler(NetMessage_SolicitRemoteAction Msg);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="Msg"></param>
    public delegate void RequestRemoteActionRecievedHandler(NetMessage_RequestRemoteAction Msg);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="Msg"></param>
    public delegate void CancelRemoteActionRecievedHandler(Guid ActionId);

    /*
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
    */

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
    public class Statistic_OutstandingBlockSends : Statistic
    {
        public Statistic_OutstandingBlockSends()
        {
            Name = @"Peers\Oustanding Block Sends";
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
        public const int TargetMillisecondsOfDataInFlight = 1500;

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
        private const int MaxConcurrentPeerRequests = 200;

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
        public readonly NetConnection Connection = new NetConnection();

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
        /// 
        /// </summary>
        private int LastManifestStateBlockDirtyCounter;

        /// <summary>
        /// 
        /// </summary>
        public bool AllowRemoteActions = false;

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
        /// 
        /// </summary>
        private StorageManager StorageManager;

        /// <summary>
        /// </summary>
        private TagRegistry TagRegistry;

        /// <summary>
        /// </summary>
        private RouteRegistry RouteRegistry;

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
        /// 
        /// </summary>
        private ulong LastRequestManifestTime = TimeUtils.Ticks;

        /// <summary>
        /// </summary>
        public const int BlockRequestTimeout = 60 * 1000;

        /// <summary>
        /// 
        /// </summary>
        private List<Guid> TagIdsInternal = new List<Guid>();

        /// <summary>
        ///     List of all client tags id's for the client.
        /// </summary>
        public List<Guid> TagIds
        {
            get
            {
                return TagIdsInternal;
            }
            set
            {
                TagIdsInternal = value;
                NetConnection.LocalTagIds = TagIdsInternal;
            }
        }

        /// <summary>
        /// </summary>
        public event BuildsRecievedHandler OnBuildsRecieved;

        /// <summary>
        /// </summary>
        public event FilteredBuildsRecievedHandler OnFilteredBuildsRecieved;

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
        public event BuildPublishedHandler OnBuildPublished;

        /// <summary>
        /// </summary>
        public event BuildUpdatedHandler OnBuildUpdated;

        /// <summary>
        /// </summary>
        public event ServerStateRecievedHandler OnServerStateRecieved;

        /// <summary>
        /// </summary>
        public event UserListRecievedHandler OnUserListRecieved;

        /// <summary>
        /// </summary>
        public event TagListRecievedHandler OnTagListRecieved;

        /// <summary>
        /// </summary>
        public event RouteListRecievedHandler OnRouteListRecieved;

        /// <summary>
        /// </summary>
        public event ClientTagsUpdatedByServerHandler OnClientTagsUpdatedByServer;

        /// <summary>
        /// </summary>
        public event RemoteActionProgressRecievedHandler OnRemoteActionProgressRecieved;

        /// <summary>
        /// </summary>
        public event SolicitRemoteActionRecievedHandler OnSolicitRemoteActionRecieved;

        /// <summary>
        /// </summary>
        public event RequestRemoteActionRecievedHandler OnRequestRemoteActionRecieved;

        /// <summary>
        /// </summary>
        public event CancelRemoteActionRecievedHandler OnCancelRemoteActionRecieved;

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

        /*
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
        */

        /// <summary>
        /// 
        /// </summary>
        private List<RoutePair> BandwidthLimitRoutes = new List<RoutePair>();

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
                    RestartConnections();
                }
            };

            NetConnection.LocalTagIds = TagIds;

            ListenConnection.OnClientConnect += PeerConnected;

            MemoryPool.PreallocateBuffers((int)BuildManifest.DefaultBlockSize, 64);
            NetConnection.PreallocateBuffers(NetConnection.MaxRecieveMessageBuffers, NetConnection.MaxSendMessageBuffers, NetConnection.MaxGenericMessageBuffers, NetConnection.MaxSmallMessageBuffers);

            /*
            try
            {
                ProcessCpuCounter = new PerformanceCounter("Process", "% Processor Time", Process.GetCurrentProcess().ProcessName, true);
                ProcessMemoryCounter = new PerformanceCounter("Process", "Working Set", Process.GetCurrentProcess().ProcessName, true);
            }
            catch (Exception Ex)
            {
                Logger.Log(LogLevel.Warning, LogCategory.Main, "Failed to load performance counters for process: {0}", Process.GetCurrentProcess().ProcessName);
            }
            */
        }

        /// <summary>
        /// </summary>
        /// <param name="guid"></param>
        public bool DeleteManifest(Guid ManifestId)
        {
            if (!Connection.IsReadyForData)
            {
                Logger.Log(LogLevel.Verbose, LogCategory.Main, "Failed to delete, no connection to server?");
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
                Logger.Log(LogLevel.Verbose, LogCategory.Main, "Failed to delete user, no connection to server?");
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
                Logger.Log(LogLevel.Verbose, LogCategory.Main, "Failed to delete user group, no connection to server?");
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
                Logger.Log(LogLevel.Verbose, LogCategory.Main, "Failed to create user group, no connection to server?");
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
                Logger.Log(LogLevel.Verbose, LogCategory.Main, "Failed to remove user group permission, no connection to server?");
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
                Logger.Log(LogLevel.Verbose, LogCategory.Main, "Failed to add user group permission, no connection to server?");
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
                Logger.Log(LogLevel.Verbose, LogCategory.Main, "Failed to remove user from user group, no connection to server?");
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
                Logger.Log(LogLevel.Verbose, LogCategory.Main, "Failed to add user to user group, no connection to server?");
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
                if (!Connection.IsConnected && !Connection.IsConnecting && !InternalConnectionsDisabled && !DisableReconnect && ServerHostname.Length > 0)
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
                    ForceBlockListUpdate = true;
                    LastManifestStateDirtyCounter = ManifestDownloadManager.StateDirtyCount;
                }

                if (ManifestDownloadManager.StateBlockDirtyCount > LastManifestStateBlockDirtyCounter)
                {
                    BlockListUpdatePending = true;
                    LastManifestStateBlockDirtyCounter = ManifestDownloadManager.StateBlockDirtyCount;
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

            if (!InternalConnectionsDisabled && !DisableReconnect)
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

            EnforceBandwidthLimits();
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
                Statistic.Get<Statistic_AverageBlockLatency>().AddSample((float)AverageBlockLatency);
                Statistic.Get<Statistic_AverageBlockSize>().AddSample((float)AverageBlockSize / 1024 / 1024);

                Statistic.Get<Statistic_ActiveBlockRequests>().AddSample(ActivePeerRequests);
                Statistic.Get<Statistic_OutstandingBlockSends>().AddSample(OutstandingBlockSends);
                Statistic.Get<Statistic_PendingBlockRequests>().AddSample(PendingBlockRequests);

                // Querying perf counter is super expensive, don't do it often.
                /*
                // Fuck it, this thing is ridiculous, it uses a ton of memory as well.
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
                */

                BlockRequestFailureRate = 0;
                BlockListUpdateRate = 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void EnforceBandwidthLimits()
        {
            lock (Peers)
            {
                foreach (Peer peer in Peers)
                {
                    // We only enforce limits on locally initiated connections, remote peers will do vis-versa.
                    if (!peer.RemoteInitiated && peer.Connection.IsReadyForData)
                    {
                        long MostRestrictiveBandwidthLimit = 0;
                        RoutePair MostRestrictiveRoute = new RoutePair { SourceTagId = Guid.Empty, DestinationTagId = Guid.Empty };

                        foreach (Guid SourceTag in TagIds)
                        {
                            foreach (Guid DestinationTag in peer.Connection.Handshake.TagIds)
                            {
                                RoutePair TargetRoute = new RoutePair { SourceTagId = SourceTag, DestinationTagId = DestinationTag };

                                long BandwidthLimit = GetBandwidthLimitForRoute(TargetRoute);
                                if ((BandwidthLimit != 0 && BandwidthLimit < MostRestrictiveBandwidthLimit) ||
                                    (MostRestrictiveRoute.DestinationTagId == Guid.Empty))
                                {
                                    MostRestrictiveBandwidthLimit = BandwidthLimit;
                                    MostRestrictiveRoute = TargetRoute;
                                }
                            }
                        }

                        // Mark this connection as going through the most restrictive route.
                        peer.Connection.Route = MostRestrictiveRoute;
                    }
                }
            }

            // Set global limits for each routes.
            NetConnection.SetBandwidthLimitRoutes(BandwidthLimitRoutes);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="TargetRoute"></param>
        /// <returns></returns>
        private long GetBandwidthLimitForRoute(RoutePair TargetRoute)
        {
            foreach (RoutePair Limit in BandwidthLimitRoutes)
            {
                if (Limit.SourceTagId == TargetRoute.SourceTagId &&
                    Limit.DestinationTagId == TargetRoute.DestinationTagId)
                {
                    return Limit.Bandwidth;
                }
            }

            return 0;
        }

        /// <summary>
        /// </summary>
        /// <param name="guid"></param>
        public bool PublishManifest(BuildManifest Manifest)
        {
            if (!Connection.IsReadyForData)
            {
                Logger.Log(LogLevel.Verbose, LogCategory.Main, "Failed to publish, no connection to server?");
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
                Logger.Log(LogLevel.Verbose, LogCategory.Main, "Failed to apply license, no connection to server?");
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
                Logger.Log(LogLevel.Verbose, LogCategory.Main, "Failed to request builds, no connection to server?");
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
        public bool RequestFilteredBuilds(List<Guid> SelectTags, List<Guid> IgnoreTags, DateTime NewerThan)
        {
            if (!Connection.IsReadyForData)
            {
                Logger.Log(LogLevel.Verbose, LogCategory.Main, "Failed to request builds, no connection to server?");
                return false;
            }

            NetMessage_GetFilteredBuilds Msg = new NetMessage_GetFilteredBuilds();
            Msg.SelectTags = SelectTags;
            Msg.IgnoreTags = IgnoreTags;
            Msg.NewerThan = NewerThan;
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
                Logger.Log(LogLevel.Verbose, LogCategory.Main, "Failed to request license info, no connection to server?");
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
                Logger.Log(LogLevel.Verbose, LogCategory.Main, "Failed to request manifests, no connection to server?");
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
        /// <param name="Id"></param>
        /// <returns></returns>
        public BuildManifest GetOrRequestManifestById(Guid Id, bool Throttle = true)
        {
            BuildManifest Manifest = ManifestRegistry.GetManifestById(Id);
            if (Manifest == null)
            {
                if (!Throttle || TimeUtils.Ticks - LastRequestManifestTime > 500)
                {
                    LastRequestManifestTime = TimeUtils.Ticks;
                    RequestManifest(Id);
                }
            }
            else
            {
                return Manifest;
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public int GetRequestedBlocksForDownload(Guid Id)
        {
            int Count = 0;
            lock (Peers)
            {
                foreach (Peer peer in Peers)
                {
                    foreach (ManifestPendingDownloadBlock Request in peer.ActiveBlockDownloads)
                    {
                        if (Request.ManifestId == Id)
                        {
                            Count++;
                        }
                    }
                }
            }
            return Count;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool RequestChooseDeletionCandidate(List<Guid> Candidates, ManifestStorageHeuristic Heuristic, List<Guid> PrioritizeKeepingTagIds, List<Guid> PrioritizeDeletingTagIds)
        {
            if (!Connection.IsReadyForData)
            {
                Logger.Log(LogLevel.Verbose, LogCategory.Main, "Failed to request deletion candidate, no connection to server?");
                return false;
            }

            NetMessage_ChooseDeletionCandidate Msg = new NetMessage_ChooseDeletionCandidate();
            Msg.CandidateManifestIds = Candidates;
            Msg.Heuristic = Heuristic;
            Msg.PrioritizeKeepingTagIds = PrioritizeKeepingTagIds;
            Msg.PrioritizeDeletingTagIds = PrioritizeDeletingTagIds;
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
                Logger.Log(LogLevel.Verbose, LogCategory.Main, "Failed to request server state, no connection to server?");
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
                Logger.Log(LogLevel.Verbose, LogCategory.Main, "Failed to request users, no connection to server?");
                return false;
            }

            NetMessage_GetUsers Msg = new NetMessage_GetUsers();
            Connection.Send(Msg);

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool RequestTagList()
        {
            if (!Connection.IsReadyForData)
            {
                Logger.Log(LogLevel.Verbose, LogCategory.Main, "Failed to request tags, no connection to server?");
                return false;
            }

            NetMessage_GetTags Msg = new NetMessage_GetTags();
            Connection.Send(Msg);

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool RequestRouteList()
        {
            if (!Connection.IsReadyForData)
            {
                Logger.Log(LogLevel.Verbose, LogCategory.Main, "Failed to request routes, no connection to server?");
                return false;
            }

            NetMessage_GetRoutes Msg = new NetMessage_GetRoutes();
            Connection.Send(Msg);

            return true;
        }

        /// <summary>
        /// </summary>
        /// <param name="Path"></param>
        public bool DeleteRoute(Guid RouteId)
        {
            if (!Connection.IsReadyForData)
            {
                Logger.Log(LogLevel.Verbose, LogCategory.Main, "Failed to delete route, no connection to server?");
                return false;
            }

            NetMessage_DeleteRoute Msg = new NetMessage_DeleteRoute();
            Msg.RouteId = RouteId;
            Connection.Send(Msg);

            return true;
        }

        /// <summary>
        /// </summary>
        /// <param name="Path"></param>
        public bool CreateRoute(Guid SourceTagId, Guid DestinationTagId, bool Blacklisted, long BandwidthLimit)
        {
            if (!Connection.IsReadyForData)
            {
                Logger.Log(LogLevel.Verbose, LogCategory.Main, "Failed to create route, no connection to server?");
                return false;
            }

            NetMessage_CreateRoute Msg = new NetMessage_CreateRoute();
            Msg.SourceTagId = SourceTagId;
            Msg.DestinationTagId = DestinationTagId;
            Msg.Blacklisted = Blacklisted;
            Msg.BandwidthLimit = BandwidthLimit;
            Connection.Send(Msg);

            return true;
        }

        /// <summary>
        /// </summary>
        /// <param name="Path"></param>
        public bool UpdateRoute(Guid RouteId, Guid SourceTagId, Guid DestinationTagId, bool Blacklisted, long BandwidthLimit)
        {
            if (!Connection.IsReadyForData)
            {
                Logger.Log(LogLevel.Verbose, LogCategory.Main, "Failed to create route, no connection to server?");
                return false;
            }

            NetMessage_UpdateRoute Msg = new NetMessage_UpdateRoute();
            Msg.RouteId = RouteId;
            Msg.SourceTagId = SourceTagId;
            Msg.DestinationTagId = DestinationTagId;
            Msg.Blacklisted = Blacklisted;
            Msg.BandwidthLimit = BandwidthLimit;
            Connection.Send(Msg);

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ManifestId"></param>
        /// <param name="TagId"></param>
        public bool AddTagToManifest(Guid ManifestId, Guid TagId)
        {
            if (!Connection.IsReadyForData)
            {
                Logger.Log(LogLevel.Verbose, LogCategory.Main, "Failed to add tag to manifest, no connection to server?");
                return false;
            }

            NetMessage_AddTagToManifest Msg = new NetMessage_AddTagToManifest();
            Msg.ManifestId = ManifestId;
            Msg.TagId = TagId;
            Connection.Send(Msg);

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ManifestId"></param>
        /// <param name="TagId"></param>
        public bool RemoveTagFromManifest(Guid ManifestId, Guid TagId)
        {
            if (!Connection.IsReadyForData)
            {
                Logger.Log(LogLevel.Verbose, LogCategory.Main, "Failed to remove tag to manifest, no connection to server?");
                return false;
            }

            NetMessage_RemoveTagFromManifest Msg = new NetMessage_RemoveTagFromManifest();
            Msg.ManifestId = ManifestId;
            Msg.TagId = TagId;
            Connection.Send(Msg);

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ManifestId"></param>
        /// <param name="TagId"></param>
        public bool AddTagToClient(string Address, Guid TagId)
        {
            if (!Connection.IsReadyForData)
            {
                Logger.Log(LogLevel.Verbose, LogCategory.Main, "Failed to add tag to client, no connection to server?");
                return false;
            }

            NetMessage_AddTagToClient Msg = new NetMessage_AddTagToClient();
            Msg.ClientAddress = Address;
            Msg.TagId = TagId;
            Connection.Send(Msg);

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ManifestId"></param>
        /// <param name="TagId"></param>
        public bool RemoveTagFromClient(string Address, Guid TagId)
        {
            if (!Connection.IsReadyForData)
            {
                Logger.Log(LogLevel.Verbose, LogCategory.Main, "Failed to remove tag from client, no connection to server?");
                return false;
            }

            NetMessage_RemoveTagFromClient Msg = new NetMessage_RemoveTagFromClient();
            Msg.ClientAddress = Address;
            Msg.TagId = TagId;
            Connection.Send(Msg);

            return true;
        }

        /// <summary>
        /// </summary>
        /// <param name="Path"></param>
        public bool DeleteTag(Guid TagId)
        {
            if (!Connection.IsReadyForData)
            {
                Logger.Log(LogLevel.Verbose, LogCategory.Main, "Failed to delete tag, no connection to server?");
                return false;
            }

            NetMessage_DeleteTag Msg = new NetMessage_DeleteTag();
            Msg.TagId = TagId;
            Connection.Send(Msg);

            return true;
        }

        /// <summary>
        /// </summary>
        /// <param name="Path"></param>
        public bool CreateTag(string Name, Color TagColor, bool Unique, Guid DecayTagId)
        {
            if (!Connection.IsReadyForData)
            {
                Logger.Log(LogLevel.Verbose, LogCategory.Main, "Failed to create tag, no connection to server?");
                return false;
            }

            NetMessage_CreateTag Msg = new NetMessage_CreateTag();
            Msg.Name = Name;
            Msg.Color = TagColor;
            Msg.Unique = Unique;
            Msg.DecayTagId = DecayTagId;
            Connection.Send(Msg);

            return true;
        }

        /// <summary>
        /// </summary>
        /// <param name="Path"></param>
        public bool RenameTag(Guid Id, string Name, Color TagColor, bool Unique, Guid DecayTagId)
        {
            if (!Connection.IsReadyForData)
            {
                Logger.Log(LogLevel.Verbose, LogCategory.Main, "Failed to create tag, no connection to server?");
                return false;
            }

            NetMessage_RenameTag Msg = new NetMessage_RenameTag();
            Msg.Id = Id;
            Msg.Name = Name;
            Msg.Color = TagColor;
            Msg.Unique = Unique;
            Msg.DecayTagId = DecayTagId;
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
                Logger.Log(LogLevel.Verbose, LogCategory.Main, "Failed to set max bandwidth, no connection to server?");
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
        public void Start(string Hostname, int Port, int ListenPortRangeMin, int ListenPortRangeMax, bool InAllowRemoteActions, List<Guid> InTagIds, BuildManifestRegistry BuildManifest, StorageManager InStorageManager, ManifestDownloadManager DownloadManager, TagRegistry InTagRegistry, RouteRegistry InRouteRegistry)
        {
            ServerHostname = Hostname;
            ServerPort = Port;
            PeerListenPortRangeMin = ListenPortRangeMin;
            PeerListenPortRangeMax = ListenPortRangeMax;
            AllowRemoteActions = InAllowRemoteActions;
            TagIds = new List<Guid>(InTagIds);
            ManifestRegistry = BuildManifest;
            ManifestDownloadManager = DownloadManager;
            ManifestDownloadManager.OnManifestRequested += Id => { RequestManifest(Id); };
            StorageManager = InStorageManager;
            StorageManager.OnRequestChooseDeletionCandidate += (Candidates, Heuristic, PrioritizeKeepingTagIds, PrioritizeDeletingTagIds) => { RequestChooseDeletionCandidate(Candidates, Heuristic, PrioritizeKeepingTagIds, PrioritizeDeletingTagIds); };
            TagRegistry = InTagRegistry;
            RouteRegistry = InRouteRegistry;

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

            // Server has sent us back a list of filtered builds from previous response.
            else if (BaseMessage is NetMessage_GetFilteredBuildsResponse)
            {
                NetMessage_GetFilteredBuildsResponse Msg = BaseMessage as NetMessage_GetFilteredBuildsResponse;

                Logger.Log(LogLevel.Verbose, LogCategory.Main, "Recieved filtered builds.");

                OnFilteredBuildsRecieved?.Invoke(Msg.Builds);
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

                Logger.Log(LogLevel.Verbose, LogCategory.Main, "Recieved relevant peer list update (count {0}).", Msg.PeerAddresses.Count);

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

                StorageManager.ResetPruneTimeout();
            }

            // Server has chosen which manifest we should delete.
            else if (BaseMessage is NetMessage_RestartPeerConnections)
            {
                NetMessage_RestartPeerConnections Msg = BaseMessage as NetMessage_RestartPeerConnections;

                Logger.Log(LogLevel.Info, LogCategory.Main, "Recieved request to restart peer connections.");

                RestartConnections();
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
                    Logger.Log(LogLevel.Error, LogCategory.Main, "Failed to process manifest response due to error: {0}", ex.ToString());
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
                    Request.QueueTime = TimeUtils.Ticks;
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

                    // Keep a log of the current queue size.
                    if (Msg.QueueDepthMs >= 0)
                    {
                        peer.RecievedNewQueueDepth(Msg.QueueDepthMs, Msg.QueueSequence);
                    }

                    if (!peer.HasActiveBlockDownload(Msg.ManifestId, Msg.BlockIndex) && peer.HasBlock(Msg.ManifestId, Msg.BlockIndex))
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
                        Logger.Log(LogLevel.Warning, LogCategory.Main, "Recieved invalid block data from: {0}", Connection.Handshake.MachineName);
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

            // Recieved tag list.
            else if (BaseMessage is NetMessage_GetTagsResponse)
            {
                NetMessage_GetTagsResponse Msg = BaseMessage as NetMessage_GetTagsResponse;

                Logger.Log(LogLevel.Verbose, LogCategory.Main, "Recieved tag list with {0} tags.", Msg.Tags.Count);

                TagRegistry.Tags = Msg.Tags;

                OnTagListRecieved?.Invoke(Msg.Tags);
            }

            // Recieved route list.
            else if (BaseMessage is NetMessage_GetRoutesResponse)
            {
                NetMessage_GetRoutesResponse Msg = BaseMessage as NetMessage_GetRoutesResponse;

                Logger.Log(LogLevel.Verbose, LogCategory.Main, "Recieved route list with {0} tags.", Msg.Routes.Count);

                RouteRegistry.Routes = Msg.Routes;

                OnRouteListRecieved?.Invoke(Msg.Routes);
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

                Logger.Log(LogLevel.Info, LogCategory.Main, "Server has enforced bandwidth limit of: {0} (and limits on {1} routes)", StringUtils.FormatAsTransferRate(Msg.BandwidthLimitGlobal), Msg.BandwidthLimitRoutes.Count);

                // Only limit in rate, out rate is limited implicitly.
                NetConnection.GlobalBandwidthThrottleIn.GlobalMaxRate = Msg.BandwidthLimitGlobal;
                BandwidthLimitRoutes = Msg.BandwidthLimitRoutes;
            }
            // ------------------------------------------------------------------------------
            // Add a tag to a client
            // ------------------------------------------------------------------------------
            else if (BaseMessage is NetMessage_AddTagToClient)
            {
                NetMessage_AddTagToClient Msg = BaseMessage as NetMessage_AddTagToClient;

                Tag Tag = TagRegistry.GetTagById(Msg.TagId);
                if (Tag == null)
                {
                    Logger.Log(LogLevel.Warning, LogCategory.Main, "Server tried to untag an unknown tag '{1}'.", Msg.TagId.ToString());
                    return;
                }

                if (Connection != this.Connection)
                {
                    Logger.Log(LogLevel.Warning, LogCategory.Main, "Non-server user attempted to add a tag to us.");
                    return;
                }

                if (!TagIds.Contains(Tag.Id))
                {
                    TagIds.Add(Tag.Id);
                    OnClientTagsUpdatedByServer?.Invoke();
                }

                Logger.Log(LogLevel.Warning, LogCategory.Main, "Server tagged client '{0}' with tag '{1}'.", Msg.ClientAddress.ToString(), Msg.TagId.ToString());
            }

            // ------------------------------------------------------------------------------
            // Remove a tag to a client.
            // ------------------------------------------------------------------------------
            else if (BaseMessage is NetMessage_RemoveTagFromClient)
            {
                NetMessage_RemoveTagFromClient Msg = BaseMessage as NetMessage_RemoveTagFromClient;

                Tag Tag = TagRegistry.GetTagById(Msg.TagId);
                if (Tag == null)
                {
                    Logger.Log(LogLevel.Warning, LogCategory.Main, "Server tried to untag an unknown tag '{1}'.", Msg.TagId.ToString());
                    return;
                }

                if (Connection != this.Connection)
                {
                    Logger.Log(LogLevel.Warning, LogCategory.Main, "Non-server user attempted to remove one of our tags.");
                    return;
                }

                TagIds.Remove(Tag.Id);
                OnClientTagsUpdatedByServer?.Invoke();

                Logger.Log(LogLevel.Warning, LogCategory.Main, "Server untagged client '{0}' with tag '{1}'.", Msg.ClientAddress.ToString(), Msg.TagId.ToString());
            }

            // ------------------------------------------------------------------------------
            // Build Published
            // ------------------------------------------------------------------------------
            else if (BaseMessage is NetMessage_BuildPublished)
            {
                NetMessage_BuildPublished Msg = BaseMessage as NetMessage_BuildPublished;

                Logger.Log(LogLevel.Info, LogCategory.Main, "Server notified us of new build published '{0}'.", Msg.Path);

                OnBuildPublished?.Invoke(Msg.Path, Msg.ManifestId);
            }

            // ------------------------------------------------------------------------------
            // Build Updated
            // ------------------------------------------------------------------------------
            else if (BaseMessage is NetMessage_BuildUpdated)
            {
                NetMessage_BuildUpdated Msg = BaseMessage as NetMessage_BuildUpdated;

                Logger.Log(LogLevel.Info, LogCategory.Main, "Server notified us of new build updated '{0}'.", Msg.Path);

                OnBuildUpdated?.Invoke(Msg.Path, Msg.ManifestId);
            }

            // ------------------------------------------------------------------------------
            // Remote action progress update
            // ------------------------------------------------------------------------------
            else if (BaseMessage is NetMessage_RemoteActionProgress)
            {
                NetMessage_RemoteActionProgress Msg = BaseMessage as NetMessage_RemoteActionProgress;

                OnRemoteActionProgressRecieved?.Invoke(Msg);
            }

            // ------------------------------------------------------------------------------
            // Server soliciting us for a remote action.
            // ------------------------------------------------------------------------------
            else if (BaseMessage is NetMessage_SolicitRemoteAction)
            {
                NetMessage_SolicitRemoteAction Msg = BaseMessage as NetMessage_SolicitRemoteAction;

                OnSolicitRemoteActionRecieved?.Invoke(Msg);
            }

            // ------------------------------------------------------------------------------
            // Request to start a remote action.
            // ------------------------------------------------------------------------------
            else if (BaseMessage is NetMessage_RequestRemoteAction)
            {
                NetMessage_RequestRemoteAction Msg = BaseMessage as NetMessage_RequestRemoteAction;

                OnRequestRemoteActionRecieved?.Invoke(Msg);
            }

            // ------------------------------------------------------------------------------
            // Request to cancel a remote action.
            // ------------------------------------------------------------------------------
            else if (BaseMessage is NetMessage_CancelRemoteAction)
            {
                NetMessage_CancelRemoteAction Msg = BaseMessage as NetMessage_CancelRemoteAction;

                OnCancelRemoteActionRecieved?.Invoke(Msg.ActionId);
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
            Msg.AllowRemoteActions = AllowRemoteActions;

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

            Msg.DiskUsage = StorageManager.GetDiskUsage();
            Msg.DiskQuota = StorageManager.GetDiskQuota();

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

            // We do this to ensure the local-ip connected to is the same one as we connect to the server on. 
            // This ensures we get the ip is routed through the correct network interfaces in the cases of using VPN's etc.
            Msg.PeerConnectionAddress = new IPEndPoint(Connection.LocalAddress.Address, ListenConnection.ListenAddress.Port);

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
                //Console.WriteLine("========== Available Blocks ==========");
                foreach (Peer peer in Peers)
                {
                    if (peer.BlockState != null)
                    {
                        //Console.WriteLine("[ Peer {0} ]", peer.Connection.Handshake == null ? "" : peer.Connection.Handshake.Username);
                        State.Union(peer.BlockState);
                        //foreach (ManifestBlockListState OtherState in State.States)
                        //{
                        //    Console.WriteLine("{0}: Active={1} Size={2}", OtherState.Id.ToString(), OtherState.IsActive, OtherState.BlockState.Size);
                        //}
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

                // Hash all the active downloads to save lookup time.
                HashSet<ManifestPendingDownloadBlock> ActiveDownloads = new HashSet<ManifestPendingDownloadBlock>();
                foreach (Peer peer in Peers)
                {
                    lock (peer.ActiveBlockDownloads)
                    {
                        foreach (ManifestPendingDownloadBlock Download in peer.ActiveBlockDownloads)
                        {
                            ActiveDownloads.Add(Download);
                        }
                    }
                }

                for (int i = 0; i < ManifestDownloadManager.DownloadQueue.Count; i++)
                {
                    ManifestPendingDownloadBlock Item = ManifestDownloadManager.DownloadQueue[i];

                    // Make sure block is not already being downloaded.
                    // TODO: Change to a set lookup, this is slow.
                    if (ActiveDownloads.Contains(Item))
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
                    bool AnyPeersWithSpace = false;
                    for (int j = 0; j < Peers.Count; j++)
                    {
                        Peer Peer = Peers[j];
                        if (!Peer.Connection.IsReadyForData)
                        {
                            continue;
                        }

                        long MaxBandwidth = Peer.GetMaxInFlightData();
                        long BandwidthAvailable = Peer.GetAvailableInFlightData();

                        if (BandwidthAvailable > 0)
                        {
                            AnyPeersWithSpace = true;
                        }

                        if (!Peer.HasBlock(Item))
                        {
                            continue;
                        }

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
                        Msg.QueueSequence = LeastLoadedPeer.QueueSequence;
                        LeastLoadedPeer.Connection.Send(Msg);

                        Item.TimeStarted = TimeUtils.Ticks;
                        Item.Size = BlockInfo.TotalSize;

                        LeastLoadedPeer.AddActiveBlockDownload(Item);
                    }
                    else
                    {
                        // Early out if we now know no peers have bandwidth available.
                        if (!AnyPeersWithSpace)
                        {
                            break;
                        }
                    }
                }

                if (Peers.Count > 0)
                {
                    PeerCycleIndex = ++PeerCycleIndex % Peers.Count;
                }
            }

            UpdatePeerQueues();
        }

        private ulong LastPeerQueueUpdate = 0;
        private const int PeerQueueUpdateInterval = 20; // 50 times a second should be more than enough.

        /// <summary>
        /// </summary>
        private void UpdatePeerQueues()
        {
            if (TimeUtils.Ticks - LastPeerQueueUpdate < PeerQueueUpdateInterval)
            {
                return;
            }

            LastPeerQueueUpdate = TimeUtils.Ticks;

            lock (Peers)
            {
                Peer LowestQueueDepthPeer = null;
                float LowestQueueDepthDistance = 0.0f;

                foreach (Peer peer in Peers)
                {
                    if (!peer.PendingQueueUpdate || peer.LastBlockQueueSequence < peer.QueueSequence)
                    {
                        continue;
                    }

                    float DistanceFromTarget = Math.Abs(peer.QueueDepthMs - TargetMillisecondsOfDataInFlight);

                    if (LowestQueueDepthPeer == null || DistanceFromTarget < LowestQueueDepthDistance)
                    {
                        LowestQueueDepthPeer = peer;
                        LowestQueueDepthDistance = DistanceFromTarget;
                    }
                }

                if (LowestQueueDepthPeer != null)
                {
                    if (LowestQueueDepthPeer.QueueDepthMs > Client.TargetMillisecondsOfDataInFlight * 1.1f)
                    {
                        LowestQueueDepthPeer.MaxInFlightRequests = Math.Max(1, LowestQueueDepthPeer.MaxInFlightRequests - 1);
                    }
                    else if (LowestQueueDepthPeer.QueueDepthMs < Client.TargetMillisecondsOfDataInFlight * 0.9f)
                    {
                        LowestQueueDepthPeer.MaxInFlightRequests = Math.Min(Peer.MaxRequestQueueSize, LowestQueueDepthPeer.MaxInFlightRequests + 1);
                    }

                    LowestQueueDepthPeer.PendingQueueUpdate = false;
                    LowestQueueDepthPeer.QueueSequence++;
                }
            }
        }

        /// <summary>
        /// </summary>
        private void UpdatePeerRequests()
        {
            while (true)
            {
                int ActiveRequests = ActivePeerRequests + OutstandingBlockSends;
                if (ActiveRequests >= MaxConcurrentPeerRequests)
                {
                    break;
                }

                Peer BestPeer = null;
                ulong BestPeerElapsed = ulong.MinValue;

                lock (Peers)
                {
                    // Remove block requests over limit.
                    foreach (Peer peer in Peers)
                    {
                        PendingBlockRequest Request;
                        while (peer.BlockRequestQueue.Count > Peer.MaxRequestQueueSize)
                        {
                            peer.BlockRequestQueue.TryDequeue(out Request);
                            Logger.Log(LogLevel.Warning, LogCategory.Main, "Peer over queue size: removed request for block {0} in manifest {1} for peer {2}.", Request.Message.BlockIndex, Request.Message.ManifestId.ToString(), Request.Requester.Address.ToString());
                        }
                    }
                    
                    // Remove all timed out block requests.
                    foreach (Peer peer in Peers)
                    {
                        while (peer.BlockRequestQueue.Count > 0 && peer.Connection.IsReadyForData)
                        {
                            PendingBlockRequest Request;
                            if (peer.BlockRequestQueue.TryPeek(out Request))
                            {
                                ulong Elapsed = TimeUtils.Ticks - Request.QueueTime;
                                if (Elapsed > BlockRequestTimeout)
                                {
                                    peer.BlockRequestQueue.TryDequeue(out Request);
                                    Logger.Log(LogLevel.Warning, LogCategory.Main, "Peer request timed out: removed request for block {0} in manifest {1} for peer {2}.", Request.Message.BlockIndex, Request.Message.ManifestId.ToString(), Request.Requester.Address.ToString());
                                }
                                else;
                                {
                                    break;
                                }
                            }
                        }
                    }

                    // Find peer with the longest time its next block has been queued for.
                    foreach (Peer peer in Peers)
                    {
                        if (peer.BlockRequestQueue.Count > 0 && peer.Connection.IsReadyForData)
                        {
                            PendingBlockRequest Request;
                            if (peer.BlockRequestQueue.TryPeek(out Request))
                            {
                                ulong Elapsed = TimeUtils.Ticks - Request.QueueTime;
                                if (Elapsed > BestPeerElapsed)
                                {
                                    BestPeer = peer;
                                    BestPeerElapsed = Elapsed;
                                }
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
                        Interlocked.Increment(ref BestPeer.OutstandingReads);

                        NetMessage_GetBlockResponse Response = new NetMessage_GetBlockResponse();
                        Response.ManifestId = Request.Message.ManifestId;
                        Response.BlockIndex = Request.Message.BlockIndex;
                        Response.QueueSequence = Request.Message.QueueSequence;

                        BlockAccessCompleteHandler Callback = bSuccess =>
                        {
                            Interlocked.Decrement(ref ActivePeerRequests);
                            Interlocked.Decrement(ref BestPeer.OutstandingReads);

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
                                            Response.QueueDepthMs = BestPeer.GetRequestQueueDepthTime();

                                            Interlocked.Increment(ref BestPeer.OutstandingSends);
                                            Interlocked.Increment(ref OutstandingBlockSends);
                                            Request.Requester.Send(Response, (bool success) => { 
                                                Interlocked.Decrement(ref OutstandingBlockSends);
                                                Interlocked.Decrement(ref BestPeer.OutstandingSends);

                                                ulong FulfillTime = TimeUtils.Ticks - Request.QueueTime;
                                                BestPeer.AverageRequestFulfillTime.Add(FulfillTime);

                                                //Console.WriteLine("Fulfill:{0} Concurrency:{1} Depth:{2}", FulfillTime, BestPeer.AverageRequestConcurrency.Get(), BestPeer.RequestQueueDepth);
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
                            Interlocked.Decrement(ref BestPeer.OutstandingReads);
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

            // Update average queue depth.     
            lock (Peers)
            {
                // Remove block requests over limit.
                foreach (Peer peer in Peers)
                {
                    int Concurrency = peer.OutstandingReads + peer.OutstandingSends;

                    peer.RequestQueueDepth = peer.BlockRequestQueue.Count + peer.OutstandingReads + peer.OutstandingSends;
                    if (Concurrency > 0)
                    {
                        peer.AverageRequestConcurrency.Add(Concurrency);
                    }
                }
            }
        }
    }
}