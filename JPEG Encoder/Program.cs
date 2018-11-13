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
            
            
            testImage.ChangeToYCbCr();

            var array = testImage.TransformMatrixToArray(testImage.R);

            var tree = new HuffmanTree();
            
            tree.Build(array);

            var encoded = tree.Encode(array);

            var decoded = tree.Decode(encoded);
//            Console.WriteLine("Decoded: ");
//            foreach (var i in decoded)
//            {
//                Console.Write(i + " ");
//            }

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


            //            HuffmanTreeTest();


        }

        private static void HuffmanTreeTest()
        {
            var input = new int[] {255, 255, 255, 254, 224, 243, 254, 10, 21, 235};
//            var input = "test";

            var huffmanTree = new HuffmanTree();

            huffmanTree.Build(input);
            Console.WriteLine(huffmanTree.Root.Right.Left.Symbol);
            foreach (var symbol in huffmanTree.Frequencies)
            {
                Console.Write(symbol.Key + ": ");
                Console.WriteLine(symbol.Value);
            }

            var encoded = huffmanTree.Encode(input);

            Console.WriteLine("Encoded: ");
            foreach (bool bit in encoded)
            {
                Console.Write((bit ? 1 : 0) + " ");
            }

            Console.WriteLine();

            var decoded = huffmanTree.Decode(encoded);
            Console.WriteLine("Decoded: ");
            foreach (var i in decoded)
            {
                Console.Write(i + " ");
            }

            Console.WriteLine("End");
        }
    }
}