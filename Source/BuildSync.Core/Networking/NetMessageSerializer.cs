using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace BuildSync.Core.Networking
{
    /// <summary>
    /// 
    /// </summary>
    public class NetMessageSerializer
    {
        private BinaryReader Reader;
        private BinaryWriter Writer;

        public bool IsLoading
        {
            get { return (Reader != null); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Reader"></param>
        public NetMessageSerializer(BinaryReader InReader)
        {
            Reader = InReader;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Writer"></param>
        public NetMessageSerializer(BinaryWriter InWriter)
        {
            Writer = InWriter;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Value"></param>
        public void SerializeEnum<T>(ref T Value) 
        {
            int Id = (int)(object)Value;
            if (IsLoading)
            {
                Id = Reader.ReadInt32();
            }
            else
            {
                Writer.Write(Id);
            }
            Value = (T)(object)Id;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Value"></param>
        public void Serialize(ref int Value)
        {
            if (IsLoading)
            {
                Value = Reader.ReadInt32();
            }
            else
            {
                Writer.Write(Value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Value"></param>
        public void Serialize(ref bool Value)
        {
            if (IsLoading)
            {
                Value = Reader.ReadBoolean();
            }
            else
            {
                Writer.Write(Value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Value"></param>
        public void Serialize(ref float Value)
        {
            if (IsLoading)
            {
                Value = Reader.ReadSingle();
            }
            else
            {
                Writer.Write(Value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Value"></param>
        public void Serialize(ref string Value)
        {
            if (IsLoading)
            {
                Value = Reader.ReadString();
            }
            else
            {
                Writer.Write(Value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Value"></param>
        public void Serialize(ref Guid Value)
        {
            byte[] GuidBytes = Value.ToByteArray();
            Serialize(ref GuidBytes);

            if (IsLoading)
            {
                Value = new Guid(GuidBytes);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Value"></param>
        public void Serialize(ref byte[] Value)
        {
            if (IsLoading)
            {
                int Length = Reader.ReadInt32();
                Value = new byte[Length];
                Reader.BaseStream.Read(Value, 0, Length);
                //Writer.Write(Value);
            }
            else
            {
                Writer.Write(Value.Length);
                Writer.BaseStream.Write(Value, 0, Value.Length);
//                Writer.Write(Value);
            }
        }
    }
}
