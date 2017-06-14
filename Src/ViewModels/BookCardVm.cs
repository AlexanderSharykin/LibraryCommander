using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;
using BusinessLogic;
using DataSchema;
using MvvmFoundation.Wpf;
using ViewModels.Dialogs;

namespace ViewModels
{
    /// <summary>
    /// View model to create, edit and search books
    /// </summary>
    public class BookCardVm: ValidationVm
    {
        private string _title;        
        private string _authors;
        private IList<Author> _selectedAuthors;

        private Category _selectedCategory;
        private string _tags;
        private IList<Tag> _selectedTags;

        private Cycle _selectedCycle;
        private FileLanguage _selectedLanguage;
        private FileFormat _selectedFormat;
        private int _year;
        private byte _volume;
        private bool _isYearKnown;
        private bool _isVolumeKnown;
        private string _filePath;
        private bool _hasAuthorSubcatalog;
        private bool _hasCycleSubcatalog;

        public BookCardVm()
        {
            AddAuthorsCmd = new RelayCommand(AddAuthors);
            AddCategoryCmd = new RelayCommand(AddCategory);
            AddTagsCmd = new RelayCommand(AddTags, CanAddTags);
            AddLanguageCmd = new RelayCommand(AddLanguage);
            AddFormatCmd = new RelayCommand(AddFormat);
            AddCycleCmd = new RelayCommand(AddCycle, CanAddCycle);
            RemoveCycleCmd = new RelayCommand(RemoveCycle);
            OpenFileCmd = new RelayCommand(OpenFile, CanOpenFile);
            Year = 1900;
            Volume = 1;

            CopyTemplateCmd = new RelayCommand(CopyTemplate);
            PasteTemplateCmd = new RelayCommand(PasteTemplate);
        }

        public BookCardVm(Book book): this()
        {
            DisplayBookData(book);
        }

        private void DisplayBookData(Book book)
        {
            BookId = book.Id;
            Title = book.Title;
            SelectedCategory = book.Category;
            SelectedLanguage = book.Language;
            SelectedAuthors = book.Authors.ToList();
            SelectedTags = book.Tags.ToList();
            SelectedCycle = book.Cycle;
            SelectedFormat = book.Formats.FirstOrDefault();
            HasAuthorSubcatalog = book.HasAuthorSubcatalog;
            HasCycleSubcatalog = book.HasCycleSubcatalog;
            if (book.Year.HasValue)
            {
                IsYearKnown = true;
                Year = book.Year.Value;
            }
            if (book.Volume.HasValue)
            {
                IsVolumeKnown = true;
                Volume = (byte) book.Volume.Value;
            }
        }

        private VisualDialogContainer Dialogs
        {
            get { return Injector.Dialogs; }            
        }

        private ILocalizationVm Localization
        {
            get { return Injector.Localization; }            
        }

        public bool EditMode { get; set; }
        
        public bool SearchMode { get; set; }

        public int BookId { get; set; }

        public string FilePath
        {
            get { return _filePath; }
            set
            {
                _filePath = value;
                OnPropertyChanged();
                OnErrorsChanged();

                if (SelectedFormat == null)
                {
                    string ext = Path.GetExtension(FilePath);
                    if (ext == null)
                        return;
                    ext = ext.Trim('.');
                    SelectedFormat = Injector.Container.GetInstance<FileFormatManager>().GetAll().FirstOrDefault(f => f.Name == ext);
                }

                if (String.IsNullOrWhiteSpace(Title))
                    Title = Path.GetFileNameWithoutExtension(FilePath);
            }
        }

        public ICommand OpenFileCmd { get; private set; }

        private void OpenFile()
        {
            var ofd = Dialogs.Get("ofd");
            var fileVm = new FileDialogVm();
            fileVm.FileName = FilePath;
            fileVm.Title = Localization.FileTitle;

            if (ofd.ShowDialog(fileVm) != true)
                return;

            FilePath = fileVm.FileName;
        }

