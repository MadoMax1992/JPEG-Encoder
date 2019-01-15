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
                const string pathFilename = "./images/Lena512x512";

                JpegEncoder.WithImageFromFile(pathFilename + ".ppm")
                    .ConvertToJpeg(1)
                    .WriteImageToDisk(pathFilename + ".jpg");
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e);
            }
        }


    }
}