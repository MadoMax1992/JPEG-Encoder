using System;
using System.Text;

namespace JPEG_Encoder.image.colors.ycbcr
{
    public class YCbCrImage : Image
    {
        public YCbCrImage(ColorChannel luminance, ColorChannel cbChannel, ColorChannel crChannel, int originalWidth,
            int originalHeight) : base(luminance, cbChannel, crChannel)
        {
            this.originalWidth = originalWidth;
            this.originalHeight = originalHeight;
        }

        public int GetHeight()
        {
            return GetChannel1().GetHeight();
        }

        public int GetWidth()
        {
            return GetChannel1().GetWidth();
        }

        public YCbCr GetPixelAt(int x, int y)
        {
            return new YCbCr((int) channel1.GetPixel(x, y),
                (int) channel2.GetPixel(x / subSampling, y / subSampling),
                (int) channel3.GetPixel(x / subSampling, y / subSampling));
        }

        public void Reduce(int subSampling)
        {
            channel2 = ReduceChannel(channel2, subSampling);
            channel3 = ReduceChannel(channel3, subSampling);
            this.subSampling = subSampling;
        }

        private ColorChannel ReduceChannel(ColorChannel channel, int factor)
        {
            int reducedHeight = channel.GetHeight() / factor;
            int reducedWidth = channel.GetWidth() / factor;
            ColorChannel result = new ColorChannel(reducedWidth, reducedHeight);
            for (int y = 0; y < reducedHeight; y++)
            {
                for (int x = 0; x < reducedWidth; x++)
                {
                    int sum = 0;
                    for (int blockY = y * factor; blockY < y * factor + factor; blockY++)
                    {
                        for (int blockX = x * factor; blockX < x * factor + factor; blockX++)
                        {
                            sum += (int) channel.GetPixel(blockX, blockY);
                        }
                    }

                    result.SetPixel(x, y, (int) Math.Round(sum / (double) (factor * factor)));
                }
            }

            return result;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < GetHeight(); i++)
            {
                for (int j = 0; j < GetWidth(); j++)
                {
                    sb.Append(GetPixelAt(j, i).ToString())
                        .Append(",");
                }

                sb.Append("\n");
            }

            return sb.ToString();
        }
    }
}