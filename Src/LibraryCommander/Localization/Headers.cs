using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryCommander.Localization
{
    /// <summary>
    /// Translations for captions used in UI Views
    /// </summary>
    public class Headers: LocalizationProvider
    {
        private static Headers _instance = new Headers();
        
        private Headers()
        {            
        }

        /// <summary>
        /// Static item accessible from all views (via {x:Static} extension)
        /// </summary>
        public static Headers Instance
        {
            get { return _instance; }
        }

        public string Name { get { return GetResource(); } }

        public string Size { get { return GetResource(); } }

        public string MainWindow { get { return GetResource(); } }

        public string CardWindow { get { return GetResource(); } }

        public string Title { get { return GetResource(); } }

        public string Authors { get { return GetResource(); } }
        
        public string Category { get { return GetResource(); } }
        
        public string Tags { get { return GetResource(); } }

        public string Cycle { get { return GetResource(); } }

        public string Language { get { return GetResource(); } }

        public string Format { get { return GetResource(); } }

        public string Year { get { return GetResource(); } }

        public string Volume { get { return GetResource(); } }

        public string File { get { return GetResource(); } }
    }
}
