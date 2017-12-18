using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Keeg.SharpBsaLib.DDS
{
    public static class DDS
    {
        public const int DDS_MAGIC = 0x20534444;    // "DDS "

        public const int DDS_FOURCC = 0x00000004;   // DDPF_FOURCC
        public const int DDS_RGB = 0x00000040;      // DDPF_RGB
        public const int DDS_RGBA = 0x00000041;     // DDPF_RGB | DDPF_ALPHAPIXELS
        public const int DDS_LUMINANCE = 0x00020000; // DDPF_LUMINANCE
        public const int DDS_ALPHA = 0x00000002;    // DDPF_ALPHA 

        public const int DDS_HEADER_FLAGS_TEXTURE = 0x00001007;     // DDSD_CAPS | DDSD_HEIGHT | DDSD_WIDTH | DDSD_PIXELFORMAT
        public const int DDS_HEADER_FLAGS_MIPMAP = 0x00020000;      // DDSD_MIPMAPCOUNT
        public const int DDS_HEADER_FLAGS_LINEARSIZE = 0x00080000;  // DDSD_LINEARSIZE
        public const int DDS_HEADER_FLAGS_VOLUME = 0x00800000;      // DDSD_DEPTH
        public const int DDS_HEADER_FLAGS_PITCH = 0x00000008;       // DDSD_PITCH

        public const int DDS_HEIGHT = 0x00000002;   // DDSD_HEIGHT
        public const int DDS_WIDTH = 0x00000004;    // DDSD_WIDTH

        public const int DDS_SURFACE_FLAGS_TEXTURE = 0x00001000; // DDSCAPS_TEXTURE
        public const int DDS_SURFACE_FLAGS_MIPMAP = 0x00400008; // DDSCAPS_COMPLEX | DDSCAPS_MIPMAP
        public const int DDS_SURFACE_FLAGS_CUBEMAP = 0x00000008; // DDSCAPS_COMPLEX
        public const int DDS_CUBEMAP_POSITIVEX = 0x00000600; // DDSCAPS2_CUBEMAP | DDSCAPS2_CUBEMAP_POSITIVEX
        public const int DDS_CUBEMAP_NEGATIVEX = 0x00000A00; // DDSCAPS2_CUBEMAP | DDSCAPS2_CUBEMAP_NEGATIVEX
        public const int DDS_CUBEMAP_POSITIVEY = 0x00001200; // DDSCAPS2_CUBEMAP | DDSCAPS2_CUBEMAP_POSITIVEY
        public const int DDS_CUBEMAP_NEGATIVEY = 0x00002200; // DDSCAPS2_CUBEMAP | DDSCAPS2_CUBEMAP_NEGATIVEY
        public const int DDS_CUBEMAP_POSITIVEZ = 0x00004200; // DDSCAPS2_CUBEMAP | DDSCAPS2_CUBEMAP_POSITIVEZ
        public const int DDS_CUBEMAP_NEGATIVEZ = 0x00008200; // DDSCAPS2_CUBEMAP | DDSCAPS2_CUBEMAP_NEGATIVEZ

        public const int DDS_CUBEMAP_ALLFACES = (DDS_CUBEMAP_POSITIVEX | DDS_CUBEMAP_NEGATIVEX
            | DDS_CUBEMAP_POSITIVEY | DDS_CUBEMAP_NEGATIVEY | DDS_CUBEMAP_POSITIVEZ | DDS_CUBEMAP_NEGATIVEZ);

        public const int DDS_CUBEMAP = 0x00000200; // DDSCAPS2_CUBEMAP

        public const int DDS_FLAGS_VOLUME = 0x00200000; // DDSCAPS2_VOLUME

        public static uint MakeFourcc(char ch0, char ch1, char ch2, char ch3)
        {
            return (Convert.ToByte(ch0)
                | ((uint)Convert.ToByte(ch1) << 8)
                | ((uint)Convert.ToByte(ch1) << 16)
                | ((uint)Convert.ToByte(ch1) << 24));
        }
    }
}
