using System;
using System.Collections.Generic;
using Microsoft.SqlServer.Server;

namespace JPEG_Encoder.encoding.huffman.model
{
    public class HuffmanTreeNode : HuffmanTreeComponent
    {
        private HuffmanTreeComponent left;
        private HuffmanTreeComponent right;

        public HuffmanTreeNode()
        {
            left = null;
            right = null;
        }

        public HuffmanTreeNode(HuffmanTreeComponent left, HuffmanTreeComponent right)
        {
            this.left = left;
            this.right = right;
        }

        public override double GetFrequency()
        {
            return left.GetFrequency() + right.GetFrequency();
        }

        public override void SetLeft(HuffmanTreeComponent newLeft)
        {
            left = newLeft;
        }

        public override void SetRight(HuffmanTreeComponent newRight)
        {
            right = newRight;
        }

        public override HuffmanTreeComponent GetLeft()
        {
            return left;
        }

        public override HuffmanTreeComponent GetRight()
        {
            return right;
        }

        public override int GetDepth(int currentDepth)
        {
            return Math.Max(left.GetDepth(currentDepth + 1), right.GetDepth(currentDepth + 1));
        }

        public override void FillCodeBook(List<CodeWord> codeWords, int currentCode, int currentLength)
        {
            left.FillCodeBook(codeWords, (currentCode << 1) + 0, currentLength + 1);
            right.FillCodeBook(codeWords, (currentCode << 1) + 1, currentLength + 1);        }

        public override void PrintCode(string currentCode)
        {
            left.PrintCode(currentCode + "0");
            right.PrintCode(currentCode + "1");
        }

        public override string ToString()
        {
            string result = "Node(";
            result += left.ToString() + ",";
            result += right.ToString();
            return result + ")";
        }
    }
}