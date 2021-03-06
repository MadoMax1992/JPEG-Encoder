using System;
using System.Diagnostics;
using JPEG_Encoder.image.colors;

namespace JPEG_Encoder.encoding.dct
{
    public class PerformanceCheck
    {
        private readonly ColorChannel _picture = new ColorChannel(3840, 2160);

        public PerformanceCheck()
        {
            FillPicture();
        }

        private void FillPicture()
        {
            for (int y = 0; y < _picture.GetHeight(); y++)
            for (int x = 0; x < _picture.GetWidth(); x++)
            {
                int value;
                value = (x + y * 8) % 256;
                _picture.SetPixel(x, y, value);
            }
        }

        public static void StartCheck()
        {
            PerformanceCheck performanceCheck = new PerformanceCheck();


            Console.WriteLine("Starting Direct DCT Benchmark");
            Stopwatch stopwatch = Stopwatch.StartNew();

            FullImageDirectTask fullImageDirectTask = new FullImageDirectTask(performanceCheck._picture);
            int count = fullImageDirectTask.Call();
            long finishedAfter = stopwatch.ElapsedMilliseconds;
            Console.WriteLine("Finished after " + finishedAfter / 1000d + " seconds");
            Console.WriteLine("Direct DCT takes " + (double) finishedAfter / count + " ms/image");
            Console.WriteLine("Managed " + count + " images");


            Console.WriteLine("Starting Separated DCT Benchmark");
            stopwatch.Restart();
            FullImageSeparatedTask fullImageSeparatedTask = new FullImageSeparatedTask(performanceCheck._picture);
            count = fullImageSeparatedTask.Call();
            finishedAfter = stopwatch.ElapsedMilliseconds;
            Console.WriteLine("Finished after " + finishedAfter / 1000d + " seconds");
            Console.WriteLine("Separated DCT takes " + (double) finishedAfter / count + " ms/image");
            Console.WriteLine("Managed " + count + " images");


            Console.WriteLine("Starting Arai Benchmark");
            stopwatch.Restart();
            FullImageAraiTask fullImageAraiTask = new FullImageAraiTask(performanceCheck._picture);
            count = fullImageAraiTask.Call();
            finishedAfter = stopwatch.ElapsedMilliseconds;
            Console.WriteLine("Finished after " + finishedAfter / 1000d + " seconds");
            Console.WriteLine("Arai takes " + (double) finishedAfter / count + " ms/image");
            Console.WriteLine("Managed " + count + " images");
            stopwatch.Stop();
        }
    }
}