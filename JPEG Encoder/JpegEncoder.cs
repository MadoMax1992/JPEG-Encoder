using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using JPEG_Encoder.segments.imageData;
using JPEG_Encoder.segments.sof0;
using JPEG_Encoder.segments.soi;
using JPEG_Encoder.segments.sos;

namespace JPEG_Encoder
{
    public class JpegEncoder
    {
        private Dictionary<int, CodeWord> _acCbCrCodeBook;
        private List<AcCategoryEncodedPair> _acCbValues;
        private List<AcCategoryEncodedPair> _acCrValues;
        private Dictionary<int, CodeWord> _acYCodeBook;
        private List<AcCategoryEncodedPair> _acYValues;
        private Dictionary<int, CodeWord> _dcCbCrCodeBook;
        private List<DcCategoryEncodedPair> _dcCbValues;
        private List<DcCategoryEncodedPair> _dcCrValues;
        private Dictionary<int, CodeWord> _dcYCodeBook;
        private List<DcCategoryEncodedPair> _dcYValues;
        private readonly Image _image;

        private JpegEncoder(Image image)
        {
            _image = image;
        }


        public static JpegEncoder WithImageFromFile(string filename)
        {
            FileStream fileStream = new FileStream(filename, FileMode.Open);
            MemoryStream memoryStream = new MemoryStream();
            
            fileStream.CopyTo(memoryStream);

            memoryStream.Seek(0,SeekOrigin.Begin);
            
            RGBImage rgbImage = RGBImage.RGBImageBuilder.From(memoryStream).Build();
            YCbCrImage yCbCrImage = ColorChannels.RgbToYCbCr(rgbImage);
            return new JpegEncoder(yCbCrImage);
        }

        public static JpegEncoder WithImage(Image image)
        {
            return new JpegEncoder(image);
        }

        public JpegEncoder ConvertToJpeg(int subsampling)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            
            ((YCbCrImage) _image).Reduce(subsampling);
            
            Console.WriteLine("Finished Subsampling in "
                              + (stopwatch.ElapsedMilliseconds / 1000d
                                 + " seconds"));
            stopwatch.Restart();
            
            PerformDCT();
            
            Console.WriteLine("Finished DCT in "
                              + (stopwatch.ElapsedMilliseconds / 1000d
                                 + " seconds"));
            stopwatch.Restart();
            
            PerformQuantization();
            
            Console.WriteLine("Finished Quantisation in "
                              + (stopwatch.ElapsedMilliseconds / 1000d
                                 + " seconds"));
            stopwatch.Restart();
            
            PerformAcDcEncoding();
            
            Console.WriteLine("Finished AcDc in "
                              + (stopwatch.ElapsedMilliseconds / 1000d
                                 + " seconds"));
            stopwatch.Restart();
            
            PerformHuffmanEncoding();
            
            Console.WriteLine("Finished Huffman in "
                              + (stopwatch.ElapsedMilliseconds / 1000d
                                 + " seconds"));
            stopwatch.Stop();
            
            return this;
        }

        private void PerformDCT()
        {
            TransformChannel(_image.GetChannel1());
            TransformChannel(_image.GetChannel2());
            TransformChannel(_image.GetChannel3());
        }

        private void PerformQuantization()
        {
            QuantizeChannel(_image.GetChannel1(), QuantizationTable.QuantizationMatrixLuminance);
            QuantizeChannel(_image.GetChannel2(), QuantizationTable.QuantizationMatrixChrominance);
            QuantizeChannel(_image.GetChannel3(), QuantizationTable.QuantizationMatrixChrominance);
        }

        private void PerformAcDcEncoding()
        {
            _dcYValues = AcDcEncoder.GetAllDCs(_image.GetChannel1());
            GetDcCbCrValues();
            _acYValues = AcDcEncoder.GetAllACs(_image.GetChannel1());
            GetAcCbCrValues();
        }

