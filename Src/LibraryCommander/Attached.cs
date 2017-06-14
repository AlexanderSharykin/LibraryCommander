using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace LibraryCommander
{
    /// <summary>
    /// Common attached properties
    /// </summary>
    public class Attached
    {
        #region IsFocused

        // source
        // Q
        // http://stackoverflow.com/questions/1356045/set-focus-on-textbox-in-wpf-from-view-model-c
        // by priyanka.sarkar
        // A
        // http://stackoverflow.com/a/1356781/1506454
        // by Anvaka

        public static readonly DependencyProperty IsFocusedProperty = DependencyProperty.RegisterAttached("IsFocused", typeof(bool), typeof(Attached),
            new UIPropertyMetadata(false, OnFocusedPropertyChanged));

        public static bool GetIsFocused(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsFocusedProperty);
        }

        public static void SetIsFocused(DependencyObject obj, bool value)
        {
            obj.SetValue(IsFocusedProperty, value);
        }

        private static void OnFocusedPropertyChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var uie = (UIElement)d;
            if ((bool)e.NewValue)
            {
                Keyboard.Focus(uie);
                uie.Focus();
            }
        }

        #endregion
    }
}
