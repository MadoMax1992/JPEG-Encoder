using System.Collections.Generic;
using BitStreams;
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

        public void Write(BitStream bos)
        {
            bos.WriteByte((byte) (tableClass << 4 | id << 0));

            for (int i = 1; i <= 16; i++)
            {
                bos.WriteByte((byte) codeWordLengthDictionary[i]);
            }

            for (int i = 0; i < codeBook.Count; i++)
            {
                CodeWord codeWord = codeBook[i];
                bos.WriteByte((byte) codeWord.GetSymbol());
            }
        }
    }
}