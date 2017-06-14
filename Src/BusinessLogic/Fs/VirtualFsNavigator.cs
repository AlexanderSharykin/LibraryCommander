using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DataSchema;

namespace BusinessLogic.Fs
{
    public class VirtualFsNavigator: FsNavigator
    {
        private VirtualFsItem _folder;
        
        public VirtualFsNavigator(string rootFolder) : base(rootFolder)
        {
            _folder = new VirtualFsItem
                      {
                          IsDirectory = true,
                          Level = VirtualFsItemLevel.None,
                          Name = rootFolder,
                      };
            Name = Localization.Instance.Library;            
        }

        public IRepository<Category> CategoryRepository { get; set; }

        public IRepository<Book> BookRepository { get; set; }

        #region Navigation properties

        public VirtualFsItemLevel Level
        {
            get { return _folder.Level; }
        }

        public Category Category
        {
            get
            {
                if ((int)Level >= (int)VirtualFsItemLevel.Category)
                    return _folder.Category;
                return null;
            }
        }

        public FileLanguage Language
        {
            get
            {
                if ((int)Level >= (int)VirtualFsItemLevel.Language)
                    return _folder.Language;
                return null;
            }
        }

        public int? IdAuthor
        {
            get
            {
                if ((int)Level >= (int)VirtualFsItemLevel.Author)
                    return _folder.IdAuthor;
                return null;
            }
        }

        public int? IdCycle
        {
            get
            {
                if ((int)Level >= (int)VirtualFsItemLevel.Cycle)
                    return _folder.IdCycle;
                return null;
            }
        }
        #endregion

        protected override FsItem GetNewSegment()
        {
            return new VirtualFsItem();
        }

        public override void UpdateSegments()
        {
            base.UpdateSegments();
            Segments[0].Name = Localization.Instance.Library;

            if (_folder == null || _folder.Category == null)
                return;

            int idx = 1;
            foreach (var fsi in Segments.Skip(idx).OfType<VirtualFsItem>())
            {
                fsi.Category = _folder.Category;
                fsi.Level = VirtualFsItemLevel.Category;
            }

            if (_folder.Language == null)
                return;

            idx++;
            foreach (var fsi in Segments.Skip(idx).OfType<VirtualFsItem>())
            {
                fsi.Language = _folder.Language;
                fsi.Level = VirtualFsItemLevel.Language;
            }

            if (_folder.IdAuthor.HasValue)
            {
                idx++;
                foreach (var fsi in Segments.Skip(idx).OfType<VirtualFsItem>())
                {
                    fsi.IdAuthor = _folder.IdAuthor;
                    fsi.Level = VirtualFsItemLevel.Author;
                }
            }

            idx++;
            foreach (var fsi in Segments.Skip(idx).OfType<VirtualFsItem>())
            {
                fsi.IdCycle = _folder.IdCycle;
                fsi.Level = VirtualFsItemLevel.Cycle;
            }
        }

        public override IList<FsItem> GetFiles()
        {
            return GetFiles(null);
        }

        public IList<FsItem> GetFiles(Book template)
        {
            IList<VirtualFsItem> result = null;

            IQueryable<Book> books = null;

            if (Level == VirtualFsItemLevel.Language)
            {
                int idCategory = _folder.Category.Id;
                int idLanguage = _folder.Language.Id;

                books = BookRepository.Query()
                    .Where(b => b.Category.Id == idCategory && b.Language.Id == idLanguage &&
                                b.HasAuthorSubcatalog == false && b.HasCycleSubcatalog == false);

                if (template != null)
                    books = books.Where(b => b.Id == template.Id);
            }

            else if (Level == VirtualFsItemLevel.Author)
            {
                int idCategory = _folder.Category.Id;
                int idLanguage = _folder.Language.Id;
                int idAuthor = _folder.IdAuthor.Value;

                books = BookRepository.Query()
                    .Where(b => b.Category.Id == idCategory && b.Language.Id == idLanguage &&
                                b.HasAuthorSubcatalog && b.HasCycleSubcatalog == false &&
                                b.Authors.All(a => a.Id == idAuthor));

                if (template != null)
                    books = books.Where(b => b.Id == template.Id);
            }

            else if (Level == VirtualFsItemLevel.Cycle)
            {
                int idCategory = _folder.Category.Id;
                int idLanguage = _folder.Language.Id;

                books = BookRepository.Query()
                    .Where(b => b.Category.Id == idCategory && b.Language.Id == idLanguage &&
                                b.HasCycleSubcatalog && b.Cycle.Id == _folder.IdCycle);

                if (_folder.IdAuthor.HasValue)
                    books = books.Where(b => b.HasAuthorSubcatalog && b.Authors.All(a => a.Id == _folder.IdAuthor));

                if (template != null)
                    books = books.Where(b => b.Id == template.Id);
            }
            else
                result = new VirtualFsItem[0];

            if (result == null)
            {
                var files = new DirectoryInfo(CurrentFolder).GetFiles();

                result = books.AsEnumerable().Select(b => CreateBookItem(b)).OrderBy(x => x.Name).ToList();
                
                // check missing books in storage and get book file size
                foreach (var b in result)
                {
                    var file = files.FirstOrDefault(f => f.Name == b.Name);
                    b.IsMissing = file == null;
                    if (false == b.IsMissing)
                        b.Length = file.Length;
                }             
            }

            return result.ToList<FsItem>();
        }

        public VirtualFsItem CreateBookItem(Book b)
        {
            var fsItem = new VirtualFsItem
                         {
                             IsDirectory = false,
                             Name = b.GetFileName(),
                             Level = Level,
                             Category = _folder.Category,
                             Language = _folder.Language,
                             Book = b
                         };
            fsItem.FullPath = Path.Combine(CurrentFolder, fsItem.Name);
            return fsItem;
        }

