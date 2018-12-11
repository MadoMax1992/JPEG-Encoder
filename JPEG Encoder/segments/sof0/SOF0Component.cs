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

        public void writeToStream(BitStreamPP os)
        {
            os.WriteInt32(_id);
            os.WriteInt32(_subSamplingFactorHorizontal);
            os.WriteInt32(_subSamplingFactorVertical);
            os.WriteInt32(_quantizationTableId);
    }
    }
}