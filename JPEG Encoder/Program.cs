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
                //const string pathFilename = "./images/Lena1024x1024";
                //const string pathFilename = "./images/Lena512x512";
                //const string pathFilename = "./images/Lena700x700";

                const string basepath = "../../../img/";
                //const string basepath = "./images/";
                const string pathFilename = basepath + "Toronto-4K";


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