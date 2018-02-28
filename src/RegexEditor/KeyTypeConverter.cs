using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Microsoft.VisualStudio.RegularExpression
{
    internal class KeyTypeConverter : System.Windows.Input.KeyConverter
    {
        public override bool CanConvertTo(System.ComponentModel.ITypeDescriptorContext context, Type destinationType)
        {
            if (((destinationType != typeof(char)) || (context == null)) || (context.Instance == null))
            {
                return base.CanConvertTo(context, destinationType);
            }

            Key instance = (Key)context.Instance;
            return instance >= Key.None && instance <= Key.OemClear;
        }

        public override object ConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(char) && value != null)
            {
                string key = base.ConvertToString(value);

                if (!string.IsNullOrEmpty(key) && key.Length == 1)
                {
                    if (char.IsLetter(key, 0))
                    {
                        return IsShiftKeyDown() ? key.ToUpper()[0] : key.ToLower()[0];
                    }
                    else if (char.IsDigit(key, 0) && !IsShiftKeyDown())
                    {
                        return key[0];
                    }
                }

                switch ((Key)value)
                {
                    case Key.Oem4: return IsShiftKeyDown() ? '{' : '[';
                    case Key.Oem6: return IsShiftKeyDown() ? '}' : ']';
                    case Key.Oem5: return IsShiftKeyDown() ? '|' : '\\';
                    case Key.D1: return '!';
                    case Key.D2: return '@';
                    case Key.D3: return '#';
                    case Key.D4: return '$';
                    case Key.D5: return '%';
                    case Key.D6: return '^';
                    case Key.D7: return '&';
                    case Key.D8: return '*';
                    case Key.D9: return '(';
                    case Key.D0: return ')';
                    case Key.Space: return ' ';
                    case Key.OemMinus: return IsShiftKeyDown() ? '_' : '-';
                    case Key.OemPlus: return IsShiftKeyDown() ? '+' : '=';
                    case Key.OemQuestion: return IsShiftKeyDown() ? '?' : '/';
                    case Key.OemSemicolon: return IsShiftKeyDown() ? ':' : ';';
                    case Key.Oem7: return IsShiftKeyDown() ? '"' : '\'';
                    case Key.OemPeriod: return IsShiftKeyDown() ? '>' : '.';
                    case Key.OemComma: return IsShiftKeyDown() ? '<' : ',';
                }

                return null;
            }
            else
            {
                return base.ConvertTo(context, culture, value, destinationType);
            }
        }

        public char? ConvertToChar(Key key)
        {
            object result = ConvertTo(key, typeof(char));
            if (result != null)
            {
                return (char)result;
            }

            return null;
        }

        private bool IsShiftKeyDown()
        {
            return Keyboard.IsKeyDown(Key.RightShift) || Keyboard.IsKeyDown(Key.LeftShift);
        }
    }
}