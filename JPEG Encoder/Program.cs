using System;

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
        private Cluster[,] _data;
        public Cluster get(int x, int y)
        {
            return _data[x, y];
        }

        public Pixel getPixel(int x, int y)
        {
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

                System.Console.WriteLine("" + width + height + depth);

                while ((line = file.ReadLine()) != null)
                {
                    System.Console.WriteLine(line);

                }
            }
            System.Console.ReadLine();
            file.Close();
        }

    }
}