using System.Collections.Generic;
using BitStreams;
using JPEG_Encoder.encoding.huffman;

namespace JPEG_Encoder.segments.dht
{
    public class HuffmanTable
    {
        private readonly List<CodeWord> _codeBook;
        private readonly Dictionary<int, int> _codeWordLengthDictionary = new Dictionary<int, int>();
        private readonly int _id;
        private readonly int _tableClass;

        public HuffmanTable(int id, int tableClass, List<CodeWord> codeBook)
        {
            _id = id;
            _tableClass = tableClass;
            _codeBook = codeBook;
            codeBook.Sort();
            SetCodeWordLengthDictionary();
        }

        private void SetCodeWordLengthDictionary()
        {
            for (int i = 1; i <= 16; i++) _codeWordLengthDictionary.Add(i, 0);

            foreach (CodeWord codeWord in _codeBook)
            {
                int codeWordLength = codeWord.GetLength();
                _codeWordLengthDictionary[codeWordLength]++;
            }
        }

        public int GetCodebookSize()
        {
            return _codeBook.Count;
        }

        public void Write(BitStream bos)
        {
            bos.WriteByte((byte) ((_tableClass << 4) | (_id << 0)));

            for (int i = 1; i <= 16; i++) bos.WriteByte((byte) _codeWordLengthDictionary[i]);

            for (int i = 0; i < _codeBook.Count; i++)
            {
                CodeWord codeWord = _codeBook[i];
                bos.WriteByte((byte) codeWord.GetSymbol());
            }
        }
    }
}