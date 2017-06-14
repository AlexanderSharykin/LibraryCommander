using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using BusinessLogic;
using BusinessLogic.Fs;
using DataSchema;
using MvvmFoundation.Wpf;
using ViewModels.Dialogs;

namespace ViewModels
{
    /// <summary>
    /// Main screen view model
    /// </summary>
    public class CommanderVm: ObservableVm
    {
        private string _currentCulture;

        private bool _searchActive;

        private ObservableCollection<PartitionVm> _partitions;
        private PartitionVm _selectedPartition;
        private Dictionary<string, FsNavigator> _partitionNavigators;

        private int _selectedPanel;        

        private VirtualFsNavigator _libraryNavigator;
        private SearchNavigator _searchNavigator;
        
        public CommanderVm()
        {
            SelectPartitionCmd = new RelayCommand<PartitionVm>(SelectPartition);
            SetCultureCmd = new RelayCommand<string>(SetCulture);
            SwitchPanelsCmd = new RelayCommand(SwitchPanels);

            PickCmd = new RelayCommand(Pick, CanPick);
            CopyFileCmd = new RelayCommand(CopyFile, CanCopyOrMoveFile);
            MoveFileCmd = new RelayCommand(MoveFile, CanCopyOrMoveFile);

            AddBookCmd = new RelayCommand(AddBook, CanAddBook);
            EditBookCmd = new RelayCommand(EditBook, CanEditBook);
            DeleteBookCmd = new RelayCommand(DeleteBook, CanDeleteBook);

            SearchCmd = new RelayCommand(Search, CanSearch);

            _libraryNavigator = Injector.Container.GetInstance<VirtualFsNavigator>();
            _searchNavigator = Injector.Container.GetInstance<SearchNavigator>();            

            NavigationPanels = new ObservableCollection<NavigatorVm> { null, null };            
            SetLibraryNavigator(_libraryNavigator);
        }

        private VisualDialogContainer Dialogs
        {
            get { return Injector.Dialogs; }            
        }

        #region Settings
        public void ApplySettings(AppSettings s)
        {
            if (s == null)
                return;

            // restore Disk location
            if (String.IsNullOrWhiteSpace(s.FsLocation) == false)
            {
                var root = Path.GetPathRoot(s.FsLocation);
                var disk = Partitions.FirstOrDefault(p => p.Name == root);
                if (disk != null)
                {
                    SelectedPartition = disk;
                    PartitionPanel.PickItemCmd.Execute(new FsItem {IsDirectory = true, FullPath = s.FsLocation});                    
                }
            }

            // restore Library location
            if (s.Category == null || String.IsNullOrWhiteSpace(s.LibraryLocation))
                return;

            var folder = new VirtualFsItem
                         {
                             FullPath = s.LibraryLocation,
                             Category = s.Category,                             
                             Level = VirtualFsItemLevel.Category
                         };

            if (s.Language != null)
            {
                folder.Language = s.Language;
                folder.Level = VirtualFsItemLevel.Language;
            }

            if (s.IdAuthor.HasValue)
            {
                folder.IdAuthor = s.IdAuthor;
                folder.Level = VirtualFsItemLevel.Author;
            }

            if (s.IdCycle.HasValue)
            {
                folder.IdCycle = s.IdCycle;
                folder.Level = VirtualFsItemLevel.Cycle;
            }
            LibraryPanel.PickItemCmd.Execute(folder);            
        }

        public AppSettings GetCurrentSettings()
        {
            return new AppSettings
                   {
                       FsLocation = PartitionPanel.CurrentFolder,
                       LibraryLocation = _libraryNavigator.CurrentFolder,
                       Category = _libraryNavigator.Category,
                       Language = _libraryNavigator.Language,
                       IdAuthor = _libraryNavigator.IdAuthor,
                       IdCycle = _libraryNavigator.IdCycle,
                   };
        }
        #endregion

        #region Culture
        public ICommand SetCultureCmd { get; private set; }

        private void SetCulture(string culture)
        {
            if (String.IsNullOrWhiteSpace(culture))
                return;
            CurrentCulture = culture;

            // change root display name for navigators
            LibraryPanel.UpdateSegments();
            if (_searchActive)
                _libraryNavigator.UpdateSegments();
            else 
                _searchNavigator.UpdateSegments();
        }

        public string CurrentCulture
        {
            get { return _currentCulture; }
            set
            {
                _currentCulture = value;
                if (CultureChanged != null)
                    CultureChanged(this, EventArgs.Empty);
            }
        }

