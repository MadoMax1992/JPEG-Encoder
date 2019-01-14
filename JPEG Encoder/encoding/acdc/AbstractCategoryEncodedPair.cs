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

        public Bit[] GetEntryCategoryEncodedAsBitArray()
        {
            string s = Convert.ToString(_entryCategoryEncoded, 2);
            Bit[] bits = new Bit[_pair];

            int i = 0;
            if (s.Length < _pair)
            {
                var diff = _pair - s.Length;
                for (i = 0; i < diff; i++)
                {
                    bits[i] = false;
                }
            }
            foreach (char c in s)
            {
                if (c == '1')
                {
                    bits[i] = true;
                }
                else
                {
                    bits[i] = false;
                }

                i++;
            }

            return bits;
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