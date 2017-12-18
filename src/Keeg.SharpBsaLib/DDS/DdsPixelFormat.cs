namespace Keeg.SharpBsaLib.DDS
{
    public class DdsPixelFormat
    {
        public uint Size { get; set; }
        public uint Flags { get; set; }
        public uint FourCC { get; set; }
        public uint RGBBitCount { get; set; }
        public uint RBitMask { get; set; }
        public uint GBitMask { get; set; }
        public uint BBitMask { get; set; }
        public uint ABitMask { get; set; }

        public DdsPixelFormat()
        {
            Size = 32; // 8 * sizeof(uint)
        }
    }
}
