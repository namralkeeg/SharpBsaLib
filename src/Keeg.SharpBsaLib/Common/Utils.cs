using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public static string DropPathRoot(string path)
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
    }
}
