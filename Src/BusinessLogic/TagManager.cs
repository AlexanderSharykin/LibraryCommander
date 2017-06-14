using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataSchema;

namespace BusinessLogic
{
    public class TagManager: Manager<Tag>
    {
        public Category Category { get; set; }

        public override IList<Tag> GetAll()
        {
            if (Category == null)
                return Repository.Query().ToList();
            return Repository.Query()
                .Where(t => t.Category.Id == Category.Id)
                .ToList();
        }

        public override bool TryAdd(Tag item)
        {
            if (item.Category == null)
                item.Category = Category;
            return base.TryAdd(item);
        }

        public override bool ValidateOnAdd(Tag tag)
        {
            if (tag == null)
            {
                Message = Localization.Instance.NullError;
                return false;
            }

            if (string.IsNullOrWhiteSpace(tag.Name))
            {
                Message = Localization.Instance.NameError;
                return false;
            }
            return true;
        }

        public override bool ValidateOnUpdate(Tag tag)
        {
            if (Repository.Query().Any(t => t.Id != tag.Id && t.Category.Id == tag.Category.Id && t.Name == tag.Name))
            {
                Message = Localization.Instance.DuplicateError;
                return false;
            }
            return true;
        }

        public override bool ValidateOnDelete(Tag tag)
        {
            // validate usage in Book.Tags

            return true;
        }
    }
}
