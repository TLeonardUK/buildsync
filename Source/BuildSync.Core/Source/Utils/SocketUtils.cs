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
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace BuildSync.Core.Utils
{
    /// <summary>
    /// 
    /// </summary>
    public struct TcpInfo
    {
        public UInt32 State;
        public UInt32 Mss;
        public UInt64 ConnectionTimeMs;
        public byte TimestampsEnabled;
        public UInt32 RttUs;
        public UInt32 MinRttUs;
        public UInt32 BytesInFlight;
        public UInt32 Cwnd;
        public UInt32 SndWnd;
        public UInt32 RcvWnd;
        public UInt32 RcvBuf;
        public UInt64 BytesOut;
        public UInt64 BytesIn;
        public UInt32 BytesReordered;
        public UInt32 BytesRetrans;
        public UInt32 FastRetrans;
        public UInt32 DupAcksIn;
        public UInt32 TimeoutEpisodes;
        public byte SynRetrans;
    }

    /// <summary>
    /// </summary>
    public static class SocketUtils
    {
        /// <summary>
        //      SIO_TCP_INFO as defined in winsdk-10/mstcpip.h
        /// </summary>
        readonly static int SIO_TCP_INFO = unchecked((int)0xD8000027);

        /// <summary>
        //      SIO_TCP_INFO as defined in winsdk-10/mstcpip.h
        /// </summary>
        readonly static int SIO_IDEAL_SEND_BACKLOG_QUERY = unchecked((int)0x4004747B);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static TcpInfo GetTcpInfo(Socket socket)
        {
            var outputArray = new byte[128];
            socket.IOControl(SIO_TCP_INFO, BitConverter.GetBytes(0), outputArray);

            GCHandle handle = GCHandle.Alloc(outputArray, GCHandleType.Pinned);
            TcpInfo tcpInfo = (TcpInfo)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(TcpInfo));
            handle.Free();

            return tcpInfo;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static int GetIdealSendBacklog(Socket socket)
        {
            var outputArray = new byte[4];
            socket.IOControl(SIO_IDEAL_SEND_BACKLOG_QUERY, null, outputArray);

            int SendBacklog = BitConverter.ToInt32(outputArray, 0);

            return SendBacklog;
        }
    }
}