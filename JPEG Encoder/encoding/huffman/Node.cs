using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace JPEG_Encoder.encoding.huffman
{
    public class Node
    {
        public int Symbol { get; set; }
        public int Frequency { get; set; }
        public Node High { get; set; }
        public Node Low { get; set; }
        public BitStreamPP BSAddress = new BitStreamPP(new MemoryStream(2));
        //public int Address { get; set; }
        //public byte Depth { get; set; }

//        public Node PushAddress(bool booly)
//        {
//            Address = (Address << 1) + (booly ? 1 : 0);
//            Depth++;
//
//            High?.PushAddress(booly);
//            Low?.PushAddress(booly);
//            
//            return this;
//        }

        public Node Fill(List<Node> input, bool skipOrderFlag = false)
        {
            input = input.OrderByDescending(node => node.Frequency).ToList();
            
            // Based on 09_chapter 4
            // Generates optimal length-limited huffman codes

            this.Frequency = input.Aggregate(0, (acc, c) => acc + c.Frequency);
            
            var ind = 0;
            var k = input.Aggregate(0, (acc, c) =>
            {
                var lg2 = utility.Log2((uint) ind++);
                return 1.5 * (lg2 + 1) * lg2 * c.Frequency > this.Frequency ? acc + 1 : acc;
            });
            
            // k1: max 2^x below k
            var k1 = 1;
            while (k1 <= k)
                k1 = k1 << 1;
            k1 = k1 >> 1;
            
            var highList = input.Take(k1).ToList();
            var lowList = input.Skip(k1).ToList();

            this.High = (new Node()).Fill(highList, true);
            this.Low = (new Node().Fill(lowList, true));

            utility.Log2((ulong) lowList.Count);

            return this;
        }

        public Node TransformHigh()
        {
            var subj = this.High;
            
            this.High = new Node()
            {
                Low = subj,
                High = new Node(),
            };
            
            return this;
        }
    }
}