        private bool CanOpenFile()
        {
            return true;
        }

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged();
                OnErrorsChanged();
            }
        }

        #region Authors
        public string Authors
        {
            get { return _authors; }
            set
            {
                _authors = value;
                OnPropertyChanged();
                OnPropertyChanged("CanCreateAuthorSubcatalog");
                OnErrorsChanged();                
            }
        }

        public bool HasAuthorSubcatalog
        {
            get { return _hasAuthorSubcatalog; }
            set
            {
                _hasAuthorSubcatalog = value;
                OnPropertyChanged();
            }
        }

        public bool CanCreateAuthorSubcatalog
        {
            get { return SelectedAuthors != null && SelectedAuthors.Count == 1; }
        }

        private IList<Author> SelectedAuthors
        {
            get { return _selectedAuthors; }
            set
            {
                _selectedAuthors = value;
                if (_selectedAuthors != null)
                {
                    Authors = String.Join(", ", _selectedAuthors.Select(a => a.Name));
                    if (_selectedAuthors.Count != 1)
                        HasAuthorSubcatalog = false;
                }                
                else
                    Authors = null;
            }
        }

        public ICommand AddAuthorsCmd { get; set; }

        private void AddAuthors()
        {
            var vm = new AttributeSelectorVm<Author>(Injector.Container.GetInstance<AuthorManager>());
            vm.Title = Localization.AuthorsTitle;
            vm.MultiSelect = true;
            if (SelectedAuthors != null)
            {
                foreach (var authorItem in vm.Items.Where(x => SelectedAuthors.Any(a => a.Id == ((Entity)x.Value).Id)))
                    authorItem.IsSelected = true;
            }

            var selector = Dialogs.Get("selection");

            if (selector.ShowDialog(vm) != true)
                return;

            SelectedAuthors = vm.SelectedValues.ToList();
        }
        #endregion

        #region Category

        public string CategoryName
        {
            get { return SelectedCategory != null ? SelectedCategory.Name : null; }
        }
        public Category SelectedCategory
        {
            get { return _selectedCategory; }
            set
            {
                if (_selectedCategory != null && value != null && _selectedCategory.Id != value.Id)
                {
                    SelectedTags = null;
                    SelectedCycle = null;
                }
                _selectedCategory = value;
                OnPropertyChanged("CategoryName");
                OnErrorsChanged("CategoryName");
            }
        }

        public ICommand AddCategoryCmd { get; private set; }

        private void AddCategory()
        {
            var vm = new AttributeSelectorVm<Category>(Injector.Container.GetInstance<CategoryManager>());
            vm.Title = Localization.CategoryTitle;

            if (SelectedCategory != null)
            {
                var catItem = vm.Items.FirstOrDefault(x => ((Entity)x.Value).Id == SelectedCategory.Id);
                if (catItem != null)
                    catItem.IsSelected = true;
            }

            var selector = Dialogs.Get("selection");

            if (selector.ShowDialog(vm) != true)
                return;

            SelectedCategory = vm.SelectedValues.FirstOrDefault();
        }
        #endregion

        #region Tags
        public string Tags
        {
            get { return _tags; }
            set
            {
                _tags = value;
                OnPropertyChanged();
                OnErrorsChanged();
            }
        }

        private IList<Tag> SelectedTags
        {
            get { return _selectedTags; }
            set
            {
                _selectedTags = value;
                if (_selectedTags != null)
                    Tags = String.Join(", ", _selectedTags.Select(a => a.Name));
                else
                    Tags = null;
            }
        }

        public ICommand AddTagsCmd { get; set; }

        private void AddTags()
        {
            var tm = Injector.Container.GetInstance<TagManager>();
            tm.Category = SelectedCategory;
            var vm = new AttributeSelectorVm<Tag>(tm);
            vm.MultiSelect = true;
            vm.Title = Localization.TagsTitle;

            if (SelectedTags != null)
            {
                foreach (var tagItem in vm.Items.Where(x => SelectedTags.Any(t => t.Id == ((Entity)x.Value).Id)))
                    tagItem.IsSelected = true;
            }

            var selector = Dialogs.Get("selection");

            if (selector.ShowDialog(vm) != true)
                return;

            SelectedTags = vm.SelectedValues.ToList();            
        }

        private bool CanAddTags()
        {
            return SelectedCategory != null;
        }
        #endregion

        #region FileLanguage
        public FileLanguage SelectedLanguage
        {
            get { return _selectedLanguage; }
            set
            {
                _selectedLanguage = value;
                OnPropertyChanged("LanguageName");
                OnErrorsChanged("LanguageName");
            }
        }

        public string LanguageName
        {
            get { return SelectedLanguage != null ? SelectedLanguage.Name : null; }
        }

        public ICommand AddLanguageCmd { get; private set; }

        private void AddLanguage()
        {
            var vm = new AttributeSelectorVm<FileLanguage>(Injector.Container.GetInstance<FileLanguageManager>());
            vm.Title = Localization.LanguageTitle;

            if (SelectedLanguage != null)
            {
                var catItem = vm.Items.FirstOrDefault(x => ((Entity)x.Value).Id == SelectedLanguage.Id);
                if (catItem != null)
                    catItem.IsSelected = true;
            }

            var selector = Dialogs.Get("selection");

            if (selector.ShowDialog(vm) != true)
                return;

            SelectedLanguage = vm.SelectedValues.FirstOrDefault();
        }
        #endregion

        #region FileFormat
        public FileFormat SelectedFormat
        {
            get { return _selectedFormat; }
            set
            {
                _selectedFormat = value;
                OnPropertyChanged("FormatName");
                OnErrorsChanged("FormatName");
            }
        }

        public string FormatName
        {
            get { return SelectedFormat != null ? SelectedFormat.Name : null; }
        }

        public ICommand AddFormatCmd { get; private set; }

        private void AddFormat()
        {
            var vm = new AttributeSelectorVm<FileFormat>(Injector.Container.GetInstance<FileFormatManager>());
            vm.Title = Localization.FormatTitle;

            if (SelectedFormat != null)
            {
                var catItem = vm.Items.FirstOrDefault(x => ((Entity)x.Value).Id == SelectedFormat.Id);
                if (catItem != null)
                    catItem.IsSelected = true;
            }

            var selector = Dialogs.Get("selection");

            if (selector.ShowDialog(vm) != true)
                return;

            SelectedFormat = vm.SelectedValues.FirstOrDefault();
        }
        #endregion

        #region Cycle
        public Cycle SelectedCycle
        {
            get { return _selectedCycle; }
            set
            {
                _selectedCycle = value;
                OnPropertyChanged();
                OnPropertyChanged("CanCreateCycleSubcatalog");                
            }
        }

        public bool HasCycleSubcatalog
        {
            get { return _hasCycleSubcatalog; }
            set
            {
                _hasCycleSubcatalog = value;
                OnPropertyChanged();
            }
        }

        public bool CanCreateCycleSubcatalog
        {
            get { return SelectedCycle != null; }
        }

        public ICommand AddCycleCmd { get; private set; }

        private void AddCycle()
        {
            var cm = Injector.Container.GetInstance<CycleManager>();
            cm.Category = SelectedCategory;
            var vm = new AttributeSelectorVm<Cycle>(cm);
            vm.Title = Localization.CycleTitle;

            if (SelectedCycle != null)
            {
                var catItem = vm.Items.FirstOrDefault(x => ((Entity)x.Value).Id == SelectedCycle.Id);
                if (catItem != null)
                    catItem.IsSelected = true;
            }

            var selector = Dialogs.Get("selection");

            if (selector.ShowDialog(vm) != true)
                return;

            SelectedCycle = vm.SelectedValues.FirstOrDefault();
        }

        private bool CanAddCycle()
        {
            return SelectedCategory != null;
        }

        public ICommand RemoveCycleCmd { get; private set; }

        private void RemoveCycle()
        {
            SelectedCycle = null;
        }

        #endregion

        public bool IsYearKnown
        {
            get { return _isYearKnown; }
            set
            {
                _isYearKnown = value;
                OnPropertyChanged();                
            }
        }

        public int Year
        {
            get { return _year; }
            set
            {
                _year = value;
                OnPropertyChanged();                
            }
        }

        public bool IsVolumeKnown
        {
            get { return _isVolumeKnown; }
            set
            {
                _isVolumeKnown = value;
                OnPropertyChanged();                
            }
        }

        public byte Volume
        {
            get { return _volume; }
            set
            {
                _volume = value;
                OnPropertyChanged();                
            }
        }

        private static Book Template;

        public ICommand CopyTemplateCmd { get; private set; }

        private void CopyTemplate()
        {
            Template = GetBook();
            Template.Title = "";            
        }
        
        public ICommand PasteTemplateCmd { get; private set; }

        private void PasteTemplate()
        {
            DisplayBookData(Template);
        }        

        public Book GetBook()
        {
            var b = new Book
            {
                Id = BookId,
                Title = Title,
                Category = SelectedCategory,
                Language = SelectedLanguage,
                Cycle = SelectedCycle,
                HasAuthorSubcatalog = HasAuthorSubcatalog,
                HasCycleSubcatalog = HasCycleSubcatalog,                
            };

            if (SelectedFormat != null)
                b.Formats.Add(SelectedFormat);

            if (SelectedAuthors != null)
                foreach (var author in SelectedAuthors)
                    b.Authors.Add(author);

            if (SelectedTags != null)
                foreach (var t in SelectedTags)
                    b.Tags.Add(t);

            if (IsYearKnown)
                b.Year = Year;

            if (IsVolumeKnown)
                b.Volume = Volume;            

            return b;
        }

        public override IEnumerable GetErrors(string propertyName = null)
        {
            if (SearchMode)
                yield break;

            string key = "Title";
            if ((propertyName ?? key) == key && String.IsNullOrWhiteSpace(Title))
                yield return new DictionaryEntry(key, Localization.TitleError);

            key = "FilePath";
            if ((propertyName ?? key) == key && BookId == 0 && String.IsNullOrWhiteSpace(FilePath))
                yield return new DictionaryEntry(key, Localization.FilePathError);

            key = "CategoryName";
            if ((propertyName ?? key) == key && SelectedCategory == null)
                yield return new DictionaryEntry(key, Localization.CategoryError);

            key = "LanguageName";
            if ((propertyName ?? key) == key && SelectedLanguage == null)
                yield return new DictionaryEntry(key, Localization.LanguageError);

            key = "FormatName";
            if ((propertyName ?? key) == key && SelectedFormat == null)
                yield return new DictionaryEntry(key, Localization.FormatError);

            key = "Authors";
            if ((propertyName ?? key) == key && (SelectedAuthors == null || SelectedAuthors.Count == 0))
                yield return new DictionaryEntry(key, Localization.AuthorsError);

            key = "Tags";
            if ((propertyName ?? key) == key && (SelectedTags == null || SelectedTags.Count == 0))
                yield return new DictionaryEntry(key, Localization.TagsError);
        }
    }
}
