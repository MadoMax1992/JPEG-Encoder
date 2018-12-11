using System;
using System.Collections.Generic;

namespace JPEG_Encoder.encoding.huffman.model
{
    public class HuffmanTreeLeaf : HuffmanTreeComponent
    {
        private readonly int _symbol;
        private readonly double _frequency;

        public HuffmanTreeLeaf(int symbol, double frequency)
        {
            _symbol = symbol;
            _frequency = frequency;
        }

        public override double GetFrequency()
        {
            return _frequency;
        }

        public override void SetLeft(HuffmanTreeComponent newLeft)
        {
        }

        public override void SetRight(HuffmanTreeComponent newRight)
        {
        }

        public int GetSymbol()
        {
            return _symbol;
        }

        public override HuffmanTreeComponent GetLeft()
        {
            return null;
        }

        public override HuffmanTreeComponent GetRight()
        {
            return null;
        }

        public override int GetDepth(int currentDepth)
        {
            return currentDepth;
        }

        public override void FillCodeBook(List<CodeWord> codeWords, int currentCode, int currentLength)
        {
            codeWords.Add(new CodeWord(_symbol, currentCode, currentLength));
        }

        public override void PrintCode(string currentCode)
        {
            Console.WriteLine(_symbol + ": " + currentCode);
        }

        public override string ToString()
        {
            return _symbol.ToString();
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
            {
                return true;
            }

            if (obj == null || GetType().Name != obj.GetType().Name)
            {
                return false;
            }

            HuffmanTreeLeaf that = (HuffmanTreeLeaf) obj;

            return _symbol == that._symbol;
        }

        public override int GetHashCode()
        {
            return _symbol;
        }
    }
}