        private void PerformHuffmanEncoding()
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
            foreach (DcCategoryEncodedPair dcCategoryEncodedPair in _dcYValues)
                symbols[index++] = dcCategoryEncodedPair.GetPair();
            _dcYCodeBook = HuffmanEncoder.Encode(symbols).ForJpeg().GetCodeBookAsDictionary();
        }

        private void HuffmanEncodeAcy()
        {
            int[] symbols = new int[_acYValues.Count];
            int index = 0;
            foreach (AcCategoryEncodedPair acCategoryEncodedPair in _acYValues)
                symbols[index++] = acCategoryEncodedPair.GetPair();
            _acYCodeBook = HuffmanEncoder.Encode(symbols).ForJpeg().GetCodeBookAsDictionary();
        }

        private void HuffmanEncodeDcCbCr()
        {
            List<int> symbols = new List<int>();
            List<DcCategoryEncodedPair> dcCbCrValues = _dcCbValues;
            dcCbCrValues.AddRange(_dcCrValues);
            foreach (DcCategoryEncodedPair dcCategoryEncodedPair in dcCbCrValues)
                symbols.Add(dcCategoryEncodedPair.GetPair());

            _dcCbCrCodeBook = HuffmanEncoder.Encode(symbols.ToArray())
                .ForJpeg()
                .GetCodeBookAsDictionary();
        }

        private void HuffmanEncodeAcCbCr()
        {
            List<int> symbols = new List<int>();
            List<AcCategoryEncodedPair> acCbCrValues = _acCbValues;
            acCbCrValues.AddRange(_acCrValues);
            foreach (AcCategoryEncodedPair acCategoryEncodedPair in acCbCrValues)
                symbols.Add(acCategoryEncodedPair.GetPair());
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
            for (int i = 0; i < channel.GetNumOfBlocks(); i++) Arai.Calc(channel.GetBlock(i));
        }

        private void QuantizeChannel(ColorChannel channel, DoubleMatrix quantizationTable)
        {
            for (int i = 0; i < channel.GetNumOfBlocks(); i++)
                CosineTransformation.Quantize(channel.GetBlock(i), quantizationTable);
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

        public void WriteImageToDisk(string filename)
        {
            try
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                
                byte[] bytes = new byte[1000000];
                BitStream bos = new BitStream(bytes, true);
                List<SegmentWriter> segmentWriters = new List<SegmentWriter>();
                segmentWriters.Add(new SoiWriter(bos));
                segmentWriters.Add(new App0Writer(bos, 0x0048, 0x0048));
                segmentWriters.Add(new DQTWriter(bos));
                segmentWriters.Add(new Sof0Writer(bos,
                    _image.GetOriginalWidth(),
                    _image.GetOriginalHeight(),
                    _image.GetSubSampling()));

                List<HuffmanTable> huffmanTables = new List<HuffmanTable>();
                huffmanTables.Add(GetHuffmanTableDcy());
                huffmanTables.Add(GetHuffmanTableAcy());
                huffmanTables.Add(GetHuffmanTableDcCbCr());
                huffmanTables.Add(GetHuffmanTableAcCbCr());
                segmentWriters.Add(new DhtWriter(bos, huffmanTables));
                segmentWriters.Add(new SosWriter(bos));
                segmentWriters.Add(new ImageDataWriter(bos, 
                    _image,
                    _dcYCodeBook, 
                    _acYCodeBook, 
                    _dcCbCrCodeBook,
                    _acCbCrCodeBook));
                segmentWriters.Add(new EoiWriter(bos));
                
                Console.WriteLine("Prepared Segments in "
                                  + (stopwatch.ElapsedMilliseconds / 1000d
                                     + " seconds"));
                stopwatch.Restart();

                foreach (SegmentWriter segmentWriter in segmentWriters)
                    segmentWriter.WriteSegment();
                
                Console.WriteLine("Wrote Segments in "
                                  + (stopwatch.ElapsedMilliseconds / 1000d
                                     + " seconds"));
                stopwatch.Restart();

                Console.WriteLine("Writing to Disk...");
                
                bos.GetStream().SetLength(bos.GetStream().Position);
                bos.SaveStreamAsFile(filename);
                
                Console.WriteLine("Write Done in " + (stopwatch.ElapsedMilliseconds / 1000d
                                                      + " seconds"));
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