using System;
using System.IO;
using System.Text;
using BitStreams;
using MathNet.Numerics.LinearAlgebra;

namespace JPEG_Encoder
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            // flip first bit of int
            // y&~(1 << 1) 

            var filename = "../../../img/blackbuck_ascii.ppm";
//            
            var testImage = new Image(filename, 8);
            
            testImage.writeJpeg();
//            
//
//            filename = filename.Substring(0, filename.Length - 4);
//            
//            testImage.ChangeToYCbCr();
//            
//            testImage.WriteImageOnlyFromY(filename);
//            testImage.WriteImageOnlyFromCb(filename);
//            testImage.WriteImageOnlyFromCr(filename);
//            testImage.WriteImage(filename);
//            
//            testImage.Cr = testImage.SubSamplingTo411(2, testImage.Cr, false);
//            testImage.Cb = testImage.SubSamplingTo411(2, testImage.Cb, false);
//
//            testImage.Cr = testImage.ReSamplingTo411(2, testImage.Cr);
//            testImage.Cb = testImage.ReSamplingTo411(2, testImage.Cb);
//            
//            testImage.WriteImageOnlyFromCb(filename + "subbed");
//            testImage.WriteImageOnlyFromCr(filename + "subbed");
//            testImage.WriteImage(filename + "subbed");

