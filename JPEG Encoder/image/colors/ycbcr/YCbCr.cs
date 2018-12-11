using System;

namespace JPEG_Encoder.image.colors.ycbcr
{
    public class YCbCr
    {
        private int y;
        private int cb;
        private int cr;

        public YCbCr(int luminanceChannel, int cb, int cr)
        {

            y = luminanceChannel;
            this.cb = cb;
            this.cr = cr;
        }

        public int GetLuminanceChannel()
        {
            return y;
        }

        public int GetCbChannel()
        {
            return cb;
        }

        public int GetCrChannel()
        {
            return cr;
        }

        public override string ToString()
        {
            return GetLuminanceChannel() + "," + GetCbChannel() + "," + GetCrChannel();
        }
    }
}