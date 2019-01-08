using System;
using System.Collections.Generic;
using System.Linq;
using CenterSpace.NMath.Core;

namespace JPEG_Encoder.image.colors
{
    public class ColorChannel
    {
        //TODO set private and change cosineTransformation
        private readonly DoubleMatrix[] _blocks;
        private readonly int _height;
        private readonly int _heightInBlocks;
        private readonly int _width;
        private readonly int _widthInBlocks;

        public ColorChannel(int width, int height)
        {
            _height = height;
            _width = width;
            _widthInBlocks = (int) Math.Ceiling(width / 8d);
            _heightInBlocks = (int) Math.Ceiling(height / 8d);
            _blocks = new DoubleMatrix[_heightInBlocks * _widthInBlocks];
            for (int i = 0; i < _heightInBlocks * _widthInBlocks; i++) _blocks[i] = new DoubleMatrix(8, 8);
        }

        public void SetPixel(int x, int y, double value)
        {
            _blocks[GetPlainIndexOfBlock(x / 8, y / 8)][y % 8, x % 8] = value;
        }

        public double GetPixel(int x, int y)
        {
            return _blocks[GetPlainIndexOfBlock(x / 8, y / 8)][y % 8, x % 8];
        }

        public int GetPlainIndexOfBlock(int x, int y)
        {
            return x + y * (_width / 8);
        }

        public DoubleMatrix GetBlock(int x, int y)
        {
            return _blocks[GetPlainIndexOfBlock(x, y)];
        }

        public DoubleMatrix GetBlock(int index)
        {
            return _blocks[index];
        }

        public List<DoubleMatrix> GetBlocks(int start, int end)
        {
            return _blocks.ToList().GetRange(start, end);
        }

        public int GetNumOfBlocks()
        {
            return _blocks.GetLength(0);
        }

        public int GetHeight()
        {
            return _height;
        }

        public int GetWidth()
        {
            return _width;
        }

        public int GetWidthInBlocks()
        {
            return _widthInBlocks;
        }

        public int GetHeightInBlocks()
        {
            return _heightInBlocks;
        }

        public void FillWithMockData()
        {
            for (int y = 0; y < GetHeight(); y++)
            for (int x = 0; x < GetWidth(); x++)
            {
                int value = x % 256;
                SetPixel(x, y, value);
            }
        }
    }
}