using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataSchema;

namespace BusinessLogic
{
    public class CategoryManager: Manager<Category>
    {
        private StorageManager _storageManager;

        private StorageManager Storage
        {
            get
            {
                if (_storageManager == null)
                    _storageManager = new StorageManager();
                return _storageManager;
            }            
        }

        protected List<Category> Categories
        {
            get { return (List<Category>)Repository.Load(); }
        }

        public override IList<Category> GetAll()
        {
            return Categories;
        }

        public override bool ValidateOnAdd(Category category)
        {
            if (Categories.Any(c => c.Name == category.Name))
            {
                Message = Localization.Instance.DuplicateError;
                return false;
            }
            return true;
        }

        public override bool TryAdd(Category category)
        {
            if (Storage.TryAddCategoryFolder(category) == false)
            {
                Message = Storage.Message;
                Operation = Storage.Operation;
                return false;
            }

            bool success = base.TryAdd(category);
            if (success)
                Categories.Add(category);
            return success;
        }

        public override bool ValidateOnUpdate(Category category)
        {
            if (category.Id <= 0)
            {
                Message = Localization.Instance.EntityError;
                return false;
            }

            if (Categories.Any(c => c.Id != category.Id && c.Name == category.Name))
            {
                Message = Localization.Instance.DuplicateError;
                return false;
            }
            
            return true;
        }

        public override bool TryUpdate(Category category)
        {            
            if (category.Id > 0)
            {
                Category copy = Repository.Load(category.Id);
                if (Storage.TryUpdateCategoryFolder(category, copy) == false)
                {
                    Message = Storage.Message;
                    Operation = Storage.Operation;
                    return false;
                }
            }

            bool success = base.TryUpdate(category);
            if (success)
            {
                int index = Categories.FindIndex(c => c.Id == category.Id);
                if (index < 0)
                    Categories.Add(category);
                else
                    Categories[index].RestoreFrom(category);
            }
            return success;
        }

        public override bool ValidateOnDelete(Category category)
        {
            if (category == null)
            {
                Message = Localization.Instance.NullError;
                return false;
            }

            if (new UsageValidator().IsCategoryUsed(category))
            {
                Message = string.Format(Localization.Instance.CategoryUsageWarning, category.Name);
                return false;
            }
            return true;
        }

        public override bool TryDelete(Category category)
        {
            bool success = base.TryDelete(category);

            if (success)
                if (Storage.TryRemoveCategoryFolder(category) == false)
                {
                    Message = Storage.Message;
                    return false;
                }

            if (success)
                Categories.Remove(category);
            return success;
        }
    }
}
