using BitStreams;

namespace JPEG_Encoder.segments.eoi
{
    public class EoiWriter : SegmentWriter
    {
        private const ushort EoiMarker = 0xD9FF;

        public EoiWriter(BitStream bitStream) : base(bitStream)
        {
        }

        public override void WriteSegment()
        {
            BitStream.WriteUInt16(EoiMarker);
        }
    }
}