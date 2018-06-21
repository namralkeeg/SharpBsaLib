using System;
using System.IO;

namespace Keeg.SharpBsaLib.Common
{
    public class PartialInputStream : Stream
    {
        #region Instance Fields
        private Stream _baseStream;
        private readonly long _start;
        private readonly long _end;
        private readonly long _length;
        private long _position;
        #endregion

        #region Constructors
        /// <summary>
        /// Initialise a new instance of the <see cref="PartialInputStream"/> class.
        /// </summary>
        /// <param name="baseStream">The <see cref="Stream"/> containing the underlying stream to use for IO.</param>
        /// <param name="start">The start of the partial data.</param>
        /// <param name="length">The length of the partial data.</param>
        public PartialInputStream(Stream baseStream, long start, long length)
        {
            _baseStream = baseStream ?? throw new ArgumentNullException(nameof(baseStream));
            _start = start;
            _length = length;
            _position = start;
            _end = _start + _length;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets a value indicating whether the current stream supports reading.
        /// </summary>
        /// <value>true.</value>
        /// <returns>true if the stream supports reading; otherwise, false.</returns>
        public override bool CanRead => true;

        /// <summary>
        /// Gets a value indicating whether the current stream supports seeking.
        /// </summary>
        /// <value>true</value>
        /// <returns>true if the stream supports seeking; otherwise, false.</returns>
        public override bool CanSeek => true;

        /// <summary>
        /// Gets a value indicating whether the current stream supports writing.
        /// </summary>
        /// <value>false</value>
        /// <returns>true if the stream supports writing; otherwise, false.</returns>
        public override bool CanWrite => false;

        /// <summary>
        /// Gets the length in bytes of the stream.
        /// </summary>
        /// <returns>A long value representing the length of the stream in bytes.</returns>
        /// <exception cref="NotSupportedException">A class derived from Stream does not support seeking. </exception>
        /// <exception cref="ObjectDisposedException">Methods were called after the stream was closed. </exception>
        public override long Length => _length;

        /// <summary>
        /// Gets / sets the position within the current stream. Relative to the base stream.
        /// </summary>
        /// <returns>The current position within the stream.</returns>
        public override long Position
        {
            get => _position - _start;
            set
            {
                long newPosition = _start + value;
                if (newPosition < _start)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Position cannot be negative.");
                }

                if (newPosition > _end)
                {
                    throw new ArgumentOutOfRangeException(nameof(value) ,"Cannot seek past the end.");
                }

                _position = newPosition;
                _baseStream.Seek(_position, SeekOrigin.Begin);
            }
        }

        public override bool CanTimeout => _baseStream.CanTimeout;
        #endregion

        /// <summary>
        /// Clears all buffers for this stream and causes any buffered data to be written to the underlying device.
        /// </summary>
        /// <exception cref="IOException">An I/O error occurs. </exception>
        public override void Flush()
        {
            // Do nothing. No writing allowed.
        }

