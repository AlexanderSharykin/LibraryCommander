using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels.Dialogs
{
    public class InputVm: ValidationVm
    {
        /// <summary>
        /// Gets or sets input hint text
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets input box title
        /// </summary>
        public string Caption { get; set; }

        public string Input { get; set; }

        public override IEnumerable GetErrors(string propertyName = null)
        {
            string key = "Input";
            if ((propertyName ?? key) == key && String.IsNullOrWhiteSpace(Input))
                yield return new DictionaryEntry(key, Injector.Localization.ValueError);
        }
    }
}
