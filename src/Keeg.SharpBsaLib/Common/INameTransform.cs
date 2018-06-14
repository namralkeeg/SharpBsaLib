using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Keeg.SharpBsaLib.Common
{
    /// <summary>
    /// INameTransform defines how file system names are transformed for use with archives, or vice versa.
    /// </summary>
    public interface INameTransform
    {
        /// <summary>
        /// Given a file name determine the transformed value.
        /// </summary>
        /// <param name="name">The name to transform.</param>
        /// <returns>The transformed file name.</returns>
        string TransformDirectory(string name);

        /// <summary>
        /// Given a directory name determine the transformed value.
        /// </summary>
        /// <param name="name">The name to transform.</param>
        /// <returns>The transformed directory name.</returns>
        string TransformFile(string name);

        /// <summary>
        /// Force a name to be valid by replacing invalid characters with a fixed value.
        /// </summary>
        /// <param name="name">The name to force valid</param>
        /// <param name="replacement">The replacement character to use.</param>
        /// <returns>Returns a valid name.</returns>
        string MakeValidName(string name, char replacement);

        /// <summary>
        /// Test a name to see if it is a valid name entry.
        /// </summary>
        /// <param name="name">The name to test.</param>
        /// <param name="relaxed">If true checking is relaxed about windows file names and absolute paths.</param>
        /// <returns>Returns true if the name is a valid name; false otherwise.</returns>
        //bool IsValidName(string name, bool relaxed);

        /// <summary>
        /// Test a name to see if it is a valid name entry.
        /// </summary>
        /// <param name="name">The name to test.</param>
        /// <returns>Returns true if the name is a valid name; false otherwise.</returns>
        bool IsValidName(string name);

        /// <summary>
        /// Get/set the path prefix to be trimmed from paths if present.
        /// </summary>
        /// <remarks>The prefix is trimmed before any conversion from a path is done.</remarks>
        //string TrimPrefix { get; set; }
    }
}
