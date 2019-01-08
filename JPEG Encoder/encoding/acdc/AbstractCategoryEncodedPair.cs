using System;
using System.Reflection.Emit;
using Microsoft.SqlServer.Server;

namespace JPEG_Encoder.encoding.acdc
{
    public abstract class AbstractCategoryEncodedPair
    {
        private int pair;
        private int entryCategoryEncoded;

        public AbstractCategoryEncodedPair(int pair, int entryCategoryEncoded)
        {
            this.pair = pair;
            this.entryCategoryEncoded = entryCategoryEncoded;
        }

        public int GetPair()
        {
            return pair;
        }

        public int GetEntryCategoryEncoded()
        {
            return entryCategoryEncoded;
        }

        public void SetEntryCategoryEncoded(int entryCategoryEncoded)
        {
            this.entryCategoryEncoded = entryCategoryEncoded;
        }

        public static int CalculateCategory(int number)
        {
            int category = (int) (uint) Math.Round(Math.Log(Math.Abs(number)) / Math.Log(2) + 0.5, MidpointRounding.AwayFromZero);

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

        public override String ToString()
        {
            return pair + ", " + entryCategoryEncoded;
        }
    }
}