using System.Collections.Generic;

namespace JPEG_Encoder
{
    public class Node
    {
        public int Symbol { get; set; }
        public int Frequency { get; set; }
        public Node Right { get; set; }
        public Node Left { get; set; }

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