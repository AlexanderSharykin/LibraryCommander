using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataSchema;

namespace BusinessLogic
{
    public class FileLanguageManager: Manager<FileLanguage>
    {
        protected List<FileLanguage> Languages
        {
            get { return (List<FileLanguage>)Repository.Load(); }
        }

        public override IList<FileLanguage> GetAll()
        {
            return Languages;
        }

        public override bool ValidateOnAdd(FileLanguage language)
        {
            if (language == null)
            {
                Message = Localization.Instance.NullError;
                return false;
            }

            if (string.IsNullOrWhiteSpace(language.Name))
            {
                Message = Localization.Instance.NameError;
                return false;
            }
            
            if (Languages.Any(f => f.Name == language.Name && f.ShortName == language.ShortName))
            {
                Message = Localization.Instance.DuplicateError;
                return false;
            }

            return true;
        }

        public override bool TryAdd(FileLanguage language)
        {
            bool success = base.TryAdd(language);
            if (success)
                Languages.Add(language);
            return success;
        }

        public override bool ValidateOnUpdate(FileLanguage language)
        {
            if (language.Id <= 0)
            {
                Message = Localization.Instance.EntityError;
                return false;
            }
            
            if (Languages.Any(f => f.Id != language.Id && f.Name == language.Name && f.ShortName == language.ShortName))
            {
                Message = Localization.Instance.DuplicateError;
                return false;
            }

            int index = Languages.FindIndex(f => f.Id == language.Id);
            if (index >= 0 && Languages[index].ShortName != language.ShortName)
            {
                var categories = new UsageValidator().GetLanguageUsages(language);
                if (categories != null && categories.Count > 0)
                {                    
                    // rename Language folder in all Category folders
                    for (int num = 0; num < categories.Count; num++)
                    {
                        var storage = new StorageManager();
                        var res = storage.TryUpdateLanguageFolder(categories[num], Languages[index], language);
                        if (res == false)
                        {
                            Message = storage.Message;                            
                            // rollback Language folder name in all Category folders
                            for (int n = 0; n < num; n++)
                                storage.TryUpdateLanguageFolder(categories[n], language, Languages[index]);
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        public override bool TryUpdate(FileLanguage language)
        {
            bool success = base.TryUpdate(language);
            if (success)
            {
                int index = Languages.FindIndex(f => f.Id == language.Id);
                if (index < 0)
                    Languages.Add(language);
                else
                    Languages[index].RestoreFrom(language);
            }
            return success;
        }

        public override bool ValidateOnDelete(FileLanguage language)
        {
            if (language == null)
            {
                Message = Localization.Instance.NullError;
                return false;
            }

            if (new UsageValidator().IsLanguageUsed(language))
            {
                Message = String.Format(Localization.Instance.LanguageUsageWarning, language.Name);
                return false;
            }
            Repository.Delete(language);
            
            return true;
        }

        public override bool TryDelete(FileLanguage language)
        {
            bool success = base.TryDelete(language);
            if (success)
                Languages.Remove(language);
            return success;
        }
    }
}
