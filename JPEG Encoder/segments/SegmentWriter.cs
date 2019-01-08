using System.IO;
using BitStreams;

namespace JPEG_Encoder.segments
{
    public abstract class SegmentWriter
    {
        protected BitStream _bitStream;

        protected SegmentWriter(BitStream bitStream)
        {
            _bitStream = bitStream;
        }

        public abstract void WriteSegment();
    }
}