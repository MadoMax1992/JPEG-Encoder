using System;

namespace JPEG_Encoder.image.colors.rgb
{
    public class RGB
    {
        private int rgb;

        public RGB(int red, int green, int blue)
        {
            this.rgb = red;
            this.rgb = (this.rgb << 8) + green;
            this.rgb = (this.rgb << 8) + blue;
        }

        public void SetRed(int red)
        {
            this.rgb = (rgb & 0xFFFF) + (red << 16);
        }

        public void SetGreen(int green)
        {
            this.rgb = (rgb & 0xFF00FF) + (green << 8);
        }

        public void SetBlue(int blue)
        {
            this.rgb = (rgb & 0xFFFF00) + blue;
        }

        public int GetRed()
        {
            return (this.rgb >> 16) & 255;
        }

        public int GetGreen()
        {
            return (this.rgb >> 8) & 255;
        }

        public int GetBlue()
        {
            return this.rgb & 255;
        }

        public double[] GetAsArray()
        {
            double[] rgbArray = new double[3];
            rgbArray[0] = this.GetRed();
            rgbArray[1] = this.GetGreen();
            rgbArray[2] = this.GetBlue();
            return rgbArray;
        }

        public override string ToString()
        {
            return GetRed() + "," + this.GetGreen() + "," + this.GetBlue();
        }
    }
}