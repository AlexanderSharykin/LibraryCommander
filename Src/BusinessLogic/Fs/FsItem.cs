using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BusinessLogic.Fs
{
    public class FsItem
    {
        private string _extension;

        public FsItem()
        {
            IsDirectory = true;
        }

        public string Name { get; set; }

        public string FullPath { get; set; }

        public bool IsMissing { get; set; }

        public long? Length { get; set; }

        public double? Size
        {
            get
            {
                if (Length.HasValue)
                    return Length/1024;
                return null;
            }
        }

        public bool IsDirectory { get; set; }

        public string Extension
        {
            get
            {
                if (false == IsDirectory && _extension == null)
                    _extension = Path.GetExtension(Name);
                return _extension;
            }
        }

        private static FsItem _gotoParent = new FsItem { Name = "." };
        public static FsItem GotoParent
        {
            get { return _gotoParent; }            
        }

        public override string ToString()
        {
            return FullPath ?? Name;
        }
    }
}
