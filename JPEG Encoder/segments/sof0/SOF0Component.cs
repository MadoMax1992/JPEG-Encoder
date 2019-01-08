using BitStreams;

namespace JPEG_Encoder.segments.sof0
{
    public class SOF0Component
    {
        private readonly int _id;
        private readonly int _subSamplingFactorVertical;
        private readonly int _subSamplingFactorHorizontal;
        private readonly int _quantizationTableId;

        public SOF0Component(int id, int subSamplingFactorVertical, int subSamplingFactorHorizontal,
            int quantizationTableId)
        {
            _id = id;
            _subSamplingFactorHorizontal = subSamplingFactorHorizontal;
            _subSamplingFactorVertical = subSamplingFactorVertical;
            _quantizationTableId = quantizationTableId;
        }

        public void WriteToStream(BitStream os)
        {
            os.WriteByte((byte) _id);
            int subSamplingFactor = 0;
            subSamplingFactor |= _subSamplingFactorHorizontal << 5;
            subSamplingFactor |= _subSamplingFactorVertical << 0;
            os.WriteByte((byte) subSamplingFactor);
            os.WriteByte((byte) _quantizationTableId);
    }
    }
}