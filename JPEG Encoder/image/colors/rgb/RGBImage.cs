using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace JPEG_Encoder.image.colors.rgb
{
    public class RGBImage : Image
    {
        internal class RGBImageBuilder
        {
            private int stride = 16;
            private int imageWidth;
            private int imageHeight;
            ColorChannel red;
            ColorChannel green;
            ColorChannel blue;

            public static RGBImageBuilder From(Stream inputStream)
            {
                return new RGBImageBuilder(inputStream);
            }

            public static RGBImageBuilder From(ColorChannel red, ColorChannel green, ColorChannel blue)
            {
                return new RGBImageBuilder(red, green, blue);
            }

            private RGBImageBuilder(ColorChannel red, ColorChannel green, ColorChannel blue)
            {
                imageWidth = red.GetWidth();
                this.imageHeight = red.GetHeight();
                this.red = new ColorChannel(GetRealWidth(), GetRealHeight());
                this.green = new ColorChannel(GetRealWidth(), GetRealHeight());
                this.blue = new ColorChannel(GetRealWidth(), GetRealHeight());
                for (int i = 0; i < red.GetHeight(); i++)
                {
                    for (int j = 0; j < red.GetWidth(); j++)
                    {
                        this.red.SetPixel(j, i, red.GetPixel(j, i));
                        this.green.SetPixel(j, i, red.GetPixel(j, i));
                        this.blue.SetPixel(j, i, red.GetPixel(j, i));
                    }
                }
            }

            private RGBImageBuilder(Stream inputStream)
            {
                Stopwatch stopwatch = Stopwatch.StartNew();

                BinaryReader binReader = new BinaryReader(inputStream);
                try
                {
                    ExtractMetaInformation(binReader);
                    InitPicture();
                    int x = 0;
                    int y = 0;
                    while (binReader.PeekChar() >= 0)
                    {
                        red.SetPixel(x, y, ReadValue(binReader));
                        green.SetPixel(x, y, ReadValue(binReader));
                        blue.SetPixel(x, y, ReadValue(binReader));
                        x++;
                        if (x % imageWidth == 0)
                        {
                            x = 0;
                            y++;
                        }
                    }
                }
                catch (IOException e)
                {
                }


                Console.WriteLine("Finished reading PPM in "
                                  + (stopwatch.ElapsedMilliseconds / 1000d
                                     + " seconds"));
                stopwatch.Stop();
            }

            private int ReadValue(BinaryReader binReader)
            {
                var value = string.Empty;
                while (!char.IsWhiteSpace((char)binReader.PeekChar()))
                {
                    value += binReader.ReadChar().ToString();
                }
                while (char.IsWhiteSpace((char)binReader.PeekChar()))
                {
                    binReader.ReadByte();
                }
                return int.Parse(value);
            }

            private void InitPicture()
            {
                red = new ColorChannel(GetRealWidth(), GetRealHeight());
                green = new ColorChannel(GetRealWidth(), GetRealHeight());
                blue = new ColorChannel(GetRealWidth(), GetRealHeight());
            }

            private void ExtractMetaInformation(BinaryReader binReader)
            {
                var headerItemCount = 0;

                while (headerItemCount < 4)
                {
                
                    var nextChar = (char) binReader.PeekChar();
                    if (nextChar == '#')
                    {
                        while (binReader.ReadChar() != '\n')
                        {
                        }
                    }
                    else if (char.IsWhiteSpace(nextChar))
                    {
                        binReader.ReadChar();
                    }
                    else
                    {                    
                        switch (headerItemCount)
                        {
                            case 0: 
                                binReader.ReadChars(2);
                                headerItemCount++;
                                break;
                            case 1: // width
                                imageWidth = ReadValue(binReader);
                                headerItemCount++;
                                break;
                            case 2: // height
                                imageHeight = ReadValue(binReader);
                                headerItemCount++;
                                break;
                            case 3: // depth
                                headerItemCount++;
                                break;
                            default:
                                throw new Exception("Error");
                        }
                    }
                }
            }

            private string ReadLine(StreamReader sc)
            {
                String result = sc.ReadLine();
                if (result.Contains("#"))
                {
                    return ReadLine(sc);
                }

                return result;
            }

            private int GetRealHeight()
            {
                int result = imageHeight;
                if (imageHeight % stride != 0)
                {
                    result = (int) Math.Ceiling((double) imageHeight / stride) * stride;
                }

                return result;
            }

            private int GetRealWidth()
            {
                int result = imageWidth;
                if (imageWidth % stride != 0)
                {
                    result = (int) Math.Ceiling((double) imageWidth / stride) * stride;
                }

                return result;
            }

            public RGBImage Build()
            {
                return new RGBImage(red, green, blue, imageWidth, imageHeight);
            }
        }

        private RGBImage(ColorChannel r, ColorChannel g, ColorChannel b, int originalWidth, int originalHeight) : base(r, g, b)
        {
            this.originalWidth = originalWidth;
            this.originalHeight = originalHeight;
        }

        public RGB GetRGBAt(int x, int y)
        {
            if (x >= GetWidth() || y >= GetHeight())
            {
                throw new ArgumentException();
            }

            if (x >= originalWidth)

            {
                x = originalWidth - 1;
            }

            if (y >= originalHeight)
            {
                y = originalHeight - 1;
            }

            return new RGB((int) GetChannel1().GetPixel(x, y),
                (int) GetChannel2().GetPixel(x, y),
                (int) GetChannel3().GetPixel(x, y));
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < GetWidth(); i++)
            {
                for (int j = 0; j < GetHeight(); j++)
                {
                    RGB pixel = GetRGBAt(i, j);
                    sb.Append(pixel.ToString()).Append(" ");
                }

                sb.Append("\n");
            }

            return sb.ToString();
        }
    }
}