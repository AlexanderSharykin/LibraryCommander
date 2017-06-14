using System;

namespace DataSchema
{
    public class FileFormat : Entity, INamedEntity
    {
        private string _extension;        

        public string Extension
        {
            get { return _extension; }
            set { _extension = value; }
        }

        public string Name
        {
            get { return Extension; }
            set { Extension = value; }
        }

        public void RestoreFrom(FileFormat original)
        {
            if (original == null)
                throw new ArgumentNullException();
            Id = original.Id;
            _extension = original.Extension;
        }

        public override string ToString()
        {
            return "." + _extension;
        }
    }
}
