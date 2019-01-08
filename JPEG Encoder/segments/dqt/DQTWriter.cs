using System.Collections;
using System.Collections.Generic;
using BitStreams;

namespace JPEG_Encoder.segments.dqt
{
    public class DQTWriter : SegmentWriter
    {
        private const ushort DQT_MARKER = 0xDBFF;
        private byte length;
        private List<QuantizationTable> tables;

        public DQTWriter(BitStream os) : base (os)
        {
            SetTables();
            length = (byte) (2 + tables.Count * 65);
        }

        private void SetTables()
        {
            tables = new List<QuantizationTable>();
            tables.Add(new QuantizationTable(0, QuantizationTable.QuantizationMatrixLuminance));
            tables.Add(new QuantizationTable(1, QuantizationTable.QuantizationMatrixChrominance));
        }

        public override void WriteSegment()
        {
            _bitStream.WriteUInt16(DQT_MARKER);
            _bitStream.WriteByte(0x00);
            _bitStream.WriteByte(length);
            foreach (QuantizationTable table in tables)
            {
                table.WriteTable(_bitStream);
            }
            
        }
    }
}