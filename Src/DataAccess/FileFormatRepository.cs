using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataSchema;

namespace DataAccess
{
    public class FileFormatRepository: EntityRepository<FileFormat>
    {
        private static List<FileFormat> _formats;

        private List<FileFormat> Formats
        {
            get
            {
                if (_formats == null)
                    _formats = base.Load().ToList();
                return _formats;
            }
        }

        public override IEnumerable<FileFormat> Load()
        {
            return Formats;
        }
    }
}
