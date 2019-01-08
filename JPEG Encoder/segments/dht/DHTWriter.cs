using System.Collections.Generic;
using BitStreams;

namespace JPEG_Encoder.segments.dht
{
    public class DHTWriter : SegmentWriter
    {
        private const ushort DHT_MARKER = 0xC4FF;

        private List<HuffmanTable> tables;

        public DHTWriter(BitStream bitStream, List<HuffmanTable> tables) : base(bitStream)
        {
            this.tables = tables;
        }

        public int GetLength()
        {
            int sum = 0;
            foreach (HuffmanTable table in tables)
            {
                sum += 17 + table.GetCodebookSize();
            }

            return(2 + sum);
        }

        public override void WriteSegment()
        {
            _bitStream.WriteUInt16(DHT_MARKER);
            int length = GetLength();
            _bitStream.WriteByte((byte) (length >> 8));
            _bitStream.WriteByte((byte) (length & 0xff));
            foreach (HuffmanTable table in tables)
            {
                table.Write(_bitStream);
            }
        }
    }
}