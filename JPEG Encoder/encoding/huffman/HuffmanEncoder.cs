using System.Collections.Generic;
using JPEG_Encoder.encoding.huffman.model;

namespace JPEG_Encoder.encoding.huffman
{
    public class HuffmanEncoder
    {
        private HuffmanTree huffmanTree;

        HuffmanEncoder()
        {
        }

        public static HuffmanEncoder Encode(int[] symbols)
        {
            HuffmanEncoder encoder = new HuffmanEncoder();
            encoder.huffmanTree = encoder.CreateHuffmanTree(encoder.HuffmanInit(symbols));
            return encoder;
        }

        public HuffmanEncoder ForJpeg()
        {
            return Canonical().WithoutFullOnes().WithLengthRestriction(16);
        }

        private HuffmanEncoder Canonical()
        {
            huffmanTree.MakeCanonical();
            return this;
        }

        private HuffmanEncoder WithoutFullOnes()
        {
            huffmanTree.ReplaceMostRight();
            return this;
        }

        private HuffmanEncoder WithLengthRestriction(int lengthRestriction)
        {
            huffmanTree.RestrictToLength(lengthRestriction);
            return this;
        }

        public Dictionary<int, CodeWord> GetCodeBookAsDictionary()
        {
            return huffmanTree.GetCodeBookAsDictionary();
        }

        List<HuffmanTreeComponent> HuffmanInit(int[] symbols)
        {
            Dictionary<int, int> frequencies = new Dictionary<int, int>();
            int totalSymbols = symbols.Length;
            foreach (int symbol in symbols)
            {
                if (frequencies.ContainsKey(symbol))
                {
                    frequencies[symbol]++;
                }
                else
                {
                    frequencies.Add(symbol, 1);
                }
            }

            List<HuffmanTreeComponent> leafs = new List<HuffmanTreeComponent>();
            foreach (KeyValuePair<int, int> frequency in frequencies)
            {
                leafs.Add(new HuffmanTreeLeaf(frequency.Key, frequency.Value / (double) totalSymbols));
            }

            leafs.Sort();
            return leafs;
        }

        HuffmanTree CreateHuffmanTree(List<HuffmanTreeComponent> nodes)
        {
            List<HuffmanTreeLeaf> symbols = new List<HuffmanTreeLeaf>();
            foreach (HuffmanTreeComponent node in nodes)
            {
                symbols.Add((HuffmanTreeLeaf) node);
            }

            if (nodes.Count > 1)
            {
                CreateHuffmanTreeNodes(nodes);
            }
            else
            {
                HuffmanTreeNode huffmanTreeNode = new HuffmanTreeNode(nodes[0], new HuffmanTreeNullLeaf());
                nodes.RemoveAt(0);
                nodes.Add(huffmanTreeNode);
            }
            return new HuffmanTree(nodes[0], symbols);
        }

        private void CreateHuffmanTreeNodes(List<HuffmanTreeComponent> nodes)
        {
            if (nodes.Count > 1)
            {
                HuffmanTreeNode huffmanTreeNode = new HuffmanTreeNode(nodes[1], nodes[0]);
                nodes.RemoveAt(0);
                nodes.RemoveAt(0);
                nodes.Add(huffmanTreeNode);
                nodes.Sort();
                CreateHuffmanTreeNodes(nodes);
            }
        }
    }
}