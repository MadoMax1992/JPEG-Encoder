using System.Diagnostics;
using CenterSpace.NMath.Core;
using JPEG_Encoder.image.colors;

namespace JPEG_Encoder.encoding.dct
{
    public class FullImageSeparatedTask
    {
        private readonly ColorChannel _channel;

        public FullImageSeparatedTask(ColorChannel channel)
        {
            _channel = channel;
        }

        public int Call()
        {
            int count = 0;
            Stopwatch stopwatch = Stopwatch.StartNew();
            while (stopwatch.ElapsedMilliseconds < 10000)
            {
                foreach (DoubleMatrix block in _channel.GetBlocks(0, _channel.GetNumOfBlocks())) DCT.Advanced(block);

                count++;
            }

            stopwatch.Stop();
            return count;
        }
    }
}