        public event EventHandler CultureChanged;

        #endregion

        #region Partitions

        public ObservableCollection<PartitionVm> Partitions
        {
            get { return _partitions; }
            set
            {
                _partitions = value;
                _partitionNavigators = _partitions.Select(x => x.Name).ToDictionary(x => x, x => new FsNavigator(x));

                SelectedPartition = _partitions.FirstOrDefault();
            }
        }

        public PartitionVm SelectedPartition
        {
            get { return _selectedPartition; }
            set
            {
                _selectedPartition = value;
                OnPropertyChanged();

                SetPartitionNavigator(_partitionNavigators[_selectedPartition.Name]);
            }
        }

        public ICommand SelectPartitionCmd { get; private set; }

        private void SelectPartition(PartitionVm p)
        {
            SelectedPartition = p;
        }
        #endregion

        #region Navigation
        public ObservableCollection<NavigatorVm> NavigationPanels { get; set; }

        public int SelectedPanel
        {
            get { return _selectedPanel; }
            set
            {
                //if (_selectedPanel == value)
                //    return;

                if (_selectedPanel != value && _selectedPanel >= 0)
                {
                    var panel = NavigationPanels[_selectedPanel];
                    if (panel != null)
                        panel.SelectedItem = null;
                }

                _selectedPanel = value;

                if (_selectedPanel >= 0)
                {
                    var panel = NavigationPanels[_selectedPanel];
                    if (panel != null)
                        panel.SelectedItem = panel.Content.FirstOrDefault();
                }
            }
        }

        private bool IsPartitionPanel { get { return _selectedPanel == 0; } }

        private NavigatorVm PartitionPanel { get { return NavigationPanels[0]; } }

        private bool IsLibraryPanel { get { return _selectedPanel == 1; } }

        private NavigatorVm LibraryPanel { get { return NavigationPanels[1]; } }

        private void SetPartitionNavigator(FsNavigator nav)
        {
            var n = new NavigatorVm(nav);            
            NavigationPanels[0] = n;
            SelectedPanel = 0;
            //n.SelectedItem = n.Content.FirstOrDefault();
        }

        private void SetLibraryNavigator(FsNavigator nav)
        {
            var n = new NavigatorVm(nav);
            n.ShowItemDetails = _searchActive;            
            NavigationPanels[1] = n;
            SelectedPanel = 1;
            //n.SelectedItem = n.Content.FirstOrDefault();            
        }

        public ICommand SwitchPanelsCmd { get; private set; }

        private void SwitchPanels()
        {
            if (SelectedPanel == 0)
                SelectedPanel = 1;
            else
                SelectedPanel = 0;
        }
        #endregion

        #region Library Commands

        public ICommand AddBookCmd { get; private set; }

        private void AddBook()
        {
            var bk = new BookCardVm();
            bk.SelectedCategory = _libraryNavigator.Category;
            bk.SelectedLanguage = _libraryNavigator.Language;
            
            ShowAddBookDialog(bk, false);
        }

        private bool CanAddBook()
        {
            return IsLibraryPanel &&
                   false == _searchActive;            
        }

        private bool ShowAddBookDialog(BookCardVm bk, bool deleteSourceFile)
        {
            var cardWindow = Dialogs.Get("bookCard");

            if (cardWindow.ShowDialog(bk) != true)
                return false;

            var m = Injector.Container.GetInstance<BookManager>();
            var book = bk.GetBook();
            bool success = m.TryAdd(book, bk.FilePath, deleteSourceFile);
            
            if (success)
            {
                UpdateBookContent(book);
            }
            else
            {
                var msg = new MessageVm {Caption = m.Operation, Text = m.Message};
                Dialogs.Get("messageBox").ShowDialog(msg);
            }
            return success;
        }

        /// <summary>
        /// Updates book file and folders in Library panel
        /// </summary>
        /// <param name="book"></param>
        private void UpdateBookContent(Book book)
        {
            var folders = _libraryNavigator.GetFolders(book);
            foreach (var fsItem in folders)
                LibraryPanel.Content.Insert(0, fsItem);

            var files = _libraryNavigator.GetFiles(book);
            foreach (var fsItem in files)
                LibraryPanel.Content.Add(fsItem);
        }

        public ICommand EditBookCmd { get; private set; }

