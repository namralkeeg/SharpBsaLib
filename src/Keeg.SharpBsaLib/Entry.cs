using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Keeg.SharpBsaLib
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class Entry : IComparable<Entry>, IEquatable<Entry>
    {
        #region Instance Fields
        protected string _name;
        protected ulong _offset;
        protected uint _compressedSize;
        protected uint _uncompressedSize;
        #endregion

        #region Properties
        public virtual string Name { get => _name; set => _name = value; }
        public virtual ulong Offset { get => _offset; set => _offset = value; }
        public virtual bool Compressed => CompressedSize > 0;
        public virtual uint CompressedSize { get => _compressedSize; set => _compressedSize = value; }
        public virtual uint UncompressedSize { get => _uncompressedSize; set => _uncompressedSize = value; }
        #endregion

        /// <summary>
		/// Cleans a name making it conform to BSA file conventions.
		/// Devices names ('c:\') and UNC share names ('\\server\share') are removed
		/// and forward slashes ('/') are converted to back slashes ('\').
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
            return Name;
        }

        #region IComparable
        public virtual int CompareTo(Entry other)
        {
            if (other == null)
            {
                return 1;
            }

            return Name.ToLower().CompareTo(other.Name.ToLower());
        }

        public static bool operator >(Entry lhs, Entry rhs)
        {
            if (lhs == null)
            {
                throw new ArgumentNullException(nameof(lhs));
            }

            if (rhs == null)
            {
                throw new ArgumentNullException(nameof(rhs));
            }

            return lhs.CompareTo(rhs) == 1;
        }

        public static bool operator <(Entry lhs, Entry rhs)
        {
            if (lhs == null)
            {
                throw new ArgumentNullException(nameof(lhs));
            }

            if (rhs == null)
            {
                throw new ArgumentNullException(nameof(rhs));
            }

            return lhs.CompareTo(rhs) == -1;
        }

        public static bool operator >=(Entry lhs, Entry rhs)
        {
            if (lhs == null)
            {
                throw new ArgumentNullException(nameof(lhs));
            }

            if (rhs == null)
            {
                throw new ArgumentNullException(nameof(rhs));
            }

            return lhs.CompareTo(rhs) >= 0;
        }

        public static bool operator <=(Entry lhs, Entry rhs)
        {
            if (lhs == null)
            {
                throw new ArgumentNullException(nameof(lhs));
            }

            if (rhs == null)
            {
                throw new ArgumentNullException(nameof(rhs));
            }

            return lhs.CompareTo(rhs) <= 0;
        }
        #endregion

        #region IEquatable
        public bool Equals(Entry other)
        {
            if (other == null)
            {
                return false;
            }

            if (this == other)
            {
                return true;
            }

            if (
                (string.Equals(Name, other.Name, StringComparison.OrdinalIgnoreCase)) &&
                (Offset == other.Offset) &&
                (CompressedSize == other.CompressedSize) &&
                (UncompressedSize == other.UncompressedSize)
                )
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return 539060726 + EqualityComparer<string>.Default.GetHashCode(Name);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var entryObj = obj as Entry;
            if (entryObj == null)
            {
                return false;
            }
            else
            {
                return Equals(entryObj);
            }
        }

        public static bool operator == (Entry lhs, Entry rhs)
        {
            if (lhs == null)
            {
                throw new ArgumentNullException(nameof(lhs));
            }

            if (rhs == null)
            {
                throw new ArgumentNullException(nameof(rhs));
            }

            return lhs.Equals(rhs);
        }

        public static bool operator != (Entry lhs, Entry rhs)
        {
            if (lhs == null)
            {
                throw new ArgumentNullException(nameof(lhs));
            }

            if (rhs == null)
            {
                throw new ArgumentNullException(nameof(rhs));
            }

            return !(lhs.Equals(rhs));
        }
        #endregion
    }
}
