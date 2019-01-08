using System;
using BitStreams;

namespace JPEG_Encoder.segments.soi
{
    public class SOIWriter : SegmentWriter
    {
        private const ushort SOI_MARKER = 0xD8FF;
        
        public SOIWriter(BitStream bitStream) : base(bitStream)
        {
        }

        public override void WriteSegment()
        {
            _bitStream.WriteUInt16(SOI_MARKER);
        }
    }
}