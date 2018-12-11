using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BitStreams;

namespace JPEG_Encoder
{
    public class BitStreamPP : BitStream
    {
        public uint BitLength { get; private set; } = 0;
        public long FullBitLength => ((base.Length - 1) << 3) | BitLength;
        
        public BitStreamPP(Stream stream, bool MSB = false) : base(stream, MSB)
        {
        }

        public BitStreamPP(Stream stream, Encoding encoding, bool MSB = false) : base(stream, encoding, MSB)
        {
        }

        public BitStreamPP(byte[] buffer, bool MSB = false) : base(buffer, MSB)
        {
        }

        public BitStreamPP(byte[] buffer, Encoding encoding, bool MSB = false) : base(buffer, encoding, MSB)
        {
        }
        
        public new void WriteBit(Bit data)
        {
            BitLength = (BitLength + 1) % 8;
            base.WriteBit(data);
        }

        public new void WriteBits(ICollection<Bit> bits)
        {
            BitLength = Convert.ToByte(((BitLength + bits.Count) % 8));
            base.WriteBits(bits);
        }

        public void WriteMarker(int marker)
        {
            WriteByte((byte) marker);
        }
    }
}