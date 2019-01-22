using BitStreams;

namespace JPEG_Encoder.segments.soi
{
    public class SoiWriter : SegmentWriter
    {
        private const ushort SoiMarker = 0xD8FF;

        public SoiWriter(BitStream bitStream) : base(bitStream)
        {
        }

        public override void WriteSegment()
        {
            BitStream.WriteUInt16(SoiMarker);
        }
    }
}