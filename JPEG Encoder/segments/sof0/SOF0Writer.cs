using System.Collections.Generic;
using BitStreams;

namespace JPEG_Encoder.segments.sof0
{
    public class Sof0Writer : SegmentWriter
    {
        private const ushort Sof0Marker = 0xC0FF;
        private List<Sof0Component> _components;

        private readonly byte _length;
        private readonly int _numberOfComponents;
        private const byte SampleRate = 8;
        private readonly int _subSampling;
        private readonly int _xImgSize;
        private readonly int _yImgSize;

        public Sof0Writer(BitStream bitStream, int xImgSize, int yImgSize, int subSampling) : base(bitStream)
        {
            _xImgSize = xImgSize;
            _yImgSize = yImgSize;
            _subSampling = subSampling;
            SetComponents();
            _numberOfComponents = _components.Count;
            _length = (byte) (8 + _numberOfComponents * 3);
            _xImgSize = xImgSize;
            _yImgSize = yImgSize;
            _subSampling = subSampling;
        }

        private void SetComponents()
        {
            _components = new List<Sof0Component>
            {
                new Sof0Component(0, _subSampling, _subSampling, 0),
                new Sof0Component(1, 1, 1, 1),
                new Sof0Component(2, 1, 1, 1)
            };
        }

        public override void WriteSegment()
        {
            BitStream.WriteUInt16(Sof0Marker);
            BitStream.WriteByte(0x00);
            BitStream.WriteByte(_length);
            BitStream.WriteByte(SampleRate);

            BitStream.WriteByte((byte) (_yImgSize >> 8));
            BitStream.WriteByte((byte) _yImgSize);
            BitStream.WriteByte((byte) (_xImgSize >> 8));
            BitStream.WriteByte((byte) _xImgSize);
            BitStream.WriteByte((byte) _numberOfComponents);
            foreach (Sof0Component sof0Component in _components) sof0Component.WriteToStream(BitStream);
        }
    }
}