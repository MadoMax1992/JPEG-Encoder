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
        private Image image;
        private Dictionary<int, CodeWord> dcYCodeBook;
        private Dictionary<int, CodeWord> acYCodeBook;
        private Dictionary<int, CodeWord> dcCbCrCodeBook;
        private Dictionary<int, CodeWord> acCbCrCodeBook;
        private int subSampling;

        public ImageDataWriter(BitStream os, Image image,
            Dictionary<int, CodeWord> dcYCodeBook,
            Dictionary<int, CodeWord> acYCodeBook,
            Dictionary<int, CodeWord> dcCbCrCodeBook,
            Dictionary<int, CodeWord> acCbCrCodeBook) : base(os)
        {
            subSampling = image.GetSubSampling();
            this.image = image;
            this.dcYCodeBook = dcYCodeBook;
            this.acYCodeBook = acYCodeBook;
            this.dcCbCrCodeBook = dcCbCrCodeBook;
            this.acCbCrCodeBook = acCbCrCodeBook;
        }

        public override void WriteSegment()
        {
            if (image.GetSubSampling() == 1)
            {
                WriteSegmentWithoutSubsampling();
            }
            else
            {
                WriteSegmentWithSubsampling();
            }
        }

        public void WriteSegmentWithoutSubsampling()
        {
            for (int currentY = 0; currentY < image.GetChannel1().GetHeightInBlocks(); currentY++)
            {
                for (int currentX = 0; currentX < image.GetChannel1().GetWidthInBlocks(); currentX++)
                {
                    WriteAcDcEncodedBlock(image.GetChannel1(), currentX, currentY, dcYCodeBook, acYCodeBook);
                    WriteAcDcEncodedBlock(image.GetChannel2(), currentX, currentY, dcCbCrCodeBook, acCbCrCodeBook);
                    WriteAcDcEncodedBlock(image.GetChannel3(), currentX, currentY, dcCbCrCodeBook, acCbCrCodeBook);
                }
            }


                while (BitStream.BitPosition != 0)
                {
                    BitStream.AdvanceBit();
                }
            
            //BitStream.flush();
            BitStream.WriteByte(0xFF);
        }

        public void WriteSegmentWithSubsampling()
        {
            for (int currentY = 0; currentY < image.GetChannel2().GetHeightInBlocks(); currentY++)
            {
                for (int currentX = 0; currentX < image.GetChannel2().GetWidthInBlocks(); currentX++)
                {
                    for (int currentYLuminance = currentY * subSampling;
                        currentYLuminance < currentY * subSampling + subSampling;
                        currentYLuminance++)
                    {
                        for (int currentXLuminance = currentX * subSampling;
                            currentXLuminance < currentX * subSampling + subSampling;
                            currentXLuminance++)
                        {
                            WriteAcDcEncodedBlock(image.GetChannel1(),
                                currentXLuminance,
                                currentYLuminance,
                                dcYCodeBook,
                                acYCodeBook);
                        }
                    }

                    WriteAcDcEncodedBlock(image.GetChannel2(), currentX, currentY, dcCbCrCodeBook, acCbCrCodeBook);
                    WriteAcDcEncodedBlock(image.GetChannel3(), currentX, currentY, dcCbCrCodeBook, acCbCrCodeBook);
                }
            }

//            BitStream.flush();
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

            List<AcRunlengthEncodedPair> acRunlengthEncodedPairs =
                AcDcEncoder.EncodeRunlength(Util.ZigzagSort(channel.GetBlock(xOfChannel,
                    yOfChannel)));
            List<AcCategoryEncodedPair> acCategoryEncodedPairs = AcDcEncoder.EncodeCategoriesAc(
                acRunlengthEncodedPairs);
            AcDcEncoder.WriteDcCoefficient(BitStream, dc, dcCodeBook);
            AcDcEncoder.WriteAcCoefficients(BitStream, acCategoryEncodedPairs, acCodeBook);
        }
    }
}