//            FileStream fstream = new FileStream("../../../img/test.ppm", FileMode.OpenOrCreate);

            MemoryStream mStream = new MemoryStream();

            mStream.SetLength(1250000);
            BitStream bitStream = new BitStream(mStream);
            

            for (int i = 0; i < 10000000; i++)
            {
                bitStream.WriteBit(1);
            }

            bitStream.SaveStreamAsFile("../../../img/test.txt");

            FileStream fileStream = new FileStream("../../../img/test.txt", FileMode.Open);
            
            BitStream bitStream2 = new BitStream(fileStream);

            Console.WriteLine((int) bitStream2.ReadBit());

        }
    }


    internal class Image
    {
        private readonly string _magicNumber;
        private readonly int _width;
        private readonly int _height;
        private readonly int _depth;
        
        public Matrix<double> R;
        public Matrix<double> G;
        public Matrix<double> B;
        public Matrix<double> Y;
        public Matrix<double> Cb;
        public Matrix<double> Cr;


        public Image(string path, int stride)
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
            while (char.IsWhiteSpace((char)binReader.PeekChar()))
            {
                binReader.ReadByte();
            };
            return int.Parse(value);
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

        public Matrix<double> SubSamplingToFactor(int factor, Matrix<double> channelMatrix, bool average)
        {
            try
            {
                var rRowCount = channelMatrix.RowCount;
                var rColumnCount = channelMatrix.ColumnCount;
            
                // Remove for performance optimization
                if (factor == 0) factor = 1;
            
                var matrixRows = rRowCount / factor;
                var matrixColumns = rColumnCount / factor;
            
                var subSampledMatrix = Matrix<double>.Build.Dense(matrixRows, matrixColumns);
            
                for (var x = 0; x < rRowCount; x += factor)
                {
                    for (var y = 0; y < rColumnCount; y += factor)
                    {
                        subSampledMatrix[x / factor, y / factor] = channelMatrix[x, y];
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
        
        public Matrix<double> SubSamplingTo411(int factor, Matrix<double> channelMatrix, bool average)
        {
            try
            {
                var rRowCount = channelMatrix.RowCount;
                var rColumnCount = channelMatrix.ColumnCount;
            
                // Remove for performance optimization
                if (factor == 0) factor = 1;
            
                var matrixRows = rRowCount;
                var matrixColumns = rColumnCount / factor;
            
                var subSampledMatrix = Matrix<double>.Build.Dense(matrixRows, matrixColumns);
            
                for (var x = 0; x < rRowCount; x++)
                {
                    for (var y = 0; y < rColumnCount; y += factor)
                    {
                        subSampledMatrix[x, y / factor] = channelMatrix[x, y];
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

        public Matrix<double> ReSamplingToFactor(int factor, Matrix<double> channelMatrix)
        {
            var reSampledMatrix = Matrix<double>.Build.Dense(Y.RowCount, Y.ColumnCount);

            try
            {

                for (var x = 0; x < reSampledMatrix.RowCount; x++)
                {
                    for (var y = 0; y < reSampledMatrix.ColumnCount; y++)
                    {
                        var index = 0;
                        while (index < factor)
                        {
                            reSampledMatrix[x, y] = channelMatrix[x / factor, y / factor];
                            index++;
                        }
                    }
                }

                return reSampledMatrix;

            }
            catch (Exception e)
            {
                Console.WriteLine("Exception caught: {0}", e);
                if ( e is ArgumentOutOfRangeException) Console.WriteLine(reSampledMatrix);
                throw;
            }
        }
        
        public Matrix<double> ReSamplingTo411(int factor, Matrix<double> channelMatrix)
        {
            var reSampledMatrix = Matrix<double>.Build.Dense(Y.RowCount, Y.ColumnCount);

            try
            {

                for (var x = 0; x < reSampledMatrix.RowCount; x++)
                {
                    for (var y = 0; y < reSampledMatrix.ColumnCount; y++)
                    {
                        var index = 0;
                        while (index < factor)
                        {
                            reSampledMatrix[x, y] = channelMatrix[x, y / factor];
                            index++;
                        }
                    }
                }

                return reSampledMatrix;

            }
            catch (Exception e)
            {
                Console.WriteLine("Exception caught: {0}", e);
                if ( e is ArgumentOutOfRangeException) Console.WriteLine(reSampledMatrix);
                throw;
            }
        }


        public void WriteImageOnlyFromY(string filename)
        {
            filename += "Y.ppm";

            var writer = new StreamWriter(filename);
            writer.WriteLine(_magicNumber);
            writer.WriteLine(_width + " " + _height);
            writer.WriteLine(_depth);
            
            for (var x = 0; x < _height; x++)
            {
                
                for (var y = 0; y < _width; y++)
                {
                    writer.WriteLine((int)Y[x, y]);
                    writer.WriteLine((int)Y[x, y]);
                    writer.WriteLine((int)Y[x, y]);
                }
            }
            
            writer.Close();
        }
            
        public void WriteImageOnlyFromCb(string filename)
        {
            filename += "Cb.ppm";
            var writer = new StreamWriter(filename);
            writer.WriteLine(_magicNumber);
            writer.WriteLine(_width + " " + _height);
            writer.WriteLine(_depth);
            
            for (var x = 0; x < _height; x++)
            {
                
                for (var y = 0; y < _width; y++)
                {
                    writer.WriteLine((int)Cb[x, y]);
                    writer.WriteLine((int)(Cb[x, y] * -0.344136));
                    writer.WriteLine((int)(Cb[x, y] * 1.772));
                }
            }
            
            writer.Close();
        }  
        
        public void WriteImageOnlyFromCr(string filename)
        {
            filename += "Cr.ppm";
            var writer = new StreamWriter(filename);
            writer.WriteLine(_magicNumber);
            writer.WriteLine(_width + " " + _height);
            writer.WriteLine(_depth);
            
            for (var x = 0; x < _height; x++)
            {
                
                for (var y = 0; y < _width; y++)
                {
                    writer.WriteLine((int)(Cr[x, y] * 1.402));
                    writer.WriteLine((int)(Cr[x, y] * -0.714136));
                    writer.WriteLine((int)(Cr[x, y]));
                }
            }
            
            writer.Close();
        }

        public void WriteImage(string filename)
        {
            filename += "New.ppm";
            var writer = new StreamWriter(filename);
            writer.WriteLine(_magicNumber);
            writer.WriteLine(_width + " " + _height);
            writer.WriteLine(_depth);
            
            for (var x = 0; x < _height; x++)
            {
                
                for (var y = 0; y < _width; y++)
                {
                    writer.WriteLine((int)(Y[x, y] + (Cb[x, y] - 127.5) * 0 + (Cr[x, y] - 127.5) * 1.402));
                    writer.WriteLine((int)(Y[x, y] + (Cb[x, y] - 127.5) * -0.344136 + (Cr[x, y] - 127.5) * -0.714136));
                    writer.WriteLine((int)(Y[x, y] + (Cb[x, y] - 127.5) * 1.772 + (Cr[x, y] - 127.5) * 0));
                }
            }
            
            writer.Close();
        }

        public void writeJpeg()
        {
            MemoryStream mStream = new MemoryStream();

            mStream.SetLength(1250000);
            BitStream bitStream = new BitStream(mStream);
            
            // SOI
            bitStream.WriteByte(0xff);
            bitStream.WriteByte(0xd8);
            
            // APP0
            bitStream.WriteByte(0xff);
            bitStream.WriteByte(0xe0);
            
            // Laenge des Segment
            bitStream.WriteByte(0x00);
            bitStream.WriteByte(0x10);
            // JFIF
            bitStream.WriteByte(0x4a);
            bitStream.WriteByte(0x46);
            bitStream.WriteByte(0x49);
            bitStream.WriteByte(0x46);
            bitStream.WriteByte(0x00);
            
            // Major Minor Revision
            bitStream.WriteByte(0x01);
            bitStream.WriteByte(0x01);
            
            // Pixelgroesse
            bitStream.WriteByte(0x00);
            
            // x-Dichte
            bitStream.WriteByte(0x00);
            bitStream.WriteByte(0x48);
            
            // y-Dichte
            bitStream.WriteByte(0x00);
            bitStream.WriteByte(0x48);
            
            bitStream.WriteByte(0x00);
            bitStream.WriteByte(0x00);
            // Ende APP0
            
            // SOF0
            bitStream.WriteByte(0xFF);
            bitStream.WriteByte(0xC0);
            
            // Frame Header Length
            bitStream.WriteByte(0x00);
            bitStream.WriteByte(0x11);
            
            // Prescision
            bitStream.WriteByte(0x08);

            // Bildgroesse y
            bitStream.WriteByte(0x00);
            bitStream.WriteByte(0xFF);

            // Bildgroesse x
            bitStream.WriteByte(0x00);
            bitStream.WriteByte(0xFF);
            
            // Komponente 1 
            bitStream.WriteByte(0x01);
            bitStream.WriteByte(0x22);
            bitStream.WriteByte(0x00);
            
            // Komponente 2
            bitStream.WriteByte(0x02);
            bitStream.WriteByte(0x11);
            bitStream.WriteByte(0x01);
            
            // Komponente 3
            bitStream.WriteByte(0x03);
            bitStream.WriteByte(0x11);
            bitStream.WriteByte(0x01);
            // ENDE SOF0
            
            
            // EOI
            bitStream.WriteByte(0xFF);
            bitStream.WriteByte(0xD9);

            
            bitStream.SaveStreamAsFile("../../../img/testImage.jpg");
        }
    }
}