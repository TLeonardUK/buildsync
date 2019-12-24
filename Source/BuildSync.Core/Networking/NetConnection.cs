using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using BuildSync.Core.Networking.Messages;
using BuildSync.Core.Utils;

namespace BuildSync.Core.Networking
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="Connection"></param>
    /// <param name="Message"></param>
    public delegate void MessageRecievedHandler(NetConnection Connection, NetMessage Message);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="Connection"></param>
    public delegate void DisconnectHandler(NetConnection Connection);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="Connection"></param>
    public delegate void ConnectHandler(NetConnection Connection);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="Connection"></param>
    public delegate void ClientConnectHandler(NetConnection Connection, NetConnection ClientConnection);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="Connection"></param>
    public delegate void ClientDisconnectHandler(NetConnection Connection, NetConnection ClientConnection);

    /// <summary>
    /// 
    /// </summary>
    public class NetConnection
    {
        private Socket Socket;
        private bool Connecting = false;
        private bool Listening = false;
        private byte[] MessageBuffer = new byte[NetMessage.HeaderSize];
        private List<NetConnection> Clients = new List<NetConnection>();
        private Queue<byte[]> SendQueue = new Queue<byte[]>();
        private bool Sending = false;

        private ulong LastKeepAliveSendTime = 0;
        private const int KeepAliveInterval = 5 * 1000;

        private NetMessage_Handshake Handshake = null;

        private Queue<Action> EventQueue = new Queue<Action>();

        private int OutstandingAsyncCalls = 0;

        /// <summary>
        /// 
        /// </summary>
        public static BandwidthThrottler GlobalBandwidthThrottleIn = new BandwidthThrottler();

        /// <summary>
        /// 
        /// </summary>
        public static BandwidthThrottler GlobalBandwidthThrottleOut = new BandwidthThrottler();

        /// <summary>
        /// 
        /// </summary>
        public static BandwidthTracker GlobalBandwidthStats = new BandwidthTracker();

        /// <summary>
        /// 
        /// </summary>
        public BandwidthTracker BandwidthStats = new BandwidthTracker();

        /// <summary>
        /// 
        /// </summary>
        public object Metadata = null;

        /// <summary>
        /// 
        /// </summary>
        public const int ProtocolVersion = 1;

        /// <summary>
        /// 
        /// </summary>
        public IPEndPoint Address;

        /// <summary>
        /// 
        /// </summary>
        public IPEndPoint ListenAddress;

        /// <summary>
        /// 
        /// </summary>
        public event MessageRecievedHandler OnMessageRecieved;

        /// <summary>
        /// 
        /// </summary>
        public event DisconnectHandler OnDisconnect;

        /// <summary>
        /// 
        /// </summary>
        public event ConnectHandler OnConnect;

        /// <summary>
        /// 
        /// </summary>
        public event ClientConnectHandler OnClientConnect;

        /// <summary>
        /// 
        /// </summary>
        public event ClientDisconnectHandler OnClientDisconnect;

        /// <summary>
        /// 
        /// </summary>
        public event MessageRecievedHandler OnClientMessageRecieved;

        /// <summary>
        /// 
        /// </summary>
        public bool IsConnected
        {
            get { return Socket?.Connected ?? false; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsConnecting
        {
            get { return Connecting; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsListening
        {
            get { return Listening; }
        }

        /// <summary>
        /// 
        /// </summary>
        public List<NetConnection> AllClients
        {
            get
            {
                lock (Clients)
                {
                    return Clients;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int SendQueueSize
        {
            get
            {
                int Size = 0;
                lock (SendQueue)
                {
                    foreach (byte[] Data in SendQueue)
                    {
                        Size += Data.Length;
                    }
                }
                return Size;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public NetConnection()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public NetConnection(Socket InSocket)
        {
            Socket = InSocket;
            Address = (IPEndPoint)Socket.RemoteEndPoint;
        }

        /// <summary>
        /// 
        /// </summary>
        ~NetConnection()
        {
            Disconnect();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Disconnect()
        {
            lock (Clients)
            {
                foreach (NetConnection Client in Clients.ToArray())
                {
                    Client.Disconnect();
                }
            }

            if (Socket != null)
            {
                //Socket.Disconnect(true);
                Socket.Close();
            }

            lock (SendQueue)
            {
                SendQueue.Clear();
                Sending = false;
            }

            // Block while any outstanding async calls are waiting to finish.
            while (OutstandingAsyncCalls > 0)
            {
                Thread.Sleep(1);
            }
            
            Listening = false;
            Connecting = false;
            Handshake = null;

            lock (EventQueue)
            {
                EventQueue.Enqueue(() => { OnDisconnect?.Invoke(this); });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Poll()
        {
            lock (Clients)
            {
                foreach (NetConnection Client in Clients.ToArray())
                {
                    Client.Poll();
                }
            }

            if (IsConnected && !IsListening && Handshake != null)
            {
                ulong Elapsed = TimeUtils.Ticks - LastKeepAliveSendTime;
                if (Elapsed > KeepAliveInterval)
                {
                    Send(new NetMessage_KeepAlive());
                    LastKeepAliveSendTime = TimeUtils.Ticks;
                }
            }

            // Fire off all queued events.
            lock (EventQueue)
            {
                while (EventQueue.Count > 0)
                {
                    EventQueue.Dequeue().Invoke();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Port"></param>
        public void BeginListen(int Port, bool ReuseAddresses = true)
        {
            if (Socket != null)
            {
                Disconnect();
            }

            Address = new IPEndPoint(IPAddress.Any, Port);

            string Hostname = Dns.GetHostName();
            IPAddress[] HostAddresses = Dns.GetHostAddresses(Hostname);
            IPAddress Ipv4Address = IPAddress.Any;
            foreach (IPAddress Address in HostAddresses)
            {
                if (Address.AddressFamily == AddressFamily.InterNetwork)
                {
                    Ipv4Address = Address;
                    break;
                }
            }

            ListenAddress = new IPEndPoint(Ipv4Address, Port);

            Socket = new Socket(Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer, 16 * 1024 * 1024);
            Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendBuffer, 16 * 1024 * 1024);

            if (ReuseAddresses)
            {
                Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, false);
                Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            }

            Console.WriteLine("Listening on {0} (Connectable on {1})", Address.ToString(), ListenAddress.ToString());

            Listening = true;
            try
            {
                Interlocked.Increment(ref OutstandingAsyncCalls); // Socket.BeginAccept

                Socket.Bind(Address);
                Socket.Listen(128);

                AsyncCallback AcceptLambda = null;
                AcceptLambda = (IAsyncResult Result) =>
                {
                    try
                    {
                        Socket ClientSocket = Socket.EndAccept(Result);

                        ClientSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer, 16 * 1024 * 1024);
                        ClientSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendBuffer, 16 * 1024 * 1024);

                        Console.WriteLine("Client connected from {0}", ClientSocket.RemoteEndPoint.ToString());

                        NetConnection ClientConnection = new NetConnection(ClientSocket);

                        // Handle messages from this client.
                        MessageRecievedHandler MessageRecievedLambda = null;
                        MessageRecievedLambda = (NetConnection, Message) =>
                        {
                            lock (Clients)
                            {
                                lock (EventQueue)
                                {
                                    EventQueue.Enqueue(() => { OnClientMessageRecieved?.Invoke(ClientConnection, Message); });
                                }
                            }
                        };

                        // Handle disconnects for this client.
                        DisconnectHandler DisconnectLambda = null;
                        DisconnectLambda = (NetConnection) =>
                        {
                            Console.WriteLine("Client disconnected.");

                            lock (Clients)
                            {
                                lock (EventQueue)
                                {
                                    EventQueue.Enqueue(() => { OnClientDisconnect?.Invoke(this, ClientConnection); });
                                }

                                Clients.Remove(ClientConnection);

                                ClientConnection.OnDisconnect -= DisconnectLambda;
                                ClientConnection.OnMessageRecieved -= MessageRecievedLambda;
                            }
                        };

                        ClientConnection.OnMessageRecieved += MessageRecievedLambda;
                        ClientConnection.OnDisconnect += DisconnectLambda;

                        // Add to client list.
                        lock (Clients)
                        {
                            Clients.Add(ClientConnection);

                            lock (EventQueue)
                            {
                                EventQueue.Enqueue(() => { OnClientConnect?.Invoke(this, ClientConnection); });
                            }
                        }

                        ClientConnection.BeginClient();

                        Interlocked.Increment(ref OutstandingAsyncCalls); // Socket.BeginAccept
                        Socket.BeginAccept(AcceptLambda, this);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Failed to accept connection {0} with error {1}", Address.ToString(), ex.Message);
                        Listening = false;
                    }
                    finally
                    {
                        Interlocked.Decrement(ref OutstandingAsyncCalls); // Socket.BeginAccept
                    }
                };

                Socket.BeginAccept(AcceptLambda, this);
            }
            catch (Exception ex)
            {
                Interlocked.Decrement(ref OutstandingAsyncCalls);
                Console.WriteLine("Failed to listen on {0} with error {1}", Address.ToString(), ex.Message);
                Listening = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Hostname"></param>
        /// <param name="Port"></param>
        public void BeginConnect(string Hostname, int Port)
        {
            if (Socket != null)
            {
                Disconnect();
            }

            Connecting = true;

            try
            {
                Interlocked.Increment(ref OutstandingAsyncCalls); // Dns.BeginGetHostEntry
                Dns.BeginGetHostEntry(Hostname, DnsResult =>
                {
                    bool BegunSocketConnect = false;

                    try
                    {
                        IPHostEntry HostEntry = Dns.EndGetHostEntry(DnsResult);

                        foreach (IPAddress address in HostEntry.AddressList)
                        {
                            if (address.AddressFamily == AddressFamily.InterNetwork ||
                                address.AddressFamily == AddressFamily.InterNetworkV6)
                            {
                                Address = new IPEndPoint(address, Port);
                            }
                        }

                        Socket = new Socket(Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                        Console.WriteLine("Connecting to {0} ({1})", Address.ToString(), HostEntry.HostName);

                        Interlocked.Increment(ref OutstandingAsyncCalls); // Socket.BeginConnect
                        BegunSocketConnect = true;
                        Socket.BeginConnect(Address, Result =>
                        {
                            try
                            {
                                Socket.EndConnect(Result);

                                Console.WriteLine("Connected to {0}", Socket.RemoteEndPoint.ToString());

                                lock (EventQueue)
                                {
                                    EventQueue.Enqueue(() => { OnConnect?.Invoke(this); });
                                }

                                // Send handshake.
                                NetMessage_Handshake HandshakeMsg = new NetMessage_Handshake();
                                HandshakeMsg.Version = ProtocolVersion;
                                Send(HandshakeMsg);

                                BeginRecievingHeader();
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Failed to connect to {0} with error {1}", Address.ToString(), ex.Message);
                            }
                            finally
                            {
                                Interlocked.Decrement(ref OutstandingAsyncCalls); // Socket.BeginConnect
                                Connecting = false;
                            }
                        },
                        this);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Failed to connect to {0} with error {1}", Hostname, ex.Message);
                        Connecting = false;
                    }
                    finally
                    {
                        if (BegunSocketConnect)
                        {
                            Interlocked.Decrement(ref OutstandingAsyncCalls); // Socket.BeginConnect
                        }
                        Interlocked.Decrement(ref OutstandingAsyncCalls); // Dns.BeginGetHostEntry
                    }

                }, null);
            }
            catch (Exception ex)
            {
                Interlocked.Decrement(ref OutstandingAsyncCalls); // Dns.BeginGetHostEntry
                Console.WriteLine("Failed to connect to {0} with error {1}", Hostname, ex.Message);
                Connecting = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Message"></param>
        public void Send(NetMessage Message)
        {
            lock (SendQueue)
            {
                SendQueue.Enqueue(Message.ToByteArray());
            }

            BeginSend();
        }

        /// <summary>
        /// 
        /// </summary>
        private void BeginSend()
        {
            lock (SendQueue)
            {
                if (Sending || SendQueue.Count == 0)
                {
                    return;
                }

                byte[] Block = SendQueue.Dequeue();
                SendBlock(Block, 0, Block.Length);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void SendBlock(byte[] Block, int Offset, int Length)
        {
            Sending = true;
            try
            {
                Interlocked.Increment(ref OutstandingAsyncCalls); // Socket.BeginSend

                int BytesToSend = GlobalBandwidthThrottleOut.Throttle(Length);

                Socket.BeginSend(Block, Offset, BytesToSend, SocketFlags.None, Result =>
                {
                    lock (SendQueue)
                    {
                        try
                        {
                            int BytesSent = Socket.EndSend(Result);
                            BandwidthStats.BytesOut(BytesSent);
                            GlobalBandwidthStats.BytesOut(BytesSent);

                            // Partial send, finish the rest.
                            if (BytesSent < Length)
                            {
                                int NextChunkOffset = Offset + BytesSent;
                                int NextChunkSize = Length - BytesSent;
                                SendBlock(Block, NextChunkOffset, NextChunkSize);
                            }
                            else
                            {
                                Sending = false;
                                BeginSend();
                            }
                        }
                        catch (Exception ex)
                        {
                            Sending = false;

                            Console.WriteLine("Failed to send to client {0}, with error {1}", Address.ToString(), ex.Message);
                            Disconnect();
                        }
                        finally
                        {
                            Interlocked.Decrement(ref OutstandingAsyncCalls); // Socket.BeginSend
                        }
                    }
                }, this);
            }
            catch (Exception ex)
            {
                Interlocked.Decrement(ref OutstandingAsyncCalls); // Socket.BeginSend

                Sending = false;

                Console.WriteLine("Failed to begin sending to client {0} with error {1}", Address.ToString(), ex.Message);
                Disconnect();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void BeginClient()
        {
            // Send handshake to client.
            NetMessage_Handshake HandshakeMsg = new NetMessage_Handshake();
            HandshakeMsg.Version = ProtocolVersion;
            Send(HandshakeMsg);

            // Start recieving from client
            try
            {
                BeginRecievingHeader();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to begin reciving from client {0} with error {1}", Address.ToString(), ex.Message);
                Disconnect();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void BeginRecievingHeader(int Offset = 0, int Size = NetMessage.HeaderSize)
        {
            try
            {
                Interlocked.Increment(ref OutstandingAsyncCalls); // Socket.BeginReceive

                int BytesToRecv = GlobalBandwidthThrottleIn.Throttle(Size);

                Socket.BeginReceive(MessageBuffer, Offset, BytesToRecv, SocketFlags.None, Result =>
                {
                    try
                    {
                        int BytesRecieved = Socket.EndReceive(Result);

                        BandwidthStats.BytesIn(BytesRecieved);
                        GlobalBandwidthStats.BytesIn(BytesRecieved);

                        if (BytesRecieved < Size)
                        {
                            BeginRecievingHeader(Offset + BytesRecieved, Size - BytesRecieved);
                            return;
                        }

                        BeginRecievingPayload();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Failed to recieve header from {0}, with error {1}", Address.ToString(), ex.Message);
                        Disconnect();
                    }
                    finally
                    {
                        Interlocked.Decrement(ref OutstandingAsyncCalls); // Socket.BeginReceive
                    }
                }, this);
            }
            catch (Exception ex)
            {
                Interlocked.Decrement(ref OutstandingAsyncCalls); // Socket.BeginReceive

                Console.WriteLine("Failed to recieve header from {0}, with error {1}", Address.ToString(), ex.Message);
                Disconnect();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void BeginRecievingPayload()
        {
            NetMessage MessageHeader = new NetMessage();
            MessageHeader.ReadHeader(MessageBuffer);

            if (MessageHeader.PayloadSize > NetMessage.MaxPayloadSize)
            {
                Console.WriteLine("Recieved message with payload above max size {0}, disconnecting", MessageHeader.PayloadSize);
                Disconnect();
                return;
            }

            // Make sure buffer has enough space.
            if (MessageBuffer.Length < NetMessage.HeaderSize + MessageHeader.PayloadSize)
            {
                Array.Resize(ref MessageBuffer, NetMessage.HeaderSize + MessageHeader.PayloadSize);
            }

            // If no payload, we have the full message.
            if (MessageHeader.PayloadSize == 0)
            {
                ProcessMessage(MessageBuffer);

                BeginRecievingHeader();
            }
            else
            {
                BeginRecievingPayloadWithOffset(0, MessageHeader.PayloadSize);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Offset"></param>
        /// <param name="Size"></param>
        private void BeginRecievingPayloadWithOffset(int Offset, int Size)
        {
            try
            {
                Interlocked.Increment(ref OutstandingAsyncCalls); // Socket.BeginReceive

                int BytesToRecv = GlobalBandwidthThrottleIn.Throttle(Size);

                Socket.BeginReceive(MessageBuffer, NetMessage.HeaderSize + Offset, BytesToRecv, SocketFlags.None, Result =>
                {
                    try
                    {
                        int BytesRecieved = Socket.EndReceive(Result);

                        BandwidthStats.BytesIn(BytesRecieved);
                        GlobalBandwidthStats.BytesIn(BytesRecieved);

                        if (BytesRecieved < Size)
                        {
                            BeginRecievingPayloadWithOffset(Offset + BytesRecieved, Size - BytesRecieved);
                            return;
                        }

                        ProcessMessage(MessageBuffer);

                        BeginRecievingHeader();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Failed to recieve payload from {0}, with error {1}", Address.ToString(), ex.Message);
                        Disconnect();
                    }
                    finally
                    {
                        Interlocked.Decrement(ref OutstandingAsyncCalls); // Socket.BeginReceive
                    }
                }, this);
            }
            catch (Exception ex)
            {
                Interlocked.Decrement(ref OutstandingAsyncCalls); // Socket.BeginReceive
                Console.WriteLine("Failed to recieve payload from {0}, with error {1}", Address.ToString(), ex.Message);
                Disconnect();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void ProcessMessage(byte[] Buffer)
        {
            NetMessage Message = NetMessage.FromByteArray(Buffer);
            if (Message != null)
            {
                //Console.WriteLine("Recieved message of type {0} from {1}", Message.GetType().Name, Address.ToString());

                if (Message is NetMessage_Handshake)
                {
                    Handshake = Message as NetMessage_Handshake;
                    if (Handshake.Version < ProtocolVersion)
                    {
                        Console.WriteLine("Client has incompatible protocol version, disconnecting.", Address.ToString());
                        Disconnect();
                    }
                }
                else if (Handshake != null)
                {
                    lock (EventQueue)
                    {
                        EventQueue.Enqueue(() => { OnMessageRecieved?.Invoke(this, Message); });
                    }
                }
                else
                {
                    Console.WriteLine("Recieved message before recieving handshake, disconnecting.", Address.ToString());
                    Disconnect();
                }
            }
            else
            {
                Console.WriteLine("Failed to decode message, disconnecting.", Address.ToString());
                Disconnect();
            }
        }
    }
}
