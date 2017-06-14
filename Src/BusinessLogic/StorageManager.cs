using System;
using System.IO;
using System.Linq;
using DataSchema;

namespace BusinessLogic
{
    /// <summary>
    /// Class performs operations with files and folders in library storage
    /// </summary>
    public class StorageManager: Manager<Book>
    {
        public static string StoragePath { get; set; }

        private string GetPath(Book book)
        {
            return Path.Combine(StoragePath, book.GetPath());
        }

        public override bool ValidateOnAdd(Book book)
        {
            return true;
        }

        public bool TryAddBook(Book book, string originalPath)
        {
            Operation = Localization.Instance.FileSave;
            try
            {
                TryCreateBookStorageFilePath(book);

                File.Copy(originalPath, GetPath(book));
                return true;
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                return false;
            }
        }

        public override bool ValidateOnUpdate(Book book)
        {
            return true;
        }

        public bool TryUpdateBook(Book book, string originalPath)
        {
            Operation = Localization.Instance.FileMove;
            try
            {
                if (File.Exists(originalPath) == false)
                {
                    Message = Localization.Instance.FileNotFound;
                    return false;
                }
                var newPath = GetPath(book);
                if (newPath == originalPath)
                    return true;

                TryCreateBookStorageFilePath(book);
                File.Move(originalPath, newPath);
                return true;
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                return false;
            }
        }

        public override bool ValidateOnDelete(Book book)
        {
            var path = GetPath(book);
            if (false == File.Exists(path))
            {
                Message = Localization.Instance.FileNotFound;
                return false;
            }
            return true;
        }

        public override bool TryDelete(Book book)
        {
            Operation = Localization.Instance.FileDelete;
            try
            {
                string originalPath = GetPath(book);
                if (File.Exists(originalPath) == false)
                {
                    Message = Localization.Instance.FileNotFound;
                    return false;
                }
                File.Delete(originalPath);
                return true;
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                return false;
            }            
        }

        private string GetCategoryFolderPath(Category c)
        {
            return Path.Combine(StoragePath, c.Name);
        }

        private string GetLanguageSubdirectoryPath(Category category, FileLanguage language)
        {
            return Path.Combine(StoragePath, category.Name, language.ShortName);
        }

        private string GetAuthorSubdirectoryPath(Category category, FileLanguage language, Author author)
        {
            return Path.Combine(StoragePath, category.Name, language.ShortName, author.Name);
        }

        private string GetCycleSubdirectoryPath(Category category, FileLanguage language, Author author, Cycle cycle)
        {
            string folder;
            if (author != null)
                folder = GetAuthorSubdirectoryPath(category, language, author);
            else
                folder = GetLanguageSubdirectoryPath(category, language);
            return Path.Combine(folder, cycle.Name);
        }

        private void TryCreateBookStorageFilePath(Book book)
        {
            TryAddLanguageFolder(book.Category, book.Language);
            if (book.HasAuthorSubcatalog)
                TryAddAuthorFolder(book.Category, book.Language, book.Authors.First());

            if (book.HasCycleSubcatalog)
                TryAddCycleFolder(book.Category, book.Language,
                                  book.HasAuthorSubcatalog ? book.Authors.First() : null,
                                  book.Cycle);
        }

        public bool TryAddCategoryFolder(Category current)
        {
            string path = GetCategoryFolderPath(current);
            return CreateDirectory(path);
        }

        public bool TryUpdateCategoryFolder(Category current, Category original)
        {
            string originalPath = GetCategoryFolderPath(original);
            if (Directory.Exists(originalPath) == false)
            {
                Message = Localization.Instance.FolderNotFound;
                return false;
            }
            string path = GetCategoryFolderPath(current);
            return RenameDirectory(originalPath, path);
        }

        public bool TryRemoveCategoryFolder(Category current)
        {
            string path = GetCategoryFolderPath(current);
            return DeleteDirectory(path);
        }

        public bool TryAddLanguageFolder(Category category, FileLanguage language)
        {            
            string path = GetLanguageSubdirectoryPath(category, language);
            return CreateDirectory(path);
        }

        public bool TryUpdateLanguageFolder(Category category, FileLanguage original, FileLanguage current)
        {
            string originalPath = GetLanguageSubdirectoryPath(category, original);
            if (Directory.Exists(originalPath) == false)
            {
                Message = string.Format("[{0}]. {1}", category.Name, Localization.Instance.FolderNotFound);
                return false;
            }
            string path = GetLanguageSubdirectoryPath(category, current);
            return RenameDirectory(originalPath, path);
        }

        public bool TryRemoveLanguageFolder(Category category, FileLanguage language)
        {
            string path = GetLanguageSubdirectoryPath(category, language);
            return DeleteDirectory(path);
        }

        public bool TryAddAuthorFolder(Category category, FileLanguage language, Author author)
        {
            //TryAddCategoryFolder(category);
            //TryAddLanguageFolder(category, language);
            string path = GetAuthorSubdirectoryPath(category, language, author);
            return CreateDirectory(path);
        }

        public bool TryAddCycleFolder(Category category, FileLanguage language, Author author, Cycle cycle)
        {
            TryAddCategoryFolder(category);
            TryAddLanguageFolder(category, language);
            string path = GetCycleSubdirectoryPath(category, language, author, cycle);
            return CreateDirectory(path);
        }

        private bool CreateDirectory(string path)
        {
            if (Directory.Exists(path))
                return true;
            try
            {
                Directory.CreateDirectory(path);
                return true;
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                return false;
            }
        }

        private bool RenameDirectory(string originalPath, string path)
        {
            if (path == originalPath)
                return true;
            try
            {
                Directory.Move(originalPath, path);
                return true;
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                return false;
            }
        }

        private bool DeleteDirectory(string path)
        {
            if (Directory.Exists(path) == false)
            {
                Message = Localization.Instance.FolderNotFound;
                return false;
            }
            try
            {
                Directory.Delete(path, true);
                return true;
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                return false;
            }
        }
    }
}
