using System.Collections.Generic;
using BitStreams;

namespace JPEG_Encoder.segments.dqt
{
    public class DQTWriter : SegmentWriter
    {
        private const ushort DQTMarker = 0xDBFF;
        private readonly byte _length;
        private List<QuantizationTable> _tables;

        public DQTWriter(BitStream os) : base(os)
        {
            SetTables();
            _length = (byte) (2 + _tables.Count * 65);
        }

        private void SetTables()
        {
            _tables = new List<QuantizationTable>();
            _tables.Add(new QuantizationTable(0, QuantizationTable.QuantizationMatrixLuminance));
            _tables.Add(new QuantizationTable(1, QuantizationTable.QuantizationMatrixChrominance));
        }

        public override void WriteSegment()
        {
            BitStream.WriteUInt16(DQTMarker);
            BitStream.WriteByte(0x00);
            BitStream.WriteByte(_length);
            foreach (QuantizationTable table in _tables) table.WriteTable(BitStream);
        }
    }
}