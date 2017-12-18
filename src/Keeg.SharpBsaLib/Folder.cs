using System.Collections.Generic;

namespace Keeg.SharpBsaLib
{
    public class Folder : IFolder
    {
        protected IList<IFolder> children = new List<IFolder>();
        protected IList<IEntry> entries = new List<IEntry>();

        public IFolder Parent { get; set; }
        public IList<IFolder> Children => children;
        public IList<IEntry> Entries => entries;
        public string Name { get; set; }

        public int CompareTo(IFolder other)
        {
            return Name.CompareTo(other.Name);
        }
    }
}
