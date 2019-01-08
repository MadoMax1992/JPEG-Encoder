using System;
using System.Collections.Generic;
using BitStreams;
using CenterSpace.NMath.Core;
using JPEG_Encoder.encoding.huffman;
using JPEG_Encoder.image.colors;

namespace JPEG_Encoder.encoding.acdc
{
    public class AcDcEncoder
    {
        public static List<DCCategoryEncodedPair> GetAllDCs(ColorChannel channel)
        {
            List<DCCategoryEncodedPair> result = new List<DCCategoryEncodedPair>(channel.GetNumOfBlocks());
            for (int i = 0; i < channel.GetNumOfBlocks(); i++)
            {
                result.Add(CalculateDifferenceDC(channel, i));
            }

            return result;
        }

        public static DCCategoryEncodedPair CalculateDifferenceDC(ColorChannel channel, int i)
        {
            int result = (int) channel.GetBlock(i)[0,0];
            if (i != 0)
            {
                result = (int) channel.GetBlock(i)[0, 0] - (int) channel.GetBlock(i - 1)[0, 0];
            }

            return new DCCategoryEncodedPair(AbstractCategoryEncodedPair.CalculateCategory(result),
                AbstractCategoryEncodedPair.EncodeCategory(result));
        }

        public static List<ACCategoryEncodedPair> GetAllACs(ColorChannel channel)
        {
            List<ACCategoryEncodedPair> result = new List<ACCategoryEncodedPair>();
            for (int i = 0; i < channel.GetNumOfBlocks(); i++)
            {
                DoubleMatrix block = channel.GetBlock(i);
                List<ACRunlengthEncodedPair> runLengthEncodedBlock = EncodeRunlength(Util.ZigzagSort(block));
                List<ACCategoryEncodedPair> categoryEncodedBlock = EncodeCategoriesAc(runLengthEncodedBlock);
                result.AddRange(categoryEncodedBlock);
            }

            return result;
        }
        
        public static List<ACRunlengthEncodedPair> EncodeRunlength(int[] zigzaged)
        {
            List<ACRunlengthEncodedPair> resultList = new List<ACRunlengthEncodedPair>();
            // loop starts at index 1 because index 0 is DC
            int zeroCount = 0;
            for (int i = 1; i < zigzaged.Length; i++)
            {
                if (zigzaged[i] != 0 || zeroCount == 15)
                {
                    resultList.Add(new ACRunlengthEncodedPair(zeroCount, zigzaged[i]));
                    zeroCount = 0;
                }
                else
                {
                    zeroCount++;
                }
            }
            // EOB
            if (zeroCount != 0 || resultList[resultList.Count - 1].GetEntry() == 0)
            {
                while (resultList.Count != 0 && resultList[resultList.Count - 1].GetEntry() == 0)
                {
                    resultList.RemoveAt(resultList.Count - 1);
                }
                resultList.Add(new ACRunlengthEncodedPair(0, 0));
            }
            return resultList;
        }
        
        public static List<ACCategoryEncodedPair> EncodeCategoriesAc(List<ACRunlengthEncodedPair> acRunlengthEncodedPairs)
        {
            List<ACCategoryEncodedPair> resultList = new List<ACCategoryEncodedPair>();
            foreach (ACRunlengthEncodedPair acRunlengthEncodedPair in acRunlengthEncodedPairs)
            {
                int category = AbstractCategoryEncodedPair.CalculateCategory(acRunlengthEncodedPair.GetEntry());
                int pair = (acRunlengthEncodedPair.getZeroCount() << 4) +
                           category;
                int categoryEncoded = ACCategoryEncodedPair.EncodeCategory(acRunlengthEncodedPair.GetEntry());
                resultList.Add(new ACCategoryEncodedPair(pair, categoryEncoded));
            }
            return resultList;
        }

        public static void writeACCoefficients(BitStream bos, List<ACCategoryEncodedPair> acEncoding,
            Dictionary<int, CodeWord> codebook)
        {
            foreach (ACCategoryEncodedPair ac in acEncoding)
            {
                CodeWord codeWord = codebook[ac.GetPair()];
                bos.WriteInt32(codeWord.GetCode());
                bos.WriteInt32(ac.GetEntryCategoryEncoded());
                Log(codeWord, ac);
            }
        }
        
        public static void writeDCCoefficient(BitStream bos, DCCategoryEncodedPair dc,
            Dictionary<int, CodeWord> codebook)
        {
            CodeWord codeWord = codebook[dc.GetPair()];
            bos.WriteInt32(codeWord.GetCode());
            bos.WriteInt32(dc.GetEntryCategoryEncoded());
            Log(codeWord, dc);
        }
        
        private static void Log(CodeWord codeWord, AbstractCategoryEncodedPair pair)
        {
            bool shouldLog = true;
            if (shouldLog)
            {
                Console.WriteLine(Util.GetBitsAsString(codeWord.GetCode(), codeWord.GetLength()));
                Console.WriteLine(Util.GetBitsAsString(pair.GetEntryCategoryEncoded(), pair.GetPair() & 0xf));
            }
        }
    }
}