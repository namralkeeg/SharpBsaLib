using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Keeg.IO;
using Keeg.IO.NumberHelpers;

namespace Keeg.SharpBsaLib
{
    public abstract class Archive : IArchive
    {
        #region Fields
        protected Endian byteOrder;
        protected Encoding encoding;
        protected Stream stream;
        protected BinaryReaderEndian binaryReader;
        #endregion

        #region Properties
        public abstract IReadOnlyCollection<IEntry> Entries { get; }
        public string FullName { get; protected set; }
        public string FullPath => Path.GetFullPath(FullName);
        public string Name => Path.GetFileName(FullName);
        #endregion

        #region Constructors
        protected Archive(string fileName) : this (fileName, Encoding.GetEncoding(1252), Endian.Little) { }
        protected Archive(string fileName, Encoding encoding) : this (fileName, encoding, Endian.Little) { }
        protected Archive(string fileName, Encoding encoding, Endian byteOrder)
        {
            FullName = fileName ?? throw new ArgumentNullException(nameof(fileName));

            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException(fileName);
            }

            this.encoding = encoding ?? throw new ArgumentNullException(nameof(encoding));
            this.byteOrder = byteOrder;
            stream = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            binaryReader = new BinaryReaderEndian(stream, this.encoding, false, Endian.Little);
        }
        #endregion

        #region Methods
        protected string NormalizeEntryPath(string fileName)
        {
            if (fileName == null)
                throw new ArgumentNullException(nameof(fileName));
            Contract.EndContractBlock();

            var sb = new StringBuilder(fileName.ToLowerInvariant());
            sb.Replace('/', '\\');
            if (sb[0] == '\\')
            {
                sb.Remove(0, 1);
            }

            return sb.ToString();
        }

        public abstract bool Open();
        public abstract bool Close();

        public bool IsArchive(string filePath)
        {
            if (filePath == null)
                throw new ArgumentNullException(nameof(filePath));
            Contract.EndContractBlock();

            if (!File.Exists(filePath))
                return false;

            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return IsArchive(fs);
            }
        }
        public abstract bool IsArchive(Stream stream);

        public abstract bool Extract(string filePath, out byte[] data);
        public abstract bool Extract(IEntry entry, out byte[] data);
        public abstract bool Extract(string filePath, string destinationFolder, bool overwrite);
        public abstract bool Extract(IEntry entry, string destinationFolder, bool overwrite);
        public abstract bool Extract(IEnumerable<string> filePaths, string destinationFolder, bool overwrite);
        public abstract bool Extract(IEnumerable<IEntry> entries, string destinationFolder, bool overwrite);
        public abstract bool ExtractToFolder(string destinationFolder, bool overwrite);

        public abstract IEnumerable<IEntry> GetEntries(IEnumerable<string> filePaths);
        public abstract IEntry GetEntry(string filePath);
        public abstract IEnumerable<IEntry> GetMatchingEntries(Regex regex);
        public abstract bool HasEntry(string filePath);

        public byte[] GetEntryChecksum(string filePath, HashAlgorithm hasher)
        {
            if (filePath == null)
                throw new ArgumentNullException(nameof(filePath));
            if (hasher == null)
                throw new ArgumentNullException(nameof(hasher));
            Contract.EndContractBlock();

            byte[] checksum = null;
            var entry = GetEntry(filePath);
            if (entry != null)
            {
                if (Extract(entry, out byte[] data))
                {
                    checksum = hasher.ComputeHash(data, 0, data.Length);
                }
            }

            return checksum;
        }

        public byte[] GetEntryChecksum(IEntry entry, HashAlgorithm hasher)
        {
            return GetEntryChecksum(entry.FullName, hasher);
        }
        #endregion
    }
}
