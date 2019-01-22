using JPEG_Encoder.image.colors;

namespace JPEG_Encoder.image
{
    public abstract class Image
    {
        protected readonly ColorChannel Channel1;
        protected ColorChannel Channel2;
        protected ColorChannel Channel3;
        protected int OriginalHeight;
        protected int OriginalWidth;
        protected int SubSampling = 1;

        protected Image(ColorChannel channel1, ColorChannel channel2, ColorChannel channel3)
        {
            Channel1 = channel1;
            Channel2 = channel2;
            Channel3 = channel3;
        }

        public ColorChannel GetChannel1()
        {
            return Channel1;
        }

        public ColorChannel GetChannel2()
        {
            return Channel2;
        }

        public ColorChannel GetChannel3()
        {
            return Channel3;
        }

        public int GetHeight()
        {
            return Channel1.GetHeight();
        }

        public int GetWidth()
        {
            return Channel1.GetWidth();
        }

        public int GetSubSampling()
        {
            return SubSampling;
        }

        public int GetOriginalHeight()
        {
            return OriginalHeight;
        }

        public int GetOriginalWidth()
        {
            return OriginalWidth;
        }
    }
}