using System.Collections.Generic;
using JPEG_Encoder.encoding.huffman;

namespace JPEG_Encoder.segments.dht
{
    public class HuffmanTable
    {
        private int tableClass;
        private List<CodeWord> codeBook;
        private Dictionary<int, int> codeWordLengthDictionary = new Dictionary<int, int>();
        private int id;

        public HuffmanTable(int id, int tableClass, List<CodeWord> codeBook)
        {
            this.id = id;
            this.tableClass = tableClass;
            this.codeBook = codeBook;
            codeBook.Sort();
            this.SetCodeWordLengthDictionary();
        }

        private void SetCodeWordLengthDictionary()
        {
            for (int i = 1; i <= 16; i++)
            {
                codeWordLengthDictionary.Add(i, 0);
            }

            foreach (CodeWord codeWord in codeBook)
            {
                int codeWordLength = codeWord.GetLength();
                codeWordLengthDictionary[codeWordLength]++;
            }
        }

        public int GetCodebookSize()
        {
            return codeBook.Count;
        }

        public void write(BitStreamPP bitStream)
        {
            bitStream.WriteInt32((tableClass << 4) + id);
            for (int i = 1; i <= 16; i++)
            {
                bitStream.WriteInt32(codeWordLengthDictionary[i]);
            }

            for (int i = 1; i < codeBook.Count; i++)
            {
                CodeWord codeWord = codeBook[i];
                bitStream.WriteInt32(codeWord.GetSymbol());
            }
        }
    }
}