using System;
using System.Net;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using BuildSync.Core.Utils;
using BuildSync.Core.Manifests;
using BuildSync.Core.Downloads;
using BuildSync.Core.Networking;
using BuildSync.Core.Networking.Messages;
using BuildSync.Core.Utils;
using BuildSync.Core.Users;

namespace BuildSync.Core
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="Connection"></param>
    /// <param name="Message"></param>
    public delegate void BuildsRecievedHandler(string RootPath, NetMessage_GetBuildsResponse.BuildInfo[] Builds);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="Connection"></param>
    /// <param name="Message"></param>
    public delegate void PermissionsUpdatedHandler();

    /// <summary>
    /// 
    /// </summary>
    public delegate void ConenctedToServerHandler();

    /// <summary>
    /// 
    /// </summary>
    public delegate void LostConnectionToServerHandler();

    /// <summary>
    /// 
    /// </summary>
    public delegate void FailedToConnectToServerHandler();

    /// <summary>
    /// 
    /// </summary>
    public delegate void UserListRecievedHandler(List<User> Users);

    /// <summary>
    /// 
    /// </summary>
    public delegate void ManifestPublishResultRecievedHandler(Guid ManifestId, PublishManifestResult Result);

    /// <summary>
    /// 
    /// </summary>
    public delegate void ManifestDeleteResultRecievedHandler(Guid ManifestId);

    /// <summary>
    /// 
    /// </summary>
    public class BuildSyncClient
    {
        /// <summary>
        /// 
        /// </summary>
        public class Peer
        {
            /// <summary>
            /// 
            /// </summary>
            public IPEndPoint Address;

            /// <summary>
            /// 
            /// </summary>
            public NetConnection Connection = new NetConnection();

            /// <summary>
            /// 
            /// </summary>
            public ulong LastConnectionAttemptTime = 0;

            /// <summary>
            /// 
            /// </summary>
            public bool RemoteInitiated = false;

            /// <summary>
            /// 
            /// </summary>
            public bool WasConnected = false;

            /// <summary>
            /// 
            /// </summary>
            public BlockListState BlockState = new BlockListState();

            /// <summary>
            /// 
            /// </summary>
            public const int BlockDownloadTimeout = 30 * 1000;

            /// <summary>
            /// 
            /// </summary>
            public List<ManifestPendingDownloadBlock> ActiveBlockDownloads = new List<ManifestPendingDownloadBlock>();

            /// <summary>
            /// 
            /// </summary>
            public long ActiveBlockDownloadSize = 0;

            /// <summary>
            /// 
            /// </summary>
            public float BlockRecieveLatency = 0.0f;

            /// <summary>
            /// 
            /// </summary>
            public float AverageBlockSize = 0.0f;

            /// <summary>
            /// 
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
            /// 
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
            /// 
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
            /// 
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
            /// 
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
                                ulong Elapsed = TimeUtils.Ticks - Download.TimeStarted;
                                BlockRecieveLatency = (BlockRecieveLatency * 0.85f) + (Elapsed * 0.15f);
                                AverageBlockSize = (AverageBlockSize * 0.85f) + (Download.Size * 0.15f);

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
            /// 
            /// </summary>
            public void RemoveActiveBlockDownload(Guid ManifestId, int BlockIndex)
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
            /// 
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
        };

        /// <summary>
        /// 
        /// </summary>
        private NetConnection Connection = new NetConnection();

        /// <summary>
        /// 
        /// </summary>
        private NetConnection ListenConnection = new NetConnection();

        /// <summary>
        /// 
        /// </summary>
        private ulong LastConnectionAttempt = 0;

        /// <summary>
        /// 
        /// </summary>
        private const int ConnectionAttemptInterval = 10 * 1000;

        /// <summary>
        /// 
        /// </summary>
        public string ServerHostname;

        /// <summary>
        /// 
        /// </summary>
        private const int TargetMillisecondsOfDataInFlight = 500;

        /// <summary>
        /// 
        /// </summary>
        public int ServerPort = 0;

        /// <summary>
        /// 
        /// </summary>
        public int PeerListenPortRangeMin = 0;

        /// <summary>
        /// 
        /// </summary>
        public int PeerListenPortRangeMax = 0;

        /// <summary>
        /// 
        /// </summary>
        private int PortIndex = 0;

        /// <summary>
        /// 
        /// </summary>
        private ulong LastListenAttempt = 0;

        /// <summary>
        /// 
        /// </summary>
        private const int ListenAttemptInterval = 2 * 1000;

        /// <summary>
        /// 
        /// </summary>
        private const int IdealPeerCount = 20;

        /// <summary>
        /// 
        /// </summary>
        private bool ConnectionInfoUpdateRequired = false;

        /// <summary>
        /// 
        /// </summary>
        private bool Started = false;

        /// <summary>
        /// 
        /// </summary>
        private bool BlockListUpdatePending = false;

        /// <summary>
        /// 
        /// </summary>
        private bool ForceBlockListUpdate = false;

        /// <summary>
        /// 
        /// </summary>
        private int LastManifestStateDirtyCounter = 0;

        /// <summary>
        /// 
        /// </summary>
        private ulong LastBlockListUpdateTime = 0;

        /// <summary>
        /// 
        /// </summary>
        private const int BlockListUpdateInterval = 10 * 1000;

        /// <summary>
        /// 
        /// </summary>
        private int PeerCycleIndex = 0;

        /// <summary>
        /// 
        /// </summary>
        public HandshakeResultType HandshakeResult
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        private BlockListState LastBlockListState = null;

        /// <summary>
        /// 
        /// </summary>
        private BuildManifestRegistry ManifestRegistry = null;

        /// <summary>
        /// 
        /// </summary>
        private List<IPEndPoint> RelevantPeerAddresses;

        /// <summary>
        /// 
        /// </summary>
        private List<Peer> Peers = new List<Peer>();

        /// <summary>
        /// 
        /// </summary>
        private ManifestDownloadManager ManifestDownloadManager = null;

        /// <summary>
        /// 
        /// </summary>
        public Queue<Action> DeferredActions = new Queue<Action>();

        /// <summary>
        /// 
        /// </summary>
        public event BuildsRecievedHandler OnBuildsRecieved;

        /// <summary>
        /// 
        /// </summary>
        public event PermissionsUpdatedHandler OnPermissionsUpdated;

        /// <summary>
        /// 
        /// </summary>
        public event ManifestPublishResultRecievedHandler OnManifestPublishResultRecieved;

        /// <summary>
        /// 
        /// </summary>
        public event ManifestDeleteResultRecievedHandler OnManifestDeleteResultRecieved;

        /// <summary>
        /// 
        /// </summary>
        public event UserListRecievedHandler OnUserListRecieved;

        /// <summary>
        /// 
        /// </summary>
        public event ConenctedToServerHandler OnConnectedToServer;

        /// <summary>
        /// 
        /// </summary>
        public event LostConnectionToServerHandler OnLostConnectionToServer;

        /// <summary>
        /// 
        /// </summary>
        public event FailedToConnectToServerHandler OnFailedToConnectToServer;

        /// <summary>
        /// 
        /// </summary>
        public bool IsConnected
        {
            get { return Connection != null ? Connection.IsConnected : false; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsConnecting
        {
            get { return Connection != null ? Connection.IsConnecting : false; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsReadyForData
        {
            get { return Connection != null ? Connection.IsReadyForData : false; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool InternalConnectionsDisabled = false;

        /// <summary>
        /// 
        /// </summary>
        public UserPermissionCollection Permissions = new UserPermissionCollection();

        /// <summary>
        /// 
        /// </summary>
        public bool ConnectionsDisabled
        {
            get
            {
                return InternalConnectionsDisabled;
            }
            set
            {
                bool Changed = (InternalConnectionsDisabled != value);
                InternalConnectionsDisabled = value;
                if (InternalConnectionsDisabled && Changed)
                {
                    RestartConnections();
                }
            }
        }

        /// <summary>
        /// 
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
        /// 
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
        /// 
        /// </summary>
        private bool InternalTrafficEnabled = true;
        public bool TrafficEnabled
        {
            get { return InternalTrafficEnabled; }
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
        public BuildSyncClient()
        {
            Connection.OnMessageRecieved += HandleMessage;
            Connection.OnConnect += (NetConnection Connection) => { OnConnectedToServer?.Invoke(); };
            Connection.OnDisconnect += (NetConnection Connection) => { OnLostConnectionToServer?.Invoke(); };
            Connection.OnConnectFailed += (NetConnection Connection) => { OnFailedToConnectToServer?.Invoke(); HandshakeResult = HandshakeResultType.Unknown; };
            Connection.OnHandshakeResult += (NetConnection Connection, HandshakeResultType ResultType) => { HandshakeResult = ResultType; };

            ListenConnection.OnClientConnect += PeerConnected;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Disconnect()
        {
            Started = false;

            RestartConnections();
        }

        /// <summary>
        /// 
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
        /// 
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
            ManifestDownloadManager.OnManifestRequested += (Guid Id) => {
                RequestManifest(Id);
            };
            Started = true;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Poll()
        {
            if (Started)
            {
                // Reconnect?
                if (!Connection.IsConnected && !Connection.IsConnecting && !InternalConnectionsDisabled)
                {
                    ulong ElapsedTime = TimeUtils.Ticks - LastConnectionAttempt;
                    if (ElapsedTime > ConnectionAttemptInterval)
                    {
                        ConnectionInfoUpdateRequired = true;
                        ConnectToServer();
                    }
                }

                // Reattempt to create listen server?
                if (!ListenConnection.IsListening && !InternalConnectionsDisabled)
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
                if ((ElapsedTime > BlockListUpdateInterval && BlockListUpdatePending) || ForceBlockListUpdate)
                {
                    SendBlockListUpdate();

                    LastBlockListUpdateTime = TimeUtils.Ticks;
                    BlockListUpdatePending = false;
                    ForceBlockListUpdate = false;
                }
            }
            else
            {
                BlockListUpdatePending = true;
                LastBlockListUpdateTime = 0;
                LastBlockListState = null;
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

            UpdateBlockDownloads();

            // Execute all deferred actions.
            lock (DeferredActions)
            {
                while (DeferredActions.Count > 0)
                {
                    DeferredActions.Dequeue().Invoke();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void ConnectToServer()
        {
            Connection.BeginConnect(ServerHostname, ServerPort);

            LastConnectionAttempt = TimeUtils.Ticks;
        }

        /// <summary>
        /// 
        /// </summary>
        private void ListenForPeers()
        {
            int PortCount = (PeerListenPortRangeMax - PeerListenPortRangeMin) + 1;
            int Port = PeerListenPortRangeMin + (PortIndex++ % PortCount);
            ListenConnection.BeginListen(Port, false);

            LastListenAttempt = TimeUtils.Ticks;
        }

        /// <summary>
        /// 
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
        /// 
        /// </summary>
        private void UpdateBlockDownloads()
        {
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
                            continue;
                        }
                    }

                    if (AlreadyDownloading)
                    {
                        continue;
                    }

                    // Get block information so we know if its small enough to add to any peers queue.
                    long BlockOffset = 0;
                    long BlockSize = 0;
                    BuildManifestFileInfo FileInfo = null;

                    if (!ManifestDownloadManager.GetBlockInfo(Item.ManifestId, Item.BlockIndex, ref FileInfo, ref BlockOffset, ref BlockSize))
                    {
                        continue;
                    }

                    // Try and find a peer with space in its download queue for this item.
                    // Cycle peers to request from so we don't always hit the first if he has everything.
                    // TODO: If recent request failed, wait.
                    for (int j = 0; j < Peers.Count; j++)
                    {
                        Peer peer = Peers[(j + PeerCycleIndex) % Peers.Count];

                        // Calculate a rough idea for how many bytes we should have in flight at a given time.
                        long MaxInFlightBytes = 0;
                        if (peer.BlockRecieveLatency < 5 || peer.AverageBlockSize < 1024)
                        {
                            MaxInFlightBytes = 1 * 1024 * 1024;
                        }
                        else
                        {
                            MaxInFlightBytes = (long)((TargetMillisecondsOfDataInFlight / peer.BlockRecieveLatency) * peer.AverageBlockSize);
                        }
                        
                        //Console.WriteLine("Target:{0} ({1} mb) Actual:{2} ({3} mb) TotalDownloads:{4}", MaxInFlightBytes, MaxInFlightBytes / 1024.0f / 1024.0f, peer.ActiveBlockDownloadSize, peer.ActiveBlockDownloadSize / 1024.0f / 1024.0f, peer.ActiveBlockDownloads.Count);

                        // TODO: Take disk queue into account.

                        if ((peer.ActiveBlockDownloadSize + BlockSize <= MaxInFlightBytes || BlockSize > MaxInFlightBytes) && peer.HasBlock(Item))
                        {
                            NetMessage_GetBlock Msg = new NetMessage_GetBlock();
                            Msg.ManifestId = Item.ManifestId;
                            Msg.BlockIndex = Item.BlockIndex;
                            peer.Connection.Send(Msg);

                            Item.TimeStarted = TimeUtils.Ticks;
                            Item.Size = BlockSize;

                            peer.AddActiveBlockDownload(Item);

                            //Console.WriteLine("Adding download (for block {0} in manifest {1}) of size {2} total queued {3}.", Msg.ManifestId, Msg.BlockIndex, BlockSize, peer.ActiveBlockDownloadSize);

                            break;
                        }
                    }

                    if (Peers.Count > 0)
                    {
                        PeerCycleIndex = (++PeerCycleIndex % Peers.Count);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="EndPoint"></param>
        /// <returns></returns>
        private Peer GetPeerByAddress(IPEndPoint EndPoint)
        {
            lock (Peers)
            {
                foreach (Peer peer in Peers)
                {
                    if (peer.Address.Equals(EndPoint))
                    {
                        return peer;
                    }
                }
            }           
            return null;
        }

        /// <summary>
        /// 
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
        /// 
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
                        if (peer == null)
                        {
                            peer = new Peer();
                            peer.Address = EndPoint;
                            peer.RemoteInitiated = false;
                            peer.LastConnectionAttemptTime = 0;
                            peer.Connection.OnMessageRecieved += HandleMessage;
                            Peers.Add(peer);
                        }
                    }

                    // Attempt connection if time has elapsed.
                    if (!peer.Connection.IsConnected && !peer.Connection.IsConnecting && !peer.RemoteInitiated)
                    {
                        ulong Elapsed = TimeUtils.Ticks - peer.LastConnectionAttemptTime;
                        if (peer.LastConnectionAttemptTime == 0 || Elapsed > ConnectionAttemptInterval)
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

                        continue;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void SendBlockListUpdate()
        {
            Logger.Log(LogLevel.Info, LogCategory.Main, "Sending block list update.");

            BlockListState State = ManifestDownloadManager.GetBlockListState();

            NetMessage_BlockListUpdate Msg = new NetMessage_BlockListUpdate();
            Msg.BlockState = State;

            // Send to server and each peer.
            foreach (Peer peer in Peers)
            {
                if (peer.Connection.IsConnected)
                {
                    Logger.Log(LogLevel.Info, LogCategory.Main, "\tSent to peer: " + peer.Connection.Address.ToString());
                    peer.Connection.Send(Msg);
                }
            }

            Logger.Log(LogLevel.Info, LogCategory.Main, "\tSent to server.");
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
        /// 
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
        /// 
        /// </summary>
        /// <param name="Path"></param>
        public bool SetUserPermissions(string Username, UserPermissionCollection Permissions)
        {
            if (!Connection.IsReadyForData)
            {
                Logger.Log(LogLevel.Warning, LogCategory.Main, "Failed to set user permissions, no connection to server?");
                return false;
            }

            NetMessage_SetUserPermissions Msg = new NetMessage_SetUserPermissions();
            Msg.Username = Username;
            Msg.Permissions = Permissions;
            Connection.Send(Msg);

            return true;
        }

        /// <summary>
        /// 
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
        /// 
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
        /// 
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
        /// 
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
        /// 
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
        /// 
        /// </summary>
        /// <param name="Connection"></param>
        /// <param name="ClientConnection"></param>
        private void PeerConnected(NetConnection Connection, NetConnection ClientConnection)
        {
            Peer peer = null;
            lock (Peers)
            {
                peer = GetPeerByAddress(ClientConnection.Address);
                if (peer == null)
                {
                    Logger.Log(LogLevel.Info, LogCategory.Peers, "Peer connected from {0}.", ClientConnection.Address.ToString());

                    peer = new Peer();
                    peer.Address = ClientConnection.Address;
                    peer.Connection = ClientConnection;
                    peer.Connection.OnMessageRecieved += HandleMessage;
                    peer.RemoteInitiated = true;
                    peer.LastConnectionAttemptTime = 0;
                    Peers.Add(peer);
                }
            }

            // Exchange block list with remote.
            BlockListUpdatePending = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Connection"></param>
        /// <param name="Message"></param>
        private void HandleMessage(NetConnection Connection, NetMessage BaseMessage)
        {
            // Server has sent us back a list of builds from a previous request.
            if (BaseMessage is NetMessage_GetBuildsResponse)
            {
                NetMessage_GetBuildsResponse Msg = BaseMessage as NetMessage_GetBuildsResponse;

                Logger.Log(LogLevel.Info, LogCategory.Main, "Recieved builds in folder: {0}", Msg.RootPath);

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

                Permissions = Msg.Permissions;
                OnPermissionsUpdated?.Invoke();
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

                Logger.Log(LogLevel.Info, LogCategory.Main, "Recieved block list update from: {0}", Connection.Address.ToString());

                Peer peer = GetPeerByConnection(Connection);
                if (peer != null)
                {
                    peer.BlockState = Msg.BlockState;
                }

                UpdateAvailableBlocks();
            }

            // Peer requested a block of data from us.
            else if (BaseMessage is NetMessage_GetBlock)
            {
                NetMessage_GetBlock Msg = BaseMessage as NetMessage_GetBlock;

                //Logger.Log(LogLevel.Info, LogCategory.Main, "Recieved request for block {0} in manifest {1} from {2}", Msg.BlockIndex, Msg.ManifestId.ToString(), Connection.Address.ToString());

                NetMessage_GetBlockResponse Response = new NetMessage_GetBlockResponse();
                Response.ManifestId = Msg.ManifestId;
                Response.BlockIndex = Msg.BlockIndex;

                ManifestDownloadManager.GetBlockData(Msg.ManifestId, Msg.BlockIndex, ref Response.Data, (bool bSuccess) =>
                {

                    lock (DeferredActions)
                    {
                        DeferredActions.Enqueue(() =>
                        {
                            if (!bSuccess)
                            {
                                ManifestDownloadManager.MarkBlockAsUnavailable(Msg.ManifestId, Msg.BlockIndex);
                                Response.Data.SetNull();
                            }

                            if (Connection.IsConnected)
                            {
                                Connection.Send(Response);
                            }

                            Response.Data.SetNull(); // Free data it's been serialized by this point.

                        });
                    }

                });
            }

            // Peer provided block of data we requested.
            else if (BaseMessage is NetMessage_GetBlockResponse)
            {
                NetMessage_GetBlockResponse Msg = BaseMessage as NetMessage_GetBlockResponse;

                if (Msg.Data == null)
                {
                    Logger.Log(LogLevel.Info, LogCategory.Main, "Recieved null block {0} in manifest {1} from {2}, looks like peer no longer has it.", Msg.BlockIndex, Msg.ManifestId.ToString(), Connection.Address.ToString());

                    lock (DeferredActions)
                    {
                        DeferredActions.Enqueue(() =>
                        {
                            Peer peer = GetPeerByConnection(Connection);
                            if (peer != null)
                            {
                                peer.SetBlockState(Msg.ManifestId, Msg.BlockIndex, false);
                            }
                        });
                    }

                    Msg.Cleanup();

                    return;
                }
                else
                {
                    //Logger.Log(LogLevel.Info, LogCategory.Main, "Recieved block {0} in manifest {1} from {2}", Msg.BlockIndex, Msg.ManifestId.ToString(), Connection.Address.ToString());
                    Peer peer = GetPeerByConnection(Connection);
                    if (peer != null)
                    {
                        peer.MarkActiveBlockDownloadAsRecieved(Msg.ManifestId, Msg.BlockIndex);
                    }

                    ManifestDownloadManager.SetBlockData(Msg.ManifestId, Msg.BlockIndex, Msg.Data, (bool bSuccess) =>
                    {
                        Msg.Cleanup();

                        lock (DeferredActions)
                        {
                            DeferredActions.Enqueue(() =>
                            {
                                // Mark block as complete.
                                if (bSuccess)
                                {
                                    ManifestDownloadManager.MarkBlockAsComplete(Msg.ManifestId, Msg.BlockIndex);
                                }

                                // Remove active download marker for this block.
                                peer.RemoveActiveBlockDownload(Msg.ManifestId, Msg.BlockIndex);
                            });
                        }

                    });
                }
            }

            // Recieved user list.
            else if (BaseMessage is NetMessage_GetUsersResponse)
            {
                NetMessage_GetUsersResponse Msg = BaseMessage as NetMessage_GetUsersResponse;

                Logger.Log(LogLevel.Info, LogCategory.Main, "Recieved users list with {0} users.", Msg.Users.Count);

                OnUserListRecieved?.Invoke(Msg.Users);
            }
        }
    }
}
