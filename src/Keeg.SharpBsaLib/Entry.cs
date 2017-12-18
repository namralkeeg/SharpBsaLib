using System;
using System.Diagnostics.Contracts;
using System.IO;

namespace Keeg.SharpBsaLib
{
    public class Entry : IEntry, IComparable<IEntry>
    {
        public virtual string Extension => Path.GetExtension(FullPath);
        public string FullName { get; set; }
        public string FullPath => Path.GetFullPath(FullName);
        public string Name => Path.GetFileName(FullName);
        public ulong NameHash { get; set; }
        public ulong PathHash { get; set; }
        public ulong Offset { get; set; }
        public virtual bool Compressed { get { return CompressedSize > 0; } }

        public uint CompressedSize { get; set; }
        public uint UncompressedSize { get; set; }

        public int CompareTo(IEntry other)
        {
            return FullName.CompareTo(other.FullName);
        }

        public static bool operator > (Entry lhs, Entry rhs)
        {
            if (lhs == null)
                throw new ArgumentNullException(nameof(lhs));
            if (rhs == null)
                throw new ArgumentNullException(nameof(rhs));
            Contract.EndContractBlock();

            return lhs.CompareTo(rhs) == 1;
        }

        public static bool operator < (Entry lhs, Entry rhs)
        {
            if (lhs == null)
                throw new ArgumentNullException(nameof(lhs));
            if (rhs == null)
                throw new ArgumentNullException(nameof(rhs));
            Contract.EndContractBlock();

            return lhs.CompareTo(rhs) == -1;
        }

        public static bool operator >= (Entry lhs, Entry rhs)
        {
            if (lhs == null)
                throw new ArgumentNullException(nameof(lhs));
            if (rhs == null)
                throw new ArgumentNullException(nameof(rhs));
            Contract.EndContractBlock();

            return lhs.CompareTo(rhs) >= 0;
        }

        public static bool operator <= (Entry lhs, Entry rhs)
        {
            if (lhs == null)
                throw new ArgumentNullException(nameof(lhs));
            if (rhs == null)
                throw new ArgumentNullException(nameof(rhs));
            Contract.EndContractBlock();

            return lhs.CompareTo(rhs) <= 0;
        }
    }
}
