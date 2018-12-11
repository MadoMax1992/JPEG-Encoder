using System;
using System.Collections.Generic;

namespace JPEG_Encoder.encoding.huffman.model
{
    public class DepthComparator : IComparer<HuffmanTreeComponent>
    {
        public int Compare(HuffmanTreeComponent o1, HuffmanTreeComponent o2)
        {
            int result;
            if (o1.GetDepth(0) == o2.GetDepth(0))
            {
                result = -o1.GetFrequency().CompareTo(o2.GetFrequency());
            }
            else
            {
                result = new decimal(o1.GetDepth(0)).CompareTo(o2.GetDepth(0));
            }

            return result;
        }
    }
}