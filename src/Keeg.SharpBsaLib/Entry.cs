using System;
using System.IO;
using System.Text;

namespace Keeg.SharpBsaLib
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class Entry : IComparable<Entry>
    {
        #region Instance Fields
        private string _name;
        private ulong _offset;
        private uint _compressedSize;
        private uint _uncompressedSize;
        #endregion

        #region Properties
        public virtual string Name { get => _name; set => _name = value; }
        public virtual ulong Offset { get => _offset; set => _offset = value; }
        public virtual bool Compressed { get { return CompressedSize > 0; } }
        public virtual uint CompressedSize { get => _compressedSize; set => _compressedSize = value; }
        public virtual uint UncompressedSize { get => _uncompressedSize; set => _uncompressedSize = value; }
        #endregion

        /// <summary>
		/// Cleans a name making it conform to Zip file conventions.
		/// Devices names ('c:\') and UNC share names ('\\server\share') are removed
		/// and back slashes ('/') are converted to forward slashes ('\').
		/// Names are made relative by trimming leading slashes which is compatible
		/// with the BSA naming convention.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string CleanName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return string.Empty;
            }

            StringBuilder sb;
            if (Path.IsPathRooted(name))
            {
                // NOTE:
                // for UNC names...  \\machine\share\yadda\yadda.txt gives \yadda\yadda.txt
                sb = new StringBuilder(name.Substring(Path.GetPathRoot(name).Length));
            }
            else
            {
                sb = new StringBuilder(name);
            }

            sb.Replace('/', '\\');
            while ((sb.Length > 0) && (sb[0] == '\\'))
            {
                sb = sb.Remove(0, 1);
            }

            return sb.ToString();
        }

        public override string ToString()
        {
            return _name;
        }

        #region IComparable<T>
        public virtual int CompareTo(Entry other)
        {
            if (other == null)
                return -1;

            return Name.CompareTo(other.Name);
        }

        public static bool operator >(Entry lhs, Entry rhs)
        {
            if (lhs == null)
                throw new ArgumentNullException(nameof(lhs));
            if (rhs == null)
                throw new ArgumentNullException(nameof(rhs));

            return lhs.CompareTo(rhs) == 1;
        }

        public static bool operator <(Entry lhs, Entry rhs)
        {
            if (lhs == null)
                throw new ArgumentNullException(nameof(lhs));
            if (rhs == null)
                throw new ArgumentNullException(nameof(rhs));

            return lhs.CompareTo(rhs) == -1;
        }

        public static bool operator >=(Entry lhs, Entry rhs)
        {
            if (lhs == null)
                throw new ArgumentNullException(nameof(lhs));
            if (rhs == null)
                throw new ArgumentNullException(nameof(rhs));

            return lhs.CompareTo(rhs) >= 0;
        }

        public static bool operator <=(Entry lhs, Entry rhs)
        {
            if (lhs == null)
                throw new ArgumentNullException(nameof(lhs));
            if (rhs == null)
                throw new ArgumentNullException(nameof(rhs));

            return lhs.CompareTo(rhs) <= 0;
        }
        #endregion
    }
}
