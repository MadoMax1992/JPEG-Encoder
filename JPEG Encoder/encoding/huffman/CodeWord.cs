using System;
using BitStreams;

namespace JPEG_Encoder.encoding.huffman
{
    public class CodeWord : IComparable<CodeWord>
    {
        private readonly int _code;
        private readonly int _length;
        private readonly int _symbol;

        public CodeWord(int symbol, int code, int length)
        {
            _symbol = symbol;
            _code = code;
            _length = length;
        }

        public int CompareTo(CodeWord other)
        {
            int result = GetLength().CompareTo(other.GetLength());
            if (result == 0) result = GetCode().CompareTo(other.GetCode());

            return result;
        }

        public int GetSymbol()
        {
            return _symbol;
        }

        public int GetCode()
        {
            return _code;
        }

        public int GetLength()
        {
            return _length;
        }

        public Bit[] GetCodeAsBitArray(int length)
        {
            string s = Convert.ToString(_code, 2);
            Bit[] bits = new Bit[length];

            int i = 0;
            if (s.Length < length)
            {
                var diff = length - s.Length;
                for (i = 0; i < diff; i++)
                {
                    bits[i] = false;
                }
            }
            foreach (char c in s)
            {
                if (c == '1')
                {
                    bits[i] = true;
                }
                else
                {
                    bits[i] = false;
                }

                i++;
            }

            return bits;
        }

        public override string ToString()
        {
            string result = _symbol + ": ";
            for (int i = _length; i > 0; i--) result += (_code >> (i - 1)) & 0x01;

            return result;
        }
    }
}