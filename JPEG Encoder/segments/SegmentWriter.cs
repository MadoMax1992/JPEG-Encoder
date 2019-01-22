using BitStreams;

namespace JPEG_Encoder.segments
{
    public abstract class SegmentWriter
    {
        protected readonly BitStream BitStream;

        protected SegmentWriter(BitStream bitStream)
        {
            BitStream = bitStream;
        }

        public abstract void WriteSegment();
    }
}