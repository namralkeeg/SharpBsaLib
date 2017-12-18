using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Keeg.SharpBsaLib
{
    public interface IEntry
    {
        string Extension { get; }
        string FullName { get; set; }
        string FullPath { get; }
        string Name { get; }
        ulong NameHash { get; set; }
        ulong PathHash { get; set; }
        ulong Offset { get; set; }
        bool Compressed { get; }
        uint CompressedSize { get; set; }
        uint UncompressedSize { get; set; }
    }
}
