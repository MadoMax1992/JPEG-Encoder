using System;
using System.Diagnostics;
using System.Threading;
using CenterSpace.NMath.Core;
using JPEG_Encoder.image.colors.rgb;
using JPEG_Encoder.image.colors.ycbcr;

namespace JPEG_Encoder.image.colors
{
    public class ColorChannels
    {
        static DoubleMatrix _conversionMatrix = new DoubleMatrix(new[,]
        {
            {0.299, 0.587, 0.114},
            {-0.168736, -0.331264, 0.5},
            {0.5, -0.418688, -0.081312}
        });

        static DoubleVector _prefixMatrix = new DoubleVector(new double[] {0, 128, 128});
        static DoubleVector _offsetMatrix = new DoubleVector(new double[] {128, 128, 128});

        public static YCbCrImage RgbToYCbCr(RGBImage rgbImage)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            ColorChannel luminance = new ColorChannel(rgbImage.GetWidth(), rgbImage.GetHeight());
            ColorChannel cbChannel = new ColorChannel(rgbImage.GetWidth(), rgbImage.GetHeight());
            ColorChannel crChannel = new ColorChannel(rgbImage.GetWidth(), rgbImage.GetHeight());


            if (rgbImage.GetHeight() < 256)
            {
                for (int y = 0; y < rgbImage.GetHeight(); y++)
                for (int x = 0; x < rgbImage.GetWidth(); x++)
                {
                    YCbCr converted = ConvertRgbtoYCbCr(rgbImage.GetRGBAt(x, y));
                    luminance.SetPixel(x, y, converted.GetLuminanceChannel());
                    cbChannel.SetPixel(x, y, converted.GetCbChannel());
                    crChannel.SetPixel(x, y, converted.GetCrChannel());
                }
            }
            else
            {
                int height = rgbImage.GetHeight();
                int threadCount = 16;
                
                CountdownEvent e = new CountdownEvent(1);

                for (int i = 0; i < height; i += height / threadCount)
                {
                    e.AddCount();
                    ThreadPool.QueueUserWorkItem(
                        state =>
                        {
                            object[] inp = state as object[];

                            //Console.WriteLine($"Thread with row {(int) inp[0]} to row {(int) inp[1]} started...");

                            for (int row = (int) inp[0]; row < (int) inp[1]; row++)
                            {
                                for (int x = 0; x < rgbImage.GetWidth(); x++)
                                {
                                    YCbCr converted = ConvertRgbtoYCbCr(rgbImage.GetRGBAt(x, row));
                                    luminance.SetPixel(x, row, converted.GetLuminanceChannel());
                                    cbChannel.SetPixel(x, row, converted.GetCbChannel());
                                    crChannel.SetPixel(x, row, converted.GetCrChannel());
                                }
                            }

                            e.Signal();
                        }, new object[] {i, i + (height / threadCount)});
                }

                e.Signal();
                e.Wait();

            }

            Console.WriteLine("Finished RGB to YCbCr conversion in "
                              + stopwatch.ElapsedMilliseconds / 1000d
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
            DoubleVector rgbMatrix = new DoubleVector(pixel.GetAsArray());

            DoubleVector yCbCrMatrix =
                DoubleVector.Add(NMathFunctions.Product(_conversionMatrix, rgbMatrix), _prefixMatrix);

            yCbCrMatrix = DoubleVector.Subtract(yCbCrMatrix, _offsetMatrix);
            return new YCbCr((int) (yCbCrMatrix[0] + 0.5),
                (int) (yCbCrMatrix[1] + 0.5),
                (int) (yCbCrMatrix[2] + 0.5));
        }

        public static int Cap(double num)
        {
            if (num < -127) return -127;

            if (num > 128) return 128;

            return 0;
        }
    }
}