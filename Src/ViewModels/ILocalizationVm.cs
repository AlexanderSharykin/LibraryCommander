namespace ViewModels
{
    /// <summary>
    /// Localization contract for view models
    /// </summary>
    public interface ILocalizationVm
    {
        string DeleteBookCaption { get; }
        
        string DeleteBookQuestion { get; }

        string DeleteItemQuestion { get; }

        string FileTitle { get; }
        string AuthorsTitle { get; }
        string CategoryTitle { get; }
        string TagsTitle { get; }
        string LanguageTitle { get; }
        string FormatTitle { get; }
        string CycleTitle { get; }

        string TitleError { get; }
        string LanguageError { get; }
        string FormatError { get; }
        string AuthorsError { get; }
        string TagsError { get; }
        string FilePathError { get; }
        string CategoryError { get; }

        string ValueError { get; }

        string Library { get; }
        string FileNotFound { get; }
        string FolderNotFound { get; }

        string DbConnectionError { get; }
        string SQLiteError { get; }
        string StorageError { get; }
        string LogRequest { get; }
    }
}
