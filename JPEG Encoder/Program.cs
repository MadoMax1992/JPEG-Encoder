using System;
using System.IO;
using System.Net.Configuration;
using System.Security.Policy;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Random;

namespace ConsoleApplication1
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Image sample = new Image();

            sample.loadPPM(@"/Users/Oli/Library/Mobile Documents/com~apple~CloudDocs/Studium/18_19_Wintersemester/Digitale Medien- und Multimediatechniken/Übung/JPEG-Encoder/img/TestBild2.ppm",
                3);
        
            sample.changeToYCbCr();
            
            sample.Cb = sample.subSamplingToFactor(0, 2, sample.Cb);
            
            Console.WriteLine(sample.Cb);
            
         
        }
    }


    internal class Image
    {
        private string magicNumber;
        private int width;
        private int height;
        private int depth;
        
        public Matrix<double> R;
        public Matrix<double> G;
        public Matrix<double> B;
        public Matrix<double> Y;
        public Matrix<double> Cb;
        public Matrix<double> Cr;
       

        public void loadPPM(string path, int stride)
        {
            FileStream stream = new FileStream(path, FileMode.Open);

            BinaryReader binReader = new BinaryReader(stream);

            int headerItemCount = 0;

            while (headerItemCount < 4)
            {
                
                char nextChar = (char) binReader.PeekChar();
                if (nextChar == '#')
                {
                    while (binReader.ReadChar() != '\n') ;
                }
                else if (Char.IsWhiteSpace(nextChar))
                {
                    binReader.ReadChar();
                }
                else
                {
                    switch (headerItemCount)
                    {
                        case 0: 
                            char[] chars = binReader.ReadChars(2);
                            this.magicNumber = chars[0].ToString() + chars[1].ToString();
                            headerItemCount++;
                            break;
                        case 1: // width
                            this.width = ReadValue(binReader);
                            headerItemCount++;
                            break;
                        case 2: // height
                            this.height = ReadValue(binReader);
                            headerItemCount++;
                            break;
                        case 3: // depth
                            this.depth = ReadValue(binReader);
                            headerItemCount++;
                            break;
                        default:
                            throw new Exception("Error");
                            
                    }
                }
            }
            

            int matrixColumns = (width / stride + (width % stride == 0 ? 0 : 1)) * stride;
            int matrixRows = (height / stride + (height % stride == 0 ? 0 : 1)) * stride;
            

            this.R = Matrix<double>.Build.Dense(matrixRows, matrixColumns);
            this.G = Matrix<double>.Build.Dense(matrixRows, matrixColumns);
            this.B = Matrix<double>.Build.Dense(matrixRows, matrixColumns);

            int index = 0;
            string nextValue = string.Empty;
            
            for (int x = 0; x < height; x++)
            {
                
                for (int y = 0; y < width; y++)
                {
                    index = 0;
                    while (index < 3)
                    {
                        nextValue = string.Empty;
                        if (binReader.BaseStream.Position != binReader.BaseStream.Length)
                        {
                            if (binReader.PeekChar() == '\n')
                            {
                                binReader.ReadChar();
                            };

                            while (!Char.IsWhiteSpace((char) binReader.PeekChar()))
                            {
                                nextValue += binReader.ReadChar();
                            }
                            
                            switch (index)
                            {
                                case 0:
                                    this.R[x, y] = double.Parse(nextValue);
                                    index = 1;
                                    break;
                                case 1:
                                    this.G[x, y] = double.Parse(nextValue);
                                    index = 2;
                                    break;
                                case 2:
                                    this.B[x, y] = double.Parse(nextValue);
                                    index = 3;
                                    break;
                            } 
                        }
                        
                    }
                }
            }
            
            binReader.Close();

            for (int x = 0; x < height; x++)
            {

                for (int y = width; y < matrixColumns; y++)
                {
                    R[x, y] = R[x, y - 1];
                    B[x, y] = B[x, y - 1];
                    G[x, y] = G[x, y - 1];

                }
            }
            
            for (int x = height; x < matrixRows; x++)
            {
                for (int y = 0; y < matrixColumns; y++)
                {
                    R[x, y] = R[x - 1, y];
                    B[x, y] = B[x - 1, y];
                    G[x, y] = G[x - 1, y];
                }
            }
        }
        
        private int ReadValue(BinaryReader binReader)
        {
            string value = string.Empty;
            while (!Char.IsWhiteSpace((char)binReader.PeekChar()))
            {
                value += binReader.ReadChar().ToString();
            }
            binReader.ReadByte();   // get rid of the whitespace.
            return int.Parse(value);
        }

        public void changeToYCbCr()
        {
            var rRowCount = this.R.RowCount;
            var rColumnCount = this.R.ColumnCount;
            this.Y = Matrix<double>.Build.Dense(rRowCount, rColumnCount);
            this.Cb = Matrix<double>.Build.Dense(rRowCount, rColumnCount);
            this.Cr = Matrix<double>.Build.Dense(rRowCount, rColumnCount);

            for (int x = 0; x < rRowCount; x++)
            {
                for (int y = 0; y < rColumnCount; y++)
                {
                    Y[x, y] = 0.299 * R[x, y] + 0.587 * G[x, y] + 0.114 * B[x, y];
                    Cb[x, y] = - 0.1687 * R[x, y] - 0.3312 * G[x, y] + 0.5 * B[x, y] + 128.0;
                    Cr[x, y] = 0.5 * R[x, y] - 0.4186 * G[x, y] - 0.0813 * B[x, y] + 128.0;
                }
            }
        }

        public Matrix<double> subSamplingToFactor(int xFactor, int yFactor, Matrix<double> channelMatrix)
        {
            var rRowCount = channelMatrix.RowCount;
            var rColumnCount = channelMatrix.ColumnCount;
            
            Console.WriteLine(channelMatrix);

            if (xFactor == 0) xFactor = 1;
            if (yFactor == 0) yFactor = 1;
            
            var matrixRows = (rRowCount / xFactor + (rRowCount % xFactor == 0 ? 0 : 1));
            var matrixColumns = (rColumnCount / yFactor + (rColumnCount % yFactor == 0 ? 0 : 1));
            
            Matrix<double> subSampledMatrix = Matrix<double>.Build.Dense(matrixRows, matrixColumns);
            
            for (int x = 0; x < rRowCount; x += xFactor)
            {
                for (int y = 0; y < rColumnCount; y += yFactor)
                {
                    subSampledMatrix[x / xFactor, y / yFactor] = channelMatrix[x, y];
                }
            }

            return subSampledMatrix;
        }
    }
}