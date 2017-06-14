using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessLogic
{
    /// <summary>
    /// Localization contract for models
    /// </summary>
    public interface ILocalizationModel
    {
        string Search { get; }

        string Library { get; }

        string AddOperation { get; }
        string UpdateOperation { get; }
        string DeleteOperation { get; }

        string EntityError { get; }
        string NullError { get; }
        string NameError { get; }
        string DuplicateError { get; }

        string FileNotFound { get; }
        string FolderNotFound { get; }

        string FileSave { get; }
        string FileMove { get; }
        string FileDelete { get; }

        string TitleError { get; }
        string LanguageError { get; }
        string FormatError { get; }
        string AuthorsError { get; }
        string CategoryError { get; }

        string LanguageUsageWarning { get; }
        string FormatUsageWarning { get; }
        string CategoryUsageWarning { get; }
        string AuthorUsageWarning { get; }
    }

    public static class Localization
    {
        public static ILocalizationModel Instance { get; set; }
    }
}
