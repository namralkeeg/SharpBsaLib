using System;
using System.IO;

namespace Keeg.SharpBsaLib.Common
{
    /// <summary>
    /// ExtendedPathFilter filters based on name, file size, and the last write time of the file.
    /// </summary>
    /// <remarks>Provides an example of how to customise filtering.</remarks>
    public class ExtendedPathFilter : PathFilter
    {
        #region Instance Fields
        private long _minSize = 0;
        private long _maxSize = long.MaxValue;
        private DateTime _minDate = DateTime.MinValue;
        private DateTime _maxDate = DateTime.MaxValue;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instance of <see cref="ExtendedPathFilter"/>
        /// </summary>
        /// <param name="filter">The filter to apply.</param>
        /// <param name="minSize">The minimum file size to include.</param>
        /// <param name="maxSize">The maximum file size to include.</param>
        public ExtendedPathFilter(string filter, long minSize, long maxSize) : base(filter)
        {
            MinSize = minSize;
            MaxSize = maxSize;
        }

        /// <summary>
        /// Creates a new instance of <see cref="ExtendedPathFilter"/>
        /// </summary>
        /// <param name="filter">The filter to apply.</param>
        /// <param name="minDate">The minimum <see cref="DateTime"/> to include.</param>
        /// <param name="maxDate">The maximum <see cref="DateTime"/> to include.</param>
        public ExtendedPathFilter(string filter, DateTime minDate, DateTime maxDate) : base(filter)
        {
            MinDate = minDate;
            MaxDate = maxDate;
        }

        /// <summary>
        /// Creates a new instance of <see cref="ExtendedPathFilter"/>
        /// </summary>
        /// <param name="filter">The filter to apply.</param>
        /// <param name="minSize">The minimum file size to include.</param>
        /// <param name="maxSize">The maximum file size to include.</param>
        /// <param name="minDate">The minimum <see cref="DateTime"/> to include.</param>
        /// <param name="maxDate">The maximum <see cref="DateTime"/> to include.</param>
        public ExtendedPathFilter(string filter, long minSize, long maxSize, DateTime minDate, DateTime maxDate) 
            : base(filter)
        {
            MinSize = minSize;
            MaxSize = maxSize;
            MinDate = minDate;
            MaxDate = maxDate;
        }
        #endregion

        #region IScanFilter 
        /// <summary>
        /// Test a filename to see if it matches the filter.
        /// </summary>
        /// <param name="name">The filename to test.</param>
        /// <returns>True if the filter matches, otherwise false.</returns>
        public override bool IsMatch(string name)
        {
            bool result = base.IsMatch(name);

            if (result)
            {
                var fileInfo = new FileInfo(name);
                try
                {
                    result =
                        (MinSize <= fileInfo.Length) &&
                        (MaxSize >= fileInfo.Length) &&
                        (MinDate <= fileInfo.LastWriteTime) &&
                        (MaxDate >= fileInfo.LastWriteTime);
                }
                catch (FileNotFoundException)
                {
                    result = false;
                }
            }

            return result;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Get/set the minimum size/length for a file that will match this filter.
        /// </summary>
        /// <remarks>The default value is zero.</remarks>
        /// <exception cref="ArgumentOutOfRangeException">Value is less than zero; greater than <see cref="MaxSize"/></exception> 
        public long MinSize
        {
            get => _minSize;
            set
            {
                if ((value < 0) || (MaxSize < value))
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                _minSize = value;
            }
        }

        /// <summary>
        /// Get/set the maximum size/length for a file that will match this filter.
        /// </summary>
        /// <remarks>The default value is <see cref="long.MaxValue"/></remarks>
        /// <exception cref="ArgumentOutOfRangeException">Value is less than zero; less than <see cref="MinSize"/></exception>
        public long MaxSize
        {
            get => _maxSize;
            set
            {
                if ((value < 0) || (MinSize > value))
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                _maxSize = value;
            }
        }

        /// <summary>
        /// Get/set the minimum <see cref="DateTime"/> value that will match for this filter.
        /// </summary>
        /// <remarks>Files with a LastWrite time less than this value are excluded by the filter.</remarks>
        public DateTime MinDate
        {
            get => _minDate;
            set
            {
                if (value > MaxDate)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Exceeds MaxDate");
                }

                _minDate = value;
            }
        }

        /// <summary>
        /// Get/set the maximum <see cref="DateTime"/> value that will match for this filter.
        /// </summary>
        /// <remarks>Files with a LastWrite time greater than this value are excluded by the filter.</remarks>
        public DateTime MaxDate
        {
            get => _maxDate;
            set
            {
                if (MinDate > value)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Smaller than MinDate");
                }

                _maxDate = value;
            }
        }
        #endregion
    }
}
