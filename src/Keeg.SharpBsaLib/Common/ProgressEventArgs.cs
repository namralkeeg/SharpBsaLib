using System;

namespace Keeg.SharpBsaLib.Common
{
    /// <summary>
    /// Event arguments during processing of a single file or directory.
    /// </summary>
    public class ProgressEventArgs : EventArgs
    {
        #region Constructors
        /// <summary>
        /// Initialise a new instance of <see cref="ProgressEventArgs"/>
        /// </summary>
        /// <param name="name">The file or directory name if known.</param>
        /// <param name="processed">The number of bytes processed so far</param>
        /// <param name="target">The total number of bytes to process, 0 if not known</param>
        public ProgressEventArgs(string name, long processed, long target)
        {
            Name = name;
            Processed = processed;
            Target = target;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The name for this event if known.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Get / set a value indicating if scanning should continue or not.
        /// </summary>
        public bool ContinuteRunning { get; set; } = true;

        /// <summary>
        /// Get a percentage representing how much of the <see cref="Target"></see> has been processed
        /// </summary>
        /// <value>0.0 to 100.0 percent; 0 if target is not known.</value>
        public float PercentComplete
        {
            get
            {
                float result;
                if (Target <= 0)
                {
                    result = 0;
                }
                else
                {
                    result = ((float)Processed / (float)Target) * 100.0f;
                }
                return result;
            }
        }

        /// <summary>
        /// The number of bytes processed.
        /// </summary>
        public long Processed { get; protected set; }

        /// <summary>
        /// The number of bytes to process.
        /// </summary>
        /// <remarks>Target may be 0 or negative if the value isnt known.</remarks>
        public long Target { get; protected set; }
        #endregion
    }
}
