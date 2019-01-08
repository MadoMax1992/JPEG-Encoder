using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace JPEG_Encoder.image.colors.rgb
{
    public class RGBImage : Image
    {
        private RGBImage(ColorChannel r, ColorChannel g, ColorChannel b, int originalWidth, int originalHeight) :
            base(r, g, b)
        {
            OriginalWidth = originalWidth;
            OriginalHeight = originalHeight;
        }

        public RGB GetRGBAt(int x, int y)
        {
            if (x >= GetWidth() || y >= GetHeight()) throw new ArgumentException();

            if (x >= OriginalWidth) x = OriginalWidth - 1;

            if (y >= OriginalHeight) y = OriginalHeight - 1;

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
                    sb.Append(pixel).Append(" ");
                }

                sb.Append("\n");
            }

            return sb.ToString();
        }

        internal class RGBImageBuilder
        {
            private ColorChannel _blue;
            private ColorChannel _green;
            private int _imageHeight;
            private int _imageWidth;
            private ColorChannel _red;
            private readonly int stride = 16;

            private RGBImageBuilder(ColorChannel red)
            {
                _imageWidth = red.GetWidth();
                _imageHeight = red.GetHeight();
                _red = new ColorChannel(GetRealWidth(), GetRealHeight());
                _green = new ColorChannel(GetRealWidth(), GetRealHeight());
                _blue = new ColorChannel(GetRealWidth(), GetRealHeight());
                for (int i = 0; i < red.GetHeight(); i++)
                for (int j = 0; j < red.GetWidth(); j++)
                {
                    _red.SetPixel(j, i, red.GetPixel(j, i));
                    _green.SetPixel(j, i, red.GetPixel(j, i));
                    _blue.SetPixel(j, i, red.GetPixel(j, i));
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
                        _red.SetPixel(x, y, ReadValue(binReader));
                        _green.SetPixel(x, y, ReadValue(binReader));
                        _blue.SetPixel(x, y, ReadValue(binReader));
                        x++;
                        if (x % _imageWidth != 0) continue;
                        x = 0;
                        y++;
                    }
                }
                catch (IOException e)
                {
                    Console.WriteLine(e);
                }


                Console.WriteLine("Finished reading PPM in "
                                  + (stopwatch.ElapsedMilliseconds / 1000d
                                     + " seconds"));
                stopwatch.Stop();
            }

            public static RGBImageBuilder From(Stream inputStream)
            {
                return new RGBImageBuilder(inputStream);
            }

            public static RGBImageBuilder From(ColorChannel red, ColorChannel green, ColorChannel blue)
            {
                return new RGBImageBuilder(red);
            }

            private int ReadValue(BinaryReader binReader)
            {
                string value = string.Empty;
                while (!char.IsWhiteSpace((char) binReader.PeekChar())) value += binReader.ReadChar().ToString();
                while (char.IsWhiteSpace((char) binReader.PeekChar())) binReader.ReadByte();
                return int.Parse(value);
            }

            private void InitPicture()
            {
                _red = new ColorChannel(GetRealWidth(), GetRealHeight());
                _green = new ColorChannel(GetRealWidth(), GetRealHeight());
                _blue = new ColorChannel(GetRealWidth(), GetRealHeight());
            }

            private void ExtractMetaInformation(BinaryReader binReader)
            {
                int headerItemCount = 0;

                while (headerItemCount < 4)
                {
                    char nextChar = (char) binReader.PeekChar();
                    if (nextChar == '#')
                        while (binReader.ReadChar() != '\n')
                        {
                        }
                    else if (char.IsWhiteSpace(nextChar))
                        binReader.ReadChar();
                    else
                        switch (headerItemCount)
                        {
                            case 0:
                                binReader.ReadChars(2);
                                headerItemCount++;
                                break;
                            case 1: // width
                                _imageWidth = ReadValue(binReader);
                                headerItemCount++;
                                break;
                            case 2: // height
                                _imageHeight = ReadValue(binReader);
                                headerItemCount++;
                                break;
                            case 3: // depth
                                ReadValue(binReader);
                                headerItemCount++;
                                break;
                            default:
                                throw new Exception("Error");
                        }
                }
            }

            private int GetRealHeight()
            {
                int result = _imageHeight;
                if (_imageHeight % stride != 0) result = (int) Math.Ceiling((double) _imageHeight / stride) * stride;

                return result;
            }

            private int GetRealWidth()
            {
                int result = _imageWidth;
                if (_imageWidth % stride != 0) result = (int) Math.Ceiling((double) _imageWidth / stride) * stride;

                return result;
            }

            public RGBImage Build()
            {
                return new RGBImage(_red, _green, _blue, _imageWidth, _imageHeight);
            }
        }
    }
}