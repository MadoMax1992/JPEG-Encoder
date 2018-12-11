using System;

namespace JPEG_Encoder.encoding.huffman
{
    public class CodeWord : IComparable<CodeWord>
    {
        private int symbol;
        private int code;
        private int length;

        public CodeWord(int symbol, int code, int length)
        {
            this.symbol = symbol;
            this.code = code;
            this.length = length;
        }

        public int GetSymbol()
        {
            return symbol; 
        }

        public int GetCode()
        {
            return code;
        }

        public int GetLength()
        {
            return length;
        }

        public override string ToString()
        {
            string result = symbol + ": ";
            for (int i = length; i > 0; i--)
            {
                result += (code >> (i - 1)) & 0x01;
            }

            return result;
        }

        public int CompareTo(CodeWord other)
        {
            int result = GetLength().CompareTo(other.GetLength());
            if (result == 0)
            {
                result = GetCode().CompareTo(other.GetCode());
            }

            return result;
        }
    }
}