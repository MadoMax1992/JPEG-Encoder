using System.Collections.Generic;
using BitStreams;

namespace JPEG_Encoder.segments.dht
{
    public class DhtWriter : SegmentWriter
    {
        private const ushort DhtMarker = 0xC4FF;

        private readonly List<HuffmanTable> _tables;

        public DhtWriter(BitStream bitStream, List<HuffmanTable> tables) : base(bitStream)
        {
            _tables = tables;
        }

        public int GetLength()
        {
            int sum = 0;
            foreach (HuffmanTable table in _tables) sum += 17 + table.GetCodebookSize();

            return 2 + sum;
        }

        public override void WriteSegment()
        {
            BitStream.WriteUInt16(DhtMarker);
            int length = GetLength();
            BitStream.WriteByte((byte) (length >> 8));
            BitStream.WriteByte((byte) (length & 0xff));
            foreach (HuffmanTable table in _tables) table.Write(BitStream);
        }
    }
}