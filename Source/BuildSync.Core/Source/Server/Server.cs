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
using System.Net;
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

namespace BuildSync.Core.Server
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="Msg"></param>
    public delegate void RequestRemoteActionRecievedHandler(NetConnection Client, NetMessage_RequestRemoteAction Msg);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="Msg"></param>
    public delegate void CancelRemoteActionRecievedHandler(NetConnection Client, Guid ActionId);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="Msg"></param>
    public delegate void SolicitAcceptRemoteActionRecievedHandler(NetConnection Client, Guid ActionId);
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="Msg"></param>
    public delegate void RemoteActionProgressRecievedHandler(NetMessage_RemoteActionProgress Msg);

    /// <summary>
    /// </summary>
    public class Server
    {
        /// <summary>
        /// 
        /// </summary>
        private class ServerClientManifestInfo
        {
            public int PeersWithAllBlocks;
        };

        /// <summary>
        /// 
        /// </summary>
        struct ManifestDeletionCandidate
        {
            public Guid Id;
            public string Path;
            public string ParentPath;
            public DateTime LastSeen;
            public DateTime CreateTime;
            public int NumberOfPeersWithFullBuild;
            public List<Guid> TagIds;
        }

        /// <summary>
        /// </summary>
        private const int ListenAttemptInterval = 2 * 1000;

        /// <summary>
        /// </summary>
        public long BandwidthLimit;

        /// <summary>
        /// </summary>
        public int MaxConnectedClients = 0;

        /// <summary>
        /// </summary>
        private ulong LastListenAttempt;

        /// <summary>
        /// </summary>
        private LicenseManager LicenseManager;

        /// <summary>
        /// </summary>
        private TagRegistry TagRegistry;

        /// <summary>
        /// 
        /// </summary>
        private RemoteActionServer RemoteActionServer;

        /// <summary>
        /// </summary>
        private RouteRegistry RouteRegistry;

        /// <summary>
        /// </summary>
        public readonly NetConnection ListenConnection = new NetConnection();

        /// <summary>
        /// </summary>
        private BuildManifestRegistry ManifestRegistry;

        /// <summary>
        /// </summary>
        private readonly NetDiscoveryServer NetDiscoveryServer = new NetDiscoveryServer();

        /// <summary>
        /// </summary>
        private int ServerPort;

        /// <summary>
        /// </summary>
        private bool Started;

        /// <summary>
        /// </summary>
        private UserManager UserManager;

        /// <summary>
        /// </summary>
        public int ClientCount => ListenConnection.AllClients.Count;

        /// <summary>
        /// 
        /// </summary>
        private Dictionary<Guid, ServerClientManifestInfo> ClientManifestInfo = new Dictionary<Guid, ServerClientManifestInfo>();

        /// <summary>
        /// 
        /// </summary>
        private List<NetConnection> ActiveBandwidthLimitsSentToClients = new List<NetConnection>();

        /// <summary>
        /// 
        /// </summary>
        private List<RoutePair> ActiveBandwidthLimits = new List<RoutePair>();

        /// <summary>
        /// 
        /// </summary>
        private long ActiveGlobalBandwidthLimit = 0;

        /// <summary>
        /// </summary>
        public event RequestRemoteActionRecievedHandler OnRequestRemoteActionRecieved;

        /// <summary>
        /// </summary>
        public event CancelRemoteActionRecievedHandler OnCancelRemoteActionRecieved;

        /// <summary>
        /// </summary>
        public event SolicitAcceptRemoteActionRecievedHandler OnSolicitAcceptRemoteActionRecieved;

        /// <summary>
        /// </summary>
        public event RemoteActionProgressRecievedHandler OnRemoteActionProgressRecieved;

        /// <summary>
        /// </summary>
        public Server()
        {
            ListenConnection.OnClientMessageRecieved += HandleMessage;
            ListenConnection.OnClientConnect += ClientConnected;
            ListenConnection.OnClientDisconnect += ClientDisconnected;

            MemoryPool.PreallocateBuffers((int)BuildManifest.DefaultBlockSize, 32);
            NetConnection.PreallocateBuffers(NetConnection.MaxRecieveMessageBuffers, NetConnection.MaxSendMessageBuffers, NetConnection.MaxGenericMessageBuffers, NetConnection.MaxSmallMessageBuffers);
        }

        /// <summary>
        /// </summary>
        public void Disconnect()
        {
            Started = false;
            ListenConnection.Disconnect();
        }

        /// <summary>
        /// </summary>
        public void Poll()
        {
            if (Started)
            {
                // Reattempt to create listen server?
                if (!ListenConnection.IsListening)
                {
                    ulong ElapsedTime = TimeUtils.Ticks - LastListenAttempt;
                    if (ElapsedTime > ListenAttemptInterval)
                    {
                        ListenForClients();
                    }
                }
            }

            // Go through each client, send them a list of peers that has blocks they are interested in.
            ListenConnection.MaxConnectedClients = MaxConnectedClients;

            List<NetConnection> ActivelyDownloadingClients = new List<NetConnection>();

            List<NetConnection> Clients = ListenConnection.AllClients;
            foreach (NetConnection Connection in Clients)
            {
                if (Connection.Metadata != null)
                {
                    ServerConnectedClient State = Connection.Metadata as ServerConnectedClient;

                    // Determine if client is actively download for bandwidth limitation later.
                    if (State.BlockState != null)
                    {
                        foreach (ManifestBlockListState ManifestState in State.BlockState.States)
                        {
                            if (ManifestState.IsActive)
                            {
                                ActivelyDownloadingClients.Add(Connection);
                                break;
                            }
                        }
                    }

                    // Send user an update of the permissions they have.
                    if (State.PermissionsNeedUpdate)
                    {
                        NetMessage_PermissionUpdate Msg = new NetMessage_PermissionUpdate();
                        Msg.Permissions = UserManager.GetUserCombinedPermissions(State.Username);
                        Connection.Send(Msg);

                        State.PermissionsNeedUpdate = false;
                    }

                    // Send user update of all peers that may have data they are after.
                    if (State.RelevantPeerAddressesNeedUpdate)
                    {
                        Logger.Log(LogLevel.Verbose, LogCategory.Peers, "----- Peer Relevant Addresses -----");
                        Logger.Log(LogLevel.Verbose, LogCategory.Peers, "Peer: {0}", State.PeerConnectionAddress == null ? "Unknown" : State.PeerConnectionAddress.ToString());

                        List<NetConnection> NewPeers = GetRelevantPeerForBlockState(Clients, Connection);

                        // If the new list is different than the old one send it.
                        bool IsDifferent = !NewPeers.IsEqual(State.RelevantPeers);

                        if (NewPeers.Count > 0)
                        {
                            for (int i = 0; i < NewPeers.Count; i++)
                            {
                                Logger.Log(LogLevel.Verbose, LogCategory.Peers, "[{0}] {1}", i, (NewPeers[i].Metadata as ServerConnectedClient).PeerConnectionAddress.ToString());
                            }
                        }
                        else
                        {
                            Logger.Log(LogLevel.Verbose, LogCategory.Peers, "\tNo Relevant Peers");
                        }

                        // Send it!
                        if (IsDifferent)
                        {
                            Logger.Log(LogLevel.Verbose, LogCategory.Peers, "Sending update as list changed.");

                            NetMessage_RelevantPeerListUpdate Msg = new NetMessage_RelevantPeerListUpdate();
                            Msg.PeerAddresses = new List<IPEndPoint>();
                            foreach (NetConnection Address in NewPeers)
                            {
                                Msg.PeerAddresses.Add((Address.Metadata as ServerConnectedClient).PeerConnectionAddress);
                            }
                            Connection.Send(Msg);
                        }
                        else
                        {
                            Logger.Log(LogLevel.Verbose, LogCategory.Peers, "Not sending update as peer lists are equal.");
                        }

                        State.RelevantPeers = NewPeers;
                        State.RelevantPeerAddressesNeedUpdate = false;
                    }
                }
            }

            EnforceBandwidthRestrictions(ActivelyDownloadingClients);

            ListenConnection.Poll();
        }

        /// <summary>
        /// 
        /// </summary>
        private void EnforceBandwidthRestrictions(List<NetConnection> ActivelyDownloadingClients)
        {
            // Build list of all routes that are currently active.
            Dictionary<RoutePair, List<NetConnection>> ActiveRouteDestinations = new Dictionary<RoutePair, List<NetConnection>>();

            foreach (NetConnection Connection in ActivelyDownloadingClients)
            {
                ServerConnectedClient State = Connection.Metadata as ServerConnectedClient;
                if (State != null)
                {
                    foreach (Guid SourceTag in Connection.Handshake.TagIds)
                    {
                        foreach (NetConnection RelevantPeer in State.RelevantPeers)
                        {
                            if (RelevantPeer.Handshake != null)
                            {
                                foreach (Guid DestinationTag in RelevantPeer.Handshake.TagIds)
                                {
                                    RoutePair Pair = new RoutePair { SourceTagId = SourceTag, DestinationTagId = DestinationTag };

                                    if (!ActiveRouteDestinations.ContainsKey(Pair))
                                    {
                                        ActiveRouteDestinations.Add(Pair, new List<NetConnection>());
                                    }

                                    if (!ActiveRouteDestinations[Pair].Contains(Connection))
                                    {
                                        ActiveRouteDestinations[Pair].Add(Connection);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // Calculate bandwidth limits per route.
            List<RoutePair> BandwidthLimits = new List<RoutePair>();
            foreach (var Pair in ActiveRouteDestinations)
            {
                // Try and find bandwidth restrictions for this route.
                Route route = RouteRegistry.GetRoute(Pair.Key.SourceTagId, Pair.Key.DestinationTagId);
                if (route != null && route.BandwidthLimit > 0)
                {
                    RoutePair NewPair = Pair.Key;
                    NewPair.Bandwidth = route.BandwidthLimit / Pair.Value.Count;

                    BandwidthLimits.Add(NewPair);
                }
            }

            // If bandwidth limits have changed, enforce them.
            bool ShouldSendUpdate = (BandwidthLimits.Count != ActiveBandwidthLimits.Count);
            if (!ShouldSendUpdate)
            {
                // Check each individual one.
                foreach (RoutePair Pair in BandwidthLimits)
                {
                    if (!ActiveBandwidthLimits.Contains(Pair))
                    {
                        ShouldSendUpdate = true;
                        break;
                    }
                }
            }

            // If there are new actively downloaded clients, enforce them again.
            if (!ShouldSendUpdate)
            {
                if (ActiveBandwidthLimitsSentToClients.Count != ActivelyDownloadingClients.Count)
                {
                    ShouldSendUpdate = true;
                }
                else
                {
                    foreach (NetConnection Connection in ActivelyDownloadingClients)
                    {
                        if (!ActiveBandwidthLimitsSentToClients.Contains(Connection))
                        {
                            ShouldSendUpdate = true;
                            break;
                        }
                    }
                }
            }

            long GlobalLimit = ActivelyDownloadingClients.Count == 0 ? 0 : BandwidthLimit / ActivelyDownloadingClients.Count;
            if (GlobalLimit != ActiveGlobalBandwidthLimit)
            {
                ShouldSendUpdate = true;
            }

            if (ShouldSendUpdate)
            {
                foreach (NetConnection Connection in ActivelyDownloadingClients)
                {
                    ServerConnectedClient State = Connection.Metadata as ServerConnectedClient;

                    NetMessage_EnforceBandwidthLimit Msg = new NetMessage_EnforceBandwidthLimit();
                    Msg.BandwidthLimitGlobal = GlobalLimit;
                    Msg.BandwidthLimitRoutes = BandwidthLimits;
                    Connection.Send(Msg);
                }

                Logger.Log(LogLevel.Info, LogCategory.Peers, "Globally limited bandwidth to {0}",
                    StringUtils.FormatAsTransferRate(GlobalLimit));

                foreach (RoutePair Pair in BandwidthLimits)
                {
                    Logger.Log(LogLevel.Info, LogCategory.Peers, "Limiting route {0} -> {1} to {2} per peer",
                        TagRegistry.GetTagById(Pair.SourceTagId).Name,
                        TagRegistry.GetTagById(Pair.DestinationTagId).Name,
                        StringUtils.FormatAsTransferRate(Pair.Bandwidth));
                }

                ActiveBandwidthLimitsSentToClients = ActivelyDownloadingClients;
                ActiveBandwidthLimits = BandwidthLimits;
                ActiveGlobalBandwidthLimit = GlobalLimit;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="Hostname"></param>
        /// <param name="Port"></param>
        public void Start(int Port, BuildManifestRegistry BuildManifest, UserManager InUserManager, LicenseManager InLicenseManager, TagRegistry InTagRegistry, RouteRegistry InRouteRegistry, RemoteActionServer InRemoteActionServer)
        {
            ServerPort = Port;
            Started = true;
            ManifestRegistry = BuildManifest;
            TagRegistry = InTagRegistry;
            RouteRegistry = InRouteRegistry;
            RemoteActionServer = InRemoteActionServer;

            LicenseManager = InLicenseManager;

            UserManager = InUserManager;
            UserManager.PermissionsUpdated += user =>
            {
                List<NetConnection> Clients = ListenConnection.AllClients;
                foreach (NetConnection Connection in Clients)
                {
                    if (Connection.Metadata != null)
                    {
                        ServerConnectedClient State = Connection.Metadata as ServerConnectedClient;
                        if (State.Username == user.Username)
                        {
                            State.PermissionsNeedUpdate = true;
                        }
                    }
                }
            };
        }

        /// <summary>
        /// </summary>
        /// <param name="Connection"></param>
        /// <param name="ClientConnection"></param>
        private void ClientConnected(NetConnection Connection, NetConnection ClientConnection)
        {
            ClientConnection.Metadata = new ServerConnectedClient();
        }

        /// <summary>
        /// </summary>
        /// <param name="Connection"></param>
        /// <param name="ClientConnection"></param>
        private void ClientDisconnected(NetConnection Connection, NetConnection ClientConnection)
        {
            UpdateGlobalBlockInformation();
        }

        /// <summary>
        /// </summary>
        /// <param name="Clients"></param>
        /// <param name="ForClient"></param>
        /// <returns></returns>
        private List<NetConnection> GetRelevantPeerForBlockState(List<NetConnection> Clients, NetConnection ForClient)
        {
            List<NetConnection> Result = new List<NetConnection>();

            ServerConnectedClient ForServerConnectedClient = ForClient.Metadata as ServerConnectedClient;
            if (ForServerConnectedClient.BlockState == null)
            {
                Logger.Log(LogLevel.Verbose, LogCategory.Peers, "No block state, cannot determine relevent peers.");
                return Result;
            }

            List<Guid> Whitelist = new List<Guid>();
            List<Guid> Blacklist = new List<Guid>();
            RouteRegistry.GetDestinationTags(ForClient.Handshake.TagIds, ref Whitelist, ref Blacklist);

            Logger.Log(LogLevel.Verbose, LogCategory.Peers, "GetRelevantPeerForBlockState: Clients={0}", Clients.Count);

            Logger.Log(LogLevel.Verbose, LogCategory.Peers, "Whitelist Tags:");
            foreach (Guid Id in Whitelist)
            {
                Logger.Log(LogLevel.Verbose, LogCategory.Peers, "\t{0}", TagRegistry.IdToString(Id));
            }

            Logger.Log(LogLevel.Verbose, LogCategory.Peers, "Blacklist Tags:");
            foreach (Guid Id in Blacklist)
            {
                Logger.Log(LogLevel.Verbose, LogCategory.Peers, "\t{0}", TagRegistry.IdToString(Id));
            }

            foreach (NetConnection Connection in Clients)
            {
                if (Connection == ForClient)
                {
                    continue;
                }

                if (Connection.Metadata == null)
                {
                    Logger.Log(LogLevel.Verbose, LogCategory.Peers, "Skipping peer, no connection metadata.");
                    continue;
                }

                ServerConnectedClient ServerConnectedClient = Connection.Metadata as ServerConnectedClient;
                if (ServerConnectedClient.PeerConnectionAddress == null)
                {
                    Logger.Log(LogLevel.Verbose, LogCategory.Peers, "Skipping peer, no connection address.");
                    continue;
                }

                if (ServerConnectedClient.BlockState == null)
                {
                    Logger.Log(LogLevel.Verbose, LogCategory.Peers, "Skipping Peer {0}, no block state.", ServerConnectedClient.PeerConnectionAddress.ToString());
                    continue;
                }

                if (RouteRegistry.Routes.Count > 0)
                {
                    Logger.Log(LogLevel.Verbose, LogCategory.Peers, "Potential Peer {0}.", ServerConnectedClient.PeerConnectionAddress.ToString());

                    bool Whitelisted = false;
                    bool Blacklisted = false;
                    foreach (Guid tag in Connection.Handshake.TagIds)
                    {
                        Logger.Log(LogLevel.Verbose, LogCategory.Peers, "\tTag: {0}", TagRegistry.IdToString(tag));
                        if (Whitelist.Contains(tag))
                        {
                            Whitelisted = true;
                        }
                        if (Blacklist.Contains(tag))
                        {
                            Blacklisted = true;
                        }
                    }

                    if (!Whitelisted)
                    {
                        Logger.Log(LogLevel.Verbose, LogCategory.Peers, "\tPeer {0} is not whitelisted.", ServerConnectedClient.PeerConnectionAddress.ToString());
                        continue;
                    }
                    if (Blacklisted)
                    {
                        Logger.Log(LogLevel.Verbose, LogCategory.Peers, "\tPeer {0} is blacklisted.", ServerConnectedClient.PeerConnectionAddress.ToString());
                        continue;
                    }
                }

                if (ServerConnectedClient.BlockState.HasAnyBlocksNeeded(ForServerConnectedClient.BlockState))
                {
                    Logger.Log(LogLevel.Verbose, LogCategory.Peers, "\tPeer {0} is ok, adding to list.", ServerConnectedClient.PeerConnectionAddress.ToString());
                    Result.Add(Connection);
                }
                else
                {
                    Logger.Log(LogLevel.Verbose, LogCategory.Peers, "\tPeer {0} does not have any blocks we need.", ServerConnectedClient.PeerConnectionAddress.ToString());
                }
            }

            return Result;
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateGlobalBlockInformation()
        {
            ClientManifestInfo.Clear();

            List<NetConnection> Clients = ListenConnection.AllClients;
            foreach (NetConnection ClientConnection in Clients)
            {
                if (ClientConnection.Metadata != null)
                {
                    ServerConnectedClient Sub = ClientConnection.Metadata as ServerConnectedClient;
                    if (Sub.BlockState != null)
                    {
                        foreach (ManifestBlockListState State in Sub.BlockState.States)
                        {
                            if (!ClientManifestInfo.ContainsKey(State.Id))
                            {
                                ClientManifestInfo.Add(State.Id, new ServerClientManifestInfo());
                            }

                            if (State.BlockState.AreAllSet(true))
                            {
                                ClientManifestInfo[State.Id].PeersWithAllBlocks++;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetPeerCountForManifest(Guid Id)
        {
            if (ClientManifestInfo.ContainsKey(Id))
            {
                return ClientManifestInfo[Id].PeersWithAllBlocks;
            }

            return 0;
        }

        /// <summary>
        /// </summary>
        /// <param name="Connection"></param>
        /// <param name="Message"></param>
        private void HandleMessage(NetConnection Connection, NetMessage BaseMessage)
        {
            // ------------------------------------------------------------------------------
            // Remote client requests all builds in a given path.
            // ------------------------------------------------------------------------------
            if (BaseMessage is NetMessage_GetBuilds)
            {
                NetMessage_GetBuilds Msg = BaseMessage as NetMessage_GetBuilds;

                Logger.Log(LogLevel.Verbose, LogCategory.Main, "Recieved request for builds in folder: {0}", Msg.RootPath);

                SendBuildsUpdate(Connection, Msg.RootPath);
            }

            // ------------------------------------------------------------------------------
            // Remote client requests all builds that match a specific criterial.
            // ------------------------------------------------------------------------------
            else if (BaseMessage is NetMessage_GetFilteredBuilds)
            {
                NetMessage_GetFilteredBuilds Msg = BaseMessage as NetMessage_GetFilteredBuilds;
                ServerConnectedClient State = Connection.Metadata as ServerConnectedClient;

                Logger.Log(LogLevel.Verbose, LogCategory.Main, "Recieved request for builds that match filter.");

                NetMessage_GetFilteredBuildsResponse ResponseMsg = new NetMessage_GetFilteredBuildsResponse();
                List<NetMessage_GetFilteredBuildsResponse.BuildInfo> Result = new List<NetMessage_GetFilteredBuildsResponse.BuildInfo>();

                foreach (BuildManifest Manifest in ManifestRegistry.Manifests)
                {
                    DateTime UpdateTime = Manifest.CreateTime;
                    if (Manifest.Metadata.ModifiedTime > UpdateTime)
                    {
                        UpdateTime = Manifest.Metadata.ModifiedTime;
                    }

                    if (UpdateTime < Msg.NewerThan)
                    {
                        continue;
                    }

                    if (Msg.SelectTags.Count > 0)
                    {
                        if (Manifest.Metadata == null || !Manifest.Metadata.TagIds.ContainsAny(Msg.SelectTags))
                        {
                            continue;
                        }
                    }

                    if (Msg.IgnoreTags.Count > 0)
                    {
                        if (Manifest.Metadata != null && Manifest.Metadata.TagIds.ContainsAny(Msg.IgnoreTags))
                        {
                            continue;
                        }
                    }

                    if (!UserManager.CheckPermission(State.Username, UserPermissionType.Read, Manifest.VirtualPath))
                    {
                        continue;
                    }

                    NetMessage_GetFilteredBuildsResponse.BuildInfo BuildInfo;
                    BuildInfo.Guid = Manifest.Guid;
                    BuildInfo.VirtualPath = Manifest.VirtualPath;
                    Result.Add(BuildInfo);
                }

                ResponseMsg.Builds = Result.ToArray();
                Connection.Send(ResponseMsg);
            }

            // ------------------------------------------------------------------------------
            // Remove client wants to publish a manifest.
            // ------------------------------------------------------------------------------
            else if (BaseMessage is NetMessage_PublishManifest)
            {
                NetMessage_PublishManifest Msg = BaseMessage as NetMessage_PublishManifest;
                ServerConnectedClient State = Connection.Metadata as ServerConnectedClient;

                NetMessage_PublishManifestResponse ResponseMsg = new NetMessage_PublishManifestResponse();
                ResponseMsg.Result = PublishManifestResult.Failed;

                Logger.Log(LogLevel.Info, LogCategory.Main, "Recieved request to publish manifest.");
                BuildManifest Manifest = null;
                try
                {
                    Manifest = BuildManifest.FromByteArray(Msg.Data);
                    Manifest.CreateTime = DateTime.UtcNow;
                    ResponseMsg.ManifestId = Msg.ManifestId;

                    if (!UserManager.CheckPermission(State.Username, UserPermissionType.Write, Manifest.VirtualPath))
                    {
                        ResponseMsg.Result = PublishManifestResult.PermissionDenied;
                    }
                    // Check something doesn't already exist at the path.
                    else if (ManifestRegistry.GetManifestByPath(Manifest.VirtualPath) != null)
                    {
                        ResponseMsg.Result = PublishManifestResult.VirtualPathAlreadyExists;
                    }
                    // Check guid doesn't exist.
                    else if (ManifestRegistry.GetManifestById(Manifest.Guid) != null)
                    {
                        ResponseMsg.Result = PublishManifestResult.GuidAlreadyExists;
                    }
                    // Good to go.
                    else
                    {
                        ManifestRegistry.RegisterManifest(Manifest);
                        ResponseMsg.Result = PublishManifestResult.Success;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log(LogLevel.Error, LogCategory.Main, "Failed to process publish request due to error: {0}", ex.Message);
                }

                Connection.Send(ResponseMsg);

                if (Manifest != null)
                {
                    SendBuildsUpdate(Connection, VirtualFileSystem.GetParentPath(Manifest.VirtualPath));
                }

                // Notify all clients a new build has been published.
                if (ResponseMsg.Result == PublishManifestResult.Success)
                {
                    NetMessage_BuildPublished PublishMsg = new NetMessage_BuildPublished();
                    PublishMsg.Path = Manifest.VirtualPath;
                    PublishMsg.ManifestId = Manifest.Guid;

                    List<NetConnection> Clients = ListenConnection.AllClients;
                    foreach (NetConnection ClientConnection in Clients)
                    {
                        if (ClientConnection.Metadata != null)
                        {
                            ServerConnectedClient Sub = ClientConnection.Metadata as ServerConnectedClient;
                            if (Sub.VersionNumeric >= 100000613)
                            {
                                if (UserManager.CheckPermission(Sub.Username, UserPermissionType.Read, Manifest.VirtualPath))
                                {
                                    ClientConnection.Send(PublishMsg);
                                }
                            }
                        }
                    }
                }
            }

            // ------------------------------------------------------------------------------
            // Block list update from peer.
            // ------------------------------------------------------------------------------
            else if (BaseMessage is NetMessage_BlockListUpdate)
            {
                NetMessage_BlockListUpdate Msg = BaseMessage as NetMessage_BlockListUpdate;

                ServerConnectedClient State = Connection.Metadata as ServerConnectedClient;

                if (State.BlockState != null)
                {
                    Logger.Log(LogLevel.Verbose, LogCategory.Main, "Recieved block list update from client, applying as patch ({0} states).", Msg.BlockState.States.Length);
                    State.BlockState = Msg.BlockState; // State.BlockState.ApplyDelta(Msg.BlockState);
                }
                else
                {
                    Logger.Log(LogLevel.Verbose, LogCategory.Main, "Recieved block list update from client, applying as new state ({0} states).", Msg.BlockState.States.Length);
                    State.BlockState = Msg.BlockState;
                }

                if (State.BlockState != null)
                {
                    foreach (ManifestBlockListState ManifestState in State.BlockState.States)
                    {
                        ManifestRegistry.MarkAsSeen(ManifestState.Id);
                    }
                }

                // Dirty all peer states to force an address update (todo: this is shit we should only update relevant peers).
                List<NetConnection> Clients = ListenConnection.AllClients;
                foreach (NetConnection ClientConnection in Clients)
                {
                    if (ClientConnection.Metadata != null)
                    {
                        ServerConnectedClient Sub = ClientConnection.Metadata as ServerConnectedClient;
                        Sub.RelevantPeerAddressesNeedUpdate = true;
                    }
                }

                UpdateGlobalBlockInformation();
                PrintClientBlockStates();
            }

            // ------------------------------------------------------------------------------
            // Client connection info update.
            // ------------------------------------------------------------------------------
            else if (BaseMessage is NetMessage_ConnectionInfo)
            {
                NetMessage_ConnectionInfo Msg = BaseMessage as NetMessage_ConnectionInfo;

                ServerConnectedClient State = Connection.Metadata as ServerConnectedClient;

                State.PeerConnectionAddress = Msg.PeerConnectionAddress;

                if (State.Username != Connection.Handshake.Username)
                {
                    State.Username = Connection.Handshake.Username;
                    State.PermissionsNeedUpdate = true;
                }

                if (State.MachineName != Connection.Handshake.MachineName)
                {
                    State.MachineName = Connection.Handshake.MachineName;
                    State.PermissionsNeedUpdate = true;
                }

                // Dirty all peer states to force an address update (todo: this is shit we should only update relevant peers).
                List<NetConnection> Clients = ListenConnection.AllClients;
                foreach (NetConnection ClientConnection in Clients)
                {
                    if (ClientConnection.Metadata != null)
                    {
                        ServerConnectedClient Sub = ClientConnection.Metadata as ServerConnectedClient;
                        Sub.RelevantPeerAddressesNeedUpdate = true;
                    }
                }

                Logger.Log(LogLevel.Info, LogCategory.Main, "Connection Info: Username=" + State.Username + " Hostname="+State.MachineName + " Address="+ State.PeerConnectionAddress);
            }

            // ------------------------------------------------------------------------------
            // Client requested a manifest for a download.
            // ------------------------------------------------------------------------------
            else if (BaseMessage is NetMessage_GetManifest)
            {
                NetMessage_GetManifest Msg = BaseMessage as NetMessage_GetManifest;

                Logger.Log(LogLevel.Info, LogCategory.Main, "Recieved request for manifest: {0}", Msg.ManifestId.ToString());

                try
                {
                    byte[] Manifest = ManifestRegistry.GetManifestBytesByid(Msg.ManifestId);
                    if (Manifest != null)
                    {
                        NetMessage_GetManifestResponse ResponseMsg = new NetMessage_GetManifestResponse();
                        ResponseMsg.ManifestId = Msg.ManifestId;
                        ResponseMsg.Data = Manifest;
                        Connection.Send(ResponseMsg);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log(LogLevel.Error, LogCategory.Main, "Failed to process manifest request due to error: {0}", ex.Message);
                }
            }

            // ------------------------------------------------------------------------------
            // Client requested a manifest is deleted.
            // ------------------------------------------------------------------------------
            else if (BaseMessage is NetMessage_DeleteManifest)
            {
                NetMessage_DeleteManifest Msg = BaseMessage as NetMessage_DeleteManifest;

                ServerConnectedClient State = Connection.Metadata as ServerConnectedClient;

                Logger.Log(LogLevel.Info, LogCategory.Main, "Recieved request for deleting manifest: {0}", Msg.ManifestId.ToString());

                try
                {
                    BuildManifest Manifest = ManifestRegistry.GetManifestById(Msg.ManifestId);
                    if (Manifest != null)
                    {
                        string Path = Manifest.VirtualPath;

                        if (!UserManager.CheckPermission(State.Username, UserPermissionType.Write, Path))
                        {
                            Logger.Log(LogLevel.Warning, LogCategory.Main, "User '{0}' tried to delete build without permission.", State.Username);
                            return;
                        }

                        ManifestRegistry.UnregisterManifest(Msg.ManifestId);

                        NetMessage_DeleteManifestResponse ResponseMsg = new NetMessage_DeleteManifestResponse();
                        ResponseMsg.ManifestId = Msg.ManifestId;
                        Connection.Send(ResponseMsg);

                        SendBuildsUpdate(Connection, VirtualFileSystem.GetParentPath(Path));
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log(LogLevel.Error, LogCategory.Main, "Failed to process manifest delete request due to error: {0}", ex.Message);
                }
            }

            // ------------------------------------------------------------------------------
            // Client requested list of users
            // ------------------------------------------------------------------------------
            else if (BaseMessage is NetMessage_GetUsers)
            {
                NetMessage_GetUsers Msg = BaseMessage as NetMessage_GetUsers;

                ServerConnectedClient State = Connection.Metadata as ServerConnectedClient;

                if (!UserManager.CheckPermission(State.Username, UserPermissionType.ModifyUsers, "") &&
                    !UserManager.HasAnyPermission(State.Username, UserPermissionType.AddUsersToGroup))
                {
                    Logger.Log(LogLevel.Warning, LogCategory.Main, "User '{0}' tried to get usernames without permission.", State.Username);
                    return;
                }

                NetMessage_GetUsersResponse ResponseMsg = new NetMessage_GetUsersResponse();
                ResponseMsg.Users = UserManager.Users;

                if (UserManager.CheckPermission(State.Username, UserPermissionType.ModifyUsers, ""))
                {
                    ResponseMsg.UserGroups = UserManager.UserGroups;
                }
                else
                {
                    foreach (UserGroup group in UserManager.UserGroups)
                    {
                        if (UserManager.CheckPermission(State.Username, UserPermissionType.AddUsersToGroup, group.Name))
                        {
                            ResponseMsg.UserGroups.Add(group);
                        }
                    }
                }

                Connection.Send(ResponseMsg);
            }

            // ------------------------------------------------------------------------------
            // Client requested list of routes
            // ------------------------------------------------------------------------------
            else if (BaseMessage is NetMessage_GetRoutes)
            {
                NetMessage_GetRoutes Msg = BaseMessage as NetMessage_GetRoutes;

                NetMessage_GetRoutesResponse ResponseMsg = new NetMessage_GetRoutesResponse();
                ResponseMsg.Routes = new List<Route>(RouteRegistry.Routes);

                Connection.Send(ResponseMsg);
            }

            // ------------------------------------------------------------------------------
            // Creates a route
            // ------------------------------------------------------------------------------
            else if (BaseMessage is NetMessage_CreateRoute)
            {
                NetMessage_CreateRoute Msg = BaseMessage as NetMessage_CreateRoute;

                ServerConnectedClient State = Connection.Metadata as ServerConnectedClient;

                if (!UserManager.CheckPermission(State.Username, UserPermissionType.ModifyRoutes, ""))
                {
                    Logger.Log(LogLevel.Warning, LogCategory.Main, "User '{0}' tried to add route without permission.", State.Username);
                    return;
                }

                RouteRegistry.CreateRoute(Msg.SourceTagId, Msg.DestinationTagId, Msg.Blacklisted, Msg.BandwidthLimit);

                // Dirty all relevant peer connections.
                List<NetConnection> Clients = ListenConnection.AllClients;
                foreach (NetConnection ClientConnection in Clients)
                {
                    if (ClientConnection.Metadata != null)
                    {
                        ServerConnectedClient Sub = ClientConnection.Metadata as ServerConnectedClient;
                        Sub.RelevantPeerAddressesNeedUpdate = true;
                    }
                }
                ActiveBandwidthLimitsSentToClients.Clear();

                Logger.Log(LogLevel.Warning, LogCategory.Main, "User '{0}' created route.", State.Username);
            }

            // ------------------------------------------------------------------------------
            // Deletes a route
            // ------------------------------------------------------------------------------
            else if (BaseMessage is NetMessage_DeleteRoute)
            {
                NetMessage_DeleteRoute Msg = BaseMessage as NetMessage_DeleteRoute;

                ServerConnectedClient State = Connection.Metadata as ServerConnectedClient;

                if (!UserManager.CheckPermission(State.Username, UserPermissionType.ModifyRoutes, ""))
                {
                    Logger.Log(LogLevel.Warning, LogCategory.Main, "User '{0}' tried to delete route without permission.", State.Username);
                    return;
                }

                RouteRegistry.DeleteRoute(Msg.RouteId);

                // Dirty all relevant peer connections.
                List<NetConnection> Clients = ListenConnection.AllClients;
                foreach (NetConnection ClientConnection in Clients)
                {
                    if (ClientConnection.Metadata != null)
                    {
                        ServerConnectedClient Sub = ClientConnection.Metadata as ServerConnectedClient;
                        Sub.RelevantPeerAddressesNeedUpdate = true;
                    }
                }
                ActiveBandwidthLimitsSentToClients.Clear();

                Logger.Log(LogLevel.Warning, LogCategory.Main, "User '{0}' delete route '{1}'.", State.Username, Msg.RouteId.ToString());
            }

            // ------------------------------------------------------------------------------
            // Update a route
            // ------------------------------------------------------------------------------
            else if (BaseMessage is NetMessage_UpdateRoute)
            {
                NetMessage_UpdateRoute Msg = BaseMessage as NetMessage_UpdateRoute;

                ServerConnectedClient State = Connection.Metadata as ServerConnectedClient;

                if (!UserManager.CheckPermission(State.Username, UserPermissionType.ModifyRoutes, ""))
                {
                    Logger.Log(LogLevel.Warning, LogCategory.Main, "User '{0}' tried to update route without permission.", State.Username);
                    return;
                }

                RouteRegistry.UpdateRoute(Msg.RouteId, Msg.SourceTagId, Msg.DestinationTagId, Msg.Blacklisted, Msg.BandwidthLimit);

                // Dirty all relevant peer connections.
                List<NetConnection> Clients = ListenConnection.AllClients;
                foreach (NetConnection ClientConnection in Clients)
                {
                    if (ClientConnection.Metadata != null)
                    {
                        ServerConnectedClient Sub = ClientConnection.Metadata as ServerConnectedClient;
                        Sub.RelevantPeerAddressesNeedUpdate = true;
                    }
                }
                ActiveBandwidthLimitsSentToClients.Clear();

                Logger.Log(LogLevel.Warning, LogCategory.Main, "User '{0}' update route '{1}'.", State.Username, Msg.RouteId.ToString());
            }

            // ------------------------------------------------------------------------------
            // Client requested list of tags
            // ------------------------------------------------------------------------------
            else if (BaseMessage is NetMessage_GetTags)
            {
                NetMessage_GetTags Msg = BaseMessage as NetMessage_GetTags;

                NetMessage_GetTagsResponse ResponseMsg = new NetMessage_GetTagsResponse();
                ResponseMsg.Tags = new List<Tag>(TagRegistry.Tags);

                Connection.Send(ResponseMsg);
            }

            // ------------------------------------------------------------------------------
            // Add a tag to a manifest.
            // ------------------------------------------------------------------------------
            else if (BaseMessage is NetMessage_AddTagToManifest)
            {
                NetMessage_AddTagToManifest Msg = BaseMessage as NetMessage_AddTagToManifest;

                ServerConnectedClient State = Connection.Metadata as ServerConnectedClient;

                BuildManifest Manifest = ManifestRegistry.GetManifestById(Msg.ManifestId);
                if (Manifest == null)
                {
                    Logger.Log(LogLevel.Warning, LogCategory.Main, "User '{0}' tried to tag an unknown build '{1}'.", State.Username, Msg.ManifestId.ToString());
                    return;
                }

                Tag Tag = TagRegistry.GetTagById(Msg.TagId);
                if (Tag == null)
                {
                    Logger.Log(LogLevel.Warning, LogCategory.Main, "User '{0}' tried to tag an unknown tag '{1}'.", State.Username, Msg.TagId.ToString());
                    return;
                }

                if (!UserManager.CheckPermission(State.Username, UserPermissionType.TagBuilds, Manifest.VirtualPath))
                {
                    Logger.Log(LogLevel.Warning, LogCategory.Main, "User '{0}' tried to tag builds without permission.", State.Username);
                    return;
                }

                ManifestRegistry.TagManifest(Msg.ManifestId, Msg.TagId);

                SendBuildsUpdate(Connection, VirtualFileSystem.GetParentPath(Manifest.VirtualPath));

                // Notify all clients a build has been updated.
                NetMessage_BuildUpdated UpdateMsg = new NetMessage_BuildUpdated();
                UpdateMsg.Path = Manifest.VirtualPath;
                UpdateMsg.ManifestId = Manifest.Guid;

                List<NetConnection> Clients = ListenConnection.AllClients;
                foreach (NetConnection ClientConnection in Clients)
                {
                    if (ClientConnection.Metadata != null)
                    {
                        ServerConnectedClient Sub = ClientConnection.Metadata as ServerConnectedClient;
                        if (Sub.VersionNumeric >= 100000613)
                        {
                            if (UserManager.CheckPermission(Sub.Username, UserPermissionType.Read, Manifest.VirtualPath))
                            {
                                ClientConnection.Send(UpdateMsg);
                            }
                        }
                    }
                }

                Logger.Log(LogLevel.Warning, LogCategory.Main, "User '{0}' tagged build '{1}' with tag '{2}'.", State.Username, Manifest.VirtualPath, Tag.Name);
            }

            // ------------------------------------------------------------------------------
            // Remove a tag to a manifest.
            // ------------------------------------------------------------------------------
            else if (BaseMessage is NetMessage_RemoveTagFromManifest)
            {
                NetMessage_RemoveTagFromManifest Msg = BaseMessage as NetMessage_RemoveTagFromManifest;

                ServerConnectedClient State = Connection.Metadata as ServerConnectedClient;

                BuildManifest Manifest = ManifestRegistry.GetManifestById(Msg.ManifestId);
                if (Manifest == null)
                {
                    Logger.Log(LogLevel.Warning, LogCategory.Main, "User '{0}' tried to untag an unknown build '{1}'.", State.Username, Msg.ManifestId.ToString());
                    return;
                }

                Tag Tag = TagRegistry.GetTagById(Msg.TagId);
                if (Tag == null)
                {
                    Logger.Log(LogLevel.Warning, LogCategory.Main, "User '{0}' tried to untag an unknown tag '{1}'.", State.Username, Msg.TagId.ToString());
                    return;
                }

                if (!UserManager.CheckPermission(State.Username, UserPermissionType.TagBuilds, Manifest.VirtualPath))
                {
                    Logger.Log(LogLevel.Warning, LogCategory.Main, "User '{0}' tried to untag builds without permission.", State.Username);
                    return;
                }

                ManifestRegistry.UntagManifest(Msg.ManifestId, Msg.TagId);

                SendBuildsUpdate(Connection, VirtualFileSystem.GetParentPath(Manifest.VirtualPath));

                // Notify all clients a build has been updated.
                NetMessage_BuildUpdated UpdateMsg = new NetMessage_BuildUpdated();
                UpdateMsg.Path = Manifest.VirtualPath;
                UpdateMsg.ManifestId = Manifest.Guid;

                List<NetConnection> Clients = ListenConnection.AllClients;
                foreach (NetConnection ClientConnection in Clients)
                {
                    if (ClientConnection.Metadata != null)
                    {
                        ServerConnectedClient Sub = ClientConnection.Metadata as ServerConnectedClient;
                        if (Sub.VersionNumeric >= 100000613)
                        {
                            if (UserManager.CheckPermission(Sub.Username, UserPermissionType.Read, Manifest.VirtualPath))
                            {
                                ClientConnection.Send(UpdateMsg);
                            }
                        }
                    }
                }

                Logger.Log(LogLevel.Warning, LogCategory.Main, "User '{0}' untagged build '{1}' with tag '{2}'.", State.Username, Manifest.VirtualPath, Tag.Name);
            }

            // ------------------------------------------------------------------------------
            // Creates a tag
            // ------------------------------------------------------------------------------
            else if (BaseMessage is NetMessage_CreateTag)
            {
                NetMessage_CreateTag Msg = BaseMessage as NetMessage_CreateTag;

                ServerConnectedClient State = Connection.Metadata as ServerConnectedClient;

                if (!UserManager.CheckPermission(State.Username, UserPermissionType.ModifyTags, ""))
                {
                    Logger.Log(LogLevel.Warning, LogCategory.Main, "User '{0}' tried to add tag without permission.", State.Username);
                    return;
                }

                TagRegistry.CreateTag(Msg.Name, Msg.Color, Msg.Unique, Msg.DecayTagId);

                Logger.Log(LogLevel.Warning, LogCategory.Main, "User '{0}' created tag '{1}'.", State.Username, Msg.Name);
            }

            // ------------------------------------------------------------------------------
            // Renames a tag
            // ------------------------------------------------------------------------------
            else if (BaseMessage is NetMessage_RenameTag)
            {
                NetMessage_RenameTag Msg = BaseMessage as NetMessage_RenameTag;

                ServerConnectedClient State = Connection.Metadata as ServerConnectedClient;

                if (!UserManager.CheckPermission(State.Username, UserPermissionType.ModifyTags, ""))
                {
                    Logger.Log(LogLevel.Warning, LogCategory.Main, "User '{0}' tried to rename tag without permission.", State.Username);
                    return;
                }

                TagRegistry.RenameTag(Msg.Id, Msg.Name, Msg.Color, Msg.Unique, Msg.DecayTagId);

                Logger.Log(LogLevel.Warning, LogCategory.Main, "User '{0}' renamed tag '{1}' to '{2}'.", State.Username, Msg.Id.ToString(), Msg.Name);
            }

            // ------------------------------------------------------------------------------
            // Deletes a tag
            // ------------------------------------------------------------------------------
            else if (BaseMessage is NetMessage_DeleteTag)
            {
                NetMessage_DeleteTag Msg = BaseMessage as NetMessage_DeleteTag;

                ServerConnectedClient State = Connection.Metadata as ServerConnectedClient;

                if (!UserManager.CheckPermission(State.Username, UserPermissionType.ModifyTags, ""))
                {
                    Logger.Log(LogLevel.Warning, LogCategory.Main, "User '{0}' tried to delete tag without permission.", State.Username);
                    return;
                }

                TagRegistry.DeleteTag(Msg.TagId);

                Logger.Log(LogLevel.Warning, LogCategory.Main, "User '{0}' deleted tag '{1}'.", State.Username, Msg.TagId.ToString());
            }

            // ------------------------------------------------------------------------------
            // Client requested to create a usergroup.
            // ------------------------------------------------------------------------------
            else if (BaseMessage is NetMessage_CreateUserGroup)
            {
                NetMessage_CreateUserGroup Msg = BaseMessage as NetMessage_CreateUserGroup;

                ServerConnectedClient State = Connection.Metadata as ServerConnectedClient;

                if (!UserManager.CheckPermission(State.Username, UserPermissionType.ModifyUsers, ""))
                {
                    Logger.Log(LogLevel.Warning, LogCategory.Main, "User '{0}' tried to set modify users without permission.", State.Username);
                    return;
                }

                Logger.Log(LogLevel.Info, LogCategory.Main, "Recieved request to create usergroup '{0}'", Msg.Name);

                UserManager.CreateGroup(Msg.Name);
            }

            // ------------------------------------------------------------------------------
            // Client requested to delete a usergroup.
            // ------------------------------------------------------------------------------
            else if (BaseMessage is NetMessage_CreateUserGroup)
            {
                NetMessage_DeleteUserGroup Msg = BaseMessage as NetMessage_DeleteUserGroup;

                ServerConnectedClient State = Connection.Metadata as ServerConnectedClient;

                if (!UserManager.CheckPermission(State.Username, UserPermissionType.ModifyUsers, ""))
                {
                    Logger.Log(LogLevel.Warning, LogCategory.Main, "User '{0}' tried to set modify users without permission.", State.Username);
                    return;
                }

                Logger.Log(LogLevel.Info, LogCategory.Main, "Recieved request to delete usergroup '{0}'", Msg.Name);

                UserManager.DeleteGroup(Msg.Name);
            }

            // ------------------------------------------------------------------------------
            // Client requested to add a given user to a user group.
            // ------------------------------------------------------------------------------
            else if (BaseMessage is NetMessage_AddUserToUserGroup)
            {
                NetMessage_AddUserToUserGroup Msg = BaseMessage as NetMessage_AddUserToUserGroup;

                ServerConnectedClient State = Connection.Metadata as ServerConnectedClient;

                if (!UserManager.CheckPermission(State.Username, UserPermissionType.ModifyUsers, "") &&
                    !UserManager.CheckPermission(State.Username, UserPermissionType.AddUsersToGroup, Msg.GroupName))
                {
                    Logger.Log(LogLevel.Warning, LogCategory.Main, "User '{0}' tried to set modify users without permission.", State.Username);
                    return;
                }

                Logger.Log(LogLevel.Info, LogCategory.Main, "Recieved request to add user '{0}' to group '{1}'", Msg.Username, Msg.GroupName);

                UserManager.AddUserToGroup(Msg.Username, Msg.GroupName);
            }

            // ------------------------------------------------------------------------------
            // Client requested to removed a given user from a user group.
            // ------------------------------------------------------------------------------
            else if (BaseMessage is NetMessage_RemoveUserFromUserGroup)
            {
                NetMessage_RemoveUserFromUserGroup Msg = BaseMessage as NetMessage_RemoveUserFromUserGroup;

                ServerConnectedClient State = Connection.Metadata as ServerConnectedClient;

                if (!UserManager.CheckPermission(State.Username, UserPermissionType.ModifyUsers, "") &&
                    !UserManager.CheckPermission(State.Username, UserPermissionType.AddUsersToGroup, Msg.GroupName))
                {
                    Logger.Log(LogLevel.Warning, LogCategory.Main, "User '{0}' tried to set modify users without permission.", State.Username);
                    return;
                }

                Logger.Log(LogLevel.Info, LogCategory.Main, "Recieved request to remove user '{0}' from group '{1}'", Msg.Username, Msg.GroupName);

                UserManager.RemoveUserFromGroup(Msg.Username, Msg.GroupName);
            }

            // ------------------------------------------------------------------------------
            // Client requested to add a given permission to the given group.
            // ------------------------------------------------------------------------------
            else if (BaseMessage is NetMessage_AddUserGroupPermission)
            {
                NetMessage_AddUserGroupPermission Msg = BaseMessage as NetMessage_AddUserGroupPermission;

                ServerConnectedClient State = Connection.Metadata as ServerConnectedClient;

                if (!UserManager.CheckPermission(State.Username, UserPermissionType.ModifyUsers, ""))
                {
                    Logger.Log(LogLevel.Warning, LogCategory.Main, "User '{0}' tried to set modify users without permission.", State.Username);
                    return;
                }

                Logger.Log(LogLevel.Info, LogCategory.Main, "Recieved request to add permission '{0}' to group '{1}'", Msg.PermissionType.ToString(), Msg.GroupName);

                UserManager.GrantPermission(Msg.GroupName, Msg.PermissionType, Msg.PermissionPath);
            }

            // ------------------------------------------------------------------------------
            // Client requested to remove a given permission from the given group.
            // ------------------------------------------------------------------------------
            else if (BaseMessage is NetMessage_RemoveUserGroupPermission)
            {
                NetMessage_RemoveUserGroupPermission Msg = BaseMessage as NetMessage_RemoveUserGroupPermission;

                ServerConnectedClient State = Connection.Metadata as ServerConnectedClient;

                if (!UserManager.CheckPermission(State.Username, UserPermissionType.ModifyUsers, ""))
                {
                    Logger.Log(LogLevel.Warning, LogCategory.Main, "User '{0}' tried to set modify users without permission.", State.Username);
                    return;
                }

                Logger.Log(LogLevel.Info, LogCategory.Main, "Recieved request to remove permission '{0}' from group '{1}'", Msg.PermissionType.ToString(), Msg.GroupName);

                UserManager.RevokePermission(Msg.GroupName, Msg.PermissionType, Msg.PermissionPath);
            }

            // ------------------------------------------------------------------------------
            // Client requested to delete a user
            // ------------------------------------------------------------------------------
            else if (BaseMessage is NetMessage_DeleteUser)
            {
                NetMessage_DeleteUser Msg = BaseMessage as NetMessage_DeleteUser;

                ServerConnectedClient State = Connection.Metadata as ServerConnectedClient;

                if (!UserManager.CheckPermission(State.Username, UserPermissionType.ModifyUsers, ""))
                {
                    Logger.Log(LogLevel.Warning, LogCategory.Main, "User '{0}' tried to delete user without permission.", State.Username);
                    return;
                }

                Logger.Log(LogLevel.Info, LogCategory.Main, "Recieved request to delete user '{0}'", Msg.Username);

                UserManager.DeleteUser(Msg.Username);
            }

            // ------------------------------------------------------------------------------
            // License info requested.
            // ------------------------------------------------------------------------------
            else if (BaseMessage is NetMessage_GetLicenseInfo)
            {
                Logger.Log(LogLevel.Info, LogCategory.Main, "Recieved request for license info.");

                NetMessage_GetLicenseInfoResponse ResponseMsg = new NetMessage_GetLicenseInfoResponse();
                ResponseMsg.License = LicenseManager.ActiveLicense;
                Connection.Send(ResponseMsg);
            }

            // ------------------------------------------------------------------------------
            // Applying license requested.
            // ------------------------------------------------------------------------------
            else if (BaseMessage is NetMessage_ApplyLicense)
            {
                NetMessage_ApplyLicense Msg = BaseMessage as NetMessage_ApplyLicense;

                Logger.Log(LogLevel.Info, LogCategory.Main, "Recieved request to apply license.");

                LicenseManager.Apply(Msg.License);

                NetMessage_GetLicenseInfoResponse ResponseMsg = new NetMessage_GetLicenseInfoResponse();
                ResponseMsg.License = LicenseManager.ActiveLicense;
                Connection.Send(ResponseMsg);
            }

            // ------------------------------------------------------------------------------
            // Server state requested
            // ------------------------------------------------------------------------------
            else if (BaseMessage is NetMessage_GetServerState)
            {
                ServerConnectedClient State = Connection.Metadata as ServerConnectedClient;

                if (!UserManager.CheckPermission(State.Username, UserPermissionType.ModifyServer, ""))
                {
                    Logger.Log(LogLevel.Warning, LogCategory.Main, "User '{0}' tried to manager server without permission.", State.Username);
                    return;
                }

                NetMessage_GetServerStateResponse ResponseMsg = new NetMessage_GetServerStateResponse();
                ResponseMsg.BandwidthLimit = BandwidthLimit;

                List<NetConnection> Clients = ListenConnection.AllClients;
                foreach (NetConnection ClientConnection in Clients)
                {
                    if (ClientConnection.Metadata != null)
                    {
                        ServerConnectedClient Sub = ClientConnection.Metadata as ServerConnectedClient;
                        if (Sub.PeerConnectionAddress != null)
                        {
                            NetMessage_GetServerStateResponse.ClientState NewState = new NetMessage_GetServerStateResponse.ClientState();
                            NewState.Username = Sub.Username;
                            NewState.MachineName = Sub.MachineName;
                            NewState.Address = Sub.PeerConnectionAddress.Address.ToString();
                            NewState.DownloadRate = Sub.DownloadRate;
                            NewState.UploadRate = Sub.UploadRate;
                            NewState.TotalDownloaded = Sub.TotalDownloaded;
                            NewState.TotalUploaded = Sub.TotalUploaded;
                            NewState.ConnectedPeerCount = Sub.ConnectedPeerCount;
                            NewState.DiskUsage = Sub.DiskUsage;
                            NewState.DiskQuota = Sub.DiskQuota;
                            NewState.Version = Sub.Version;
                            NewState.TagIds = ClientConnection.Handshake.TagIds;

                            ResponseMsg.ClientStates.Add(NewState);
                        }
                    }
                }

                foreach (RemoteActionServerState Action in RemoteActionServer.States)
                {
                    NetMessage_GetServerStateResponse.RemoteInstallState NewState = new NetMessage_GetServerStateResponse.RemoteInstallState();
                    NewState.Requester = Action.ForClient.Handshake.MachineName;
                    NewState.AssignedTo = Action.AllocatedClient == null ? "Pending" : Action.AllocatedClient.Handshake.MachineName;
                    NewState.Progress = Action.Progress;
                    NewState.ProgressText = Action.ProgressText;
                    NewState.Id = Action.Id;

                    ResponseMsg.InstallStates.Add(NewState);
                }

                Connection.Send(ResponseMsg);
            }

            // ------------------------------------------------------------------------------
            // Recieved client state update.
            // ------------------------------------------------------------------------------
            else if (BaseMessage is NetMessage_ClientStateUpdate)
            {
                NetMessage_ClientStateUpdate Msg = BaseMessage as NetMessage_ClientStateUpdate;

                ServerConnectedClient State = Connection.Metadata as ServerConnectedClient;

                State.TotalDownloaded = Msg.TotalDownloaded;
                State.TotalUploaded = Msg.TotalUploaded;
                State.DownloadRate = Msg.DownloadRate;
                State.UploadRate = Msg.UploadRate;
                State.ConnectedPeerCount = Msg.ConnectedPeerCount;
                State.DiskUsage = Msg.DiskUsage;
                State.DiskQuota = Msg.DiskQuota;
                State.Version = Msg.Version;
                State.VersionNumeric = StringUtils.ConvertSemanticVerisonNumber(State.Version);
                State.AllowRemoteActions = Msg.AllowRemoteActions;

                Connection.MessageVersion = State.VersionNumeric;
            }

            // ------------------------------------------------------------------------------
            // Changing server max bandwidth
            // ------------------------------------------------------------------------------
            else if (BaseMessage is NetMessage_SetServerMaxBandwidth)
            {
                ServerConnectedClient State = Connection.Metadata as ServerConnectedClient;

                if (!UserManager.CheckPermission(State.Username, UserPermissionType.ModifyServer, ""))
                {
                    Logger.Log(LogLevel.Warning, LogCategory.Main, "User '{0}' tried to manager server without permission.", State.Username);
                    return;
                }

                NetMessage_SetServerMaxBandwidth Msg = BaseMessage as NetMessage_SetServerMaxBandwidth;
                BandwidthLimit = Msg.BandwidthLimit;

                Logger.Log(LogLevel.Warning, LogCategory.Main, "User '{0}' changed global bandwidth limit to {1}.", State.Username, StringUtils.FormatAsTransferRate(Msg.BandwidthLimit));
            }

            // ------------------------------------------------------------------------------
            // Select a manifest for the client to delete.
            // ------------------------------------------------------------------------------
            else if (BaseMessage is NetMessage_ChooseDeletionCandidate)
            {
                ServerConnectedClient State = Connection.Metadata as ServerConnectedClient;

                NetMessage_ChooseDeletionCandidate Msg = BaseMessage as NetMessage_ChooseDeletionCandidate;

                // Sort first by number 
                List<ManifestDeletionCandidate> Candidates = new List<ManifestDeletionCandidate>();
                foreach (Guid Id in Msg.CandidateManifestIds)
                {
                    BuildManifest Manifest = ManifestRegistry.GetManifestById(Id);

                    ManifestDeletionCandidate Candidate;
                    Candidate.Id = Id;
                    Candidate.Path = Manifest != null ? Manifest.VirtualPath : "";
                    Candidate.ParentPath = VirtualFileSystem.GetParentPath(Candidate.Path);
                    Candidate.LastSeen = ManifestRegistry.GetLastSeenTime(Id); // This is always going to basically be this timestamp ... how is this helpful? 
                    Candidate.CreateTime = Manifest != null ? Manifest.CreateTime : DateTime.UtcNow;
                    Candidate.NumberOfPeersWithFullBuild = Math.Max(0, GetPeerCountForManifest(Id) - 1); // Don't take our peer into account.
                    Candidate.TagIds = Manifest != null && Manifest.Metadata != null ? Manifest.Metadata.TagIds : new List<Guid>();
                    Candidates.Add(Candidate);
                }

                if (Candidates.Count > 0)
                {
                    // Split into 3 lists, prioritize keep, normal, prioritize delete.
                    List<ManifestDeletionCandidate> PriorityKeep = new List<ManifestDeletionCandidate>();
                    List<ManifestDeletionCandidate> PriorityDelete = new List<ManifestDeletionCandidate>();
                    List<ManifestDeletionCandidate> NoPriority = new List<ManifestDeletionCandidate>();

                    foreach (ManifestDeletionCandidate Candidate in Candidates)
                    {
                        bool Keep = Candidate.TagIds.ContainsAny(Msg.PrioritizeKeepingTagIds);
                        bool Delete = Candidate.TagIds.ContainsAny(Msg.PrioritizeDeletingTagIds);

                        if (Keep)
                        {
                            PriorityKeep.Add(Candidate);
                        }
                        else if (Delete)
                        {
                            PriorityDelete.Add(Candidate);
                        }
                        else 
                        {
                            NoPriority.Add(Candidate);
                        }
                    }

                    // Sort each list independently.
                    SortDeletionCandidates(PriorityKeep, Msg.Heuristic);
                    SortDeletionCandidates(PriorityDelete, Msg.Heuristic);
                    SortDeletionCandidates(NoPriority, Msg.Heuristic);

                    // Combine into final list.
                    Candidates.Clear();
                    Candidates.AddRange(PriorityDelete);
                    Candidates.AddRange(NoPriority);
                    Candidates.AddRange(PriorityKeep);

                    Logger.Log(LogLevel.Info, LogCategory.Main, "User '{0}' asked us to select manifest to delete (using heuristic {1}), priority order:", State.Username, Msg.Heuristic.ToString());
                    foreach (ManifestDeletionCandidate Candidate in Candidates)
                    {
                        Logger.Log(LogLevel.Info, LogCategory.Main, "\tManifest Id={0} PeersWithFullBuild={1} LastSeen={2} CreateTime={3} PrioritizeKeep={4} PrioritizeDelete={5}", Candidate.Id, Candidate.NumberOfPeersWithFullBuild, Candidate.LastSeen.ToString(), Candidate.CreateTime.ToString(), PriorityKeep.Contains(Candidate), PriorityDelete.Contains(Candidate));
                    }

                    Guid ChosenCandidate = Candidates[0].Id;
                    
                    NetMessage_ChooseDeletionCandidateResponse Response = new NetMessage_ChooseDeletionCandidateResponse();
                    Response.ManifestId = ChosenCandidate;
                    Connection.Send(Response);
                }
            }

            // ------------------------------------------------------------------------------
            // Add a tag to a client
            // ------------------------------------------------------------------------------
            else if (BaseMessage is NetMessage_AddTagToClient)
            {
                NetMessage_AddTagToClient Msg = BaseMessage as NetMessage_AddTagToClient;

                ServerConnectedClient State = Connection.Metadata as ServerConnectedClient;

                Tag Tag = TagRegistry.GetTagById(Msg.TagId);
                if (Tag == null)
                {
                    Logger.Log(LogLevel.Warning, LogCategory.Main, "User '{0}' tried to untag an unknown tag '{1}'.", State.Username, Msg.TagId.ToString());
                    return;
                }

                if (!UserManager.CheckPermission(State.Username, UserPermissionType.ModifyServer, ""))
                {
                    Logger.Log(LogLevel.Warning, LogCategory.Main, "User '{0}' tried to tag client without permission.", State.Username);
                    return;
                }

                // Forward to client to untag themselves.
                List<NetConnection> Clients = ListenConnection.AllClients;
                foreach (NetConnection ClientConnection in Clients)
                {
                    if (ClientConnection.Metadata != null)
                    {
                        ServerConnectedClient Sub = ClientConnection.Metadata as ServerConnectedClient;
                        if (Sub.PeerConnectionAddress != null && Sub.PeerConnectionAddress.Address.ToString() == Msg.ClientAddress)
                        {
                            ClientConnection.Send(Msg);
                        }
                    }
                }

                Logger.Log(LogLevel.Warning, LogCategory.Main, "User '{0}' tagged client '{1}' with tag '{2}'.", State.Username, Msg.ClientAddress.ToString(), Msg.TagId.ToString());
            }

            // ------------------------------------------------------------------------------
            // Remove a tag to a manifest.
            // ------------------------------------------------------------------------------
            else if (BaseMessage is NetMessage_RemoveTagFromClient)
            {
                NetMessage_RemoveTagFromClient Msg = BaseMessage as NetMessage_RemoveTagFromClient;

                ServerConnectedClient State = Connection.Metadata as ServerConnectedClient;

                Tag Tag = TagRegistry.GetTagById(Msg.TagId);
                if (Tag == null)
                {
                    Logger.Log(LogLevel.Warning, LogCategory.Main, "User '{0}' tried to untag an unknown tag '{1}'.", State.Username, Msg.TagId.ToString());
                    return;
                }

                if (!UserManager.CheckPermission(State.Username, UserPermissionType.ModifyServer, ""))
                {
                    Logger.Log(LogLevel.Warning, LogCategory.Main, "User '{0}' tried to untag client without permission.", State.Username);
                    return;
                }

                // Forward to client to untag themselves.
                List<NetConnection> Clients = ListenConnection.AllClients;
                foreach (NetConnection ClientConnection in Clients)
                {
                    if (ClientConnection.Metadata != null)
                    {
                        ServerConnectedClient Sub = ClientConnection.Metadata as ServerConnectedClient;
                        if (Sub.PeerConnectionAddress != null && Sub.PeerConnectionAddress.Address.ToString() == Msg.ClientAddress)
                        {
                            ClientConnection.Send(Msg);
                        }
                    }
                }

                Logger.Log(LogLevel.Warning, LogCategory.Main, "User '{0}' untagged client '{1}' with tag '{2}'.", State.Username, Msg.ClientAddress.ToString(), Msg.TagId.ToString());
            }

            // ------------------------------------------------------------------------------
            // Request to start a remote action.
            // ------------------------------------------------------------------------------
            else if (BaseMessage is NetMessage_RequestRemoteAction)
            {
                NetMessage_RequestRemoteAction Msg = BaseMessage as NetMessage_RequestRemoteAction;

                OnRequestRemoteActionRecieved?.Invoke(Connection, Msg);
            }

            // ------------------------------------------------------------------------------
            // Request to cancel a remote action.
            // ------------------------------------------------------------------------------
            else if (BaseMessage is NetMessage_CancelRemoteAction)
            {
                NetMessage_CancelRemoteAction Msg = BaseMessage as NetMessage_CancelRemoteAction;

                OnCancelRemoteActionRecieved?.Invoke(Connection, Msg.ActionId);
            }

            // ------------------------------------------------------------------------------
            // Reply to solicitation for remote action.
            // ------------------------------------------------------------------------------
            else if (BaseMessage is NetMessage_SolicitAcceptRemoteAction)
            {
                NetMessage_SolicitAcceptRemoteAction Msg = BaseMessage as NetMessage_SolicitAcceptRemoteAction;

                OnSolicitAcceptRemoteActionRecieved?.Invoke(Connection, Msg.ActionId);
            }

            // ------------------------------------------------------------------------------
            // Remote action progress update
            // ------------------------------------------------------------------------------
            else if (BaseMessage is NetMessage_RemoteActionProgress)
            {
                NetMessage_RemoteActionProgress Msg = BaseMessage as NetMessage_RemoteActionProgress;

                OnRemoteActionProgressRecieved?.Invoke(Msg);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void SortDeletionCandidates(List<ManifestDeletionCandidate> Candidates, ManifestStorageHeuristic Heuristic)
        {
            switch (Heuristic)
            {
                case ManifestStorageHeuristic.Oldest:
                    {
                        Candidates.Sort((Item1, Item2) =>
                        {
                            return Item1.CreateTime.CompareTo(Item2.CreateTime);
                        });

                        break;
                    }
                case ManifestStorageHeuristic.LeastAvailable:
                    {
                        Candidates.Sort((Item1, Item2) =>
                        {
                            // Our aim is to maintain highest availability of all builds, to do that:
                            //      Compare first by number of peers, the ones with the most are the lowest priority.
                            //      If same number of peers, prioritize deletion of ones seen most recently.
                            if (Item1.NumberOfPeersWithFullBuild == Item2.NumberOfPeersWithFullBuild)
                            {
                                return -Item1.LastSeen.CompareTo(Item2.LastSeen);
                            }
                            else
                            {
                                return -Item1.NumberOfPeersWithFullBuild.CompareTo(Item2.NumberOfPeersWithFullBuild);
                            }
                        });

                        break;
                    }
                case ManifestStorageHeuristic.OldestInLargestContainer:
                    {
                        Dictionary<string, List<ManifestDeletionCandidate>> PoolTables = new Dictionary<string, List<ManifestDeletionCandidate>>();

                        foreach (ManifestDeletionCandidate Item in Candidates)
                        {
                            if (!PoolTables.ContainsKey(Item.ParentPath))
                            {
                                PoolTables.Add(Item.ParentPath, new List<ManifestDeletionCandidate>());
                            }
                            PoolTables[Item.ParentPath].Add(Item);
                        }

                        // Sort each list by time.
                        List<List<ManifestDeletionCandidate>> Pools = new List<List<ManifestDeletionCandidate>>();

                        foreach (var Pair in PoolTables)
                        {
                            Pair.Value.Sort((Item1, Item2) =>
                            {
                                return Item1.CreateTime.CompareTo(Item2.CreateTime);
                            });

                            Pools.Add(Pair.Value);
                        }

                        // Sort each pools from largest to smallest.
                        Pools.Sort((Item1, Item2) => 
                        {
                            return Item2.Count.CompareTo(Item1.Count);
                        });

                        // Generate candidate list.
                        Candidates.Clear();
                        foreach (var Pool in Pools)
                        {
                            foreach (var Candidate in Pool)
                            {
                                Pool.Add(Candidate);
                            }
                        }

                        break;
                    }
            }
        }

        /// <summary>
        /// </summary>
        private void ListenForClients()
        {
            ListenConnection.BeginListen(ServerPort);
            LastListenAttempt = TimeUtils.Ticks;

            NetDiscoveryServer.ResponseData.Name = Dns.GetHostName();
            NetDiscoveryServer.ResponseData.Address = ListenConnection.ListenAddress.Address.ToString();
            NetDiscoveryServer.ResponseData.Port = ListenConnection.ListenAddress.Port;
        }

        /// <summary>
        /// </summary>
        private void PrintClientBlockStates()
        {
            List<NetConnection> Clients = ListenConnection.AllClients;

            Logger.Log(LogLevel.Verbose, LogCategory.Peers, "=======================");
            foreach (NetConnection Connection in Clients)
            {
                if (Connection.Metadata == null)
                {
                    continue;
                }

                ServerConnectedClient ServerConnectedClient = Connection.Metadata as ServerConnectedClient;
                if (ServerConnectedClient.BlockState == null)
                {
                    continue;
                }

                Logger.Log(LogLevel.Verbose, LogCategory.Peers, "Peer[{0}]", ServerConnectedClient.PeerConnectionAddress == null ? "Unknown" : ServerConnectedClient.PeerConnectionAddress.ToString());
                for (int i = 0; i < ServerConnectedClient.BlockState.States.Length; i++)
                {
                    ManifestBlockListState State = ServerConnectedClient.BlockState.States[i];
                    Logger.Log(LogLevel.Verbose, LogCategory.Peers, "\tManifest[{0}] Id={1} Active={2}", i, State.Id.ToString(), State.IsActive);
                    if (State.BlockState.Ranges != null)
                    {
                        for (int j = 0; j < State.BlockState.Ranges.Count; j++)
                        {
                            Logger.Log(LogLevel.Verbose, LogCategory.Peers, "\t\tRegion[{0}] Start={1} End={2} Active={3}", j, State.BlockState.Ranges[j].Start, State.BlockState.Ranges[j].End, State.BlockState.Ranges[j].State);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="Root"></param>
        private void SendBuildsUpdate(NetConnection Connection, string RootPath)
        {
            ServerConnectedClient State = Connection.Metadata as ServerConnectedClient;

            List<string> Children = ManifestRegistry.GetVirtualPathChildren(RootPath);

            NetMessage_GetBuildsResponse ResponseMsg = new NetMessage_GetBuildsResponse();
            ResponseMsg.RootPath = RootPath;
            List<NetMessage_GetBuildsResponse.BuildInfo> Result = new List<NetMessage_GetBuildsResponse.BuildInfo>();

            // Folders first.
            int Index = 0;
            for (int i = 0; i < Children.Count; i++)
            {
                BuildManifest Manifest = ManifestRegistry.GetManifestByPath(Children[i]);
                if (Manifest == null)
                {
                    if (UserManager.CheckPermission(State.Username, UserPermissionType.Read, Children[i], false, true))
                    {
                        Result.Add(
                            new NetMessage_GetBuildsResponse.BuildInfo
                            {
                                Guid = Guid.Empty,
                                CreateTime = new DateTime(i), // We do this just to keep everything sorted correctly by date time.
                                VirtualPath = Children[i],
                                AvailablePeers = 0,
                                LastSeenOnPeer = DateTime.UtcNow,
                                TotalSize = 0,
                                Tags = new Tag[0]
                            }
                        ); 
                    }
                }
            }

            // Builds second.
            if (UserManager.CheckPermission(State.Username, UserPermissionType.Read, RootPath))
            {
                for (int i = 0; i < Children.Count; i++)
                {
                    BuildManifest Manifest = ManifestRegistry.GetManifestByPath(Children[i]);
                    if (Manifest != null)
                    {
                        List<Tag> Tags = new List<Tag>();
                        if (Manifest.Metadata != null)
                        {
                            for (int j = 0; j < Manifest.Metadata.TagIds.Count; j++)
                            {
                                Tag Tag = TagRegistry.GetTagById(Manifest.Metadata.TagIds[j]);
                                if (Tag != null)
                                {
                                    Tags.Add(Tag);
                                }
                            }
                        }

                        Result.Add(
                            new NetMessage_GetBuildsResponse.BuildInfo
                            {
                                Guid = Manifest.Guid,
                                CreateTime = Manifest.CreateTime,
                                VirtualPath = Children[i],
                                AvailablePeers = (ulong)GetPeerCountForManifest(Manifest.Guid),
                                LastSeenOnPeer = ManifestRegistry.GetLastSeenTime(Manifest.Guid),
                                TotalSize = (ulong)Manifest.GetTotalSize(),
                                Tags = Tags.ToArray()
                            }
                        );

                        Index++;
                    }
                }
            }

            ResponseMsg.Builds = Result.ToArray();
            Connection.Send(ResponseMsg);
        }
    }
}