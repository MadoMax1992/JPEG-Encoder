namespace JPEG_Encoder.encoding.acdc
{
    public class ACRunlengthEncodedPair
    {
        private int zeroCount;
        private int entry;

        public ACRunlengthEncodedPair(int zeroCount, int entry)
        {
            this.zeroCount = zeroCount;
            this.entry = entry;
        }
        
        public int getZeroCount()
        {
            return zeroCount;
        }

        public int GetEntry()
        {
            return entry;
        }

        public override string ToString()
        {
            return "(" + zeroCount + "," + entry + ")";
        }
    }
}