using System.Collections.Generic;
using BitStreams;

namespace JPEG_Encoder
{
    public class Node
    {
        public int Symbol { get; set; }
        public int Frequency { get; set; }
        public Node Right { get; set; }
        public Node Left { get; set; }
        public int Address { get; set; }
        public byte Depth { get; set; }

        public Node PushAddress(bool booly)
        {
            Address = (Address << 1) + (booly ? 1 : 0);
            Depth++;

            Right?.PushAddress(booly);
            Left?.PushAddress(booly);
            
            return this;
        }

        public List<bool> Traverse(int symbol, List<bool> data)
        {
            // Leaf
            if (Right == null && Left == null)
            {
                return symbol.Equals(Symbol) ? data : null;
            }

            List<bool> left = null;
            List<bool> right = null;

            if (Left != null)
            {
                var leftPath = new List<bool>();
                leftPath.AddRange(data);
                leftPath.Add(false);

                left = Left.Traverse(symbol, leftPath);
            }
            
            
            // zusätzliche Abfrage, falls Symbol schon gefunden wurde -> nicht ausführen
            if (Right != null)
            {
                List<bool> rightPath = new List<bool>();
                rightPath.AddRange(data);
                rightPath.Add(true);
                right = Right.Traverse(symbol, rightPath);
            }
 
            if (left != null)
            {
                return left;
            }
            else
            {
                return right;
            }
        }
    }
}