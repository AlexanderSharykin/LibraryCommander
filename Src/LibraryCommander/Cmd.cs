using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace LibraryCommander
{
    /// <summary>
    /// Class provides attached property to shorten Button hotkey declaration via KeyBinding
    /// </summary>
    public static class Cmd
    {
        public static readonly DependencyProperty HotkeyProperty = 
            DependencyProperty.RegisterAttached("Hotkey", typeof(string), typeof(Cmd), new PropertyMetadata(null, HotkeyChangedCallback));

        public static string GetHotkey(DependencyObject obj)
        {
            return (string)obj.GetValue(HotkeyProperty);
        }

        public static void SetHotkey(DependencyObject obj, string value)
        {
            obj.SetValue(HotkeyProperty, value);
        }

        private static readonly char _cmdJoinChar = '+';
        private static readonly char _cmdNameChar = '_';

        private static string NormalizeName(string name)
        {
            // + symbol in names is prohibited by NameScope (throws exception)
            return name.Replace(_cmdJoinChar, _cmdNameChar);
        }

        private static void HotkeyChangedCallback(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var btn = obj as Button;
            if (btn == null)
                return;

            Window parentWindow = Window.GetWindow(btn);
            if (parentWindow == null)
                return;

            KeyBinding kb = null;

            string hkOld = (string) e.OldValue;

            // find and remove key binding with old hotkey
            if (false == String.IsNullOrWhiteSpace(hkOld))
            {
                hkOld = NormalizeName(hkOld);
                kb = parentWindow.InputBindings
                    .OfType<KeyBinding>()
                    .FirstOrDefault(k => hkOld == (string) k.GetValue(FrameworkElement.NameProperty));

                if (kb != null)
                    parentWindow.InputBindings.Remove(kb);
            }

            string hkNew = (string) e.NewValue;

            if (String.IsNullOrWhiteSpace(hkNew))
                return;

            // create key binding with new hotkey

            var keys = hkNew.Split(new [] { _cmdJoinChar }, StringSplitOptions.RemoveEmptyEntries);

            ModifierKeys modifier = ModifierKeys.None;
            ModifierKeys m;

            // parse hotkey string and extract modifiers and main key
            string strKey = null;
            foreach (string k in keys)
            {
                if (Enum.TryParse(k, out m))
                    modifier = modifier | m;
                else
                {
                    // more than one Key is not supported
                    if (strKey != null)
                        return;

                    strKey = k;
                }
            }

            Key key;
            if (false == Enum.TryParse(strKey, out key))
                return;

            // Key + Modifier
            kb = new KeyBinding {Key = key, Modifiers = modifier};
            
            // x:Name
            kb.SetValue(FrameworkElement.NameProperty, NormalizeName(hkNew));

            // Command
            var cmdBinding = new Binding("Command") {Source = btn};
            BindingOperations.SetBinding(kb, InputBinding.CommandProperty, cmdBinding);

            // Command Parameter
            var paramBinding = new Binding("CommandParameter") {Source = btn};
            BindingOperations.SetBinding(kb, InputBinding.CommandParameterProperty, paramBinding);

            // Adding hotkey to Window
            parentWindow.InputBindings.Add(kb);
        }        
    }
}
