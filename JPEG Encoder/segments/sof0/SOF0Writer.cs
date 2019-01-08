using System.Collections.Generic;
using System.Linq;
using BitStreams;

namespace JPEG_Encoder.segments.sof0
{
    public class SOF0Writer : SegmentWriter
    {
        private const ushort SOFOMARKER = 0xC0FF;

        private byte length;
        private byte sampleRate = 8;
        private int yImgSize;
        private int xImgSize;
        private int subsampling;
        private int numberOfComponents;
        private List<SOF0Component> components;

        public SOF0Writer(BitStream bitStream, int xImgSize, int yImgSize, int subsampling) : base(bitStream)
        {
            this.xImgSize = xImgSize;
            this.yImgSize = yImgSize;
            this.subsampling = subsampling;
            SetComponents();
            numberOfComponents = components.Count;
            length = (byte) (8 + numberOfComponents * 3);
            this.xImgSize = xImgSize;
            this.yImgSize = yImgSize;
            this.subsampling = subsampling;
        }

        private void SetComponents()
        {
            components = new List<SOF0Component>();
            components.Add(new SOF0Component(0, subsampling, subsampling, 0));
            components.Add(new SOF0Component(1, 1, 1, 1));
            components.Add(new SOF0Component(2, 1, 1, 1));
        }

        public override void WriteSegment()
        {
            _bitStream.WriteUInt16(SOFOMARKER);
            _bitStream.WriteByte(0x00);
            _bitStream.WriteByte(length);
            _bitStream.WriteByte(sampleRate);

            _bitStream.WriteByte((byte) (yImgSize >> 8));
            _bitStream.WriteByte((byte) yImgSize);
            _bitStream.WriteByte((byte) (xImgSize >> 8));
            _bitStream.WriteByte((byte) xImgSize);
            _bitStream.WriteByte((byte) numberOfComponents);
            foreach (SOF0Component sof0Component in components)
            {
                sof0Component.WriteToStream(_bitStream);
            }
        }
    }
}