using System;
using BitStreams;

namespace JPEG_Encoder
{
    public static class Utility
    {
        // Custom fast log2 for positive int/longs result will be +-1 todo find out weather + or - 1
        public static int Log2(ulong value)
        {
            int i;
            
            for (i = -1; value != 0; i++)
                value >>= 1;

            return i == -1 ? 0 : i;
        }

        // Custom fast Power for positive ints
        public static int Pow(int basis, int power)
        {
            int result = 1;
            
            for (int i = 0; i < power; i++)
            {
                result *= basis;
            }

            return result;
        }

        public static Bit[] GetYLastBitsOfX(int input, int targetLength)
        {
            int inputLength = 1;

            if (input != 0)
            {
                inputLength = targetLength;
                for (int k = 1 << (targetLength-1); (input & k) == 0; k >>= 1)
                    inputLength--;
            }
            
            Bit[] bits = new Bit[targetLength];
            
            for (int i = targetLength - inputLength; i < targetLength; i++)
                bits[i] = (input & (1 << (targetLength - i - 1))) != 0;
            
            return bits;
        }

        public static Bit[] GetYLastBitsOfXUsingString(int input, int targetLength)
        {
            string s = Convert.ToString(input, 2);
            Bit[] bits = new Bit[targetLength];

            int i = 0;
            
            if (s.Length < targetLength)
                i = targetLength - s.Length;

            foreach (char c in s)
                bits[i++] = c == '1';
            return bits;
        }
    }
}