using System;
using System.Collections.Generic;
using BitStreams;

namespace JPEG_Encoder.segments.app0
{
    public class APP0Writer : SegmentWriter
    {
        private const ushort APP0MARKER = 0xE0FF;
        private byte[] JFIF_STRING = {0x4A, 0x46, 0x49, 0x46, 0x00};

        private int length = 16;
        private byte major = 0x01;
        private byte minor = 0x01;
        private byte pixelUnit = 0;
        private byte xDensity;
        private byte yDensity;
        private byte xThumb = 0;
        private byte yThumb = 0;
        private List<byte> thumbnail = new List<byte>();

        public APP0Writer(BitStream bitStream, byte xDensity, byte yDensity) : base(bitStream)
        {
            this.xDensity = xDensity;
            this.yDensity = yDensity;
        }

        private void SetXDensity(byte xDensity)
        {
            if (xDensity > 0 && xDensity <= 0xFFFF)
            {
            }
            else
            {
                throw new ArgumentException();
            }
        }

        private void SetYDensity(byte yDensity)
        {
            if (yDensity > 0 && yDensity <= 0xFFFF)
            {
            }
            else
            {
                throw new ArgumentException();
            }        
        }

        public override void WriteSegment()
        {
            _bitStream.WriteUInt16(APP0MARKER); 
            _bitStream.WriteByte(0x00);
            _bitStream.WriteByte(0x10);
            _bitStream.WriteBytes(JFIF_STRING, 40L, false);
            _bitStream.WriteByte(major);
            _bitStream.WriteByte(minor);
            _bitStream.WriteByte(pixelUnit);
            _bitStream.WriteByte(0x00);
            _bitStream.WriteByte(xDensity);
            _bitStream.WriteByte(0x00);
            _bitStream.WriteByte(yDensity);
            _bitStream.WriteByte(xThumb);
            _bitStream.WriteByte(yThumb);
            foreach (byte thumbnailByte in thumbnail)
            {
                _bitStream.WriteByte(thumbnailByte);
            }
        }
        
    }
}