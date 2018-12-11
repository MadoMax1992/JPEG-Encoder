using System.Collections.Generic;
using System.Linq;

namespace JPEG_Encoder.segments.sof0
{
    public class SOF0Writer : SegmentWriter
    {
        private const int SOFOMARKER = 0xFFC0;

        private int length;
        private int sampleRate = 8;
        private int yImgSize;
        private int xImgSize;
        private int subsampling;
        private int numberOfComponents;
        private List<SOF0Component> components;

        public SOF0Writer(BitStreamPP bitStream, int xImgSize, int yImgSize, int subsampling) : base(bitStream)
        {
            this.xImgSize = xImgSize;
            this.yImgSize = yImgSize;
            this.subsampling = subsampling;
            setComponents();
            numberOfComponents = components.Count;
            length = 8 + numberOfComponents * 3;
            this.xImgSize = xImgSize;
            this.yImgSize = yImgSize;
            this.subsampling = subsampling;
        }

        private void setComponents()
        {
            components = new List<SOF0Component>();
            components.Add(new SOF0Component(0, subsampling, subsampling, 0));
            components.Add(new SOF0Component(1, 1, 1, 1));
            components.Add(new SOF0Component(2, 1, 1, 1));
        }

        public override void WriteSegment()
        {
            _bitStream.WriteMarker(SOFOMARKER);
            _bitStream.WriteInt32(length);
            _bitStream.WriteInt32(sampleRate);
            _bitStream.WriteInt32(yImgSize);
            _bitStream.WriteInt32(xImgSize);
            _bitStream.WriteInt32(numberOfComponents);
            foreach (SOF0Component sof0Component in components)
            {
                sof0Component.writeToStream(_bitStream);
            }
        }
    }
}