using System;
using System.Text;

namespace Cthulhu.Serialization
{
    public class MemoryReader
    {
        private readonly ReadOnlyMemory<byte> _memory;

        public int Position { get; set; }
        public int Length => _memory.Length;

        private ReadOnlySpan<byte> Here => _memory.Span.Slice(Position);

        public MemoryReader(ReadOnlyMemory<byte> memory) => _memory = memory;

        public byte ReadByte()
        {
            var result = _memory.Span[Position++];
            return result;
        }

        public bool ReadBoolean() => ReadByte() != 0;

        public short ReadInt16()
        {
            var result = BitConverter.ToInt16(Here);
            Position += 2;
            return result;
        }

        public int ReadInt32()
        {
            var result = BitConverter.ToInt32(Here);
            Position += 4;
            return result;
        }

        public uint ReadUInt32()
        {
            var result = BitConverter.ToUInt32(Here);
            Position += 4;
            return result;
        }

        public long ReadInt64()
        {
            var result = BitConverter.ToInt64(Here);
            Position += 8;
            return result;
        }

        public ulong ReadUInt64()
        {
            var result = BitConverter.ToUInt64(Here);
            Position += 8;
            return result;
        }

        public float ReadSingle()
        {
            var result = BitConverter.ToSingle(Here);
            Position += 4;
            return result;
        }

        public double ReadDouble()
        {
            var result = BitConverter.ToDouble(Here);
            Position += 8;
            return result;
        }

        public string ReadBString()
        {
            var span = Here;
            int length = span[0];
            var result = Encoding.UTF8.GetString(span.Slice(1, length));
            Position += length + 1;
            return result;
        }

        public Guid ReadGuid()
        {
            var result = new Guid(Here.Slice(0, 16));
            Position += 16;
            return result;
        }
    }
}