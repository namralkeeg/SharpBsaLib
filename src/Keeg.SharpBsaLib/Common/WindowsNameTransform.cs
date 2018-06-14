using System;
using System.IO;
using System.Text;

namespace Keeg.SharpBsaLib.Common
{
    public class WindowsNameTransform : INameTransform
    {
        #region Class Fields
        /// <summary>
        /// In this case we need Windows' invalid path characters.
        /// Path.GetInvalidPathChars() only returns a subset of invalid on all platforms.
        /// </summary>
        private static readonly char[] invalidEntryChars;
        #endregion

        #region Instance Fields
        private const int MaxPath = 260;
        private const char DefaultReplacementChar = '_';
        private string _baseDirectory;
        private bool _trimIncomingPaths = false;
        private char _replacementChar = DefaultReplacementChar;
        #endregion

        #region Constructors
        static WindowsNameTransform()
        {
            var invalidChars = Path.GetInvalidPathChars();
            int invalidCharsLength = invalidChars.Length + 3;
            invalidEntryChars = new char[invalidCharsLength];
            Array.Copy(invalidChars, invalidEntryChars, invalidChars.Length);
            // extra characters for masks, etc.
            invalidEntryChars[invalidCharsLength - 1] = '*';
            invalidEntryChars[invalidCharsLength - 2] = '?';
            invalidEntryChars[invalidCharsLength - 3] = ':';
        }

        public WindowsNameTransform()
        {
            // Do nothing.
        }

        public WindowsNameTransform(string baseDirectory, bool trimIncomingPaths, char replacementChar)
        {
            BaseDirectory = baseDirectory ?? throw new ArgumentNullException(nameof(baseDirectory));
            TrimIncomingPaths = trimIncomingPaths;
            ReplacementChar = replacementChar;
        }

        public WindowsNameTransform(string baseDirectory) : this (baseDirectory, false, DefaultReplacementChar)
        {
            // Do nothing.
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets / sets a value containing the target directory to prefix values with.
        /// </summary>
        public string BaseDirectory
        {
            get => _baseDirectory;
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException(nameof(value));
                }

                _baseDirectory = Path.GetFullPath(value);
            }
        }

        /// <summary>
        /// Gets / sets a value indicating if paths on incoming values should be removed.
        /// </summary>
        public bool TrimIncomingPaths { get => _trimIncomingPaths; set => _trimIncomingPaths = value; }

        /// <summary>
        /// Gets / sets the character to replace invalid characters during transformations.
        /// </summary>
        public char ReplacementChar
        {
            get => _replacementChar;
            set
            {
                for (int i = 0; i < invalidEntryChars.Length; i++)
                {
                    if (invalidEntryChars[i] == value)
                    {
                        throw new ArgumentException("Invalid replacement character.");
                    }
                }

                if ((Path.DirectorySeparatorChar == value) || (Path.AltDirectorySeparatorChar == value))
                {
                    throw new ArgumentException("Invalid replacement character.");
                }

                _replacementChar = value;
            }
        }
        #endregion

        /// <summary>
        /// Test a name to see if it is a valid name for a windows filename.
        /// </summary>
		/// <param name="name">The name to test.</param>
		/// <returns>Returns true if the name is a valid windows file name; false otherwise.</returns>
		/// <remarks>
        /// The filename isnt a true windows path in some fundamental ways like no absolute paths, 
        /// no rooted paths etc.
        /// </remarks>
        public virtual bool IsValidName(string name)
        {
            bool result =
                (!string.IsNullOrEmpty(name)) &&
                (name.Length <= MaxPath) &&
                (string.Compare(name, MakeValidName(name, ReplacementChar), StringComparison.Ordinal) == 0);

            return result;
        }

        /// <summary>
        /// Force a name to be valid by replacing invalid characters with a fixed value
        /// </summary>
        /// <param name="name">The name to make valid</param>
        /// <param name="replacement">The replacement character to use for any invalid characters.</param>
        /// <returns>Returns a valid name</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="PathTooLongException"></exception>
        public virtual string MakeValidName(string name, char replacement)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            var validName = Utils.DropPathRoot(name.Replace('/', Path.DirectorySeparatorChar));

            // Drop any leading slashes.
            while ((validName.Length > 0) && (validName[0] == Path.DirectorySeparatorChar))
            {
                validName = validName.Remove(0, 1);
            }

            // Drop any trailing slashes.
            while ((validName.Length > 0) && (validName[validName.Length - 1] == Path.DirectorySeparatorChar))
            {
                validName = validName.Remove(validName.Length - 1, 1);
            }

            // Convert consecutive \\ characters to \
            int index = validName.IndexOf(string.Format("{0}{0}", Path.DirectorySeparatorChar), StringComparison.Ordinal);
            while (index >= 0)
            {
                validName = validName.Remove(index, 1);
                index = validName.IndexOf(string.Format("{0}{0}", Path.DirectorySeparatorChar), StringComparison.Ordinal);
            }

            // Convert any invalid characters using the replacement one.
            index = validName.IndexOfAny(invalidEntryChars);
            if (index >= 0)
            {
                var builder = new StringBuilder(validName);

                while (index >= 0)
                {
                    builder[index] = replacement;

                    if (index >= validName.Length)
                    {
                        index = -1;
                    }
                    else
                    {
                        index = validName.IndexOfAny(invalidEntryChars, index + 1);
                    }
                }
                validName = builder.ToString();
            }

            // Check for names greater than MaxPath characters.
            if (validName.Length > MaxPath)
            {
                throw new PathTooLongException();
            }

            return validName;
        }

        /// <summary>
        /// Transform a directory name to a windows directory name.
        /// </summary>
		/// <param name="name">The directory name to transform.</param>
		/// <returns>The transformed name.</returns>
        /// <exception cref="ArgumentNullException">Name cannot be <see cref="null"/> or empty.</exception>
        public virtual string TransformDirectory(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            name = TransformFile(name);
            if (name.Length > 0)
            {
                while (name.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal))
                {
                    name = name.Remove(name.Length - 1, 1);
                }
            }
            else
            {
                throw new Exception("Cannot have an empty directory name");
            }

            return name;
        }

        /// <summary>
        /// Transform a file name to a windows style one.
        /// </summary>
        /// <param name="name">The file name to transform.</param>
        /// <returns>The transformed name.</returns>
        /// <exception cref="PathTooLongException"><see cref="Path.Combine(string, string)"/></exception>
        public virtual string TransformFile(string name)
        {
            if (name != null)
            {
                name = MakeValidName(name, ReplacementChar);

                if (TrimIncomingPaths)
                {
                    name = Path.GetFileName(name);
                }

                // This may exceed windows length restrictions.
                // Combine will throw a PathTooLongException in that case.
                if (BaseDirectory != null)
                {
                    name = Path.Combine(BaseDirectory, name);
                }
            }
            else
            {
                name = string.Empty;
            }
            return name;
        }
    }
}
