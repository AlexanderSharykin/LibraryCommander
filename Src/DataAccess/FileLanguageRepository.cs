using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataSchema;

namespace DataAccess
{
    public class FileLanguageRepository: EntityRepository<FileLanguage>
    {
        private static List<FileLanguage> _languages;

        private List<FileLanguage> Languages
        {
            get
            {
                if (_languages == null)
                    _languages = base.Load().ToList();
                return _languages;
            }
        }

        public override IEnumerable<FileLanguage> Load()
        {
            return Languages;
        }
    }
}
