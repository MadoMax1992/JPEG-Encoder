using System;

namespace JPEG_Encoder
{
    public static class utility
    {
        public static int Log2(ulong value) {
            int i;
            for (i = -1; value != 0; i++)
                value >>= 1;

            return (i == -1) ? 0 : i;
        }
    }
}