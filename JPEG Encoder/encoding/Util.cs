using System;
using System.Collections.Generic;
using System.Linq;
using BitStreams;
using CenterSpace.NMath.Core;

namespace JPEG_Encoder.encoding
{
    public static class Util
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

        private static int _counter;
        private static Bit[] _bitBuffer = new Bit[8];
        
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

        public static void WriteBitsForAcDc(IEnumerable<Bit> bits, BitStream bos, int length)
        {
            if (length == 0) return;
            foreach (Bit bit in bits)
            {
                Write(bit, bos);
            }
        }

        private static void Write(Bit bit, BitStream bos)
        {
            _bitBuffer[_counter] = bit;
            _counter++;
            if (_counter != 8) return;
            bos.WriteBits(_bitBuffer);
            bool twoFiveFive = _bitBuffer.All(testBit => testBit);

            if (twoFiveFive)
            {
                bos.WriteByte(0x00);
            }

            _bitBuffer = new Bit[8];
            _counter = 0;
        }

        public static void Flush(BitStream bos)
        {
            if (_counter != 0)
            {
                bos.WriteBits(_bitBuffer);
            }
        }
    }
}