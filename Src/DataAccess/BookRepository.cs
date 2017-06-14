using System;
using System.Collections.Generic;
using System.Linq;
using DataSchema;

namespace DataAccess
{
    public class BookRepository: EntityRepository<Book>
    {
        public override IEnumerable<Book> Load(object template)
        {
            var book = template as Book;
            if (book == null)
                return Query().ToList();

            IQueryable<Book> results = Query();

            if (book.Category != null)
                results = results.Where(b => b.Category.Id == book.Category.Id);

            if (book.Language != null)
                results = results.Where(b => b.Language.Id == book.Language.Id);

            if (book.Cycle != null)
                results = results.Where(b => b.Cycle.Id == book.Cycle.Id);

            if (String.IsNullOrWhiteSpace(book.Title) == false)
                results = results.Where(b => b.Title.Contains(book.Title));

            if (book.Tags.Count > 0)
                foreach (var tag in book.Tags)                
                    results = results.Where(b => b.Tags.Any(t => t.Id == tag.Id));

            if (book.Authors.Count > 0)
                foreach (var author in book.Authors)
                    results = results.Where(b => b.Authors.Any(a => a.Id == author.Id));

            return results.ToList();
        }
    }
}
