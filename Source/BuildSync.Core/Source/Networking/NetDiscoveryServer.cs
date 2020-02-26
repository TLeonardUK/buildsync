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
using System.Net;
using System.Net.Sockets;
using BuildSync.Core.Utils;

namespace BuildSync.Core.Networking
{
    /// <summary>
    /// </summary>
    public class NetDiscoveryServer
    {
        public NetDiscoveryData ResponseData;
        private UdpClient Client;

        /// <summary>
        /// 
        /// </summary>

        public NetDiscoveryServer()
        {
            Client = new UdpClient(NetDiscoveryClient.ProbePort);

            try
            {
                Client.BeginReceive(UdpPacketRecieved, null);
            }
            catch (Exception Ex)
            {
                Logger.Log(LogLevel.Info, LogCategory.IO, "Failed to begin recieving udp packets with error: {0}", Ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Close()
        {
            if (Client != null)
            {
                Client.Close();
                Client = null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        private void UdpPacketRecieved(IAsyncResult result)
        {
            if (Client == null)
            {
                return;
            }

            try
            {
                IPEndPoint SourceEndPoint = new IPEndPoint(IPAddress.Any, NetDiscoveryClient.ProbePort);
                byte[] Packet = Client.EndReceive(result, ref SourceEndPoint);

                NetDiscoveryProbe Data = FileUtils.ReadFromArray<NetDiscoveryProbe>(Packet);

                Logger.Log(LogLevel.Info, LogCategory.IO, "Recieved udp packet for network discovery from: {0}", SourceEndPoint.ToString());

                ResponseData.Version = AppVersion.VersionString;
                ResponseData.ProtocolVersion = AppVersion.ProtocolVersion;

                byte[] Response = FileUtils.WriteToArray(ResponseData);

                Client.Send(Response, Response.Length, new IPEndPoint(SourceEndPoint.Address, NetDiscoveryClient.ResponsePort));
            }
            catch (Exception Ex)
            {
                Logger.Log(LogLevel.Info, LogCategory.IO, "Failed to recieved network discovery udp packet with error: {0}", Ex.Message);
            }
            finally
            {
                if (Client != null)
                {
                    try
                    {
                        Client.BeginReceive(UdpPacketRecieved, null);
                    }
                    catch (Exception Ex)
                    {
                        Logger.Log(LogLevel.Info, LogCategory.IO, "Failed to begin recieving udp packets with error: {0}", Ex.Message);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        ~NetDiscoveryServer()
        {
            Close();
        }
    }
}