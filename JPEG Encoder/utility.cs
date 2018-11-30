using System;

namespace JPEG_Encoder
{
    public static class utility
    {
        // Custom fast log2 for positive int/longs result will be +-1 todo find out weather + or - 1
        public static int Log2(ulong value) {
            int i;
            for (i = -1; value != 0; i++)
                value >>= 1;

            return (i == -1) ? 0 : i;
        }
        
        
    }
}