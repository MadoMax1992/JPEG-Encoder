using System.Collections.Generic;
using BitStreams;
using JPEG_Encoder.encoding;
using JPEG_Encoder.encoding.acdc;
using JPEG_Encoder.encoding.huffman;
using JPEG_Encoder.image;
using JPEG_Encoder.image.colors;

namespace JPEG_Encoder.segments.imageData
{
    public class ImageDataWriter : SegmentWriter
    {
        private readonly Image _image;
        private readonly Dictionary<int, CodeWord> _dcYCodeBook;
        private readonly Dictionary<int, CodeWord> _acYCodeBook;
        private readonly Dictionary<int, CodeWord> _dcCbCrCodeBook;
        private readonly Dictionary<int, CodeWord> _acCbCrCodeBook;
        private readonly int _subSampling;

        public ImageDataWriter(BitStream os, Image image,
            Dictionary<int, CodeWord> dcYCodeBook,
            Dictionary<int, CodeWord> acYCodeBook,
            Dictionary<int, CodeWord> dcCbCrCodeBook,
            Dictionary<int, CodeWord> acCbCrCodeBook) : base(os)
        {
            _subSampling = image.GetSubSampling();
            _image = image;
            _dcYCodeBook = dcYCodeBook;
            _acYCodeBook = acYCodeBook;
            _dcCbCrCodeBook = dcCbCrCodeBook;
            _acCbCrCodeBook = acCbCrCodeBook;
        }

        public override void WriteSegment()
        {
            if (_image.GetSubSampling() == 1)
            {
                WriteSegmentWithoutSubsampling();
            }
            else
            {
                WriteSegmentWithSubsampling();
            }
        }

        private void WriteSegmentWithoutSubsampling()
        {
            for (int currentY = 0; currentY < _image.GetChannel1().GetHeightInBlocks(); currentY++)
            {
                for (int currentX = 0; currentX < _image.GetChannel1().GetWidthInBlocks(); currentX++)
                {
                    WriteAcDcEncodedBlock(_image.GetChannel1(), currentX, currentY, _dcYCodeBook, _acYCodeBook);
                    WriteAcDcEncodedBlock(_image.GetChannel2(), currentX, currentY, _dcCbCrCodeBook, _acCbCrCodeBook);
                    WriteAcDcEncodedBlock(_image.GetChannel3(), currentX, currentY, _dcCbCrCodeBook, _acCbCrCodeBook);
                }
            }

           Util.Flush(BitStream);
           BitStream.WriteByte(0xFF);
        }

        private void WriteSegmentWithSubsampling()
        {
            for (int currentY = 0; currentY < _image.GetChannel2().GetHeightInBlocks(); currentY++)
            {
                for (int currentX = 0; currentX < _image.GetChannel2().GetWidthInBlocks(); currentX++)
                {
                    for (int currentYLuminance = currentY * _subSampling;
                        currentYLuminance < currentY * _subSampling + _subSampling;
                        currentYLuminance++)
                    {
                        for (int currentXLuminance = currentX * _subSampling;
                            currentXLuminance < currentX * _subSampling + _subSampling;
                            currentXLuminance++)
                        {
                            WriteAcDcEncodedBlock(_image.GetChannel1(),
                                currentXLuminance,
                                currentYLuminance,
                                _dcYCodeBook,
                                _acYCodeBook);
                        }
                    }

                    WriteAcDcEncodedBlock(_image.GetChannel2(), currentX, currentY, _dcCbCrCodeBook, _acCbCrCodeBook);
                    WriteAcDcEncodedBlock(_image.GetChannel3(), currentX, currentY, _dcCbCrCodeBook, _acCbCrCodeBook);
                }
            }
            
            Util.Flush(BitStream);
            BitStream.WriteByte(0xFF);
        }

        private void WriteAcDcEncodedBlock(ColorChannel channel, int xOfChannel, int yOfChannel,
            Dictionary<int, CodeWord> dcCodeBook,
            Dictionary<int, CodeWord> acCodeBook)
        {
            DcCategoryEncodedPair dc = AcDcEncoder.CalculateDifferenceDc(channel,
                channel
                    .GetPlainIndexOfBlock(
                        xOfChannel,
                        yOfChannel));

            List<AcRunlengthEncodedPair> acRunLengthEncodedPairs =
                AcDcEncoder.EncodeRunlength(Util.ZigzagSort(channel.GetBlock(xOfChannel,
                    yOfChannel)));
            List<AcCategoryEncodedPair> acCategoryEncodedPairs = AcDcEncoder.EncodeCategoriesAc(
                acRunLengthEncodedPairs);
            AcDcEncoder.WriteDcCoefficient(BitStream, dc, dcCodeBook);
            AcDcEncoder.WriteAcCoefficients(BitStream, acCategoryEncodedPairs, acCodeBook);
        }
    }
}