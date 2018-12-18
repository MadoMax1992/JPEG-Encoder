using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using CenterSpace.NMath.Core;
using JPEG_Encoder.image.colors;

namespace JPEG_Encoder.encoding.dct
{
    public class PerformanceCheck
    {
        private ColorChannel picture = new ColorChannel(3840, 2160);

        public PerformanceCheck()
        {
            FillPicture();
        }

        private void FillPicture()
        {
            for (int y = 0; y < picture.GetHeight(); y++)
            {
                for (int x = 0; x < picture.GetWidth(); x++)
                {
                    int value;
                    value = (x + (y * 8)) % 256;
                    picture.SetPixel(x, y, value);
                }
            }
        }

        public List<DoubleMatrix> GetBlockAsList(int start, int end)
        {
            return picture.GetBlocks(start, end);
        }

        public int GetNumOfBlocks()
        {
            return picture.GetNumOfBlocks();
        }

        public void print()
        {
            for (int row = 0; row < picture.GetHeight(); row++)
            {
                for (int col = 0; col < picture.GetWidth(); col++)
                {
                    Console.WriteLine(picture.GetPixel(row, col) + " ");
                }

                Console.WriteLine();
            }
        }

        public static void StartCheck()
        {
            PerformanceCheck performanceCheck = new PerformanceCheck();


            Console.WriteLine("Starting Direct DCT Benchmark");
            Stopwatch stopwatch = Stopwatch.StartNew();

            FullImageDirectTask fullImageDirectTask = new FullImageDirectTask(performanceCheck.picture);
            int count = fullImageDirectTask.Call();
            long finishedAfter = stopwatch.ElapsedMilliseconds;
            Console.WriteLine("Finished after " +  finishedAfter / 1000d + " seconds");
            Console.WriteLine("Direct DCT takes " + (double) finishedAfter / count + " ms/image");
            Console.WriteLine("Managed " + count + " images");


            Console.WriteLine("Starting Separated DCT Benchmark");
            stopwatch.Restart();
            FullImageSeparatedTask fullImageSeparatedTask = new FullImageSeparatedTask(performanceCheck.picture);
            count = fullImageSeparatedTask.Call();
            finishedAfter = stopwatch.ElapsedMilliseconds;
            Console.WriteLine("Finished after " + finishedAfter / 1000d + " seconds");
            Console.WriteLine("Separated DCT takes " + (double) finishedAfter / count + " ms/image");
            Console.WriteLine("Managed " + count + " images");


            Console.WriteLine("Starting Arai Benchmark");
            stopwatch.Restart();
            FullImageAraiTask fullImageAraiTask = new FullImageAraiTask(performanceCheck.picture);
            count = fullImageAraiTask.Call();
            finishedAfter = stopwatch.ElapsedMilliseconds;
            Console.WriteLine("Finished after " + finishedAfter / 1000d + " seconds");
            Console.WriteLine("Arai takes " + (double) finishedAfter / count + " ms/image");
            Console.WriteLine("Managed " + count + " images");
            stopwatch.Stop();
        }
    }
}