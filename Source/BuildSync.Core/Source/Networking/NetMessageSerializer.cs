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
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace BuildSync.Core.Networking
{
    /// <summary>
    /// 
    /// </summary>
    public class NetMessageSerializer
    {
        private BinaryReader Reader;
        private BinaryWriter Writer;

        public bool FailedOutOfMemory
        {
            get;
            set;
        } = false;

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
        public void Serialize(ref long Value)
        {
            if (IsLoading)
            {
                Value = Reader.ReadInt64();
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
        public void Serialize(ref ulong Value)
        {
            if (IsLoading)
            {
                Value = Reader.ReadUInt64();
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
        public void Serialize(ref DateTime Value)
        {
            if (IsLoading)
            {
                Value = new DateTime(Reader.ReadInt64());
            }
            else
            {
                Writer.Write(Value.Ticks);
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
            if (IsLoading)
            {
                byte[] GuidBytes = new byte[16];
                Reader.BaseStream.Read(GuidBytes, 0, 16);
                Value = new Guid(GuidBytes);
            }
            else
            {
                byte[] GuidBytes = Value.ToByteArray();
                Writer.BaseStream.Write(GuidBytes, 0, GuidBytes.Length);
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
                if (Length == -1)
                {
                    Value = null;
                }
                else
                {
                    Value = new byte[Length]; 
                    Reader.BaseStream.Read(Value, 0, Length);
                }
            }
            else
            {
                if (Value == null)
                {
                    Writer.Write(-1);
                }
                else
                { 
                    Writer.Write(Value.Length);
                    Writer.BaseStream.Write(Value, 0, Value.Length);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Value"></param>
        public bool Serialize(ref NetCachedArray Value, int Capacity, bool FailIfNoMemory = false)
        {
            if (IsLoading)
            {
                int Length = Reader.ReadInt32();
                if (Length == -1)
                {
                    Value.SetNull();
                }
                else
                {
                    if (!Value.Resize(Length, Capacity, FailIfNoMemory))
                    {
                        FailedOutOfMemory = true;
                        return false;
                    }
                    Reader.BaseStream.Read(Value.Data, 0, Length);
                }
            }
            else
            {
                if (Value.Data == null)
                {
                    Writer.Write(-1);
                }
                else
                {
                    Writer.Write(Value.Length);
                    Writer.BaseStream.Write(Value.Data, 0, Value.Length);
                }
            }

            return true;
        }
    }
}
