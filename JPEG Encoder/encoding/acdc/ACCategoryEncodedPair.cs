namespace JPEG_Encoder.encoding.acdc
{
    public class ACCategoryEncodedPair : AbstractCategoryEncodedPair
    {
        public ACCategoryEncodedPair(int pair, int entryCategoryEncoded) : base(pair, entryCategoryEncoded){}

        public int GetZeroCount()
        {
            return (GetPair() >> 4) & 0xF;
        }

        public int GetCategory()
        {
            return GetPair() & 0xF;
        }

        public override string ToString()
        {
            return "(" + GetZeroCount() + "," + GetCategory() + "), " +
                   (GetEntryCategoryEncoded() == 0 && GetCategory() == 0
                       ? ""
                       : Util.GetBitsAsString(GetEntryCategoryEncoded(), GetCategory()));
        }
    }
}