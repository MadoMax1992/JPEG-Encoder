using System.Collections.Generic;

namespace JPEG_Encoder.encoding.huffman.model
{
    public class DepthComparator : IComparer<HuffmanTreeComponent>
    {
        public int Compare(HuffmanTreeComponent o1, HuffmanTreeComponent o2)
        {
            int result = 0;
            if (o2 != null && o1 != null && o1.GetDepth(0) == o2.GetDepth(0))
                result = -o1.GetFrequency().CompareTo(o2.GetFrequency());
            else if (o1 != null)
                if (o2 != null)
                    result = new decimal(o1.GetDepth(0)).CompareTo(o2.GetDepth(0));

            return result;
        }
    }
}