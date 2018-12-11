using System;
using System.Collections.Generic;
using System.Linq;
using CenterSpace.NMath.Core;
using MathNet.Numerics.LinearAlgebra;

namespace JPEG_Encoder.image.colors
{
    public class ColorChannel
    {
        private DoubleMatrix[] blocks;
        private int height;
        private int width;
        private int widthInBlocks;
        private int heightInBlocks;

        public ColorChannel(int width, int height)
        {
            this.height = height;
            this.width = width;
            widthInBlocks = (int) Math.Ceiling(width / 8d);
            heightInBlocks = (int) Math.Ceiling(height / 8d);
            blocks = new DoubleMatrix[heightInBlocks * widthInBlocks];
            for (int i = 0; i < heightInBlocks * widthInBlocks; i++)
            {
                blocks[i] = new DoubleMatrix(8, 8);
            }
        }

        public void SetPixel(int x, int y, double value)
        {
            blocks[GetPlainIndexOfBlock(x / 8, y / 8)][y % 8, x % 8] = value;
        }

        public double GetPixel(int x, int y)
        {
            return blocks[GetPlainIndexOfBlock(x / 8, y / 8)][y % 8, x % 8];
        }

        public int GetPlainIndexOfBlock(int x, int y)
        {
            return (x + (y * (width / 8)));
        }

        public DoubleMatrix GetBlock(int x, int y)
        {
            return blocks[GetPlainIndexOfBlock(x, y)];
        }

        public DoubleMatrix GetBlock(int index)
        {
            return blocks[index];
        }

        public List<DoubleMatrix> GetBlocks(int start, int end)
        {
            return blocks.ToList().GetRange(start, end);
        }

        public int GetNumOfBlocks()
        {
            return blocks.GetLength(0);
        }

        public int GetHeight()
        {
            return height;
        }

        public int GetWidth()
        {
            return width;
        }

        public int GetWidthInBlocks()
        {
            return widthInBlocks;
        }

        public int GetHeightInBlocks()
        {
            return heightInBlocks;
        }

        public void FillWithMockData()
        {
            for (int y = 0; y < GetHeight(); y++)
            {
                for (int x = 0; x < GetWidth(); x++)
                {
                    int value = x % 256;
                    SetPixel(x, y, value);
                }
            }
        }
    }
}