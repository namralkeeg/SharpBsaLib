using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace Keeg.SharpBsaLib
{
    public interface IArchive
    {
        IReadOnlyCollection<Entry> Entries { get; }
        string FullName { get; }
        string FullPath { get; }
        string Name { get; }

        bool Open();
        bool Close();
        bool IsArchive(string filePath);
        bool IsArchive(Stream stream);

        bool Extract(string filePath, out byte[] data);
        bool Extract(Entry entry, out byte[] data);
        bool Extract(string filePath, string destinationFolder, bool overwrite);
        bool Extract(Entry entry, string destinationFolder, bool overwrite);
        bool Extract(IEnumerable<string> filePaths, string destinationFolder, bool overwrite);
        bool Extract(IEnumerable<Entry> entries, string destinationFolder, bool overwrite);
        bool ExtractToFolder(string destinationFolder, bool overwrite);

        IEnumerable<Entry> GetEntries(IEnumerable<string> filePaths);
        Entry GetEntry(string filePath);
        IEnumerable<Entry> GetMatchingEntries(Regex regex);
        bool HasEntry(string filePath);

        byte[] GetEntryChecksum(string filePath, HashAlgorithm hasher);
        byte[] GetEntryChecksum(Entry entry, HashAlgorithm hasher);
    }
}
