using System;
using System.IO;

namespace Keeg.SharpBsaLib.Common
{
    internal static class Utils
    {
        static Utils()
        {

        }

        /// <summary>
        /// Remove any root element present in the path.
        /// </summary>
        /// <param name="path"><see cref="string"/> containing the path information.</param>
        /// <returns>The path with the root removed if it exists, otherwise path.</returns>
        /// <remarks>Path isn't checked for validity like with the <see cref="System.IO.Path"/> class</remarks>
        internal static string DropPathRoot(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return string.Empty;
            }

            string result = path;
            if ((path[0] == '\\') || (path[0] == '/'))
            {
                if ((path.Length > 1) && ((path[1] == '\\') || (path[1] == '/')))
                {
                    int index = 2;
                    int elements = 2;

                    // Scan for two separate elements \\machine\share\restofpath
                    while ((index <= path.Length) &&
                        (((path[index] != '\\') && (path[index] != '/')) || (--elements > 0)))
                    {
                        index++;
                    }

                    index++;

                    if (index < path.Length)
                    {
                        result = path.Substring(index);
                    }
                    else
                    {
                        result = "";
                    }
                }
            }
            else if ((path.Length > 1) && (path[1] == ':'))
            {
                int dropCount = 2;
                if ((path.Length > 2) && ((path[2] == '\\') || (path[2] == '/')))
                {
                    dropCount = 3;
                }

                result = result.Remove(0, dropCount);
            }

            return result;
        }

        /// <summary>
        /// Copy the contents of one <see cref="Stream"/> to another.
        /// </summary>
        /// <param name="source">The stream to source data from.</param>
        /// <param name="destination">The stream to write data to.</param>
        /// <param name="buffer">The buffer to use during copying.</param>
        internal static void CopyStream(Stream source, Stream destination, byte[] buffer)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (destination == null)
            {
                throw new ArgumentNullException(nameof(destination));
            }

            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            // Ensure a reasonable size of buffer is used without being prohibitive.
            if (buffer.Length < 128)
            {
                throw new ArgumentException("Buffer is too small", nameof(buffer));
            }

            int read;
            while ((read = source.Read(buffer, 0, buffer.Length)) > 0)
            {
                destination.Write(buffer, 0, read);
            }

            destination.Flush();
        }

        /// <summary>
        /// Copy the contents of one <see cref="Stream"/> to another.
        /// </summary>
		/// <param name="source">The stream to source data from.</param>
		/// <param name="destination">The stream to write data to.</param>
		/// <param name="buffer">The buffer to use during copying.</param>
		/// <param name="progressHandler">The <see cref="EventHandler"> and <see cref="ProgressEventArgs"/> progress handler event</see> to use.</param>
        /// <param name="interval">The minimum <see cref="TimeSpan"/> between progress updates.</param>
        /// <param name="sender">The source for this event.</param>
        /// <param name="name">The name to use with the event.</param>
		/// <remarks>This form is specialised for use within #Bsa to support events during archive operations.</remarks>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="IOException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        internal static void CopyStream(Stream source, Stream destination,
            byte[] buffer, EventHandler<ProgressEventArgs> progressHandler, TimeSpan interval, object sender, string name)
        {
            CopyStream(source, destination, buffer, progressHandler, interval, sender, name, -1);
        }

        /// <summary>
        /// Copy the contents of one <see cref="Stream"/> to another.
        /// </summary>
		/// <param name="source">The stream to source data from.</param>
		/// <param name="destination">The stream to write data to.</param>
		/// <param name="buffer">The buffer to use during copying.</param>
		/// <param name="progressHandler">The <see cref="EventHandler"> and <see cref="ProgressEventArgs"/> progress handler event</see> to use.</param>
        /// <param name="interval">The minimum <see cref="TimeSpan"/> between progress updates.</param>
        /// <param name="sender">The source for this event.</param>
        /// <param name="name">The name to use with the event.</param>
		/// <param name="fixedTarget">A predetermined fixed target value to use with progress updates.
		/// If the value is negative the target is calculated by looking at the stream.</param>
		/// <remarks>This form is specialised for use within #Bsa to support events during archive operations.</remarks>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="IOException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        internal static void CopyStream(Stream source, Stream destination, byte[] buffer, 
            EventHandler<ProgressEventArgs> progressHandler, TimeSpan interval, 
            object sender, string name, long fixedTarget)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (destination == null)
            {
                throw new ArgumentNullException(nameof(destination));
            }

            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            if (progressHandler == null)
            {
                throw new ArgumentNullException(nameof(progressHandler));
            }

            if (buffer.Length < 128)
            {
                throw new ArgumentException("Buffer is too small", nameof(buffer));
            }

            bool copying = true;

            DateTime marker = DateTime.Now;
            long processed = 0;
            long target = 0;

            if (fixedTarget >= 0)
            {
                target = fixedTarget;
            }
            else if (source.CanSeek)
            {
                target = source.Length - source.Position;
            }

            // Always fire 0% progress..
            var args = new ProgressEventArgs(name, processed, target);
            progressHandler(sender, args);

            bool progressFired = true;

            while (copying)
            {
                int bytesRead = source.Read(buffer, 0, buffer.Length);
                if (bytesRead > 0)
                {
                    processed += bytesRead;
                    progressFired = false;
                    destination.Write(buffer, 0, bytesRead);
                }
                else
                {
                    destination.Flush();
                    copying = false;
                }

                if (DateTime.Now - marker > interval)
                {
                    progressFired = true;
                    marker = DateTime.Now;
                    args = new ProgressEventArgs(name, processed, target);
                    progressHandler(sender, args);

                    copying = args.ContinueRunning;
                }
            }

            if (!progressFired)
            {
                args = new ProgressEventArgs(name, processed, target);
                progressHandler(sender, args);
            }
        }
    }
}
