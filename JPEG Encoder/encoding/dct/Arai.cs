using System;
using System.Diagnostics;
using MathNet.Numerics.LinearAlgebra;

namespace JPEG_Encoder.encoding.dct
{
    public static class Arai
    {
        private static readonly double[] C = 
        {
            1, 
            Math.Cos(Math.PI / 16 * 1),
            Math.Cos(Math.PI / 16 * 2),
            Math.Cos(Math.PI / 16 * 3),
            Math.Cos(Math.PI / 16 * 4),
            Math.Cos(Math.PI / 16 * 5),
            Math.Cos(Math.PI / 16 * 6),
            Math.Cos(Math.PI / 16 * 7)
        };
        
        private static readonly double[] A =
        {
            1,
            C[4],
            C[2] - C[6],
            C[4],
            C[6] + C[2],
            C[6]
        };
        
        private static readonly double[] S =
        {
            1d / (2 * Math.Sqrt(2)),
            1d / (4 * C[1]),
            1d / (4 * C[2]),
            1d / (4 * C[3]),
            1d / (4 * C[4]),
            1d / (4 * C[5]),
            1d / (4 * C[6]),
            1d / (4 * C[7])
        };

        public static void TestArai()
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
            Matrix<double> araiDct = Calc(X);
            Console.WriteLine("Arai DCT:");
            Console.WriteLine(araiDct);
            Console.Write("Elapsed MilliSeconds: ");
            Console.WriteLine(stopwatch.ElapsedMilliseconds);
            
            Console.WriteLine("Inverse Arai:");
            Console.WriteLine(CalcReverse(araiDct));
            Console.Write("Elapsed MilliSeconds: ");
            Console.WriteLine(stopwatch.ElapsedMilliseconds);
            
            stopwatch.Stop();
            
        }
        
        public static Matrix<double> Calc(Matrix<double> input)
        {
            int i = 0;
            foreach (Vector<double> row in input.EnumerateRows())
            {
                input.SetRow(i++, AraiVector(row));
            }

            i = 0;
            foreach (Vector<double> column in input.EnumerateColumns())
            {
                input.SetColumn(i++, AraiVector(column));
            }
            
            return input;
        }
        
        public static Matrix<double> CalcReverse(Matrix<double> input)
        {
            int i = 0;
            foreach (Vector<double> row in input.EnumerateRows())
            {
                input.SetRow(i++, ReverseArai(row));
            }

            i = 0;
            foreach (Vector<double> column in input.EnumerateColumns())
            {
                input.SetColumn(i++, ReverseArai(column));
            }
            
            return input;
        }

        private static Vector<double> AraiVector(Vector<double> vector)
        {
            /*
             * Based upon
             * Fast discrete cosine transform algorithms (C#)
             * 
             * Copyright (c) 2017 Project Nayuki. (MIT License)
             * https://www.nayuki.io/page/fast-discrete-cosine-transform-algorithms
             */
            // Arai, Agui, Nakajima, 1988.
            // https://web.stanford.edu/class/ee398a/handouts/lectures/07-TransformCoding.pdf#page=30
            double v0 = vector[0] + vector[7];
            double v1 = vector[1] + vector[6];
            double v2 = vector[2] + vector[5];
            double v3 = vector[3] + vector[4];
            double v4 = vector[3] - vector[4];
            double v5 = vector[2] - vector[5];
            double v6 = vector[1] - vector[6];
            double v7 = vector[0] - vector[7];

            double v8 = v0 + v3;
            double v9 = v1 + v2;
            double v10 = v1 - v2;
            double v11 = v0 - v3;
            double v12 = -v4 - v5;
            double v13 = (v5 + v6) * A[3];
            double v14 = v6 + v7;

            double v17 = (v10 + v11) * A[1];
            double v18 = (v12 + v14) * A[5];

            double v19 = -v12 * A[2] - v18;
            double v20 = v14 * A[4] - v18;

            double v23 = v13 + v7;
            double v24 = v7 - v13;

            vector[0] = S[0] * (v8 + v9);
            vector[1] = S[1] * (v23 + v20);
            vector[2] = S[2] * (v17 + v11);
            vector[3] = S[3] * (v24 - v19);
            vector[4] = S[4] * (v8 - v9);
            vector[5] = S[5] * (v19 + v24);
            vector[6] = S[6] * (v11 - v17);
            vector[7] = S[7] * (v23 - v20);

            return vector;
        }

        public static Vector<double> ReverseArai(Vector<double> vector)
        {
            double v15 = vector[0] / S[0];
            double v26 = vector[1] / S[1];
            double v21 = vector[2] / S[2];
            double v28 = vector[3] / S[3];
            double v16 = vector[4] / S[4];
            double v25 = vector[5] / S[5];
            double v22 = vector[6] / S[6];
            double v27 = vector[7] / S[7];

            double v19 = (v25 - v28) / 2;
            double v20 = (v26 - v27) / 2;
            double v23 = (v26 + v27) / 2;
            double v24 = (v25 + v28) / 2;

            double v7 = (v23 + v24) / 2;
            double v11 = (v21 + v22) / 2;
            double v13 = (v23 - v24) / 2;
            double v17 = (v21 - v22) / 2;

            double v8 = (v15 + v16) / 2;
            double v9 = (v15 - v16) / 2;

            double v18 = (v19 - v20) * A[5]; // Different from original
            double v12 = (v19 * A[4] - v18) / (A[2] * A[5] - A[2] * A[4] - A[4] * A[5]);
            double v14 = (v18 - v20 * A[2]) / (A[2] * A[5] - A[2] * A[4] - A[4] * A[5]);

            double v6 = v14 - v7;
            double v5 = v13 / A[3] - v6;
            double v4 = -v5 - v12;
            double v10 = v17 / A[1] - v11;

            double v0 = (v8 + v11) / 2;
            double v1 = (v9 + v10) / 2;
            double v2 = (v9 - v10) / 2;
            double v3 = (v8 - v11) / 2;

            vector[0] = (v0 + v7) / 2;
            vector[1] = (v1 + v6) / 2;
            vector[2] = (v2 + v5) / 2;
            vector[3] = (v3 + v4) / 2;
            vector[4] = (v3 - v4) / 2;
            vector[5] = (v2 - v5) / 2;
            vector[6] = (v1 - v6) / 2;
            vector[7] = (v0 - v7) / 2;

            return vector;
        }
    }
}