        /// <summary>
        /// Reads a sequence of bytes from the current stream and advances the position within the stream by the number of bytes read.
        /// </summary>
        /// <param name="buffer">An array of bytes. When this method returns, the buffer contains the specified byte array with the values between offset and (offset + count - 1) replaced by the bytes read from the current source.</param>
        /// <param name="offset">The zero-based byte offset in buffer at which to begin storing the data read from the current stream.</param>
        /// <param name="count">The maximum number of bytes to be read from the current stream.</param>
        /// <returns>
        /// The total number of bytes read into the buffer. This can be less than the number of bytes requested if that many bytes are not currently available, or zero (0) if the end of the stream has been reached.
        /// </returns>
        /// <exception cref="ArgumentException">The sum of offset and count is larger than the buffer length.</exception>
        /// <exception cref="ArgumentNullException">Buffer is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Offset or count is negative.</exception>
        /// <exception cref="IOException">An I/O error occurs.</exception>
        /// <exception cref="NotSupportedException">The stream does not support reading. </exception>
        /// <exception cref="ObjectDisposedException">Methods were called after the stream was closed.</exception>
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer), "Buffer is null.");
            }

            if (offset + count > buffer.Length)
            {
                throw new ArgumentException("The sum of offset and count is larger than the buffer length.");
            }

            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), $"{nameof(offset)} is negative.");
            }

            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), $"{nameof(count)} is negative.");
            }

            // Just return if count is 0 or the stream is at the end.
            if (count == 0 || ((_end - _position) == 0))
            {
                return 0;
            }

            lock (_baseStream)
            {
                if (count > _end - _position)
                {
                    count = (int)(_end - _position);
                }

                // Protect against Stream implementations that throw away their buffer on every Seek
                if (_baseStream.Position != _position)
                {
                    _baseStream.Seek(_position, SeekOrigin.Begin);
                }

                int readCount = _baseStream.Read(buffer, offset, count);
                if (readCount > 0)
                {
                    _position += readCount;
                }

                return readCount;
            }
        }

        /// <summary>
        /// Read a byte from this stream.
        /// </summary>
        /// <returns>Returns the byte read or -1 on end of stream.</returns>
        /// <exception cref="ObjectDisposedException">Methods were called after the stream was closed.</exception>
        public override int ReadByte()
        {
            if (_position >= _end)
            {
                return -1;
            }

            lock (_baseStream)
            {
                _baseStream.Seek(_position++, SeekOrigin.Begin);
                return _baseStream.ReadByte();
            }
        }

        /// <summary>
        /// When overridden in a derived class, sets the position within the current stream.
        /// </summary>
        /// <param name="offset">A byte offset relative to the origin parameter.</param>
        /// <param name="origin">A value of type <see cref="SeekOrigin"></see> indicating the reference point used to obtain the new position.</param>
        /// <returns>The new position within the current stream.</returns>
        /// <exception cref="IOException">An I/O error occurs.</exception>
        /// <exception cref="NotSupportedException">The stream does not support seeking, such as if the stream is constructed from a pipe or console output.</exception>
        /// <exception cref="ObjectDisposedException">Methods were called after the stream was closed.</exception>
        public override long Seek(long offset, SeekOrigin origin)
        {
            long newPosition = _position;

            switch (origin)
            {
                case SeekOrigin.Begin:
                    newPosition = _start + offset;
                    break;
                case SeekOrigin.Current:
                    newPosition = _position + offset;
                    break;
                case SeekOrigin.End:
                    newPosition = _end + offset;
                    break;
                default:
                    break;
            }

            if (newPosition < _start)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), "Position cannot be negative.");
            }

            if (newPosition > _end)
            {
                throw new IOException("Cannot seek past the end.");
            }

            _position = newPosition;
            _baseStream.Seek(_position, SeekOrigin.Begin);
            return _position;
        }

        /// <summary>
        /// When overridden in a derived class, sets the length of the current stream.
        /// </summary>
        /// <param name="value">The desired length of the current stream in bytes.</param>
        /// <exception cref="NotSupportedException">The stream does not support both writing and seeking, such as if the stream is constructed from a pipe or console output. </exception>
        /// <exception cref="IOException">An I/O error occurs. </exception>
        /// <exception cref="ObjectDisposedException">Methods were called after the stream was closed. </exception>
        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Writes a sequence of bytes to the current stream and advances the current position within this stream by the number of bytes written.
        /// </summary>
        /// <param name="buffer">An array of bytes. This method copies count bytes from buffer to the current stream.</param>
        /// <param name="offset">The zero-based byte offset in buffer at which to begin copying bytes to the current stream.</param>
        /// <param name="count">The number of bytes to be written to the current stream.</param>
        /// <exception cref="IOException">An I/O error occurs. </exception>
        /// <exception cref="NotSupportedException">The stream does not support writing. </exception>
        /// <exception cref="ObjectDisposedException">Methods were called after the stream was closed. </exception>
        /// <exception cref="ArgumentNullException">buffer is null. </exception>
        /// <exception cref="ArgumentException">The sum of offset and count is greater than the buffer length. </exception>
        /// <exception cref="ArgumentOutOfRangeException">offset or count is negative. </exception>
        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }
    }
}
