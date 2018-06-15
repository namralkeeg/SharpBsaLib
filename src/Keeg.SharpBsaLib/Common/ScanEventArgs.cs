using System;

namespace Keeg.SharpBsaLib.Common
{
    /// <summary>
    /// Event arguments for scanning.
    /// </summary>
    public class ScanEventArgs : EventArgs
    {
        /// <summary>
        /// Initialize a new instance of <see cref="ScanEventArgs"/>
        /// </summary>
        /// <param name="name">The file or directory name.</param>
        public ScanEventArgs(string name)
        {
            Name = name;
        }

        /// <summary>
        /// The file or directory name for this event.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Get / set a value indicating if scanning should continue or not.
        /// </summary>
        public bool ContinueRunning { get; set; } = true;
    }
}
