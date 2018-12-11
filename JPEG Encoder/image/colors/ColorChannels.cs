using System;
using System.Diagnostics;
using CenterSpace.NMath.Core;
using JPEG_Encoder.image.colors.rgb;
using JPEG_Encoder.image.colors.ycbcr;

namespace JPEG_Encoder.image.colors
{
    public class ColorChannels
    {
        public static YCbCrImage RGBToYCbCr(RGBImage rgbImage)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            ColorChannel luminance = new ColorChannel(rgbImage.GetWidth(), rgbImage.GetHeight());
            ColorChannel cbChannel = new ColorChannel(rgbImage.GetWidth(), rgbImage.GetHeight());
            ColorChannel crChannel = new ColorChannel(rgbImage.GetWidth(), rgbImage.GetHeight());
            for (int y = 0; y < rgbImage.GetHeight(); y++)
            {
                for (int x = 0; x < rgbImage.GetWidth(); x++)
                {
                    YCbCr converted = ConvertRgbtoYCbCr(rgbImage.GetRGBAt(x, y));
                    luminance.SetPixel(x, y, converted.GetLuminanceChannel());
                    cbChannel.SetPixel(x, y, converted.GetCbChannel());
                    crChannel.SetPixel(x, y, converted.GetCrChannel());
                }
            }

            Console.WriteLine("Finished RGB to YCbCr conversion in "
                              + (stopwatch.ElapsedMilliseconds / 1000d)
                              + " seconds");

            stopwatch.Stop();

            return new YCbCrImage(luminance,
                cbChannel,
                crChannel,
                rgbImage.GetOriginalWidth(),
                rgbImage.GetOriginalHeight());
        }

        public static YCbCr ConvertRgbtoYCbCr(RGB pixel)
        {
            DoubleVector prefixMatrix = new DoubleVector(new double[]{0, 128, 128});
            DoubleMatrix conversionMatrix = new DoubleMatrix(new double[,]
            {
                {0.299, 0.587, 0.114},
                {-0.168736, -0.331264, 0.5},
                {0.5, -0.418688, -0.081312}
            });
            DoubleVector offsetMatrix = new DoubleVector(new double[]{128, 128, 128});
            
            DoubleVector rgbMatrix = new DoubleVector(pixel.GetAsArray());
            DoubleVector hilfsVector = NMathFunctions.Product(conversionMatrix, rgbMatrix);
            DoubleVector yCbCrMatrix = DoubleVector.Add(hilfsVector, prefixMatrix);
            DoubleVector resultMatrix = DoubleVector.Subtract(yCbCrMatrix, offsetMatrix);
            return new YCbCr((int) Math.Round(resultMatrix[0]),
                (int) Math.Round(resultMatrix[1]),
                (int) Math.Round(resultMatrix[2]));
        }

        public static int Cap(double num)
        {
            if (num < -127)
            {
                return -127;
            }

            if (num > 128)
            {
                return 128;
            }

            return 0;
        }
    }
}