using System;
using System.Collections.Generic;

namespace Keeg.SharpBsaLib
{
    public interface IFolder : IComparable<IFolder>
    {
        IFolder Parent { get; set; }
        IList<IFolder> Children { get; }
        IList<IEntry> Entries { get; }
        string Name { get; set; }
    }
}
