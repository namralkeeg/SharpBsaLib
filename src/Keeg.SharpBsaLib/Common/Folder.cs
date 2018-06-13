using System.Collections.Generic;

namespace Keeg.SharpBsaLib.Common
{
    public class Folder /*: IFolder*/
    {
        protected List<Folder> children = new List<Folder>();
        protected List<Entry> entries = new List<Entry>();

        public Folder Parent { get; set; }
        public IList<Folder> Children => children;
        public IList<Entry> Entries => entries;
        public string Name { get; set; }

        public int CompareTo(Folder other)
        {
            return Name.CompareTo(other.Name);
        }
    }
}
