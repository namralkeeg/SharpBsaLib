namespace Keeg.SharpBsaLib.DDS
{
    public class DdsHeader
    {
        public uint Size { get; set; }
        public uint HeaderFlags { get; set; }
        public uint Height { get; set; }
        public uint Width { get; set; }
        public uint PitchOrLinearSize { get; set; }
        public uint Depth { get; set; } // only if DDS_HEADER_FLAGS_VOLUME is set in HeaderFlags
        public uint MipMapCount { get; set; }
        public uint[] Reserved1 { get; set; } // [11]
        public DdsPixelFormat PixelFormat { get; set; } // ddspf
        public uint SurfaceFlags;
        public uint CubemapFlags;
        public uint[] Reserved2; // [3]

        public DdsHeader()
        {
            Size = 124; // (9 * 4) + (8 * 4) + (14 * 4)
            Reserved1 = new uint[11];
            PixelFormat = new DdsPixelFormat();
            Reserved2 = new uint[3];
        }
    }
}
