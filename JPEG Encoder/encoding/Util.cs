using System;
using BitStreams;
using CenterSpace.NMath.Core;

namespace JPEG_Encoder.encoding
{
    public class Util
    {
        private static readonly int[] IndexXzigzag =
        {
            0, 1, 0, 0, 1, 2, 3, 2,
            1, 0, 0, 1, 2, 3, 4, 5,
            4, 3, 2, 1, 0, 0, 1, 2,
            3, 4, 5, 6, 7, 6, 5, 4,
            3, 2, 1, 0, 1, 2, 3, 4,
            5, 6, 7, 7, 6, 5, 4, 3,
            2, 3, 4, 5, 6, 7, 7, 6,
            5, 4, 5, 6, 7, 7, 6, 7
        };

        private static readonly int[] IndexYzigzag =
        {
            0, 0, 1, 2, 1, 0, 0, 1,
            2, 3, 4, 3, 2, 1, 0, 0,
            1, 2, 3, 4, 5, 6, 5, 4,
            3, 2, 1, 0, 0, 1, 2, 3,
            4, 5, 6, 7, 7, 6, 5, 4,
            3, 2, 1, 2, 3, 4, 5, 6,
            7, 7, 6, 5, 4, 3, 4, 5,
            6, 7, 7, 6, 5, 6, 7, 7
        };

        public static int[] ZigzagSort(DoubleMatrix matrix)
        {
            int[] result = new int[64];
            for (int i = 0; i < result.Length; i++) result[i] = Round(matrix[IndexYzigzag[i], IndexXzigzag[i]]);

            return result;
        }

        private static int Round(double toRound)
        {
            return (int) (Math.Sign(toRound) * Math.Round(Math.Abs(toRound)));
        }

        public static string GetBitsAsString(int bits, int length)
        {
            string result = "";
            for (int i = length - 1; i >= 0; i--) result += "" + ((bits >> i) & 0x1);

            return result;
        }

        public static void WriteBits(BitStream bos, int bits, int length)
        {
            for (int i = length - 1; i >= 0; i--)
            {
                var test = (bits << i) & 0x1;
                bos.WriteBit((bits >> i) & 0x1);
            }
        }
    }
}