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

namespace LibraryCommander.Dialogs
{
    /// <summary>
    /// Interaction logic for InputBox.xaml
    /// </summary>
    public partial class InputBox : Window, IVisualDialog
    {
        public InputBox()
        {
            SaveCmd = new RelayCommand(Save);
            CloseCmd = new RelayCommand(CloseDialog);
            InitializeComponent();
        }

        public bool? ShowDialog(object dataContext)
        {
            DataContext = dataContext;
            return ShowDialog();
        }

        public ICommand SaveCmd { get; private set; }
        public ICommand CloseCmd { get; private set; }

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

        private void InputBox_OnLoaded(object sender, RoutedEventArgs e)
        {
            TxtInput.Focus();
        }
    }
}
