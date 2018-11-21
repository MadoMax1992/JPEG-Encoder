using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using BitStreams;

namespace JPEG_Encoder
{
    public class HuffmanTree
    {
        private readonly List<Node> _nodes = new List<Node>();
        public Node Root { get; private set; }
        public readonly Dictionary<int, int> Frequencies = new Dictionary<int, int>();
        public readonly Dictionary<int, Node> LookupTable = new Dictionary<int, Node>();

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
                Node tmp = new Node
                {
                    Symbol = symbol.Key,
                    Frequency = symbol.Value
                };

                _nodes.Add(tmp);

                LookupTable.Add(symbol.Key, tmp);
            }

            Node right = null;
            while (_nodes.Count > 1)
            {
                var orderedNodes = _nodes.OrderBy(node => node.Frequency).ToList();
                //Console.WriteLine("node: " + orderedNodes.Take(1).ToArray()[0].Symbol);
                if (orderedNodes.Count >= 2)
                {
                    var taken = new List<Node>();
                    if (right == null) 
                    {	
                        //Take first two items	
                        taken = orderedNodes.Take(2).ToList(); 	
                    }	
                    else	
                    {	
                        //Only take one item	
                        taken = orderedNodes.Take(1).ToList();	
                        taken.Add(right);	
                    }

                    //Create a parent node by combining the frequencies
                    var parent = new Node
                    {
                        Symbol = 256,
                        Frequency = taken[0].Frequency + taken[1].Frequency,
                        Left = taken[0],
                        Right = taken[1]
                    };

                    taken[0].PushAddress(false);
                    taken[1].PushAddress(true);

                    _nodes.Remove(taken[0]);
                    _nodes.Remove(taken[1]);
                    _nodes.Add(parent);
                    right = parent;
                }
            }

            Root = _nodes.FirstOrDefault();
        }

        public void ShiftMostRightSymbol()
        {
            Node mostRightNode = null;
            var current = Root;
            
            if (current.Right == null)
                throw new Exception("no right node at all");

            while (current.Right.Right != null)
            {
                current = current.Right;
            }

            mostRightNode = current.Right;
            var secondMostRightNode = current;
            
            // Unshifts a 0 to the inverted Address for the lookup
            mostRightNode.Depth++;
            
            // Adjust the tree as we still use it to decode
            var wildcard = new Node();

            secondMostRightNode.Right = new Node
            {
                Symbol = 256,
                Left = mostRightNode,
                Right = wildcard
            };
        }

        public BitStreamPP Encode(IEnumerable<int> source)
        {
            var encodedSource = new BitStreamPP(new MemoryStream());
            encodedSource.AutoIncreaseStream = true;


            foreach (var symbol in source)
            {
                for (int i = 0; i < LookupTable[symbol].Depth; i++)
                {
                    encodedSource.WriteBit((LookupTable[symbol].Address >> i) % 2);
                }
            }

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