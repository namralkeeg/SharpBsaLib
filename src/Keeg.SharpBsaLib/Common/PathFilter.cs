using System;
using System.IO;

namespace Keeg.SharpBsaLib.Common
{
    /// <summary>
    /// PathFilter filters directories and files using a form of 
    /// <see cref="System.Text.RegularExpressions.Regex">regular expressions</see> by full path name.
    /// See <see cref="NameFilter">NameFilter</see> for more detail on filtering.
    /// </summary>
    public class PathFilter : INameFilter
    {
        #region Instance Fields
        protected readonly NameFilter _nameFilter;
        #endregion

        #region Constructors
        /// <summary>
        /// Create a new instance of <see cref="PathFilter"/>
        /// </summary>
        /// <param name="filter">The <see cref="NameFilter">filter</see> expression to apply.</param>
        public PathFilter(string filter)
        {
            if (string.IsNullOrEmpty(filter))
            {
                throw new ArgumentNullException(nameof(filter));
            }

            _nameFilter = new NameFilter(filter);
        }
        #endregion

        #region IScanFilter Members
        /// <summary>
        /// Tests a name to see if it matches the filter.
        /// </summary>
        /// <param name="name">The name to be tested.</param>
        /// <returns>True if the name matches, otherwise false.</returns>
        public virtual bool IsMatch(string name)
        {
            bool result = false;

            if (!string.IsNullOrEmpty(name))
            {
                string processed = (name.Length > 0) ? Path.GetFullPath(name) : string.Empty;
                result = _nameFilter.IsMatch(processed);
            }

            return result;
        }
        #endregion
    }
}
