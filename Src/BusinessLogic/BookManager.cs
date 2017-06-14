using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DataSchema;

namespace BusinessLogic
{
    public class BookManager: Manager<Book>
    {        
        private StorageManager _storageManager;

        private StorageManager StorageManager
        {
            get
            {
                if (_storageManager == null)
                    _storageManager = new StorageManager();
                return _storageManager;
            }            
        }

        private bool Verify(Book book)
        {
            if (book == null)
            {
                Message = Localization.Instance.NullError;
                return false;
            }

            if (string.IsNullOrWhiteSpace(book.Title))
            {
                Message = Localization.Instance.TitleError;
                return false;
            }

            if (book.Category == null)
            {
                Message = Localization.Instance.CategoryError;
                return false;
            }

            if (book.Language == null)
            {
                Message = Localization.Instance.LanguageError;
                return false;
            }

            if (book.Authors.Count == 0)
            {
                Message = Localization.Instance.AuthorsError;
                return false;
            }

            if (book.Formats.Count == 0)
            {
                Message = Localization.Instance.FormatError;
                return false;
            }
            return true;
        }

        public override bool ValidateOnAdd(Book book)
        {
            return Verify(book);
        }

        public bool TryAdd(Book book, string filename, bool removeSource)
        {
            Operation = Localization.Instance.AddOperation;

            if (ValidateOnAdd(book) == false)
                return false;

            if (StorageManager.TryAddBook(book, filename) == false)
            {
                Message = StorageManager.Message;
                return false;
            }
            
            Repository.Save(book);
            
            if (removeSource)
            {
                File.Delete(filename);
            }

            return true;
        }

        public override bool ValidateOnUpdate(Book book)
        {
            return Verify(book);
        }

        public bool TryUpdate(Book book, string filename)
        {
            if (StorageManager.TryUpdateBook(book, filename) == false)
            {
                Message = StorageManager.Message;
                return false;
            }
            return base.TryUpdate(book);
        }

        public override bool ValidateOnDelete(Book book)
        {
            if (book == null)
            {
                Message = Localization.Instance.NullError;
                return false;
            }
            return true;
        }

        public override bool TryDelete(Book book)
        {
            if (StorageManager.TryDelete(book) == false)
            {
                Message = StorageManager.Message;
                return false;
            }
            return base.TryDelete(book);
        }
    }
}
