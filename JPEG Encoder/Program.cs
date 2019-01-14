using System;
using System.IO;

namespace JPEG_Encoder
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            //PerformanceCheck.StartCheck();

            const string pathFilename = "../../../img/Lena32x32";
            try
            {
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