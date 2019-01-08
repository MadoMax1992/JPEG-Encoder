using System.Xml;
using CenterSpace.NMath.Core;
using MathNet.Numerics.LinearAlgebra;

namespace JPEG_Encoder.encoding.dct
{
    public class CosineTransformation
    {
        public static DoubleMatrix Arai(DoubleMatrix x)
        {
            return null;
        }

        public static void Direct(DoubleMatrix x)
        {
            DCT.Naive(x);
        }

        public static void Separated(DoubleMatrix x)
        {            
            DCT.Advanced(x);
        }

        public static DoubleMatrix invert(DoubleMatrix y)
        {
            return DCT.Invert(y);
        }

        public static void Quantize(DoubleMatrix transformedMatrix, DoubleMatrix quantizationMatrix)
        {
            DoubleMatrix.Divide(transformedMatrix, quantizationMatrix, transformedMatrix);
        }
    }
}