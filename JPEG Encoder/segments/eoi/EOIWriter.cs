namespace JPEG_Encoder.segments.eoi
{
    public class EOIWriter : SegmentWriter
    {
        private const int EOI_MARKER = 0xFFD9;

        public EOIWriter(BitStreamPP bitStream) : base(bitStream)
        {
            
        }

        public override void WriteSegment()
        {
            _bitStream.WriteMarker(EOI_MARKER);
        }
    }
}