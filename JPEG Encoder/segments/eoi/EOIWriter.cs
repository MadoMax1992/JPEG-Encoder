using BitStreams;

namespace JPEG_Encoder.segments.eoi
{
    public class EOIWriter : SegmentWriter
    {
        private const ushort EOI_MARKER = 0xD9FF;

        public EOIWriter(BitStream bitStream) : base(bitStream)
        {
            
        }

        public override void WriteSegment()
        {
            _bitStream.WriteUInt16(EOI_MARKER);
        }
    }
}