using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MvvmFoundation.Wpf;
using ViewModels.Dialogs;

namespace LibraryCommander
{
    /// <summary>
    /// Interaction logic for SelectorWindow.xaml
    /// </summary>
    public partial class SelectorWindow : Window, IVisualDialog
    {
        public SelectorWindow()
        {
            SaveCmd = new RelayCommand(Save);
            SearchCmd = new RelayCommand(Search);
            CloseCmd = new RelayCommand(CloseDialog);
            InitializeComponent();
        }

        public bool? ShowDialog(object dataContext)
        {
            DataContext = dataContext;
            return ShowDialog();
        }

        public ICommand SearchCmd { get; private set; }
        public ICommand SaveCmd { get; private set; }
        public ICommand CloseCmd { get; private set; }

        private void Search()
        {
            TxtSearch.Focus();
        }

        private void Save()
        {
            DialogResult = true;
            Close();
        }

        private void CloseDialog()
        {
            DialogResult = false;
            Close();
        }

        private void SelectorWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (false == LstItems.HasItems)
                return;
            LstItems.SelectedIndex = 0;
            LstItems.Focus();
        }
    }
}
