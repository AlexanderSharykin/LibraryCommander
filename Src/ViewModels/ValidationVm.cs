using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class ValidationVm: ObservableVm,  INotifyDataErrorInfo
    {
        public virtual IEnumerable GetErrors(string propertyName = null)
        {
            return Enumerable.Empty<DictionaryEntry>();
        }

        public bool HasErrors
        {
            get { return GetErrors().OfType<object>().Any(); }
        }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        protected virtual void OnErrorsChanged(DataErrorsChangedEventArgs e)
        {
            if (ErrorsChanged != null)
            {
                ErrorsChanged(this, e);
            }
        }

        protected virtual void OnErrorsChanged([CallerMemberName] string propertyName = null)
        {
            OnErrorsChanged(new DataErrorsChangedEventArgs(propertyName));
        }
    }
}
