using BitStreams;

namespace JPEG_Encoder.segments.app0
{
    public class App0Writer : SegmentWriter
    {
        private const ushort App0Marker = 0xE0FF;
        private readonly byte[] _jfifString = {0x4A, 0x46, 0x49, 0x46, 0x00};

        private const byte Major = 0x01;
        private const byte Minor = 0x01;
        private const byte PixelUnit = 0;
        //private readonly List<byte> _thumbnail = new List<byte>();
        private readonly byte _xDensity;
        private const byte XThumb = 0;
        private readonly byte _yDensity;
        private const byte YThumb = 0;

        public App0Writer(BitStream bitStream, byte xDensity, byte yDensity) : base(bitStream)
        {
            _xDensity = xDensity;
            _yDensity = yDensity;
        }

        public override void WriteSegment()
        {
            BitStream.WriteUInt16(App0Marker);
            BitStream.WriteByte(0x00);
            BitStream.WriteByte(0x10);
            BitStream.WriteBytes(_jfifString, 40L);
            BitStream.WriteByte(Major);
            BitStream.WriteByte(Minor);
            BitStream.WriteByte(PixelUnit);
            BitStream.WriteByte(0x00);
            BitStream.WriteByte(_xDensity);
            BitStream.WriteByte(0x00);
            BitStream.WriteByte(_yDensity);
            BitStream.WriteByte(XThumb);
            BitStream.WriteByte(YThumb);
            //remove comment for thumbnail usage
            //foreach (byte thumbnailByte in _thumbnail) BitStream.WriteByte(thumbnailByte);
        }
    }
}