using BitStreams;

namespace JPEG_Encoder.segments.soi
{
    public class SosComponent
    {
        private readonly int _id;
        private readonly int _huffmanTableIdAc;
        private readonly int _huffmanTableIdDc;

        public SosComponent(int id, int huffmanTableIdAc, int huffmanTableIdDc)
        {
            _id = id;
            _huffmanTableIdAc = huffmanTableIdAc;
            _huffmanTableIdDc = huffmanTableIdDc;
        }

        public void WriteComponent(BitStream bos)
        {
            bos.WriteByte((byte) _id);
            bos.WriteByte((byte) ((_huffmanTableIdAc << 4) | (_huffmanTableIdDc << 0)));
        }
    }
}