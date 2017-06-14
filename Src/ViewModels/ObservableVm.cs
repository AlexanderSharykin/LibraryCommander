using System.Runtime.CompilerServices;
using MvvmFoundation.Wpf;

namespace ViewModels
{
    public class ObservableVm: ObservableObject
    {
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            RaisePropertyChanged(propertyName);            
        }
    }
}
