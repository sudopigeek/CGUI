using System;

namespace CGUI
{
    /// <summary>
    /// CGUI Extension class.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Converts a ConsoleKey instance value into a string.
        /// </summary>
        /// <param name="key">The ConsoleKey instance.</param>
        /// <returns></returns>
        public static string ConvertToString(this ConsoleKey key)
        {
            switch (key)
            {
                case ConsoleKey.A: return "A";
                case ConsoleKey.Add: return "Add";
                case ConsoleKey.Applications: return "Applications";
                case ConsoleKey.Attention:  return "Attention";
                case ConsoleKey.B: return "B";
                case ConsoleKey.Backspace: return "Backspace";
                case ConsoleKey.BrowserBack: return "BrowserBack";
                case ConsoleKey.BrowserFavorites: return "BrowserFavorites";
                case ConsoleKey.BrowserForward: return "BrowserForward";
                case ConsoleKey.BrowserHome: return "BrowserHome";
                case ConsoleKey.BrowserRefresh: return "BrowserRefresh";
                case ConsoleKey.BrowserSearch: return "BrowserSearch";
                case ConsoleKey.BrowserStop: return "BrowserStop";
                case ConsoleKey.C: return "C";
                case ConsoleKey.Clear: return "Clear";
                case ConsoleKey.CrSel: return "CrSel";
                case ConsoleKey.D: return "D";
                case ConsoleKey.D0: return "0";
                case ConsoleKey.D1: return "1";
                case ConsoleKey.D2: return "2";
                case ConsoleKey.D3: return "3";
                case ConsoleKey.D4: return "4";
                case ConsoleKey.D5: return "5";
                case ConsoleKey.D6: return "6";
                case ConsoleKey.D7: return "7";
                case ConsoleKey.D8: return "8";
                case ConsoleKey.D9: return "9";
                case ConsoleKey.Decimal: return "Decimal";
                case ConsoleKey.Delete: return "Delete";
                case ConsoleKey.Divide: return "Divide";
                case ConsoleKey.DownArrow: return "DownArrow";
                case ConsoleKey.E: return "E";
                case ConsoleKey.End: return "End";
                case ConsoleKey.Enter: return "Enter";
                case ConsoleKey.EraseEndOfFile: return "EraseEndOfFile";
                case ConsoleKey.Escape: return "Escape";
                case ConsoleKey.Execute: return "Execute";
                case ConsoleKey.ExSel: return "ExSel";
                case ConsoleKey.F: return "F";
                case ConsoleKey.F1: return "F1";
                case ConsoleKey.F10: return "F10";
                case ConsoleKey.F11: return "F11";
                case ConsoleKey.F12: return "F12";
                case ConsoleKey.F13: return "F13";
                case ConsoleKey.F14: return "F14";
                case ConsoleKey.F15: return "F15";
                case ConsoleKey.F16: return "F16";
                case ConsoleKey.F17: return "F17";
                case ConsoleKey.F18: return "F18";
                case ConsoleKey.F19: return "F19";
                case ConsoleKey.F2: return "F2";
                case ConsoleKey.F20: return "F20";
                case ConsoleKey.F21: return "F21";
                case ConsoleKey.F22: return "F22";
                case ConsoleKey.F23: return "F23";
                case ConsoleKey.F24: return "F24";
                case ConsoleKey.F3: return "F3";
                case ConsoleKey.F4: return "F4";
                case ConsoleKey.F5: return "F5";
                case ConsoleKey.F6: return "F6";
                case ConsoleKey.F7: return "F7";
                case ConsoleKey.F8: return "F8";
                case ConsoleKey.F9: return "F9";
                case ConsoleKey.G: return "G";
                case ConsoleKey.H: return "H";
                case ConsoleKey.Help: return "Help";
                case ConsoleKey.Home: return "Home";
                case ConsoleKey.I: return "I";
                case ConsoleKey.Insert: return "Insert";
                case ConsoleKey.J: return "J";
                case ConsoleKey.K: return "K";
                case ConsoleKey.L: return "L";
                case ConsoleKey.LaunchApp1: return "LaunchApp1";
                case ConsoleKey.LaunchApp2: return "LaunchApp2";
                case ConsoleKey.LaunchMail: return "LaunchMail";
                case ConsoleKey.LaunchMediaSelect: return "LaunchMediaSelect";
                case ConsoleKey.LeftArrow: return "LeftArrow";
                case ConsoleKey.LeftWindows: return "LeftWindows";
                case ConsoleKey.M: return "M";
                case ConsoleKey.MediaNext: return "MediaNext";
                case ConsoleKey.MediaPlay: return "MediaPlay";
                case ConsoleKey.MediaPrevious: return "MediaPrevious";
                case ConsoleKey.MediaStop: return "MediaStop";
                case ConsoleKey.Multiply: return "Multiply";
                case ConsoleKey.N: return "N";
                case ConsoleKey.NoName: return "NoName";
                case ConsoleKey.NumPad0: return "NumPad0";
                case ConsoleKey.NumPad1: return "NumPad1";
                case ConsoleKey.NumPad2: return "NumPad2";
                case ConsoleKey.NumPad3: return "NumPad3";
                case ConsoleKey.NumPad4: return "NumPad4";
                case ConsoleKey.NumPad5: return "NumPad5";
                case ConsoleKey.NumPad6: return "NumPad6";
                case ConsoleKey.NumPad7: return "NumPad7";
                case ConsoleKey.NumPad8: return "NumPad8";
                case ConsoleKey.NumPad9: return "NumPad9";
                case ConsoleKey.O: return "O";
                case ConsoleKey.Oem1: return "Oem1";
                case ConsoleKey.Oem102: return "Oem102";
                case ConsoleKey.Oem2: return "Oem2";
                case ConsoleKey.Oem3: return "Oem3";
                case ConsoleKey.Oem4: return "Oem4";
                case ConsoleKey.Oem5: return "Oem5";
                case ConsoleKey.Oem6: return "Oem6";
                case ConsoleKey.Oem7: return "Oem7";
                case ConsoleKey.Oem8: return "Oem8";
                case ConsoleKey.OemClear: return "OemClear";
                case ConsoleKey.OemComma: return "OemComma";
                case ConsoleKey.OemMinus: return "OemMinus";
                case ConsoleKey.OemPeriod: return "OemPeriod";
                case ConsoleKey.OemPlus: return "OemPlus";
                case ConsoleKey.P: return "P";
                case ConsoleKey.Pa1: return "Pa1";
                case ConsoleKey.Packet: return "Packet";
                case ConsoleKey.PageDown: return "PageDown";
                case ConsoleKey.PageUp: return "PageUp";
                case ConsoleKey.Pause: return "Pause";
                case ConsoleKey.Play: return "Play";
                case ConsoleKey.Print: return "Print";
                case ConsoleKey.PrintScreen: return "PrintScreen";
                case ConsoleKey.Process: return "Process";
                case ConsoleKey.Q: return "Q";
                case ConsoleKey.R: return "R";
                case ConsoleKey.RightArrow: return "RightArrow";
                case ConsoleKey.RightWindows: return "RightWindows";
                case ConsoleKey.S: return "S";
                case ConsoleKey.Select: return "Select";
                case ConsoleKey.Separator: return "Separator";
                case ConsoleKey.Sleep: return "Sleep";
                case ConsoleKey.Spacebar: return "Spacebar";
                case ConsoleKey.Subtract: return "Subtract";
                case ConsoleKey.T: return "T";
                case ConsoleKey.Tab: return "Tab";
                case ConsoleKey.U: return "U";
                case ConsoleKey.UpArrow: return "UpArrow";
                case ConsoleKey.V: return "V";
                case ConsoleKey.VolumeDown: return "VolumeDown";
                case ConsoleKey.VolumeMute: return "VolumeMute";
                case ConsoleKey.VolumeUp: return "VolumeUp";
                case ConsoleKey.W: return "W";
                case ConsoleKey.X: return "X";
                case ConsoleKey.Y: return "Y";
                case ConsoleKey.Z: return "Z";
                case ConsoleKey.Zoom: return "Zoom";
                default: return null;
            }
        }
        /// <summary>
        /// Converts a ControlType instance value into a string.
        /// </summary>
        /// <param name="cType">The ControlType instance.</param>
        /// <returns></returns>
        public static string ConvertToString(this ControlType cType)
        {
            switch (cType)
            {
                case ControlType.Button: return "Button";
                case ControlType.Label: return "Label";
                case ControlType.Line: return "Line";
                case ControlType.Picture: return "Picture";
                case ControlType.Rectangle: return "Rectangle";
                case ControlType.TextBox: return "TextBox";
                default: return null;
            }
        }
    }
}
