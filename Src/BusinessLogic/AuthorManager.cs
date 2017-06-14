using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataSchema;

namespace BusinessLogic
{
    public class AuthorManager: Manager<Author>
    {
        public override bool ValidateOnAdd(Author item)
        {
            if (item == null)
            {
                Message = Localization.Instance.NullError;
                return false;
            }

            if (string.IsNullOrWhiteSpace(item.Name))
            {
                Message = Localization.Instance.NameError;
                return false;
            }
            return true;
        }

        public override bool ValidateOnUpdate(Author item)
        {
            if (Repository.Query().Any(a => a.Id != item.Id && a.Name == item.Name))
            {
                Message = Localization.Instance.DuplicateError;
                return false;
            }
            return true;
        }

        public override bool ValidateOnDelete(Author item)
        {
            if (new UsageValidator().IsAuthorUsed(item))
            {
                Message = string.Format(Localization.Instance.AuthorUsageWarning, item.Name);
                return false;
            }
            return true;
        }
    }
}
