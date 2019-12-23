using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace BuildSync.Core.Networking
{
    public class NetMessage
    {
        public const int HeaderSize = 8;
        public const int MaxPayloadSize = 128 * 1024 * 1024;

        public int Id = 0;
        public int PayloadSize = 0;

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

        protected virtual void SerializePayload(NetMessageSerializer reader)
        {
            // Implement in derived class.
        }

        public static void LoadMessageTypes()
        {
            Type[] subTypes = (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                               from assemblyType in domainAssembly.GetTypes()
                               where typeof(NetMessage).IsAssignableFrom(assemblyType)
                               select assemblyType).ToArray();

            foreach (Type type in subTypes)
            {
                MessageTypes.Add(type.Name.GetHashCode(), type);
            }
        }

        public byte[] ToByteArray()
        {
            MemoryStream dataStream = new MemoryStream();
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

            return dataStream.ToArray();
        }

        public static NetMessage FromByteArray(byte[] Buffer)
        {
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

            Msg = Activator.CreateInstance(MessageTypes[Msg.Id]) as NetMessage;
            if (Msg == null)
            {
                return null;
            }

            using (MemoryStream dataStream = new MemoryStream(Buffer, HeaderSize, Buffer.Length - HeaderSize, false))
            {
                using (BinaryReader dataReader = new BinaryReader(dataStream))
                {
                    try
                    {
                        Msg.SerializePayload(new NetMessageSerializer(dataReader));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Failed to decode message, with error {0}", ex.Message);
                    }

                    return Msg;
                }
            }
        }
    }
}
