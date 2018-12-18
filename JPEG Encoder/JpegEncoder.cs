using System.Collections.Generic;
using System.Deployment.Internal;
using System.IO;
using JPEG_Encoder.encoding.dct;
using JPEG_Encoder.encoding.huffman;
using JPEG_Encoder.image;
using JPEG_Encoder.image.colors;
using JPEG_Encoder.image.colors.rgb;
using JPEG_Encoder.image.colors.ycbcr;

namespace JPEG_Encoder
{
    public class JpegEncoder
    {
        private Image _image;

        public static JpegEncoder WithImageFromFile(string filename)
        {
            RGBImage rgbImage = RGBImage.RGBImageBuilder.From(new FileStream(filename, FileMode.Open)).Build();
            YCbCrImage yCbCrImage = ColorChannels.RGBToYCbCr(rgbImage);
            return new JpegEncoder(yCbCrImage);
        }

        public static JpegEncoder WithImage(Image image)
        {
            return new JpegEncoder(image);
        }

        private JpegEncoder(Image image)
        {
            _image = image;
        }

        public JpegEncoder ConvertToJpeg(int subsampling)
        {
            ((YCbCrImage) _image).Reduce(subsampling);
            PerformDCT();
            // PerformQuantization();
            // PerformAcDcEncoding();
            // PerformHuffmanEncoding();
            return this;
        }

        JpegEncoder PerformDCT()
        {
            TransformChannel(_image.GetChannel1());
            TransformChannel(_image.GetChannel2());
            TransformChannel(_image.GetChannel3());
            return this;
        }

        private void TransformChannel(ColorChannel channel)
        {
            for (int i = 0; i < channel.GetNumOfBlocks(); i++)
            {
                Arai.Calc(channel.GetBlock(i));
            }
        }
        
        
    }
}