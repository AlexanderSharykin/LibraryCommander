using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryCommander.Localization
{
    /// <summary>
    /// Translations for command buttons used in main View
    /// </summary>
    public class Commands: LocalizationProvider
    {
        private static Commands _instance = new Commands();

        private Commands()
        {            
        }

        /// <summary>
        /// Static item accessible from view (via {x:Static} extension)
        /// </summary>
        public static Commands Instance { get { return _instance; } }

        public string Cmd { get { return GetResource(); } }

        public string Pick { get { return GetResource(); } }

        public string Add { get { return GetResource(); } }

        public string Edit { get { return GetResource(); } }

        public string Copy { get { return GetResource(); } }

        public string Move { get { return GetResource(); } }

        public string Del { get { return GetResource(); } }

        public string Quit { get { return GetResource(); } }

        public string Search { get { return GetResource(); } }

        public string Save { get { return GetResource(); } }

        public string Close { get { return GetResource(); } }
    }
}
