using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Keeg.SharpBsaLib.DDS
{
    public class DdsHeaderDx10
    {
        DXGI_FORMAT DxgiFormat { get; set; }
        DDS_RESOURCE_DIMENSION ResourceDimension { get; set; }
        uint MiscFlag { get; set; } // see D3D11_RESOURCE_MISC_FLAG
        uint ArraySize { get; set; }
        uint MiscFlags2 { get; set; } // see DDS_MISC_FLAGS2
    }
}
