using System;
using System.Collections.Generic;
using System.IO;
using BitStreams;
using CenterSpace.NMath.Core;
using JPEG_Encoder.encoding.acdc;
using JPEG_Encoder.encoding.dct;
using JPEG_Encoder.encoding.huffman;
using JPEG_Encoder.image;
using JPEG_Encoder.image.colors;
using JPEG_Encoder.image.colors.rgb;
using JPEG_Encoder.image.colors.ycbcr;
using JPEG_Encoder.segments;
using JPEG_Encoder.segments.app0;
using JPEG_Encoder.segments.dht;
using JPEG_Encoder.segments.dqt;
using JPEG_Encoder.segments.eoi;
using JPEG_Encoder.segments.sof0;
using JPEG_Encoder.segments.soi;

namespace JPEG_Encoder
{
    public class JpegEncoder
    {
        private Image _image;
        private List<DCCategoryEncodedPair> _dcYValues;
        private List<ACCategoryEncodedPair> _acYValues;
        private List<DCCategoryEncodedPair> _dcCbValues;
        private List<ACCategoryEncodedPair> _acCbValues; 
        private List<DCCategoryEncodedPair> _dcCrValues;
        private List<ACCategoryEncodedPair> _acCrValues;
        private Dictionary<int, CodeWord> _dcYCodeBook;
        private Dictionary<int, CodeWord> _acYCodeBook;
        private Dictionary<int, CodeWord> _dcCbCrCodeBook;
        private Dictionary<int, CodeWord> _acCbCrCodeBook;
        

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
            PerformQuantization();
            PerformAcDcEncoding();
            PerformHuffmanEncoding();
            return this;
        }

        void PerformDCT()
        {
            TransformChannel(_image.GetChannel1());
            TransformChannel(_image.GetChannel2());
            TransformChannel(_image.GetChannel3());
        }

        void PerformQuantization()
        {
            QuantizeChannel(_image.GetChannel1(), QuantizationTable.QuantizationMatrixLuminance);
            QuantizeChannel(_image.GetChannel2(), QuantizationTable.QuantizationMatrixChrominance);
            QuantizeChannel(_image.GetChannel3(), QuantizationTable.QuantizationMatrixChrominance);
        }

        void PerformAcDcEncoding()
        {
            _dcYValues = AcDcEncoder.GetAllDCs(_image.GetChannel1());
            GetDcCbCrValues();
            _acYValues = AcDcEncoder.GetAllACs(_image.GetChannel1());
            GetAcCbCrValues();
        }
        
        void PerformHuffmanEncoding()
        {
            HuffmanEncodeDcy();
            HuffmanEncodeAcy();
            HuffmanEncodeDcCbCr();
            HuffmanEncodeAcCbCr();
        }
        
        private void HuffmanEncodeDcy()
        {
            int[] symbols = new int[_dcYValues.Count];
            int index = 0;
            foreach (DCCategoryEncodedPair dcCategoryEncodedPair in _dcYValues)
            {
                symbols[index++] = dcCategoryEncodedPair.GetPair();
            }
            _dcYCodeBook = HuffmanEncoder.Encode(symbols).ForJpeg().GetCodeBookAsDictionary();
        }
        
        private void HuffmanEncodeAcy()
        {
            int[] symbols = new int[_acYValues.Count];
            int index = 0;
            foreach (ACCategoryEncodedPair acCategoryEncodedPair in _acYValues)
            {
                symbols[index++] = acCategoryEncodedPair.GetPair();
            }
            _acYCodeBook = HuffmanEncoder.Encode(symbols).ForJpeg().GetCodeBookAsDictionary();
        }
        
        private void HuffmanEncodeDcCbCr()
        {
            List<int> symbols = new List<int>();
            List<DCCategoryEncodedPair> dcCbCrValues = _dcCbValues;
            dcCbCrValues.AddRange(_dcCrValues);
            foreach (DCCategoryEncodedPair dcCategoryEncodedPair in dcCbCrValues)
            {
                symbols.Add(dcCategoryEncodedPair.GetPair());
            }

            _dcCbCrCodeBook = HuffmanEncoder.Encode(symbols.ToArray())
                .ForJpeg()
                .GetCodeBookAsDictionary();
        }
        
