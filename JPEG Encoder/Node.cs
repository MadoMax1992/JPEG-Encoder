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
    }
}