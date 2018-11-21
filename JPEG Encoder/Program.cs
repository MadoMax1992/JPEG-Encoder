using System;

namespace JPEG_Encoder
{
    static class Program
    {
        public static void Main(string[] args)
        {
            // flip first bit of int
            // y&~(1 << 1) 

            const string filename = "../../../img/TestPicture2.ppm";

            var testImage = new Image(filename, 4);

//            Image.WriteJpeg();
//            
//            
//            testImage.ChangeToYCbCr();
//
            var array = testImage.TransformMatrixToArray(testImage.R);
            array = new[] {255, 255, 25, 25, 25, 0, 10, 10, 0, 0, 0, 255, 255};

            var tree = new HuffmanTree();

            tree.Build(array);
            tree.ShiftMostRightSymbol();

            var encoded = tree.Encode(array);

            Console.WriteLine("Encoded: ");

            for (int i = 0; i < encoded.FullBitLength; i++)
            {
                Console.Write(encoded.ReadBit().AsInt() + " ");
            }
            encoded.Seek(0,0);

            Console.WriteLine();
            var decoded = tree.Decode(encoded);
            Console.WriteLine("Decoded: ");
            foreach (var i in decoded)
            {
                Console.Write(i + " ");
            }

//            filename = filename.Substring(0, filename.Length - 4);
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
            //
            //
            //            var mStream = new MemoryStream();
            //            var bitStream = new BitStream(mStream);
            //
            //            for (var i = 0; i < 10; i++)
            //            {
            //                bitStream.GetStream().SetLength(bitStream.GetStream().Length + 1);
            //
            //                bitStream.WriteBit(1);
            //            }
            //            
            //            Console.WriteLine(bitStream.Length);
            //
            //            bitStream.SaveStreamAsFile("../../../img/test.txt");
            //
            //            var fileStream = new FileStream("../../../img/test.txt", FileMode.Open);
            //            
            //            var bitStream2 = new BitStream(fileStream);
            //
            //            for (var i = 0; i < 10; i++)
            //            {
            //                Console.WriteLine((int) bitStream2.ReadBit());
            //            }
        }

        private static void HuffmanTreeTest()
        {
            var input = new[]
            {
                1, 5, 4, 3, 4, 5, 20, 54, 65, 65, 65, 4, 33, 23, 24, 26, 27, 2, 3, 4, 2, 1, 2, 3, 2, 1, 2, 3, 2, 3, 4,
                5, 6, 4, 3, 2, 1, 2, 3, 5, 6, 12, 10, 12, 13, 14, 15, 17, 17, 20, 12, 32, 43, 43, 43, 32, 32, 32
            };

            var huffmanTree = new HuffmanTree();

            huffmanTree.Build(input);

            foreach (var symbol in huffmanTree.Frequencies)
            {
                Console.Write(symbol.Key + ": ");
                Console.WriteLine(symbol.Value);
            }

            var input2 = new[] {1};

            var encoded = huffmanTree.Encode(input2);

            Console.WriteLine("Encoded: ");
            for (int i = 0; i < encoded.Length; i++)
            {
                Console.Write(encoded.ReadBit().AsInt() + " ");
            }

            Console.WriteLine();

            var decoded = huffmanTree.Decode(encoded);
            Console.WriteLine("Decoded: ");
            foreach (var i in decoded)
            {
                Console.Write(i + " ");
            }

            Console.WriteLine();
            Console.WriteLine("End");
        }
    }
}