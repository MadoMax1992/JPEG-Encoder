using System;
using System.IO;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Complex;

namespace ConsoleApplication1
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Image sample = new Image();
            sample.loadPPM(@"img\red-blue-light.ppm");

        }
    }

    internal class Pixel
    {
        //aufsplitten in 3 kanäle
        private byte[] _data = new byte[3];
                
    }

    internal class Cluster
    {
        private Pixel[,] _data = new Pixel[16, 16];

        public Pixel get(int x, int y)
        {   
            return _data[x, y];
        }
    }

    internal class Image
    {
        public Matrix<double> R;
        public Matrix<double> G;
        public Matrix<double> B;


        private Cluster[,] _data;
        public Cluster get(int x, int y)
        {
            return _data[x, y];
        }

        public Pixel getPixel(int x, int y)
        {
            //Zugriff auf ein zwei D Array wie in lektion 86 (C++ Kurs)
            return this.get(x / 16, y / 16).get(x % 16, y % 16);
        }

        public void loadPPM(string path)
        {
            System.IO.StreamReader file =
                new System.IO.StreamReader(path);
            if (file.ReadLine() == "P3")
            {
                string line;

                while ((line = file.ReadLine())[0] == '#') ;

                int width = int.Parse(line.Split(null)[0]);
                int height = int.Parse(line.Split(null)[1]);
                int depth = int.Parse(file.ReadLine());

                //System.Console.WriteLine("" + width + height + depth);
                char number;
                if ((file.Peek())== 32){
                    file.Read();
                }
                else
                {
                    number = (char) file.Read();
                    System.Console.WriteLine(number);
                }


                string content = file.ReadToEnd();

                //R = Matrix<double>.Build.Dense(16, 16);
                //G = Matrix<double>.Build.Dense(16, 16);
                //B = Matrix<double>.Build.Dense(16, 16);

                //for (var x = 0; x < 16; x++)
                //{

                //    for (var y = 0; y < 16; y++)
                //    {
                //        R[x, y] = file.Read;
                //        G[x, y] = ReadValue(binReader);
                //        B[x, y] = ReadValue(binReader);
                //    }
                //}


                System.Console.WriteLine(content);
                

                //while ((line = file.ReadLine()) != null)
                //{
                    
                //    System.Console.WriteLine(line);

                //}
            }
            System.Console.ReadLine();
            file.Close();
        }

    }
}