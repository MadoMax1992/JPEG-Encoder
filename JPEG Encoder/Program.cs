using System;
using System.IO;

namespace JPEG_Encoder
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            //PerformanceCheck.StartCheck();

            try
            {
                JpegEncoder.WithImageFromFile("../../../img/Lenna16x16.ppm")
                    .ConvertToJpeg(1)
                    .WriteImageToDisk();
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e);
            }

        }


    }
}