using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic;

namespace LibraryCommander.Localization
{
    /// <summary>
    /// Translations for messages used in Model classes
    /// </summary>
    public class LocalizationModel : LocalizationProvider, ILocalizationModel
    {
        public string Search { get { return GetResource(); } }

        public string Library { get { return GetResource(); } }

        public string AddOperation { get { return GetResource(); } }
        public string UpdateOperation { get { return GetResource(); } }
        public string DeleteOperation { get { return GetResource(); } }

        public string EntityError { get { return GetResource(); } }        
        public string NullError { get { return GetResource(); } }
        public string NameError { get { return GetResource(); } }
        public string DuplicateError { get { return GetResource(); } }

        public string FileNotFound { get { return GetResource(); } }
        public string FolderNotFound { get { return GetResource(); } }

        public string FileSave { get { return GetResource(); } }
        public string FileMove { get { return GetResource(); } }
        public string FileDelete { get { return GetResource(); } }

        public string TitleError { get { return GetResource(); } }
        public string LanguageError { get { return GetResource(); } }
        public string FormatError { get { return GetResource(); } }
        public string AuthorsError { get { return GetResource(); } }                
        public string CategoryError { get { return GetResource(); } }

        public string LanguageUsageWarning { get { return GetResource(); } }
        public string FormatUsageWarning { get { return GetResource(); } }
        public string CategoryUsageWarning { get { return GetResource(); } }
        public string AuthorUsageWarning { get { return GetResource(); } }
    }
}
