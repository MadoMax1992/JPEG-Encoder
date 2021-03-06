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
        public static List<DcCategoryEncodedPair> GetAllDCs(ColorChannel channel)
        {
            List<DcCategoryEncodedPair> result = new List<DcCategoryEncodedPair>(channel.GetNumOfBlocks());
            for (int i = 0; i < channel.GetNumOfBlocks(); i++) result.Add(CalculateDifferenceDc(channel, i));

            return result;
        }

        public static DcCategoryEncodedPair CalculateDifferenceDc(ColorChannel channel, int i)
        {
            int result = (int) channel.GetBlock(i)[0, 0];
            if (i != 0) result = (int) channel.GetBlock(i)[0, 0] - (int) channel.GetBlock(i - 1)[0, 0];

            return new DcCategoryEncodedPair(AbstractCategoryEncodedPair.CalculateCategory(result),
                AbstractCategoryEncodedPair.EncodeCategory(result));
        }

        public static List<AcCategoryEncodedPair> GetAllACs(ColorChannel channel)
        {
            List<AcCategoryEncodedPair> result = new List<AcCategoryEncodedPair>();
            for (int i = 0; i < channel.GetNumOfBlocks(); i++)
            {
                DoubleMatrix block = channel.GetBlock(i);
                List<AcRunlengthEncodedPair> runLengthEncodedBlock = EncodeRunlength(Util.ZigzagSort(block));
                List<AcCategoryEncodedPair> categoryEncodedBlock = EncodeCategoriesAc(runLengthEncodedBlock);
                result.AddRange(categoryEncodedBlock);
            }

            return result;
        }

        public static List<AcRunlengthEncodedPair> EncodeRunlength(int[] zigzaged)
        {
            List<AcRunlengthEncodedPair> resultList = new List<AcRunlengthEncodedPair>();
            // loop starts at index 1 because index 0 is DC
            int zeroCount = 0;
            for (int i = 1; i < zigzaged.Length; i++)
                if (zigzaged[i] != 0 || zeroCount == 15)
                {
                    resultList.Add(new AcRunlengthEncodedPair(zeroCount, zigzaged[i]));
                    zeroCount = 0;
                }
                else
                {
                    zeroCount++;
                }

            // EOB
            if (zeroCount == 0 && resultList[resultList.Count - 1].GetEntry() != 0) return resultList;
            while (resultList.Count != 0 && resultList[resultList.Count - 1].GetEntry() == 0)
                resultList.RemoveAt(resultList.Count - 1);
            resultList.Add(new AcRunlengthEncodedPair(0, 0));

            return resultList;
        }

        public static List<AcCategoryEncodedPair> EncodeCategoriesAc(
            IEnumerable<AcRunlengthEncodedPair> acRunLengthEncodedPairs)
        {
            List<AcCategoryEncodedPair> resultList = new List<AcCategoryEncodedPair>();
            foreach (AcRunlengthEncodedPair acRunLengthEncodedPair in acRunLengthEncodedPairs)
            {
                int category = AbstractCategoryEncodedPair.CalculateCategory(acRunLengthEncodedPair.GetEntry());
                int pair = (acRunLengthEncodedPair.GetZeroCount() << 4) +
                           category;
                int categoryEncoded = AbstractCategoryEncodedPair.EncodeCategory(acRunLengthEncodedPair.GetEntry());
                resultList.Add(new AcCategoryEncodedPair(pair, categoryEncoded));
            }

            return resultList;
        }

        public static void WriteAcCoefficients(BitStream bos, IEnumerable<AcCategoryEncodedPair> acEncoding,
            Dictionary<int, CodeWord> codeBook)
        {
            foreach (AcCategoryEncodedPair ac in acEncoding)
            {
                CodeWord codeWord = codeBook[ac.GetPair()];
                Util.WriteBitsForAcDc(codeWord.GetCodeAsBitArray(codeWord.GetLength()), bos, codeWord.GetLength());
                Util.WriteBitsForAcDc(ac.GetEntryCategoryEncodedAsBitArray(ac.GetCategory()), bos, ac.GetCategory());
                Log(codeWord, ac);
            }
        }

        public static void WriteDcCoefficient(BitStream bos, DcCategoryEncodedPair dc,
            Dictionary<int, CodeWord> codeBook)
        {
            CodeWord codeWord = codeBook[dc.GetPair()];
            Util.WriteBitsForAcDc(codeWord.GetCodeAsBitArray(codeWord.GetLength()), bos, codeWord.GetLength());
            Util.WriteBitsForAcDc(dc.GetEntryCategoryEncodedAsBitArray(dc.GetPair()), bos, dc.GetPair());
            Log(codeWord, dc);
        }

        private static void Log(CodeWord codeWord, AbstractCategoryEncodedPair pair)
        {
            const bool shouldLog = false;
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (!shouldLog) return;
            Console.WriteLine(Util.GetBitsAsString(codeWord.GetCode(), codeWord.GetLength()));
            Console.WriteLine(Util.GetBitsAsString(pair.GetEntryCategoryEncoded(), pair.GetPair() & 0xf));
        }
    }
}