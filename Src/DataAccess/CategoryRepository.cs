using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataSchema;

namespace DataAccess
{
    public class CategoryRepository: EntityRepository<Category>
    {
        private static List<Category> _categories;

        private List<Category> Categories
        {
            get
            {
                if (_categories == null)
                    _categories = base.Load().ToList();
                return _categories;
            }
        }        

        public override IEnumerable<Category> Load()
        {
            return Categories;
        }
    }
}
