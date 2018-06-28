using System;
using System.IO;
using System.Text;

namespace Keeg.SharpBsaLib.Common
{
    /// <summary>
    /// 
    /// </summary>
    internal static partial class BinaryReaderHelpers
    {
        /// <summary>
        /// 
        /// </summary>
        private enum Prefix : byte
        {
            PByte,
            PUInt16,
            PUInt32,
            PUInt64
        }

        /// <summary>
        /// BSA's use Windows-1252 encoding by default.
        /// </summary>
        private static readonly Encoding defaultEncoding = Encoding.GetEncoding(1252);

        #region Helper Extension Functions
        /// <summary>
        /// 
        /// </summary>
        /// <param name="binaryReader"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        internal static short ReadInt16BE(this BinaryReader binaryReader)
        {
            var value = binaryReader?.ReadInt16() ?? throw new ArgumentNullException(nameof(binaryReader));
            if (BitConverter.IsLittleEndian)
            {
                value = value.Swap();
            }

            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="binaryReader"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        internal static int ReadInt32BE(this BinaryReader binaryReader)
        {
            var value = binaryReader?.ReadInt32() ?? throw new ArgumentNullException(nameof(binaryReader));
            if (BitConverter.IsLittleEndian)
            {
                value = value.Swap();
            }

            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="binaryReader"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        internal static long ReadInt64BE(this BinaryReader binaryReader)
        {
            var value = binaryReader?.ReadInt64() ?? throw new ArgumentNullException(nameof(binaryReader));
            if (BitConverter.IsLittleEndian)
            {
                value = value.Swap();
            }

            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="binaryReader"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        internal static ushort ReadUInt16BE(this BinaryReader binaryReader)
        {
            var value = binaryReader?.ReadUInt16() ?? throw new ArgumentNullException(nameof(binaryReader));
            if (BitConverter.IsLittleEndian)
            {
                value = value.Swap();
            }

            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="binaryReader"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        internal static uint ReadUInt32BE(this BinaryReader binaryReader)
        {
            var value = binaryReader?.ReadUInt32() ?? throw new ArgumentNullException(nameof(binaryReader));
            if (BitConverter.IsLittleEndian)
            {
                value = value.Swap();
            }

            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="binaryReader"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        internal static ulong ReadUInt64BE(this BinaryReader binaryReader)
        {
            var value = binaryReader?.ReadUInt64() ?? throw new ArgumentNullException(nameof(binaryReader));
            if (BitConverter.IsLittleEndian)
            {
                value = value.Swap();
            }

            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="binaryReader"></param>
        /// <returns></returns>
        internal static string ReadBString(this BinaryReader binaryReader)
        {
            return ReadPrefixString(binaryReader, Prefix.PByte, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="binaryReader"></param>
        /// <returns></returns>
        internal static string ReadBZString(this BinaryReader binaryReader)
        {
            return ReadPrefixString(binaryReader, Prefix.PByte, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="binaryReader"></param>
        /// <returns></returns>
        internal static string ReadWString(this BinaryReader binaryReader)
        {
            return ReadPrefixString(binaryReader, Prefix.PUInt16, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="binaryReader"></param>
        /// <returns></returns>
        internal static string ReadWZString(this BinaryReader binaryReader)
        {
            return ReadPrefixString(binaryReader, Prefix.PUInt16, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="binaryReader"></param>
        /// <returns></returns>
        internal static string ReadZString(this BinaryReader binaryReader)
        {
            return ReadTerminatedString(binaryReader, '\0');
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        private static void StripTrailingNulls(ref byte[] value)
        {
            var i = value?.Length - 1 ?? throw new ArgumentNullException(nameof(value));
            while ((value[i] == 0) && (i >= 0))
            {
                --i;
            }

            Array.Resize(ref value, i + 1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="binaryReader"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        private static string ReadPrefixString(BinaryReader binaryReader, Prefix prefix)
        {
            return ReadPrefixString(binaryReader, prefix, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="binaryReader"></param>
        /// <param name="prefix"></param>
        /// <param name="isNullTerminated"></param>
        /// <returns></returns>
        private static string ReadPrefixString(BinaryReader binaryReader, Prefix prefix, bool isNullTerminated)
        {
            ulong byteCount = 0;
            switch (prefix)
            {
                case Prefix.PByte: byteCount = binaryReader.ReadByte(); break;
                case Prefix.PUInt16: byteCount = binaryReader.ReadUInt16(); break;
                case Prefix.PUInt32: byteCount = binaryReader.ReadUInt32(); break;
                case Prefix.PUInt64: byteCount = binaryReader.ReadUInt64(); break;
                default: byteCount = 0; break;
            }

            var data = binaryReader.ReadBytes((int)byteCount);
            if (isNullTerminated)
            {
                StripTrailingNulls(ref data);
            }

            var preString = defaultEncoding.GetString(data);

            return preString;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="binaryReader"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        private static string ReadTerminatedString(BinaryReader binaryReader, char end)
        {
            StringBuilder s = new StringBuilder();
            char c = (char)binaryReader.BaseStream.ReadByte();

            while (c != end)
            {
                s.Append(c);
                c = (char)binaryReader.BaseStream.ReadByte();
            }

            return s.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="binaryReader"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        private static string ReadFixedString(BinaryReader binaryReader, int length)
        {
            if (binaryReader == null)
            {
                throw new ArgumentNullException(nameof(binaryReader));
            }

            if (length < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(length));
            }

            var data = binaryReader.ReadBytes(length);
            StripTrailingNulls(ref data);
            var value = defaultEncoding.GetString(data);

            return value;
        }
        #endregion
    }
}
