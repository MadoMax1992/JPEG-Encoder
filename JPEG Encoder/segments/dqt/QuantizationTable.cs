using BitStreams;
using CenterSpace.NMath.Core;
using JPEG_Encoder.encoding;

namespace JPEG_Encoder.segments.dqt
{
    public class QuantizationTable
    {
        public static readonly DoubleMatrix QuantizationMatrixLuminance = new DoubleMatrix(new double[,]
        {
            {16, 11, 10, 16, 24, 40, 51, 61},
            {12, 12, 14, 19, 26, 58, 60, 55},
            {14, 13, 16, 24, 40, 57, 69, 56},
            {14, 17, 22, 29, 51, 87, 80, 62},
            {18, 22, 37, 56, 68, 109, 103, 77},
            {24, 35, 55, 64, 81, 104, 113, 92},
            {49, 64, 78, 87, 103, 121, 120, 101},
            {72, 92, 95, 98, 112, 100, 103, 99}
        });

        public static readonly DoubleMatrix QuantizationMatrixChrominance = new DoubleMatrix(new double[,]
        {
            {17, 18, 24, 47, 99, 99, 99, 99},
            {18, 21, 26, 66, 99, 99, 99, 99},
            {24, 26, 56, 99, 99, 99, 99, 99},
            {47, 66, 99, 99, 99, 99, 99, 99},
            {99, 99, 99, 99, 99, 99, 99, 99},
            {99, 99, 99, 99, 99, 99, 99, 99},
            {99, 99, 99, 99, 99, 99, 99, 99},
            {99, 99, 99, 99, 99, 99, 99, 99}
        });

        private readonly byte _id;
        private readonly DoubleMatrix _table;

        public QuantizationTable(byte id, DoubleMatrix table)
        {
            _id = id;
            _table = table;
        }

        public void WriteTable(BitStream bos)
        {
            bos.WriteBit(_id == 0 ? 0 : 1);
            bos.WriteBit(0);
            bos.WriteBit(0);
            bos.WriteBit(0);

            bos.WriteBit(0);
            bos.WriteBit(0);
            bos.WriteBit(0);
            bos.WriteBit(0);


            ZigzagSort(bos);
        }

        private void ZigzagSort(BitStream bos)
        {
            int[] zigzaged = Util.ZigzagSort(_table);
            foreach (int tableEntry in zigzaged) bos.WriteByte((byte) tableEntry);
        }
    }
}