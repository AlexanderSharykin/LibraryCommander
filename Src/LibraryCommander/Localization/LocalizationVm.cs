using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModels;

namespace LibraryCommander.Localization
{
    /// <summary>
    /// Translations for messages used in View-Model classes
    /// </summary>
    public class LocalizationVm: LocalizationProvider, ILocalizationVm
    {
        public string DeleteBookCaption { get { return GetResource(); } }

        public string DeleteBookQuestion { get { return GetResource(); } }

        public string DeleteItemQuestion { get { return GetResource(); } }

        public string FileTitle { get { return GetResource(); } }
        public string AuthorsTitle { get { return GetResource(); } }
        public string CategoryTitle { get { return GetResource(); } }
        public string TagsTitle { get { return GetResource(); } }
        public string LanguageTitle { get { return GetResource(); } }
        public string FormatTitle { get { return GetResource(); } }
        public string CycleTitle { get { return GetResource(); } }

        public string TitleError { get { return GetResource(); } }
        public string LanguageError { get { return GetResource(); } }
        public string FormatError { get { return GetResource(); } }
        public string AuthorsError { get { return GetResource(); } }
        public string TagsError { get { return GetResource(); } }
        public string FilePathError { get { return GetResource(); } }
        public string CategoryError { get { return GetResource(); } }

        public string ValueError { get { return GetResource(); } }

        public string Library { get { return GetResource(); } }

        public string FileNotFound { get { return GetResource(); } }
        public string FolderNotFound { get { return GetResource(); } }
    }
}
