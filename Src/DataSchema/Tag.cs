using System;

namespace DataSchema
{
    public class Tag : Entity, INamedEntity
    {
        private Category _category;

        public Category Category
        {
            get { return _category; }
            set { _category = value; }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
    }
}
