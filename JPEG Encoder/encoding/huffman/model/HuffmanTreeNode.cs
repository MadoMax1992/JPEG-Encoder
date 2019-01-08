using System;
using System.Collections.Generic;

namespace JPEG_Encoder.encoding.huffman.model
{
    public class HuffmanTreeNode : HuffmanTreeComponent
    {
        private HuffmanTreeComponent _left;
        private HuffmanTreeComponent _right;

        public HuffmanTreeNode()
        {
            _left = null;
            _right = null;
        }

        public HuffmanTreeNode(HuffmanTreeComponent left, HuffmanTreeComponent right)
        {
            _left = left;
            _right = right;
        }

        public override double GetFrequency()
        {
            return _left.GetFrequency() + _right.GetFrequency();
        }

        public override void SetLeft(HuffmanTreeComponent newLeft)
        {
            _left = newLeft;
        }

        public override void SetRight(HuffmanTreeComponent newRight)
        {
            _right = newRight;
        }

        public override HuffmanTreeComponent GetLeft()
        {
            return _left;
        }

        public override HuffmanTreeComponent GetRight()
        {
            return _right;
        }

        public override int GetDepth(int currentDepth)
        {
            return Math.Max(_left.GetDepth(currentDepth + 1), _right.GetDepth(currentDepth + 1));
        }

        public override void FillCodeBook(List<CodeWord> codeWords, int currentCode, int currentLength)
        {
            _left.FillCodeBook(codeWords, (currentCode << 1) + 0, currentLength + 1);
            _right.FillCodeBook(codeWords, (currentCode << 1) + 1, currentLength + 1);
        }

        public override void PrintCode(string currentCode)
        {
            _left.PrintCode(currentCode + "0");
            _right.PrintCode(currentCode + "1");
        }

        public override string ToString()
        {
            string result = "Node(";
            result += _left + ",";
            result += _right.ToString();
            return result + ")";
        }
    }
}