        public bool IsBookInCurrentFolder(Book book)
        {
            return CurrentFolder == Path.Combine(RootFolder, book.GetFolderName());
        }

        public override IList<FsItem> GetFolders()
        {
            return GetFolders(null);
        }

        public IList<FsItem> GetFolders(Book template)
        {            
            IList<FsItem> result;
            if (Level == VirtualFsItemLevel.None)
            {
                result = CategoryRepository.Load()
                    .Where(c => template == null || c.Id == template.Category.Id)
                    .Select(c => new VirtualFsItem
                                 {
                                     Name = c.Name,
                                     Category = c,
                                     Level = VirtualFsItemLevel.Category
                                 })                    
                    .OfType<FsItem>()
                    .OrderBy(x => x.Name)
                    .ToList();
            }

            else if (Level == VirtualFsItemLevel.Category)
            {
                var c = _folder.Category;
                var idCategory = c.Id;

                IQueryable<Book> books = BookRepository.Query().Where(b => b.Category.Id == idCategory);
                if (template != null)
                    books = books.Where(b => b.Id == template.Id);

                result = books                    
                    .Select(b => b.Language)
                    .Distinct()
                    .AsEnumerable()
                    .Select(x => new VirtualFsItem
                                 {
                                     Name = x.Name,
                                     Category = c,
                                     Language = x,
                                     Level = VirtualFsItemLevel.Language
                                 })                    
                    .OfType<FsItem>()
                    .OrderBy(x => x.Name)
                    .ToList();
            }

            else if (Level == VirtualFsItemLevel.Language)
            {
                var c = _folder.Category;
                int idCategory = c.Id;

                var lang = _folder.Language;
                int idLanguage = lang.Id;

                var books = BookRepository.Query();
                if (template != null)
                    books = books.Where(b => b.Id == template.Id);

                result = books
                    .Where(b => b.Category.Id == idCategory && b.Language.Id == idLanguage &&
                                (b.HasAuthorSubcatalog || b.HasCycleSubcatalog))
                    .AsEnumerable()
                    .Select(b => new
                                 {
                                     Name = b.HasAuthorSubcatalog ? b.Authors.First().Name : b.Cycle.Name,
                                     Id = b.HasAuthorSubcatalog  ? b.Authors.First().Id : b.Cycle.Id,
                                     AuthorSubcatalog = b.HasAuthorSubcatalog
                                 })
                    .Distinct()
                    .Select(b => new VirtualFsItem
                                 {
                                     Name = b.Name,
                                     Category = c,
                                     Language = lang,
                                     IdAuthor = b.AuthorSubcatalog ? (int?)b.Id : null,
                                     IdCycle = b.AuthorSubcatalog ? null : (int?)b.Id,
                                     Level = b.AuthorSubcatalog ? VirtualFsItemLevel.Author : VirtualFsItemLevel.Cycle
                                 })                    
                    .OfType<FsItem>()
                    .OrderBy(x => x.Name)
                    .ToList();
            }

            else if (Level == VirtualFsItemLevel.Author)
            {
                var c = _folder.Category;
                int idCategory = c.Id;
                
                var lang = _folder.Language;
                int idLanguage = lang.Id;

                int idAuthor = _folder.IdAuthor.Value;

                var books = BookRepository.Query();
                if (template != null)
                    books = books.Where(b => b.Id == template.Id);

                result = books
                    .Where(b => b.Category.Id == idCategory && b.Language.Id == idLanguage &&
                                b.HasAuthorSubcatalog && b.HasCycleSubcatalog && b.Authors.All(a => a.Id == idAuthor))
                                    
                    .Select(b => b.Cycle)
                    .Distinct()
                    .AsEnumerable()    
                    .Select(x => new VirtualFsItem
                    {
                        Name = x.Name,
                        Category = c,
                        Language = lang,
                        IdAuthor = idAuthor,
                        IdCycle = x.Id,
                        Level = VirtualFsItemLevel.Cycle
                    })                    
                    .OfType<FsItem>()
                    .OrderBy(x => x.Name)
                    .ToList();
            }
            else 
                result = new FsItem[0];

            var folders = new DirectoryInfo(CurrentFolder).GetDirectories();

            foreach (var fsItem in result)
            {
                fsItem.IsDirectory = true;                
                fsItem.IsMissing = false == folders.Any(f => f.Name == fsItem.Name);
                fsItem.FullPath = Path.Combine(CurrentFolder, fsItem.Name) + Path.DirectorySeparatorChar;
            }
            return result;
        }

        /// <summary>
        /// Performs operation with an item (file or folder)
        /// </summary>
        /// <param name="item"></param>
        public override bool PickItem(FsItem item)
        {
            if (item == null)
                return false;

            // open file
            if (false == item.IsDirectory)
                return base.PickItem(item);

            // navigate to parent
            if (item.Name == FsItem.GotoParent.Name && CanNavigateToParentFolder)
            {
                if (_folder.Level == VirtualFsItemLevel.Cycle && _folder.IdAuthor == null)
                    _folder.Level = VirtualFsItemLevel.Language;
                else
                    _folder.Level = (VirtualFsItemLevel)((int)_folder.Level - 1);
                

                CurrentFolder = GetParentFolder();
                return true;
            }

            // open folder
            if (item is VirtualFsItem)
            {                
                _folder = (VirtualFsItem) item;
                CurrentFolder = _folder.FullPath.TrimEnd(Path.DirectorySeparatorChar);
                return true;
            }
            
            return false;
        }
    }
}
