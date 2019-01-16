using MathNet.Numerics.Properties;

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
        public static int Pow(int basis, uint power)
        {
            int result = 1;
            
            for (int i = 0; i < power; i++)
            {
                result *= basis;
            }

            return result;
        }
    }
}