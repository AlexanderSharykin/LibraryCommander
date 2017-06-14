using System;

namespace DataSchema
{
    public class FileLanguage : Entity, INamedEntity
    {
        private string _name;
        private string _shortName;

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                if (String.IsNullOrWhiteSpace(ShortName))
                    ShortName = _name;
            }
        }
        
        public string ShortName
        {
            get { return _shortName; }
            set { _shortName = value; }
        }

        public void RestoreFrom(FileLanguage original)
        {
            if (original == null)
                throw new ArgumentNullException();
            Id = original.Id;
            _name = original.Name;
            _shortName = original.ShortName;
        }
    }
}
