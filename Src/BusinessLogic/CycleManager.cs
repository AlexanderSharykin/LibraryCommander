using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataSchema;

namespace BusinessLogic
{
    public class CycleManager: Manager<Cycle>
    {
        public Category Category { get; set; }

        public override IList<Cycle> GetAll()
        {
            return Repository.Query()
                .Where(t => Category == null || t.Category.Id == Category.Id)
                .ToList();
        }

        public override bool TryAdd(Cycle item)
        {
            if (item.Category == null)
                item.Category = Category;
            return base.TryAdd(item);
        }

        public override bool ValidateOnAdd(Cycle cycle)
        {
            if (Repository.Query().Any(t => t.Category.Id == cycle.Category.Id && t.Name == cycle.Name))
            {
                Message = Localization.Instance.DuplicateError;
                return false;
            }
            return true;
        }

        public override bool ValidateOnUpdate(Cycle cycle)
        {
            if (cycle.Id <= 0)
            {
                Message = Localization.Instance.EntityError;
                return false;
            }

            if (Repository.Query().Any(t => t.Id != cycle.Id && t.Category.Id == cycle.Category.Id && t.Name == cycle.Name))
            {
                Message = Localization.Instance.DuplicateError;
                return false;
            }
            
            return true;
        }

        public override bool ValidateOnDelete(Cycle cycle)
        {
            if (cycle == null)
            {
                Message = Localization.Instance.NullError;
                return false;
            }

            return true;
        }
    }
}
