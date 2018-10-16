using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPEG_Encoder
{
    class Program
    {
        static void Main(string[] args)
        {


            //2-D-Datenstruktur ?
            List<RGBPixel> RGBImage = new List<RGBPixel>();
            RGBPixel data = new RGBPixel();

            // Example

            //data.greenValue = 5;
            //data.blueValue = 5;
            //data.redValue = 5;
            //data.xPixelCord = 1;
            //data.yPixelCord = 2;

            //Data.Add(data);



            string line;


            // Öffnet ein Stream, liest die Datei ein und gibt Zeilenweise die rgb pixel aus

            try
            {
                System.IO.StreamReader file =
                new System.IO.StreamReader(@"C:\Users\Max-FH\Documents\Visual Studio 2017\Projects\JPEG Encoder\Bilder\red-blue-light.ppm");
                while ((line = file.ReadLine()) != null)
                {
                    System.Console.WriteLine(line);

                }
                file.Close();
            }
            catch (Exception ex)
            {
                //Aussagekräftige meldung
            }


            // lässt die console offen  
            System.Console.ReadLine();

        }
    }
}
