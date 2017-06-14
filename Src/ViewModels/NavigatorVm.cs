using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using BusinessLogic;
using BusinessLogic.Fs;
using MvvmFoundation.Wpf;
using ViewModels.Dialogs;

namespace ViewModels
{
    /// <summary>
    /// Wrapper for FsNavigator with notifications
    /// </summary>
    public class NavigatorVm: ObservableVm
    {
        private FsNavigator _navigator;
        private ICommand _pickItemCmd;               
        private ObservableCollection<FsItem> _content;
        private FsItem _selectedItem;

        public NavigatorVm(FsNavigator navigator)
        {
            _navigator = navigator;
            GetContent();
        }

        public bool ShowItemDetails { get; set; }

        public string CurrentFolder
        {
            get
            {
                return _navigator.CurrentFolder;
            }
        }

        public FsItem SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                OnPropertyChanged();
            }
        }

        public IList<FsItem> Segments
        {
            get { return _navigator.Segments; }
        }

        public bool CanNavigateToParentFolder
        {
            get
            {                
                return _navigator.CanNavigateToParentFolder;
            }
        }

        public ObservableCollection<FsItem> Content
        {
            get { return _content; }
            set
            {
                _content = value;
                OnPropertyChanged();
            }
        }

        private void GetContent()
        {
            Content = new ObservableCollection<FsItem>(_navigator.GetFolders().Concat(_navigator.GetFiles()));
            SelectedItem = Content.FirstOrDefault();
        }

        public ICommand PickItemCmd
        {
            get
            {
                if (_pickItemCmd == null)
                    _pickItemCmd = new RelayCommand<object>(PickItem, CanPickItem);
                return _pickItemCmd;
            }
        }

        private void PickItem(object item)
        {
            var fsItem = item as FsItem;

            if (fsItem == null)
                return;

            if (fsItem.IsMissing)
            {
                var msg = new MessageVm
                          {
                              Caption = Injector.Localization.Library,                                  
                              Text = fsItem.IsDirectory
                                  ? Injector.Localization.FolderNotFound
                                  : Injector.Localization.FileNotFound,
                          };
                var w = Injector.Dialogs.Get("messageBox");
                w.ShowDialog(msg);
                return;
            }

            bool navigate = _navigator.PickItem(fsItem);
            if (false == navigate)
                return;

            OnPropertyChanged("CurrentFolder");
            OnPropertyChanged("Segments");
            OnPropertyChanged("CanNavigateToParentFolder");
            GetContent();
        }

        public void UpdateSegments()
        {
            _navigator.UpdateSegments();
            OnPropertyChanged("Segments");
        }

        private bool CanPickItem(object item)
        {
            return item != null;
        }
    }
}
