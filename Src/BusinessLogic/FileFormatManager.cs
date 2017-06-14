using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataSchema;

namespace BusinessLogic
{
    public class FileFormatManager: Manager<FileFormat>
    {
        protected List<FileFormat> Formats
        {
            get { return Repository.Load().ToList(); }
        }

        public override IList<FileFormat> GetAll()
        {
            return Formats;
        }

        public override bool ValidateOnAdd(FileFormat format)
        {
            if (Formats.Any(f => f.Extension == format.Extension))
            {
                Message = Localization.Instance.DuplicateError;
                return false;
            }
            return true;
        }

        public override bool TryAdd(FileFormat format)
        {
            bool success = base.TryAdd(format);
            if (success)
                Formats.Add(format);
            return success;
        }

        public override bool ValidateOnUpdate(FileFormat format)
        {
            if (format.Id <= 0)
            {
                Message = Localization.Instance.EntityError;
                return false;
            }

            if (Formats.Any(f => f.Id != format.Id && f.Extension == format.Extension))
            {
                Message = Localization.Instance.DuplicateError;
                return false;
            }

            return true;
        }

        public override bool TryUpdate(FileFormat format)
        {
            bool success = base.TryUpdate(format);
            if (success)
            {
                int index = Formats.FindIndex(f => f.Id == format.Id);
                if (index < 0)
                    Formats.Add(format);
                else
                    Formats[index].RestoreFrom(format);
            }

            return success;
        }

        public override bool ValidateOnDelete(FileFormat format)
        {
            if (format == null)
            {
                Message = Localization.Instance.NullError;
                return false;
            }

            if (new UsageValidator().IsFormatUsed(format))
            {
                Message = string.Format(Localization.Instance.FormatUsageWarning, format.Extension);
                return false;
            }

            return true;
        }

        public override bool TryDelete(FileFormat format)
        {
            bool success = base.TryDelete(format);
            if (success)
                Formats.Remove(format);
            return success;
        }
    }
}
