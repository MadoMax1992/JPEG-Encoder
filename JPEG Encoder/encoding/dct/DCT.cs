using System;
using System.Diagnostics;
using System.Linq;
using CenterSpace.NMath.Core;
using MathNet.Numerics.LinearAlgebra;

namespace JPEG_Encoder.encoding.dct
{
    public static class DCT
    {
        private static readonly DoubleMatrix C = new DoubleMatrix(new[,]
                                                                    {{
                                                                        1d / Math.Sqrt(2), 1, 1, 1, 1, 1, 1, 1
                                                                    }});

        private static readonly DoubleMatrix A;

        private static readonly DoubleMatrix A_T;

        static DCT()
        {
            A = new DoubleMatrix(new double[8,8]);
            for (int n = 0; n < 8; n++)
            {
                for (int k = 0; k < 8; k++)
                {
                    double a_k_n = C.ElementAt(k) * Math.Sqrt(2d / 8) * Math.Cos((2 * n + 1) * (k * Math.PI / (2 * 8)));
                    A[k, n] = a_k_n;
                }
            }

            A_T = A.Transpose();
        }
        public static void TestDCT()
        {
            double[,] array = {
                {28,34,19,18,22,17,17,17 },
                {31,36,20,31,166,184,177,140 },
                {17,17,17,95,198,185,152,160 },
                {24,20,21,42,43,41,99,150 },
                {19,18,19,71,63,99,98,62 },
                {81,103,90,118,26,31,23,22 },
                {161,160,163,148,142,146,155,96 },
                {158,139,153,148,154,155,142,35 }
            };

            var stopwatch = Stopwatch.StartNew();
            stopwatch.Start();
            DoubleMatrix X = new DoubleMatrix(array);
            DoubleMatrix naiveDct = Naive(X);
            Console.WriteLine("Naive DCT:");
            Console.WriteLine(naiveDct);
            Console.Write("Elapsed MilliSeconds: ");
            Console.WriteLine(stopwatch.ElapsedMilliseconds);

            stopwatch.Restart();
            
            Console.WriteLine("Inverse Naive:");
            Console.WriteLine(Invert(naiveDct));
            Console.Write("Elapsed MilliSeconds: ");
            Console.WriteLine(stopwatch.ElapsedMilliseconds);
            
            stopwatch.Restart();
            
            X = new DoubleMatrix(array);
            DoubleMatrix advancedDct = Advanced(X);
            Console.WriteLine("Advanced DCT:");
            Console.WriteLine(advancedDct);
            Console.Write("Elapsed MilliSeconds: ");
            Console.WriteLine(stopwatch.ElapsedMilliseconds);
            
            stopwatch.Restart();
            
            Console.WriteLine("Inverse Advanced:");
            Console.WriteLine(Invert(advancedDct));
            Console.Write("Elapsed MilliSeconds: ");
            Console.WriteLine(stopwatch.ElapsedMilliseconds);
            
            stopwatch.Stop();
        }

        public static DoubleMatrix Naive(DoubleMatrix input)
        {
            int n = input.Rows;
            DoubleMatrix Y = new DoubleMatrix(n, n);
            for (int j = 0; j < n; j++)
            {
                for (int i = 0; i < n; i++)
                {
                    double temp = 0;
                    for (int x = 0; x < n; x++)
                    {
                        for (int y = 0; y < n; y++)
                        {
                            temp += input[y, x]
                                    * Math.Cos((2 * x + 1) * i * Math.PI / (2d * n))
                                    * Math.Cos((2 * y + 1) * j * Math.PI / (2d * n));
                        }
                    }

                    double Y_i_j = 2d / n * C.ElementAt(i) * C.ElementAt(j) * temp;
                    Y[j, i] = Y_i_j;
                }
            }

            return Y; 
        }

        public static DoubleMatrix Advanced(DoubleMatrix x)
        {
            return NMathFunctions.Product(NMathFunctions.Product(A, x), A_T);
        }

        public static DoubleMatrix Invert(DoubleMatrix Y)
        {
            var n = Y.Rows;
            var X = new DoubleMatrix(n, n);
            for (var y = 0; y < n; y++)
            {
                for (var x = 0; x < n; x++)
                {
                    double xXy = 0;
                    for (var i = 0; i < n; i++)
                    {
                        for (var j = 0; j < n; j++)
                        {
                            xXy += 2d / n * C.ElementAt(i) * C.ElementAt(j) * Y[j, i]
                                     * Math.Cos((2 * x + 1) * i * Math.PI / (2d * n))
                                     * Math.Cos((2 * y + 1) * j * Math.PI / (2d * n));
                        }
                    }
                    X[y, x] = xXy;
                }
            }
            return X;
        }
    }
}