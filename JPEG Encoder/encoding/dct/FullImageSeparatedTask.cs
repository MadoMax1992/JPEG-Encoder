using System.Diagnostics;
using CenterSpace.NMath.Core;
using JPEG_Encoder.image.colors;

namespace JPEG_Encoder.encoding.dct
{
    public class FullImageSeparatedTask
    {
        private ColorChannel channel;

        public FullImageSeparatedTask(ColorChannel channel)
        {
            this.channel = channel;
        }

        public int Call()
        {
            int count = 0;
            Stopwatch stopwatch = Stopwatch.StartNew();
            while (stopwatch.ElapsedMilliseconds < 10000)
            {
                foreach (DoubleMatrix block in channel.GetBlocks(0, channel.GetNumOfBlocks()))
                {
                    CosineTransformation.separated(block);
                }

                count++;
            }
            stopwatch.Stop();
            return count;
        }
    }
}