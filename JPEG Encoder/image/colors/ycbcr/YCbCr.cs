namespace JPEG_Encoder.image.colors.ycbcr
{
    public class YCbCr
    {
        private readonly int _cb;
        private readonly int _cr;
        private readonly int _y;

        public YCbCr(int luminanceChannel, int cb, int cr)
        {
            _y = luminanceChannel;
            _cb = cb;
            _cr = cr;
        }

        public int GetLuminanceChannel()
        {
            return _y;
        }

        public int GetCbChannel()
        {
            return _cb;
        }

        public int GetCrChannel()
        {
            return _cr;
        }

        public override string ToString()
        {
            return GetLuminanceChannel() + "," + GetCbChannel() + "," + GetCrChannel();
        }
    }
}