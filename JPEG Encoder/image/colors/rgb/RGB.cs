namespace JPEG_Encoder.image.colors.rgb
{
    public class RGB
    {
        private int _rgb;

        public RGB(int red, int green, int blue)
        {
            _rgb = red;
            _rgb = (_rgb << 8) + green;
            _rgb = (_rgb << 8) + blue;
        }

        public void SetRed(int red)
        {
            _rgb = (_rgb & 0xFFFF) + (red << 16);
        }

        public void SetGreen(int green)
        {
            _rgb = (_rgb & 0xFF00FF) + (green << 8);
        }

        public void SetBlue(int blue)
        {
            _rgb = (_rgb & 0xFFFF00) + blue;
        }

        private int GetRed()
        {
            return (_rgb >> 16) & 255;
        }

        private int GetGreen()
        {
            return (_rgb >> 8) & 255;
        }

        private int GetBlue()
        {
            return _rgb & 255;
        }

        public double[] GetAsArray()
        {
            double[] rgbArray = new double[3];
            rgbArray[0] = GetRed();
            rgbArray[1] = GetGreen();
            rgbArray[2] = GetBlue();
            return rgbArray;
        }

        public override string ToString()
        {
            return GetRed() + "," + GetGreen() + "," + GetBlue();
        }
    }
}