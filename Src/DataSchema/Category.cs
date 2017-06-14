using System;

namespace DataSchema
{
    public class Category : Entity, INamedEntity
    {
        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public void RestoreFrom(Category original)
        {
            if (original == null)
                throw new ArgumentNullException();
            Id = original.Id;
            _name = original.Name;
        }
    }
}
