namespace DataSchema
{
    public class Cycle : Entity, INamedEntity
    {
        private string _name;
        private Category _category;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public Category Category
        {
            get { return _category; }
            set { _category = value; }
        }
    }
}
