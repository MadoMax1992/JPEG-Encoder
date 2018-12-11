using System.Collections.Generic;

namespace JPEG_Encoder.segments.dht
{
    public class DHTWriter : SegmentWriter
    {
        private const int DHT_MARKER = 0xFFC4;

        private List<HuffmanTable> tables;

        public DHTWriter(BitStreamPP bitStream, List<HuffmanTable> tables) : base(bitStream)
        {
            this.tables = tables;
        }

        public int GetLength()
        {
            int sum = 0;
            foreach (HuffmanTable table in tables)
            {
                sum += (17 + table.GetCodebookSize());
            }

            return 2 + sum;
        }

        public override void WriteSegment()
        {
            _bitStream.WriteMarker(DHT_MARKER);
            _bitStream.WriteInt32(GetLength());
            foreach (HuffmanTable table in tables)
            {
                table.write(_bitStream);
            }
        }
    }
}