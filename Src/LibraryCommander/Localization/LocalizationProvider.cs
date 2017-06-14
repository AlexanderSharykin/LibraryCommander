using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using LibraryCommander.Properties;
using ViewModels;

namespace LibraryCommander.Localization
{
    /// <summary>
    /// Class provides Resources-based localization with runtime-switch support
    /// </summary>
    public class LocalizationProvider: ObservableVm, IDisposable
    {
        #region Static

        private static CultureInfo _defaultCulture = new CultureInfo("en");
        private static CultureInfo _currentCulture;

        private static IList<LocalizationProvider> providers = new List<LocalizationProvider>();       

        public static CultureInfo CurrentCulture
        {
            get { return _currentCulture ?? _defaultCulture; }
            set
            {
                if (_currentCulture != null && _currentCulture.Name == value.Name)
                    return;

                _currentCulture = value;
                foreach(var p in providers)
                    p.Invalidate();
            }
        }

        public static CultureInfo DefaultCulture
        {
            get { return _defaultCulture; }
            set { _defaultCulture = value; }
        }

        #endregion

        public LocalizationProvider()
        {
            providers.Add(this);            
        }

        private Dictionary<string, string> _cache = new Dictionary<string, string>();

        protected string GetResource([CallerMemberName]string resourceKey = null)
        {
            string resource;
            // trying to get string from Cache            
            if (_cache.TryGetValue(resourceKey, out resource))
                return resource;

            // trying to get string from resources for Current culture
            resource = Resources.ResourceManager.GetString(resourceKey, CurrentCulture);

            if (resource == null && CurrentCulture.Name != DefaultCulture.Name)
                // trying to get string from resources for Default culture
                resource = Resources.ResourceManager.GetString(resourceKey, DefaultCulture);

            // if localized string was not found in Resources, use resourceKey
            // it helps to add less strings to En Resources (property names are in English)
            // for other culture it allows to notice mistake without throwing exception
            if (resource == null)
                resource = resourceKey;

            // add resolved string to cache
            _cache.Add(resourceKey, resource);

            return resource;
        }

        protected virtual void Invalidate()
        {
            _cache.Clear();
            OnPropertyChanged(String.Empty);
        }

        public void Dispose()
        {
            providers.Remove(this);
        }
    }
}
