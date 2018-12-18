using System;
using System.Diagnostics;
using CenterSpace.NMath.Core;
using JPEG_Encoder.image.colors;

namespace JPEG_Encoder.encoding.dct
{
    public class FullImageAraiTask
    {
        private ColorChannel channel;

        public FullImageAraiTask(ColorChannel channel)
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
                    Arai.Calc(block);
                }

                count++;
            }
            stopwatch.Stop();
            return count;
        }
    }
}