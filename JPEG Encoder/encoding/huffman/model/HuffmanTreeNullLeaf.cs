using System.Collections.Generic;

namespace JPEG_Encoder.encoding.huffman.model
{
    public class HuffmanTreeNullLeaf : HuffmanTreeLeaf
    {
        public HuffmanTreeNullLeaf() : base(int.MinValue, 0.0)
        {
        }

        public override void PrintCode(string currentCode)
        {
        }

        public override void FillCodeBook(List<CodeWord> codeWords, int currentCode, int currentLength)
        {
        }

        public override string ToString()
        {
            return "null";
        }
    }
}