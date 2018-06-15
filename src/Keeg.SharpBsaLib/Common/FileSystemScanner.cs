using System;
using System.Collections.Generic;
using System.IO;

namespace Keeg.SharpBsaLib.Common
{
    public class FileSystemScanner
    {
        #region Instance Fields
        private INameFilter _fileFilter;
        private INameFilter _directoryFilter;
        private bool _alive;
        #endregion

        #region Constructors
        /// <summary>
        /// Initialise a new instance of <see cref="FileSystemScanner"></see>
        /// </summary>
        /// <param name="filter">The <see cref="PathFilter">file filter</see> to apply when scanning.</param>
        public FileSystemScanner(string fileFilter)
        {
            _fileFilter = new PathFilter(fileFilter);
        }

        /// <summary>
        /// Initialise a new instance of <see cref="FileSystemScanner"></see>
        /// </summary>
        /// <param name="fileFilter">The file <see cref="INameFilter">filter</see> to apply.</param>
        public FileSystemScanner(INameFilter fileFilter)
        {
            _fileFilter = fileFilter;
        }

        /// <summary>
        /// Initialise a new instance of <see cref="FileSystemScanner"></see>
        /// </summary>
        /// <param name="fileFilter">The <see cref="PathFilter">file filter</see> to apply.</param>
        /// <param name="directoryFilter">The <see cref="PathFilter"> directory filter</see> to apply.</param>
        public FileSystemScanner(string fileFilter, string directoryFilter) : this(fileFilter)
        {
            _directoryFilter = new PathFilter(directoryFilter);
        }

        /// <summary>
        /// Initialise a new instance of <see cref="FileSystemScanner"></see>
        /// </summary>
        /// <param name="fileFilter">The file <see cref="INameFilter">filter</see>  to apply.</param>
        /// <param name="directoryFilter">The directory <see cref="INameFilter">filter</see>  to apply.</param>
        public FileSystemScanner(INameFilter fileFilter, INameFilter directoryFilter) : this(fileFilter)
        {
            _directoryFilter = directoryFilter;
        }
        #endregion

        #region Delegates
        /// <summary>
        /// Delegate to invoke when a directory is processed.
        /// </summary>
        public event EventHandler<DirectoryEventArgs> ProcessDirectory;

        /// <summary>
        /// Delegate to invoke when a file is processed.
        /// </summary>
        public event EventHandler<ScanEventArgs> ProcessFile;

        /// <summary>
        /// Delegate to invoke when processing for a file has finished.
        /// </summary>
        public event EventHandler<ScanEventArgs> CompletedFile;

        /// <summary>
        /// Delegate to invoke when a directory failure is detected.
        /// </summary>
        public event EventHandler<ScanFailureEventArgs> DirectoryFailure;

        /// <summary>
        /// Delegate to invoke when a file failure is detected.
        /// </summary>
        public event EventHandler<ScanFailureEventArgs> FileFailure;
        #endregion

        /// <summary>
        /// Raise the DirectoryFailure event.
        /// </summary>
        /// <param name="directory">The directory name.</param>
        /// <param name="e">The exception detected.</param>
        private bool OnDirectoryFailure(string directory, Exception e)
        {
            var handler = DirectoryFailure;
            bool result = (handler != null);
            if (result)
            {
                var args = new ScanFailureEventArgs(directory, e);
                handler(this, args);
                _alive = args.ContinueRunning;
            }

            return result;
        }

        /// <summary>
        /// Raise the FileFailure event.
        /// </summary>
        /// <param name="file">The file name.</param>
        /// <param name="e">The exception detected.</param>
        private bool OnFileFailure(string file, Exception e)
        {
            var handler = FileFailure;
            bool result = (handler != null);
            if (result)
            {
                var args = new ScanFailureEventArgs(file, e);
                FileFailure(this, args);
                _alive = args.ContinueRunning;
            }
            return result;
        }

        /// <summary>
        /// Raise the ProcessFile event.
        /// </summary>
        /// <param name="file">The file name.</param>
        void OnProcessFile(string file)
        {
            var handler = ProcessFile;

            if (handler != null)
            {
                var args = new ScanEventArgs(file);
                handler(this, args);
                _alive = args.ContinueRunning;
            }
        }

        /// <summary>
        /// Raise the complete file event
        /// </summary>
        /// <param name="file">The file name</param>
        private void OnCompleteFile(string file)
        {
            var handler = CompletedFile;

            if (handler != null)
            {
                var args = new ScanEventArgs(file);
                handler(this, args);
                _alive = args.ContinueRunning;
            }
        }

        /// <summary>
        /// Raise the ProcessDirectory event.
        /// </summary>
        /// <param name="directory">The directory name.</param>
        /// <param name="hasMatchingFiles">Flag indicating if the directory has matching files.</param>
        void OnProcessDirectory(string directory, bool hasMatchingFiles)
        {
            var handler = ProcessDirectory;

            if (handler != null)
            {
                var args = new DirectoryEventArgs(directory, hasMatchingFiles);
                handler(this, args);
                _alive = args.ContinueRunning;
            }
        }

        /// <summary>
        /// Scan a directory.
        /// </summary>
        /// <param name="directory">The base directory to scan.</param>
        /// <param name="recurse">True to recurse subdirectories, false to scan a single directory.</param>
        public void Scan(string directory, bool recurse)
        {
            _alive = true;
            ScanDir(directory, recurse);
        }

        void ScanDir(string directory, bool recurse)
        {
            try
            {
                var unfilteredNames = Directory.GetFiles(directory);
                var filteredNames = new List<string>();
                bool hasMatch = false;
                for (int fileIndex = 0; fileIndex < unfilteredNames.Length; fileIndex++)
                {
                    if (_fileFilter.IsMatch(unfilteredNames[fileIndex]))
                    {
                        filteredNames.Add(unfilteredNames[fileIndex]);
                        hasMatch = true;
                    }
                }

                OnProcessDirectory(directory, hasMatch);

                if (_alive && hasMatch)
                {
                    foreach (string fileName in filteredNames)
                    {
                        try
                        {
                            OnProcessFile(fileName);
                            if (!_alive)
                            {
                                break;
                            }
                        }
                        catch (Exception e)
                        {
                            if (!OnFileFailure(fileName, e))
                            {
                                throw;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if (!OnDirectoryFailure(directory, e))
                {
                    throw;
                }
            }

            if (_alive && recurse)
            {
                try
                {
                    string[] names = Directory.GetDirectories(directory);
                    foreach (string fulldir in names)
                    {
                        if ((_directoryFilter == null) || (_directoryFilter.IsMatch(fulldir)))
                        {
                            ScanDir(fulldir, true);
                            if (!_alive)
                            {
                                break;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    if (!OnDirectoryFailure(directory, e))
                    {
                        throw;
                    }
                }
            }
        }
    }
}
