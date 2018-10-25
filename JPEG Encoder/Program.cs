using System;
using System.IO;
using MathNet.Numerics.LinearAlgebra;

namespace JPEG_Encoder
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            var sample = new Image();

            sample.LoadPpm(@"img\black-white.ppm",
                6);
        
            sample.ChangeToYCbCr();
            
            Console.WriteLine("R: " + sample.R);
            Console.WriteLine("G: " + sample.G);
            Console.WriteLine("B: " + sample.B);
            Console.WriteLine("Y: " + sample.Y);
            Console.WriteLine("Cb: " + sample.Cb);
            Console.WriteLine("Cr: " + sample.Cr);
            
           
            
            sample.Cb = sample.SubSamplingToFactor(0, 2, sample.Cb);

            Console.WriteLine("Cb: " + sample.Cb);


        }
    }


    internal class Image
    {
        private string _magicNumber;
        private int _width;
        private int _height;
        private int _depth;
        
        public Matrix<double> R;
        public Matrix<double> G;
        public Matrix<double> B;
        public Matrix<double> Y;
        public Matrix<double> Cb;
        public Matrix<double> Cr;
       

        public void LoadPpm(string path, int stride)
        {
            var stream = new FileStream(path, FileMode.Open);

            var binReader = new BinaryReader(stream);

            var headerItemCount = 0;

            while (headerItemCount < 4)
            {
                
                var nextChar = (char) binReader.PeekChar();
                if (nextChar == '#')
                {
                    while (binReader.ReadChar() != '\n')
                    {
                    }
                }
                else if (char.IsWhiteSpace(nextChar))
                {
                    binReader.ReadChar();
                }
                else
                {                    
                    switch (headerItemCount)
                    {
                        case 0: 
                            var chars = binReader.ReadChars(2);
                            _magicNumber = chars[0] + chars[1].ToString();
                            headerItemCount++;
                            break;
                        case 1: // width
                            _width = ReadValue(binReader);
                            headerItemCount++;
                            break;
                        case 2: // height
                            _height = ReadValue(binReader);
                            headerItemCount++;
                            break;
                        case 3: // depth
                            _depth = ReadValue(binReader);
                            headerItemCount++;
                            break;
                        default:
                            throw new Exception("Error");
                            
                    }
                }
            }


            var matrixColumns = ((_width - 1) / stride + 1) * stride;
            var matrixRows = ((_height - 1) / stride + 1) * stride;
            

            R = Matrix<double>.Build.Dense(matrixRows, matrixColumns);
            G = Matrix<double>.Build.Dense(matrixRows, matrixColumns);
            B = Matrix<double>.Build.Dense(matrixRows, matrixColumns);
            
            for (var x = 0; x < _height; x++)
            {
                
                for (var y = 0; y < _width; y++)
                {
                    R[x, y] = ReadValue(binReader);
                    G[x, y] = ReadValue(binReader);
                    B[x, y] = ReadValue(binReader);
                }
            }
            
            binReader.Close();

            for (var x = 0; x < _height; x++)
            {

                for (var y = _width; y < matrixColumns; y++)
                {
                    R[x, y] = R[x, y - 1];
                    B[x, y] = B[x, y - 1];
                    G[x, y] = G[x, y - 1];

                }
            }
            
            for (var x = _height; x < matrixRows; x++)
            {
                for (var y = 0; y < matrixColumns; y++)
                {
                    R[x, y] = R[x - 1, y];
                    B[x, y] = B[x - 1, y];
                    G[x, y] = G[x - 1, y];
                }
            }            
        }
        
        private static int ReadValue(BinaryReader binReader)
        {
            var value = string.Empty;
            while (!char.IsWhiteSpace((char)binReader.PeekChar()))
            {
                value += binReader.ReadChar().ToString();
            }
            binReader.ReadByte();   // get rid of the whitespace.

            try
            {
                return int.Parse(value);
            }
            catch (System.FormatException e)
            {
                return 0;
            }



        }

        public void ChangeToYCbCr()
        {
            try
            {
                var rRowCount = R.RowCount;
                var rColumnCount = R.ColumnCount;
                Y = Matrix<double>.Build.Dense(rRowCount, rColumnCount);
                Cb = Matrix<double>.Build.Dense(rRowCount, rColumnCount);
                Cr = Matrix<double>.Build.Dense(rRowCount, rColumnCount);

                for (var x = 0; x < rRowCount; x++)
                {
                    for (var y = 0; y < rColumnCount; y++)
                    {
                        Y[x, y] = 0.299 * R[x, y] + 0.587 * G[x, y] + 0.114 * B[x, y];
                        Cb[x, y] = - 0.1687 * R[x, y] - 0.3312 * G[x, y] + 0.5 * B[x, y] + 255.0 / 2;
                        Cr[x, y] = 0.5 * R[x, y] - 0.4186 * G[x, y] - 0.0813 * B[x, y] + 255.0 / 2;
                    }
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine("Exception caught: {0}", e);
            }
            
        }

        // Methoden zum testen und zur Verstandniss
        public Matrix<double> MaxSubSamplingToFactor(int Factor, Matrix<double> channelMatrix)
        {
            var rowCount = channelMatrix.RowCount;
            var columnCount = channelMatrix.ColumnCount;
            if (Factor == 0) Factor = 1;
            var subSampleMatrix = Matrix<double>.Build.Dense(rowCount, columnCount);

            

            return subSampleMatrix;
        }
        public Matrix<double> SubSamplingToFactor(int xFactor, int yFactor, Matrix<double> channelMatrix)
        {
            try
            {
                var rRowCount = channelMatrix.RowCount;
                var rColumnCount = channelMatrix.ColumnCount;
            
                if (xFactor == 0) xFactor = 1;
                if (yFactor == 0) yFactor = 1;
            
                //Matrix wird hier verkleinert
                var matrixRows = (rRowCount / xFactor + (rRowCount % xFactor == 0 ? 0 : 1));
                var matrixColumns = (rColumnCount / yFactor + (rColumnCount % yFactor == 0 ? 0 : 1));
            
                var subSampledMatrix = Matrix<double>.Build.Dense(matrixRows, matrixColumns);
            
                for (int x = 0; x < rRowCount; x += xFactor)
                {
                    for (int y = 0; y < rColumnCount; y += yFactor)
                    {
                        subSampledMatrix[x / xFactor, y / yFactor] = channelMatrix[x, y];
                    }
                }

                return subSampledMatrix;
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine("Exception caught: {0}", e);
                throw;
            }
            
        }
    }
}
