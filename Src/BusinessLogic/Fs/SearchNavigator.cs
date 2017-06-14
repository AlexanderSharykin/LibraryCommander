using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DataSchema;

namespace BusinessLogic.Fs
{
    public class SearchNavigator: FsNavigator
    {
        private List<FsItem> _empty = new List<FsItem>();
        private List<FsItem> _files;
        private Book _searchTemplate;

        public SearchNavigator(string rootFolder) : base(rootFolder)
        {
            Name = Localization.Instance.Search;
        }

        public IRepository<Book> Repository { get; set; }

        public Book SearchTemplate
        {
            get { return _searchTemplate; }
            set
            {
                _searchTemplate = value;
                // remove results of the previous search
                _files = null;
            }
        }
        
        /// <summary>
        /// Gets list of book which match search template
        /// </summary>
        /// <returns></returns>
        public override IList<FsItem> GetFiles()
        {
            if (_files == null)
            {
                _files = Repository.Load(SearchTemplate)
                    .Select(b => new VirtualFsItem
                                 {
                                     IsDirectory = false,
                                     Book = b,
                                     Name = b.GetFileName(),
                                     FullPath = Path.Combine(RootFolder, b.GetPath())
                                 })
                    .OrderBy(x => x.Name)
                    .ToList<FsItem>();

                // check missing books in storage and get book file size
                foreach (var fsItem in _files)
                {
                    if (File.Exists(fsItem.FullPath))
                    {
                        var fi = new FileInfo(fsItem.FullPath);
                        fsItem.Length = fi.Length;
                    }
                    else
                        fsItem.IsMissing = true;
                }
            }
            return _files;
        }

        public override IList<FsItem> GetFolders()
        {
            return _empty;
        }

        public override void UpdateSegments()
        {
            base.UpdateSegments();
            Segments[0].Name = Localization.Instance.Search;
        }
    }
}
