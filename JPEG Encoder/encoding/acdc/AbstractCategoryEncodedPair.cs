using System;
using BitStreams;

namespace JPEG_Encoder.encoding.acdc
{
    public abstract class AbstractCategoryEncodedPair
    {
        private int _entryCategoryEncoded;
        private readonly int _pair;

        public AbstractCategoryEncodedPair(int pair, int entryCategoryEncoded)
        {
            _pair = pair;
            _entryCategoryEncoded = entryCategoryEncoded;
        }

        public int GetPair()
        {
            return _pair;
        }

        public int GetEntryCategoryEncoded()
        {
            return _entryCategoryEncoded;
        }

        public Bit[] GetEntryCategoryEncodedAsBitArray(int length)
        {
            try
            {
                int len = 0;

                if (_entryCategoryEncoded != 0)
                    len = 8;
                    for (int k = 256; (_entryCategoryEncoded & k) == 0; k >>= 1)
                        len--;

                if (length == 0 && len != 0) length = 1;
                
                Bit[] bits = new Bit[length];

                int i = 0;
                if (len < length)
                {
                    var diff = length - len;
                    for (i = 0; i < diff; i++)
                    {
                        bits[i] = false;
                    }
                }

                for(int k = len; k != 0; k--, i++)
                {
                    bits[i] = ((_entryCategoryEncoded >> k) & 1) == 1;
                }

                return bits;
            }
            catch (IndexOutOfRangeException e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void SetEntryCategoryEncoded(int entryCategoryEncoded)
        {
            _entryCategoryEncoded = entryCategoryEncoded;
        }

        public static int CalculateCategory(int number)
        {
            int category = (int) (uint) Math.Round(Math.Log(Math.Abs(number)) / Math.Log(2) + 0.5,
                MidpointRounding.AwayFromZero);

            return category;
        }

        public static int EncodeCategory(int toEncode)
        {
            int result;
            int category = CalculateCategory(toEncode);
            if (toEncode < 0)
            {
                int maxValueCategory = (int) (Math.Pow(2, category) - 1);
                result = Math.Abs(toEncode) ^ maxValueCategory;
            }
            else
            {
                result = toEncode;
            }

            return result;
        }

        public override string ToString()
        {
            return _pair + ", " + _entryCategoryEncoded;
        }
    }
}