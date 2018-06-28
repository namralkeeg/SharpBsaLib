namespace Keeg.SharpBsaLib.Common
{
    /// <summary>
    /// 
    /// </summary>
    internal static class ByteSwap
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static ushort Swap(this ushort value)
        {
            return (ushort)(((value >> 8) & 0x00FF) | ((value << 8) & 0xFF00));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static short Swap(this short value)
        {
            return (short)Swap((ushort)value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static uint Swap(this uint value)
        {
            uint x = ((value >> 16) & 0x0000FFFF) | ((value << 16) & 0xFFFF0000);
            return ((x & 0xFF00FF00) >> 8 | (x & 0x00FF00FF) << 8);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static int Swap(this int value)
        {
            return (int)Swap((uint)value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static ulong Swap(this ulong value)
        {
            // swap adjacent 32-bit blocks
            ulong x = (value >> 32) | (value << 32);
            // swap adjacent 16-bit blocks
            x = ((x & 0xFFFF0000FFFF0000) >> 16) | ((x & 0x0000FFFF0000FFFF) << 16);
            // swap adjacent 8-bit blocks
            return ((x & 0xFF00FF00FF00FF00) >> 8) | ((x & 0x00FF00FF00FF00FF) << 8);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static long Swap(this long value)
        {
            return (long)Swap((ulong)value);
        }
    }
}
