namespace DataSchema
{
    public class AppSettings: Entity
    {
        private string _culture;
        private string _fsLocation;
        private Category _category;
        private FileLanguage _language;
        private int? _idAuthor;
        private int? _idCycle;
        private string _libraryLocation;

        public string Culture
        {
            get { return _culture; }
            set { _culture = value; }
        }

        public string FsLocation
        {
            get { return _fsLocation; }
            set { _fsLocation = value; }
        }

        public string LibraryLocation
        {
            get { return _libraryLocation; }
            set { _libraryLocation = value; }
        }

        public Category Category
        {
            get { return _category; }
            set { _category = value; }
        }

        public FileLanguage Language
        {
            get { return _language; }
            set { _language = value; }
        }

        public int? IdAuthor
        {
            get { return _idAuthor; }
            set { _idAuthor = value; }
        }

        public int? IdCycle
        {
            get { return _idCycle; }
            set { _idCycle = value; }
        }
    }
}
