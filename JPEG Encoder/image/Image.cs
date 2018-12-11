using JPEG_Encoder.image.colors;

namespace JPEG_Encoder.image
{
    public abstract class Image
    {
        protected ColorChannel channel1;
        protected ColorChannel channel2;
        protected ColorChannel channel3;
        protected int originalHeight;
        protected int originalWidth;
        protected int subSampling = 1;

        public Image(ColorChannel channel1, ColorChannel channel2, ColorChannel channel3)
        {
            this.channel1 = channel1;
            this.channel2 = channel2;
            this.channel3 = channel3;
        }

        public ColorChannel GetChannel1()
        {
            return channel1;
        }

        public ColorChannel GetChannel2()
        {
            return channel2;
        }

        public ColorChannel GetChannel3()
        {
            return channel3;
        }

        public int GetHeight()
        {
            return channel1.GetHeight();
        }

        public int GetWidth()
        {
            return channel1.GetWidth();
        }

        public int GetSubSampling()
        {
            return subSampling;
        }

        public int GetOriginalHeight()
        {
            return originalHeight;
        }

        public int GetOriginalWidth()
        {
            return originalWidth;
        } 
    }
}