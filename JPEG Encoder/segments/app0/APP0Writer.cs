using System;
using System.Collections.Generic;

namespace JPEG_Encoder.segments.app0
{
    public class APP0Writer : SegmentWriter
    {
        private const int APP0MARKER = 0xFFE0;
        private int[] JFIF_STRING = {0x4A, 0x46, 0x49, 0x00};

        private int length = 16;
        private int major = 1;
        private int minor = 1;
        private int pixelUnit = 0;
        private int xDensityLow;
        private int xDensityHigh;
        private int yDensityLow;
        private int yDensityHigh;
        private int xThumb = 0;
        private int yThumb = 0;
        private List<byte> thumbnail = new List<byte>();

        public APP0Writer(BitStreamPP bitStream, int xDensity, int yDensity) : base(bitStream)
        {
            SetXDensity(xDensity);
            SetYDensity(yDensity);
        }

        private void SetXDensity(int xDensity)
        {
            if (xDensity > 0 && xDensity <= 0xFFFF)
            {
                xDensityHigh = (xDensity & 0xFF00) >> 8;
                xDensityLow = xDensity & 0xFF;
            }
            else
            {
                throw new ArgumentException();
            }
        }

        private void SetYDensity(int yDensity)
        {
            if (yDensity > 0 && yDensity <= 0xFFFF)
            {
                xDensityHigh = (yDensity & 0xFF00) >> 8;
                xDensityLow = yDensity & 0xFF;
            }
            else
            {
                throw new ArgumentException();
            }        
        }

        public override void WriteSegment()
        {
            _bitStream.WriteMarker(APP0MARKER);
            _bitStream.WriteInt32((length & 0xFF00) >> 8);
            _bitStream.WriteInt32(length & 0xFF);
            foreach (int t in JFIF_STRING)
            {
                _bitStream.WriteInt32(t);
            }
            _bitStream.WriteInt32(major);
            _bitStream.WriteInt32(minor);
            _bitStream.WriteInt32(pixelUnit);
            _bitStream.WriteInt32(xDensityHigh);
            _bitStream.WriteInt32(xDensityLow);
            _bitStream.WriteInt32(yDensityHigh);
            _bitStream.WriteInt32(yDensityLow);
            _bitStream.WriteInt32(xThumb);
            _bitStream.WriteInt32(yThumb);
            foreach (byte thumbnailByte in thumbnail)
            {
                _bitStream.WriteByte(thumbnailByte);
            }
        }
        
    }
}