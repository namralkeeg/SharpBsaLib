namespace Keeg.SharpBsaLib.Common
{
    /// <summary>
    /// Event arguments for directories.
    /// </summary>
    public class DirectoryEventArgs : ScanEventArgs
    {
        /// <summary>
        /// Initialize an instance of <see cref="DirectoryEventArgs"></see>.
        /// </summary>
        /// <param name="name">The name for this directory.</param>
        /// <param name="hasMatchingFiles">Flag value indicating if any matching files are contained in this directory.</param>
        public DirectoryEventArgs(string name, bool hasMatchingFiles) : base(name)
        {
            HasMatchingFiles = hasMatchingFiles;
        }

        /// <summary>
        /// Get a value indicating if the directory contains any matching files or not.
        /// </summary>
        public bool HasMatchingFiles { get; protected set; }
    }
}
