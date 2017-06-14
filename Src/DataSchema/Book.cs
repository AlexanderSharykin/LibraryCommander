using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DataSchema
{
    public class Book : Entity, INamedEntity
    {
        private string _title;
        private Category _category;
        private ICollection<Author> _authors;
        private ICollection<Tag> _tags;
        private FileLanguage _language;
        private ICollection<FileFormat> _formats;
        private Cycle _cycle;
        private int? _volume;
        private int? _year;
        private bool _hasCycleSubcatalog;
        private bool _hasAuthorSubcatalog;

        public Book()
        {
            _authors = new HashSet<Author>();
            _tags = new HashSet<Tag>();
            _formats = new HashSet<FileFormat>();
        }
        
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        public string Name
        {
            get { return Title; }
            set { Title = value; }
        }

        public Category Category
        {
            get { return _category; }
            set { _category = value; }
        }

        public FileLanguage Language
        {
            get { return _language; }
            set { _language = value; }
        }
        
        public Cycle Cycle
        {
            get { return _cycle; }
            set { _cycle = value; }
        }

        public bool HasCycleSubcatalog
        {
            get { return _hasCycleSubcatalog; }
            set { _hasCycleSubcatalog = value; }
        }

        public int? Volume
        {
            get { return _volume; }
            set { _volume = value; }
        }
        
        public int? Year
        {
            get { return _year; }
            set { _year = value; }
        }
        
        public ICollection<Author> Authors
        {
            get { return _authors;  }
            private set { _authors = value; }
        }

        public bool HasAuthorSubcatalog
        {
            get { return _hasAuthorSubcatalog; }
            set { _hasAuthorSubcatalog = value; }
        }

        public ICollection<Tag> Tags
        {
            get { return _tags; }
            private set { _tags = value; }
        }

        public ICollection<FileFormat> Formats
        {
            get { return _formats; }
            private set { _formats = value; }
        }

        public string GetFileName()
        {
            var authors = Authors.Where(a => a.Name != "#");
            return (authors.Any()
                ? String.Format("{0}. {1}{2}",
                    String.Join(", ", Authors),
                    Volume.HasValue ? String.Format("{0} - ", Volume.Value) : "",
                    Title)
                : Title)
                   + Formats.First();
        }

        public string GetFolderName()
        {
            return Path.Combine(
                 Category != null ? Category.Name : string.Empty,
                 Language != null ? Language.Name : string.Empty,
                 HasAuthorSubcatalog && Authors.Count > 0 ? Authors.First().Name : string.Empty,
                 HasCycleSubcatalog && Cycle != null ? Cycle.Name : string.Empty
                );
        }

        public string GetPath()
        {
            return Path.Combine(GetFolderName(), GetFileName());
        }
    }
}
