using System;
using System.Collections.Generic;
using System.IO;

namespace JPEG_Encoder.encoding.huffman.model
{
    public class HuffmanTreeOld
    {
        public Node Root = new Node();
        public readonly BitStreamPP[] LookupTable;

        public void Build(IEnumerable<int> source)
        {
            // setup
            
            var nodes = new List<Node>();
            
            foreach (var i in source)
            {
                var nodeInd = nodes.FindIndex(el => el.Symbol == i);
                
                if (nodeInd >= 0)
                {
                    nodes[nodeInd].Frequency++;
                }
                else
                {
                    nodes.Add(new Node()
                    {
                        Frequency = 1,
                        Symbol = i,
                    });
                }
            }

            Root.Fill(nodes);
        }

        public void ShiftMostRightSymbol()
        {
            Node mostRightNode = null;
            var current = Root;
            
            if (current.High == null)
                throw new Exception("no right node at all");

            while (current.High.High != null)
            {
                current = current.High;
            }

            mostRightNode = current.High;
            var secondMostRightNode = current;
            
            // Unshifts a 0 to the inverted Address for the lookup
//            mostRightNode.Depth++;
            
            // Adjust the tree as we still use it to decode
            var wildcard = new Node();

            secondMostRightNode.High = new Node
            {
                Symbol = 256,
                Low = mostRightNode,
                High = wildcard
            };
        }

        public BitStreamPP Encode(IEnumerable<int> source)
        {
            var encodedSource = new BitStreamPP(new MemoryStream());
            encodedSource.AutoIncreaseStream = true;


//            foreach (var symbol in source)
//            {
//                for (int i = 0; i < LookupTable[symbol].Depth; i++)
//                {
//                    encodedSource.WriteBit((LookupTable[symbol].Address >> i) % 2);
//                }
//            }

            encodedSource.Seek(0,0);
            
            return encodedSource;
        }

        public IEnumerable<int> Decode(BitStreamPP bits)
        {
            var current = Root;
            var decoded = new List<int>();

            for (int i = 0; i < bits.FullBitLength; i++)
            {
                var bit = bits.ReadBit().AsBool();
                if (bit)
                {
                    if (current.High != null)
                    {
                        current = current.High;
                    }
                }
                else
                {
                    if (current.Low != null)
                    {
                        current = current.Low;
                    }
                }

                if (IsLeaf(current))
                {
                    decoded.Add(current.Symbol);
                    current = Root;
                }
            }

            return decoded.ToArray();
        }


        private static bool IsLeaf(Node node)
        {
            return node.Low == null && node.High == null;
        }
    }
}