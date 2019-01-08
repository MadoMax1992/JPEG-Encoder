namespace JPEG_Encoder.encoding.acdc
{
    public class AcRunlengthEncodedPair
    {
        private readonly int _entry;
        private readonly int _zeroCount;

        public AcRunlengthEncodedPair(int zeroCount, int entry)
        {
            _zeroCount = zeroCount;
            _entry = entry;
        }

        public int GetZeroCount()
        {
            return _zeroCount;
        }

        public int GetEntry()
        {
            return _entry;
        }

        public override string ToString()
        {
            return "(" + _zeroCount + "," + _entry + ")";
        }
    }
}