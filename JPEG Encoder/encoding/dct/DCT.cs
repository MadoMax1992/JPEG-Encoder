using System;
using System.Diagnostics;
using MathNet.Numerics.LinearAlgebra;

namespace JPEG_Encoder.encoding.dct
{
    public static class DCT
    {
        private static readonly Matrix<double> C = Matrix<double>.Build.DenseOfArray(new[,]{
                                                                                    {
                                                                                        1d / Math.Sqrt(2), 1, 1, 1, 1, 1, 1, 1
                                                                                    }});

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
            Matrix<double> X = Matrix<double>.Build.DenseOfArray(array);
            Matrix<double> naiveDct = Naive(X);
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
            
            X = Matrix<double>.Build.DenseOfArray(array);
            Matrix<double> advancedDct = Advanced(X);
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

        public static Matrix<double> Naive(Matrix<double> input)
        {
            //Blocksize N in Formel
            const int blockSize = 8;
            Matrix<double> result = Matrix<double>.Build.Dense(input.RowCount, input.ColumnCount);
            //Zweiter Teil der Formel

            for (int i = 0; i < blockSize; i++)
            {
                for (int j = 0; j < blockSize; j++)
                {
                    double secondPart = 0;

                    for (int x = 0; x < input.RowCount; x++)
                    {

                        for (int y = 0; y < input.ColumnCount; y++)
                        {
                            secondPart += input.At(x, y) * CosOperation(x,i) * CosOperation(y, j);
                        }
                    }
                    result[i, j] = FirstPart(i, j) * secondPart;
                }
            }
            return result;
            
            double FirstPart(int i, int j)
            {
                double firstC;
                double secondC;
                if (i == 0)
                {
                    firstC = 1.0 / Math.Sqrt(2);
                }
                else
                {
                    firstC = 1.0;
                }
                if (j == 0)
                {
                    secondC = 1.0 / Math.Sqrt(2);
                }
                else
                {
                    secondC = 1.0;
                }
                return 2.0 / blockSize * firstC * secondC;
            }
            double CosOperation(int inputIndex, int transformIndex)
            {
                return Math.Cos((2.0 * inputIndex + 1.0) * transformIndex * Math.PI / (2.0 * blockSize));
            }
        }

        public static Matrix<double> Advanced(Matrix<double> X)
        {
            Matrix<double> a = Matrix<double>.Build.Dense(8, 8);
            
            for (int n = 0; n < 8; n++)
            {
                for (int k = 0; k < 8; k++)
                {
                    double aKN = C.At(k, 0) * Math.Sqrt(2d / 8) * Math.Cos((2 * n + 1) * (k * Math.PI / (2 * 8)));
                    a[k, n] = aKN;
                }
            }
            Matrix<double> aT = a.Transpose();
            
            a.Multiply(X).Multiply(aT, X);
            
            return X;
        }

        public static Matrix<double> Invert(Matrix<double> Y)
        {
            var n = Y.RowCount;
            var X = Matrix<double>.Build.Dense(n, n);
            for (var y = 0; y < n; y++)
            {
                for (var x = 0; x < n; x++)
                {
                    double xXy = 0;
                    for (var i = 0; i < n; i++)
                    {
                        for (var j = 0; j < n; j++)
                        {
                            xXy += 2d / n * C.At(i, 0) * C.At(j, 0) * Y.At(j, i)
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