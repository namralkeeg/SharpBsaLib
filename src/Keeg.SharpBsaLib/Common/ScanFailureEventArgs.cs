using System;

namespace Keeg.SharpBsaLib.Common
{
    /// <summary>
    /// Arguments passed when scan failures are detected.
    /// </summary>
    public class ScanFailureEventArgs : EventArgs
    {
        /// <summary>
        /// Initialise a new instance of <see cref="ScanFailureEventArgs"></see>
        /// </summary>
		/// <param name="name">The name to apply.</param>
        /// <param name="exception">The exception to use.</param>
        public ScanFailureEventArgs(string name, Exception exception)
        {
            Name = name;
            Exception = exception;
        }

        /// <summary>
        /// The applicable name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The applicable exception.
        /// </summary>
        public Exception Exception { get; }

        /// <summary>
        /// Get / set a value indicating wether scanning should continue.
        /// </summary>
        public bool ContinueRunning { get; set; } = true;
    }
}
