using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ViewModels.Dialogs;

namespace LibraryCommander
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IVisualDialog
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SelectItemOnContentClick(object sender, MouseButtonEventArgs e)
        {
            var li = sender as ListBoxItem;
            if (li != null)
                li.IsSelected = true;            
        }

        public bool? ShowDialog(object dataContext)
        {
            DataContext = dataContext;
            return ShowDialog();
        }

        private void QuitClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
