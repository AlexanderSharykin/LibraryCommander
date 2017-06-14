using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataSchema;

namespace BusinessLogic.Fs
{
    public class VirtualFsItem: FsItem
    {
        public VirtualFsItemLevel Level { get; set; }

        public Category Category { get; set; }

        public FileLanguage Language { get; set; }

        public int? IdAuthor { get; set; }

        public int? IdCycle { get; set; }

        public Book Book { get; set; }
    }
}
