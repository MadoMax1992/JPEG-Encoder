using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace JPEG_Encoder
{
    public class HuffmanTree
    {
        private readonly List<Node> nodes = new List<Node>();
        public Node Root { get; private set; }
        public readonly Dictionary<int, int> Frequencies = new Dictionary<int, int>();

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
                nodes.Add(new Node()
                {
                    Symbol = symbol.Key,
                    Frequency = symbol.Value
                });
            }

            while (nodes.Count > 1)
            {
                var orderedNodes = nodes.OrderBy(node => node.Frequency).ToList();

                if (orderedNodes.Count >= 2)
                {
                    //Take first two items
                    var taken = orderedNodes.Take(2).ToList();
                    
                    //Create a parent node by combining the frequencies
                    var parent = new Node()
                    {
                        Symbol = 256,
                        Frequency = taken[0].Frequency + taken[1].Frequency,
                        Left = taken[0],
                        Right = taken[1]
                    };

                    nodes.Remove(taken[0]);
                    nodes.Remove(taken[1]);
                    nodes.Add(parent);
                }

                Root = nodes.FirstOrDefault();
            }
        }

        public BitArray Encode(IEnumerable<int> source)
        {
            var encodedSource = new List<bool>();

            foreach (var symbol in source)
            {
                var encodedSymbol = Root.Traverse(symbol, new List<bool>());
                Console.WriteLine(encodedSymbol.ToArray()[0]);
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

                // ReSharper disable once InvertIf
                if (IsLeaf(current))
                {
                    decoded.Add(current.Symbol);
                    current = Root;
                }
            }

            return decoded.ToArray();
        }


        private bool IsLeaf(Node node)
        {
            return (node.Left == null && node.Right == null);
        }
    }
}