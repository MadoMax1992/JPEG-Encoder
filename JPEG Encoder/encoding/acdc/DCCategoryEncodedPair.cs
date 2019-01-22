namespace JPEG_Encoder.encoding.acdc
{
    public class DcCategoryEncodedPair : AbstractCategoryEncodedPair
    {
        public DcCategoryEncodedPair(int pair, int entryCategoryEncoded) : base(pair, entryCategoryEncoded)
        {
        }

        public override string ToString()
        {
            return GetPair() + ", " + Util.GetBitsAsString(GetEntryCategoryEncoded(), GetPair());
        }
    }
}