using System.IO;
using BitStreams;

namespace JPEG_Encoder.segments
{
    public abstract class SegmentWriter
    {
        protected BitStreamPP _bitStream;

        protected SegmentWriter(BitStreamPP bitStream)
        {
            _bitStream = bitStream;
        }

        public abstract void WriteSegment();
    }
}