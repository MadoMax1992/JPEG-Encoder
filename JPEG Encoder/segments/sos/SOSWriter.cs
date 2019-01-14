using System.Collections.Generic;
using BitStreams;
using JPEG_Encoder.segments.soi;

namespace JPEG_Encoder.segments.sos
{
    public class SosWriter : SegmentWriter
    {
        private const ushort SosMarker = 0xDAFF;

        private readonly int _length;
        private List<SosComponent> _components;
        private readonly int _numOfComponents;
        private const int StartOfSpectralSelection = 0;
        private const int EndOfSpectralSelection = 63;
        private const int SuccessiveApproximation = 0;

        public SosWriter(BitStream os) : base(os)
        {
            SetComponents();
            _numOfComponents = _components.Count;
            _length = 6 + 2 * _components.Count;
        }

        private void SetComponents()
        {
            _components = new List<SosComponent>
            {
                new SosComponent(0, 0, 0), new SosComponent(1, 1, 1), new SosComponent(2, 1, 1)
            };
        }

        public override void WriteSegment()
        {
            BitStream.WriteUInt16(SosMarker);
            BitStream.WriteByte((byte) (_length >> 8));
            BitStream.WriteByte((byte) (_length & 0xff));
            BitStream.WriteByte((byte) _numOfComponents);
            foreach (SosComponent component in _components)
            {
                component.WriteComponent(BitStream);
            }
            BitStream.WriteByte(StartOfSpectralSelection);
            BitStream.WriteByte(EndOfSpectralSelection);
            BitStream.WriteByte(SuccessiveApproximation);
        }
    }
}