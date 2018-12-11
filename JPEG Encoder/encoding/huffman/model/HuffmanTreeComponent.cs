using System;
using System.Collections.Generic;

namespace JPEG_Encoder.encoding.huffman.model
{
    public abstract class HuffmanTreeComponent : IComparable<HuffmanTreeComponent>
    {
        public abstract double GetFrequency();

        public abstract void SetLeft(HuffmanTreeComponent newLeft);

        public abstract void SetRight(HuffmanTreeComponent newRight);

        public abstract HuffmanTreeComponent GetLeft();

        public abstract HuffmanTreeComponent GetRight();

        public abstract int GetDepth(int currentDepth);

        public abstract void FillCodeBook(List<CodeWord> codeWords, int currentCode, int currentLength);

        public abstract void PrintCode(string currentCode);

        public int CompareTo(HuffmanTreeComponent other)
        {
            return GetFrequency().CompareTo(other.GetFrequency());
        }
    }
}