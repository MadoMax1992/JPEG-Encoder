using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPEG_Encoder
{
    class RGBPixel
    {
        public int xPixelCord { get; set; }
        public int yPixelCord { get; set; }
        public byte greenValue { get; set; }
        public byte redValue { get; set; }
        public byte blueValue { get; set; }
    }
}
