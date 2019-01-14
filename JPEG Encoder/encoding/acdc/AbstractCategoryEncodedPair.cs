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
                string s = Convert.ToString(_entryCategoryEncoded, 2);
                if (length == 0 && s.Length != 0) length = 1;
                Bit[] bits = new Bit[length];
                
                int i = 0;
                if (s.Length < length)
                {
                    var diff = length - s.Length;
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
            catch (IndexOutOfRangeException e)
            {
                Console.WriteLine("hier");
                Console.WriteLine();
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