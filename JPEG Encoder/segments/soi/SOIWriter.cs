namespace JPEG_Encoder.segments.soi
{
    public class SOIWriter : SegmentWriter
    {
        private const int SOI_MARKER = 0xFFD8;
        
        public SOIWriter(BitStreamPP bitStream) : base(bitStream)
        {
        }

        public override void WriteSegment()
        {
            _bitStream.WriteMarker(SOI_MARKER);
        }
    }
}