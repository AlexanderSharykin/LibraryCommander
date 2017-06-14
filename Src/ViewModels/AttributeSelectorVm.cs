using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Input;
using BusinessLogic;
using DataSchema;
using MvvmFoundation.Wpf;
using ViewModels.Dialogs;

namespace ViewModels
{
    /// <summary>
    /// Class provides funtionality to select, add, edit, delete items of collection
    /// </summary>
    public class AttributeSelectorVm: ObservableVm
    {
        private bool _multiSelect;
        private ObservableCollection<SelectionItem> _items;
        private string _searchPattern;
        private SelectionItem _selectedItem;

        public AttributeSelectorVm()
        {
            AddItemCmd = new RelayCommand(AddItem);
            EditItemCmd = new RelayCommand(EditItem);
            DeleteItemCmd = new RelayCommand(DeleteItem);
        }

        public string Title { get; set; }

        public bool MultiSelect
        {
            get { return _multiSelect; }
            set
            {
                _multiSelect = value;
                OnPropertyChanged();
            }
        }

        public SelectionItem SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<SelectionItem> Items
        {
            get { return _items; }
            set
            {
                _items = value;
                OnPropertyChanged();
            }
        }

        public IEnumerable<SelectionItem> SelectedItems
        {
            get
            {
                return Items.Where(x => x.IsSelected);
            }
        }

        public string SearchPattern
        {
            get { return _searchPattern; }
            set
            {
                _searchPattern = value;
                OnPropertyChanged();
                ICollectionView cv = CollectionViewSource.GetDefaultView(Items);
                if (String.IsNullOrWhiteSpace(_searchPattern))
                    cv.Filter = null;
                else
                {
                    _searchPattern = _searchPattern.ToLower();
                    cv.Filter = o => ((SelectionItem) o).Description.ToLower().Contains(_searchPattern);
                }
            }
        }

        public ICommand AddItemCmd { get; private set; }

        protected virtual void AddItem() { Debug.WriteLine("Add Item");}
        

        public ICommand EditItemCmd { get; private set; }

        protected virtual void EditItem() { Debug.WriteLine("Edit Item"); }


        public ICommand DeleteItemCmd { get; private set; }

        protected virtual void DeleteItem() { Debug.WriteLine("Delete Item"); }
    }

    /// <summary>
    /// Generic version of AttributeSelectorVm where items collection is obtained from specialized Manager
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AttributeSelectorVm<T> : AttributeSelectorVm where T : Entity, INamedEntity, new()
    {
        private Manager<T> _manager;        

        public AttributeSelectorVm(Manager<T> manager)
        {
            _manager = manager;
            
            var items = _manager.GetAll().Select(x => new SelectionItem {Description = x.Name, Value = x});
            Items = new ObservableCollection<SelectionItem>(items);
        }

        public IEnumerable<T> SelectedValues
        {
            get
            {
                return SelectedItems.Select(x => x.Value).OfType<T>();
            }
        }

        protected override void AddItem()
        {
            // request new item
            var inputBox = Injector.Dialogs.Get("inputBox");
            var vm = new InputVm { Caption = Title };
            if (inputBox.ShowDialog(vm) != true)
                return;

            // add item to collection
            var item = new T { Name = vm.Input };
            if (_manager.TryAdd(item))
                Items.Add(new SelectionItem { Description = item.Name, Value = item});
            else            
                ShowManagerMessage();
        }

        protected override void EditItem()
        {
            if (SelectedItem == null)
                return;

            // request item edit
            var inputBox = Injector.Dialogs.Get("inputBox");
            var vm = new InputVm
                     {
                         Caption = Title, 
                         Input = SelectedItem.Description
                     };
            if (inputBox.ShowDialog(vm) != true)
                return;

            // update item in collection
            T item = (T) SelectedItem.Value;
            item.Name = vm.Input;
            if (_manager.TryUpdate(item))
                SelectedItem.Description = item.Name;
            else
                ShowManagerMessage();
        }

        protected override void DeleteItem()
        {
            if (SelectedItem == null)
                return;

            // confirm deletion
            var msgBox = Injector.Dialogs.Get("messageBox");
            var vm = new MessageVm
            {
                Caption = Title,
                Text = String.Format(Injector.Localization.DeleteItemQuestion, SelectedItem.Description),
                No = true
            };

            msgBox.ShowDialog(vm);

            if (vm.DialogResult.ToString() != "Yes")
                return;            

            // delete item from collection
            T item = (T)SelectedItem.Value;
            if (_manager.TryDelete(item))
                Items.Remove(SelectedItem);
            else           
                ShowManagerMessage();
            
        }

        /// <summary>
        /// Show dialog with message from Manager about last operation
        /// </summary>
        private void ShowManagerMessage()
        {
            var vm = new MessageVm
                     {
                         Caption = _manager.Operation,
                         Text = _manager.Message,
                     };
            Injector.Dialogs.Get("messageBox").ShowDialog(vm);
        }
    }
}
