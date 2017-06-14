using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataSchema;

namespace BusinessLogic
{
    public class UsageValidator
    {
        public static IRepository<Book> Repository { get; set; }

        public bool IsCategoryUsed(Category category)
        {
            if (category == null)
                return false;
            return Repository.Query().Any(b => b.Category != null && b.Category.Id == category.Id);
        }

        public bool IsLanguageUsed(FileLanguage language)
        {
            if (language == null)
                return false;
            return Repository.Query().Any(b => b.Language != null && b.Language.Id == language.Id);
        }

        public IList<Category> GetLanguageUsages(FileLanguage language)
        {
            if (language == null)
                return null;
            return Repository.Query()
                .Where(b => b.Language != null && b.Language.Id == language.Id)
                .Select(b => b.Category)
                .Distinct()
                .ToList();
        }

        public bool IsFormatUsed(FileFormat format)
        {
            if (format == null)
                return false;
            return Repository.Query().Any(b => b.Formats.Any(f => f.Id == format.Id));
        }

        public bool IsAuthorUsed(Author author)
        {
            if (author == null)
                return false;
            return Repository.Query().Any(b => b.Authors.Any(f => f.Id == author.Id));
        }
    }
}
