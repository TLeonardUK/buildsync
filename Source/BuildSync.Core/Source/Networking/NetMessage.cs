﻿/*
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

using BuildSync.Core.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace BuildSync.Core.Networking
{
    public class NetMessage
    {
        public const int HeaderSize = 8;
        public const int MaxPayloadSize = (100 * 1024 * 1024);

        public int Id = 0;
        public int PayloadSize = 0;

        public virtual bool DoesRecieverHandleCleanup
        {
            get
            {
                return false;
            }
        }

        private static Dictionary<int, Type> MessageTypes = new Dictionary<int, Type>();

        public void ReadHeader(byte[] Buffer)
        {
            Id = BitConverter.ToInt32(Buffer, 0);
            PayloadSize = BitConverter.ToInt32(Buffer, 4);
        }

        private void WriteHeader(BinaryWriter writer)
        {
            writer.Write(Id);
            writer.Write(PayloadSize);
        }

        internal virtual void Cleanup()
        {
            // Implement in derived class.
        }

        protected virtual void SerializePayload(NetMessageSerializer reader)
        {
            // Implement in derived class.
        }

        public static void LoadMessageTypes()
        {
            lock (MessageTypes)
            {
                if (MessageTypes.Count > 0)
                {
                    return;
                }
                MessageTypes.Clear();

                foreach (Assembly Assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    try
                    {
                        foreach (Type Type in Assembly.GetTypes())
                        {
                            if (typeof(NetMessage).IsAssignableFrom(Type))
                            {
                                MessageTypes.Add(Type.Name.GetHashCode(), Type);
                            }
                        }

                    }
                    catch (ReflectionTypeLoadException)
                    {
                        // Skip this assembly, for some reason any runtime generated (via dynamic complilation) assemblies
                        // cannot have their types examined.
                    }
                }
            }
        }

        public int ToByteArray(ref byte[] Output)
        {
            ExpandableMemoryStream dataStream = new ExpandableMemoryStream(Output);
            //MemoryStream dataStream = new MemoryStream(Output);
            BinaryWriter dataWriter = new BinaryWriter(dataStream);

            dataStream.Seek(HeaderSize, SeekOrigin.Begin);

            long PayloadStart = dataStream.Position;
            SerializePayload(new NetMessageSerializer(dataWriter));
            Id = GetType().Name.GetHashCode();
            PayloadSize = (int)(dataStream.Position - PayloadStart);

            dataStream.Seek(0, SeekOrigin.Begin);
            WriteHeader(dataWriter);

            dataWriter.Close();
            dataStream.Close();

            Output = dataStream.GetBuffer();
            return HeaderSize + PayloadSize;
        }

        public static NetMessage FromByteArray(byte[] Buffer, out bool WasMemoryAvailable)
        {
            WasMemoryAvailable = true;

            if (MessageTypes.Count == 0)
            {
                LoadMessageTypes();
            }

            NetMessage Msg = new NetMessage();
            Msg.ReadHeader(Buffer);

            if (!MessageTypes.ContainsKey(Msg.Id))
            {
                return null;
            }

            //Stopwatch stop1 = new Stopwatch();
            //stop1.Start();

            Msg = Activator.CreateInstance(MessageTypes[Msg.Id]) as NetMessage;
            if (Msg == null)
            {
                return null;
            }

            //stop1.Stop();
            //Console.WriteLine("  - new instance {0}", ((float)stop1.ElapsedTicks / (Stopwatch.Frequency / 1000.0)));
            //stop1.Restart();

            using (MemoryStream dataStream = new MemoryStream(Buffer, HeaderSize, Buffer.Length - HeaderSize, false))
            {
                using (BinaryReader dataReader = new BinaryReader(dataStream))
                {
                    //stop1.Stop();
                    //Console.WriteLine("  - new readers {0}", ((float)stop1.ElapsedTicks / (Stopwatch.Frequency / 1000.0)));

                    try
                    {
                        //Stopwatch stop = new Stopwatch();
                        //stop.Start();

                        NetMessageSerializer Serializer = new NetMessageSerializer(dataReader);
                        Msg.SerializePayload(Serializer);

                        if (Serializer.FailedOutOfMemory)
                        {
                            WasMemoryAvailable = false;
                            return null;
                        }

                        //stop.Stop();
                        //Console.WriteLine("  - deserialize {0}", ((float)stop.ElapsedTicks / (Stopwatch.Frequency / 1000.0)));
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(LogLevel.Error, LogCategory.Transport, "Failed to decode message, with error {0}", ex.Message);
                    }

                    return Msg;
                }
            }
        }
    }
}