        private void EditBook()
        {
            var fsItem = LibraryPanel.SelectedItem as VirtualFsItem;
            if (fsItem == null)
                return;

            var bk = new BookCardVm(fsItem.Book);
            bk.FilePath = fsItem.FullPath;
            bk.EditMode = true;

            var cardWindow = Dialogs.Get("bookCard");

            if (cardWindow.ShowDialog(bk) != true)
                return;

            var m = Injector.Container.GetInstance<BookManager>();
            var book = bk.GetBook();
            bool success = m.TryUpdate(book, fsItem.FullPath);

            if (success)
            {
                if (_libraryNavigator.IsBookInCurrentFolder(book) == false)
                    LibraryPanel.Content.Remove(fsItem);
                UpdateBookContent(book);
            }
            else
            {
                var msg = new MessageVm {Caption = m.Operation, Text = m.Message};
                Dialogs.Get("messageBox").ShowDialog(msg);
            }
        }

        private bool CanEditBook()
        {
            return IsLibraryPanel && 
                false == _searchActive &&
                LibraryPanel.SelectedItem != null && 
                false == LibraryPanel.SelectedItem.IsDirectory;
        }

        public ICommand DeleteBookCmd { get; private set; }

        private void DeleteBook()
        {
            var fsItem = LibraryPanel.SelectedItem as VirtualFsItem;
            if (fsItem == null)
                return;

            var confirm = new MessageVm
                          {
                              Caption = Injector.Localization.DeleteBookCaption, 
                              Text = String.Format(Injector.Localization.DeleteBookQuestion, fsItem.Name), 
                              No = true
                          };
            Dialogs.Get("messageBox").ShowDialog(confirm);

            if (confirm.DialogResult.ToString() != "Yes")
                return;

            var book = new BookCardVm(fsItem.Book).GetBook();

            var m = Injector.Container.GetInstance<BookManager>();
            bool success = m.TryDelete(book);

            if (success)
            {
                LibraryPanel.Content.Remove(fsItem);
                return;
            }

            var msg = new MessageVm { Caption = m.Operation, Text = m.Message };
            Dialogs.Get("messageBox").ShowDialog(msg);
        }

        private bool CanDeleteBook()
        {
            return IsLibraryPanel &&
                   LibraryPanel.SelectedItem != null &&
                   false == LibraryPanel.SelectedItem.IsDirectory;
        }

        #endregion

        #region Files commands

        public ICommand CopyFileCmd { get; private set; }

        private void CopyFile()
        {
            var bk = GetBookToCopy();
            ShowAddBookDialog(bk, false);
        }

        public ICommand MoveFileCmd { get; private set; }

        private void MoveFile()
        {
            var bk = GetBookToCopy();
            bool success = ShowAddBookDialog(bk, true);
            if (success)
                PartitionPanel.Content.Remove(PartitionPanel.SelectedItem);
        }

        private BookCardVm GetBookToCopy()
        {
            var bk = new BookCardVm();
            var fsItem = PartitionPanel.SelectedItem;
            bk.FilePath = fsItem.FullPath;

            bk.SelectedCategory = _libraryNavigator.Category;
            bk.SelectedLanguage = _libraryNavigator.Language;
            return bk;
        }

        private bool CanCopyOrMoveFile()
        {
            return IsPartitionPanel &&
                   PartitionPanel.SelectedItem != null &&
                   false == PartitionPanel.SelectedItem.IsDirectory;
        }

        #endregion


        public ICommand PickCmd { get; private set; }

        private void Pick()
        {
            var navigator = NavigationPanels[_selectedPanel];
            navigator.PickItemCmd.Execute(navigator.SelectedItem);
        }

        private bool CanPick()
        {
            if (_selectedPanel < 0)
                return false;
            return NavigationPanels[_selectedPanel].SelectedItem != null;
        }


        public ICommand SearchCmd { get; private set; }

        private void Search()
        {
            if (_searchActive)
            {
                _searchActive = false;
                SetLibraryNavigator(_libraryNavigator);
            }
            else
            {
                var bk = new BookCardVm();
                bk.SearchMode = true;

                var cardWindow = Dialogs.Get("bookCard");

                if (cardWindow.ShowDialog(bk) != true)
                    return;

                _searchNavigator.SearchTemplate = bk.GetBook();

                _searchActive = true;
                SetLibraryNavigator(_searchNavigator);
            }
        }

        private bool CanSearch()
        {
            return IsLibraryPanel;
        }
    }
}
