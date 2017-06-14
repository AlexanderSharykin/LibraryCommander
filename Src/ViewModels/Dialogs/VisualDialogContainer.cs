using System;
using System.Collections;
using System.Collections.Generic;

namespace ViewModels.Dialogs
{
    /// <summary>
    /// Provides concrete implementation of a VisualDialog for registered keys
    /// </summary>
    public class VisualDialogContainer
    {
        private readonly Hashtable _dialogTypes = new Hashtable();
        private readonly Hashtable _initializers = new Hashtable();

        /// <summary>
        /// Gets a VisualDialog for the registered key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public IVisualDialog Get(string key)
        {
            var type = (Type)_dialogTypes[key];
            if (type == null)
                throw new KeyNotFoundException("Requested key is not registered");

            var dialog = (IVisualDialog)Activator.CreateInstance(type);

            var initAction = (Action<IVisualDialog>)_initializers[key];
            if (initAction != null)
                initAction(dialog);
            return dialog;
        }

        /// <summary>
        /// Sets a VisualDialog type for the key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        public void Set<T>(string key) where T: IVisualDialog, new()
        {
            Set<T>(key, null);            
        }

        public void Set<T>(string key, Action<T> initilizer) where T : IVisualDialog, new()
        {
            _dialogTypes[key] = typeof(T);
            if (initilizer != null)
            {
                Action<IVisualDialog> initAction = (IVisualDialog d) => initilizer((T) d);
                _initializers[key] = initAction;
            }
        }
    }
}