        private void HuffmanEncodeAcCbCr()
        {
            List<int> symbols = new List<int>();
            List<ACCategoryEncodedPair> acCbCrValues = _acCbValues;
            acCbCrValues.AddRange(_acCrValues);
            foreach (ACCategoryEncodedPair acCategoryEncodedPair in acCbCrValues)
            {
                symbols.Add(acCategoryEncodedPair.GetPair());
            }
            _acCbCrCodeBook = HuffmanEncoder.Encode(symbols.ToArray())
                .ForJpeg()
                .GetCodeBookAsDictionary();
        }
        
        private void GetDcCbCrValues()
        {
            _dcCbValues = AcDcEncoder.GetAllDCs(_image.GetChannel2());
            _dcCrValues = AcDcEncoder.GetAllDCs(_image.GetChannel3());
        }

        private void GetAcCbCrValues()
        {
            _acCbValues = AcDcEncoder.GetAllACs(_image.GetChannel2());
            _acCrValues = AcDcEncoder.GetAllACs(_image.GetChannel3());
        }

        private void TransformChannel(ColorChannel channel)
        {
            for (int i = 0; i < channel.GetNumOfBlocks(); i++)
            {
                Arai.Calc(channel.GetBlock(i));
            }
        }

        private void QuantizeChannel(ColorChannel channel, DoubleMatrix quantizationTable)
        {
            for (int i = 0; i < channel.GetNumOfBlocks(); i++)
            {
                CosineTransformation.Quantize(channel.GetBlock(i), quantizationTable);
            }
        }

        private HuffmanTable GetHuffmanTableDcy()
        {
            return new HuffmanTable(0, 0, new List<CodeWord>(_dcYCodeBook.Values));
        }

        private HuffmanTable GetHuffmanTableDcCbCr()
        {
            return new HuffmanTable(1, 0, new List<CodeWord>(_dcCbCrCodeBook.Values));
        }

        private HuffmanTable GetHuffmanTableAcy()
        {
            return new HuffmanTable(0, 1, new List<CodeWord>(_acYCodeBook.Values));
        }

        private HuffmanTable GetHuffmanTableAcCbCr()
        {
            return new HuffmanTable(1, 1, new List<CodeWord>(_acCbCrCodeBook.Values));
        }

        public void WriteImageToDisk()
        {
            try
            {
                byte[] bytes = new byte[1000000];
                BitStream bos = new BitStream(bytes);
                List<SegmentWriter> segmentWriters = new List<SegmentWriter>();
                segmentWriters.Add(new SOIWriter(bos));
                segmentWriters.Add(new APP0Writer(bos, 0x0048, 0x0048));
                segmentWriters.Add(new DQTWriter(bos));
                segmentWriters.Add(new SOF0Writer(bos,
                    _image.GetOriginalWidth(),
                    _image.GetOriginalHeight(),
                    _image.GetSubSampling()));

                List<HuffmanTable> huffmanTables = new List<HuffmanTable>();
                huffmanTables.Add(GetHuffmanTableDcy());
                huffmanTables.Add(GetHuffmanTableAcy());
                huffmanTables.Add(GetHuffmanTableDcCbCr());
                huffmanTables.Add(GetHuffmanTableAcCbCr());
                segmentWriters.Add(new DHTWriter(bos, huffmanTables));
                //segmentWriters.Add(new SOSWriter(bos));
                //segmentWriters.Add(new ImageDataWriter());
                segmentWriters.Add(new EOIWriter(bos));
                foreach (SegmentWriter segmentWriter in segmentWriters)
                {
                    segmentWriter.WriteSegment();
                }

                bos.GetStream().SetLength(bos.GetStream().Position);
                bos.SaveStreamAsFile("../../../img/TestPicture.jpg");
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e);
            }
            catch (IOException e)
            {
                Console.WriteLine(e);
            }
        }
        
    }
}