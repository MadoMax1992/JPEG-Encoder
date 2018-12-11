using CenterSpace.NMath.Core;
using MathNet.Numerics.LinearAlgebra;

namespace JPEG_Encoder.encoding.dct
{
    public class CosineTransformation
    {
        //TODO Matrix<double> rausnehmen

        public static DoubleMatrix arai(DoubleMatrix x)
        {
            return new DoubleMatrix(Arai.Calc(Matrix<double>.Build.DenseOfArray(x.ToArray())).ToArray());
        }

        public static DoubleMatrix direct(DoubleMatrix x)
        {
            return new DoubleMatrix(DCT.Advanced(Matrix<double>.Build.DenseOfArray(x.ToArray())).ToArray());
        }

        public static void separated(DoubleMatrix x)
        {
            Matrix<double> matrix = Matrix<double>.Build.DenseOfArray(x.ToArray());
            
            DCT.Advanced(matrix);
        }

        public static DoubleMatrix invert(DoubleMatrix y)
        {
            return new DoubleMatrix(DCT.Invert(Matrix<double>.Build.DenseOfArray(y.ToArray())).ToArray());
        }

//        public static void quantize(DoubleMatrix transformedMatrix, DoubleMatrix quantizationMatrix)
//        {
//            transformedMatrix.divi(quantizationMatrix, transformedMatrix);
//        }
    }
}