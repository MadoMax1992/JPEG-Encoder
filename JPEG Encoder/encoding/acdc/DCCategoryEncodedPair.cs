namespace JPEG_Encoder.encoding.acdc
{
    public class DCCategoryEncodedPair : AbstractCategoryEncodedPair
    {
        public DCCategoryEncodedPair(int pair, int entryCategoryEncoded) : base(pair, entryCategoryEncoded) {}

        public override string ToString()
        {
            return GetPair() + ", " + Util.GetBitsAsString(GetEntryCategoryEncoded(), GetPair());
        }
    }
}