using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace JPEG_Encoder
{
    public class HuffmanTree
    {
        private readonly List<Node> _nodes = new List<Node>();
        public Node Root { get; private set; }
        public readonly Dictionary<int, int> Frequencies = new Dictionary<int, int>();
        public readonly Dictionary<byte, int> LookupTable = new Dictionary<byte, int>();

        public void Build(IEnumerable<int> source)
        {
            foreach (var i in source)
            {
                if (!Frequencies.ContainsKey(i))
                {
                    Frequencies.Add(i, 0);
                }

                Frequencies[i]++;
            }

            foreach (var symbol in Frequencies)
            {
                _nodes.Add(new Node
                {
                    Symbol = symbol.Key,
                    Frequency = symbol.Value
                });
            }

            while (_nodes.Count > 1)
            {
                var orderedNodes = _nodes.OrderBy(node => node.Frequency).ToList();
                Console.WriteLine("node: " + orderedNodes.Take(1).ToArray()[0].Symbol);
                if (orderedNodes.Count >= 2)
                {
                    //Take first two items
                    var taken = orderedNodes.Take(2).ToList(); 

                    
                    //Create a parent node by combining the frequencies
                    var parent = new Node
                    {
                        Symbol = 256,
                        Frequency = taken[0].Frequency + taken[1].Frequency,
                        Left = taken[0],
                        Right = taken[1]
                    };

                    _nodes.Remove(taken[0]);
                    _nodes.Remove(taken[1]);
                    _nodes.Add(parent);
                }

            }
            
            Root = _nodes.FirstOrDefault();
        }

        public void ShiftMostRightSymbol()
        {
            Node mostRightNode = null;
            var current = Root;

            while (current.Right != null)
            {
                current = current.Right;
            }

            mostRightNode = current;
            
        }

        public BitArray Encode(IEnumerable<int> source)
        {
            var encodedSource = new List<bool>();

            foreach (var symbol in source)
            {
                var encodedSymbol = Root.Traverse(symbol, new List<bool>());
                encodedSource.AddRange(encodedSymbol);
            }
            
            var bits = new BitArray(encodedSource.ToArray());

            return bits;
        }

        public IEnumerable<int> Decode(BitArray bits)
        {
            var current = Root;
            var decoded = new List<int>();

            foreach (bool bit in bits)
            {
                if (bit)
                {
                    if (current.Right != null)
                    {
                        current = current.Right;
                    }
                }
                else
                {
                    if (current.Left != null)
                    {
                        current = current.Left;
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
            return node.Left == null && node.Right == null;
        }
    }
}