﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using CGUI.Shapes;

namespace CGUI
{
    internal class Internal
    {
        internal static Color screenColor;
        internal static int screenWidth;
        internal static int screenHeight;
        internal static int GetHorizontalCenter(string input = null)
        {
            if (input == null)
                return (int)Math.Ceiling((double)screenWidth / 2);
            else
            {
                int strLength = input.Length * 8;
                if (strLength <= screenWidth)
                {
                    if (((screenWidth - strLength) / 2).ToString().Contains("."))
                        return (int)Math.Ceiling(((double)screenWidth - strLength) / 2);
                    else      
                        return (screenWidth - strLength) / 2;
                }
                return -1;
            }      
        }
        internal static int GetVerticalCenter(string input = null)
        {
            if (input == null)
                return (int)Math.Ceiling((double)screenHeight / 2);
            else
            {
                if (((screenHeight - 12) / 2).ToString().Contains("."))
                    return (int)Math.Ceiling(((double)screenHeight - 12) / 2);
                else
                    return (screenHeight - 12) / 2;
            }
        }
        internal static int GetFirstTabControl(ControlList controls)
        {
            for (int i = 0; i < controls.Count; i++)
            {
                if (controls[i].controlType == ControlType.TextBox || controls[i].controlType == ControlType.Button)
                {
                    return i;
                }
            }
            return -1;
        }
    }
    public partial class VGADriver
    {
        internal static DoubleBufferedVMWareSVGAII driver;
        internal static Screen currentScreen = null;
        internal static Control currentControl = null;
        internal static int tcount = 0;
        internal static int index = 0;
        private static int pointer = 0;
        internal static List<Control> tabControls = new List<Control>();
        internal static List<Control> invalidControls = new List<Control>();
        private static int GetControlIndex(Control control)
        {
            for (int i = 0; i < tabControls.Count; i++)
            {
                if (tabControls[i].controlType == control.controlType && tabControls[i].X == control.X && tabControls[i].Y == control.Y)
                {
                    return i;
                }
            }
            return -1;
        }
        internal static void ValidateControls()
        {
            invalidControls.Clear();
            foreach (Control c in currentScreen.Controls)
            {
                Status status = ControlStatus.GetStatus(c);
                if (status == Status.ControlOutOfBounds || status == Status.OriginOutOfBounds)
                {
                    invalidControls.Add(c);
                }
            }

            if (invalidControls.Count > 0)
            {
                ErrorScreen();
            }
        }
        internal static void ErrorScreen()
        {
            driver.DoubleBuffer_Clear((uint)Color.Red.ToArgb());
            driver.DoubleBuffer_Update();
            uint y = 12;
            driver._DrawACSIIString("The following control(s) are partially or entirely out of bounds:", (uint)Color.White.ToArgb(), 20, 0);
            driver.DoubleBuffer_Update();
            foreach (Control c in invalidControls)
            {
                driver._DrawACSIIString(Extensions.ConvertToString(c.controlType) + " at X=" + c.X + ", Y=" + c.Y, (uint)Color.White.ToArgb(), 10, y);
                driver.DoubleBuffer_Update();
                y += 12;
            }
            bool wait = true;
            while (wait)
            {
                Console.ReadKey(true);
            }
        }
        internal static void SetText(TextBox tbox, string text)
        {
            if (text.Length <= tbox.cLength)
            {
                // Get current text:
                string prevText = "";
                if (tbox.Mask != '~')
                {
                    for (int i2 = 0; i2 < tbox.txt.Length; i2++)
                    {
                        prevText += tbox.Mask;
                    }
                }
                else
                {
                    prevText = tbox.txt.ToString();
                }
                // Clear text in textbox:
                if (tbox == currentControl)
                    driver._DrawACSIIString(prevText, (uint)tbox.BackColor.ToArgb(), (uint)tbox.X + 1, (uint)tbox.Y);
                else
                    driver._DrawACSIIString(prevText, (uint)tbox.UnfocusBackColor.ToArgb(), (uint)tbox.X + 1, (uint)tbox.Y);
                driver.DoubleBuffer_Update();
                // Draw new string:
                if (tbox.Mask != '~')
                {
                    uint i = (uint)tbox.X + 1;
                    for (int i2 = 0; i2 < text.Length; i2++)
                    {
                        if (tbox == currentControl)
                            driver._DrawACSIIString(tbox.Mask.ToString(), (uint)tbox.ForeColor.ToArgb(), i, (uint)tbox.Y);
                        else
                            driver._DrawACSIIString(tbox.Mask.ToString(), (uint)tbox.UnfocusForeColor.ToArgb(), i, (uint)tbox.Y);
                        driver.DoubleBuffer_Update();
                        i += 8;
                    }
                }
                else
                {
                    if (tbox == currentControl)
                        driver._DrawACSIIString(text, (uint)tbox.ForeColor.ToArgb(), (uint)tbox.X + 1, (uint)tbox.Y);
                    else
                        driver._DrawACSIIString(text, (uint)tbox.UnfocusForeColor.ToArgb(), (uint)tbox.X + 1, (uint)tbox.Y);
                    driver.DoubleBuffer_Update();
                }
                tbox.txt.Clear();
                tbox.txt.Append(text);
                tbox.pos = text.Length;
            }
            else
            {
                string substr = text.Substring(0, tbox.cLength);
                // Get current text:
                string prevText = "";
                if (tbox.Mask != '~')
                {
                    for (int i2 = 0; i2 < tbox.txt.Length; i2++)
                    {
                        prevText += tbox.Mask;
                    }
                }
                else
                {
                    prevText = tbox.txt.ToString();
                }
                // Clear text in textbox:
                driver._DrawACSIIString(prevText, (uint)tbox.BackColor.ToArgb(), (uint)tbox.X + 1, (uint)tbox.Y);
                driver.DoubleBuffer_Update();
                // Draw new string:
                if (tbox.Mask != '~')
                {
                    uint i = (uint)tbox.X + 1;
                    for (int i2 = 0; i2 < substr.Length; i2++)
                    {
                        driver._DrawACSIIString(tbox.Mask.ToString(), (uint)tbox.ForeColor.ToArgb(), i, (uint)tbox.Y);
                        driver.DoubleBuffer_Update();
                        i += 8;
                    }
                }
                else
                {
                    driver._DrawACSIIString(substr, (uint)tbox.ForeColor.ToArgb(), (uint)tbox.X + 1, (uint)tbox.Y);
                    driver.DoubleBuffer_Update();
                }
                tbox.txt.Clear();
                tbox.txt.Append(substr);
                tbox.pos = substr.Length;
            }
        }
        internal static void DrawPlaceholder(TextBox tbox)
        {
            // Draw placeholder:
            if (tbox.Placeholder != "")
            {
                string output;
                if (tbox.Placeholder.Length > tbox.cLength)
                    output = tbox.Placeholder.Substring(0, tbox.cLength);
                else
                    output = tbox.Placeholder;
                driver._DrawACSIIString(output, (uint)tbox.PlaceholderColor.ToArgb(), (uint)tbox.X + 1, (uint)tbox.Y);
                driver.DoubleBuffer_Update();
            }
        }
        internal static void ErasePlaceholder(TextBox tbox)
        {
            // Erase placeholder:
            if (tbox == currentControl)
                driver._DrawACSIIString(tbox.Placeholder, (uint)tbox.BackColor.ToArgb(), (uint)tbox.X + 1, (uint)tbox.Y);
            else
                driver._DrawACSIIString(tbox.Placeholder, (uint)tbox.UnfocusBackColor.ToArgb(), (uint)tbox.X + 1, (uint)tbox.Y);
            driver.DoubleBuffer_Update();
        }
        internal static void Focus(Control control)
        {
            if (control.controlType == ControlType.Button)
            {
                Button button = (Button)control;
                driver.DoubleBuffer_DrawFillRectangle((uint)button.X, (uint)button.Y, ((uint)button.txt.Length * 8) + 20, 13 + 10, (uint)button.BackColor.ToArgb());
                driver.DoubleBuffer_Update();
                driver._DrawACSIIString(button.txt, (uint)button.TextColor.ToArgb(), (uint)button.X + 10, (uint)button.Y + 5);
            }
            else if (control.controlType == ControlType.TextBox)
            {
                TextBox tbox = (TextBox)control;
                driver.DoubleBuffer_DrawFillRectangle((uint)tbox.X, (uint)tbox.Y, (uint)(tbox.cLength * 8) + 4, 15, (uint)tbox.BackColor.ToArgb());
                driver.DoubleBuffer_Update();
                if (tbox.Mask != '~')
                {
                    StringBuilder b = new StringBuilder("");
                    for (int i = 0; i < tbox.txt.Length; i++)
                    {
                        b.Append(tbox.Mask);
                    }
                    driver._DrawACSIIString(b.ToString(), (uint)tbox.ForeColor.ToArgb(), (uint)tbox.X + 1, (uint)tbox.Y);
                }
                else
                {
                    driver._DrawACSIIString(tbox.txt.ToString(), (uint)tbox.ForeColor.ToArgb(), (uint)tbox.X + 1, (uint)tbox.Y);
                }                
            }
            driver.DoubleBuffer_Update();
        }
        internal static void Unfocus(Control control)
        {
            switch (control.controlType)
            {
                case ControlType.TextBox:
                    TextBox tbox = (TextBox)control;
                    driver.DoubleBuffer_DrawFillRectangle((uint)tbox.X, (uint)tbox.Y, (uint)(tbox.cLength * 8) + 4, 15, (uint)tbox.UnfocusBackColor.ToArgb());
                    driver.DoubleBuffer_Update();
                    if (tbox.Mask != '~')
                    {
                        StringBuilder b = new StringBuilder("");
                        for (int i = 0; i < tbox.txt.Length; i++)
                        {
                            b.Append(tbox.Mask);
                        }
                        driver._DrawACSIIString(b.ToString(), (uint)tbox.UnfocusForeColor.ToArgb(), (uint)tbox.X + 1, (uint)tbox.Y);
                    }
                    else
                    {
                        driver._DrawACSIIString(tbox.txt.ToString(), (uint)tbox.UnfocusForeColor.ToArgb(), (uint)tbox.X + 1, (uint)tbox.Y);
                    }
                    driver.DoubleBuffer_Update();
                    if (tbox.txt.ToString() == "")
                        DrawPlaceholder(tbox);
                    break;
                case ControlType.Button:
                    Button button = (Button)control;
                    driver.DoubleBuffer_DrawFillRectangle((uint)button.X, (uint)button.Y, ((uint)button.txt.Length * 8) + 20, 13 + 10, (uint)button.UnfocusBackColor.ToArgb());
                    driver.DoubleBuffer_Update();
                    driver._DrawACSIIString(button.txt, (uint)button.UnfocusTextColor.ToArgb(), (uint)button.X + 10, (uint)button.Y + 5);
                    driver.DoubleBuffer_Update();
                    break;
            }
        }
        internal static void Focus(Control control, int x, int y, int tcount)
        {
            if (tcount >= 1)
            {
                Control frst;
                if (control == null)
                    frst = tabControls[index];
                else
                    frst = control;

                currentControl = frst;
                if (frst.controlType == ControlType.Button)
                {
                    Button first = (Button)frst;                   
                    Focus(first);                   
                    bool rkey = true;
                    while (rkey)
                    {
                        System.ConsoleKeyInfo info = Console.ReadKey(true);
                        if (info.Key == ConsoleKey.Tab)
                        {
                            //Move to next control (if one exists):
                            Next();
                        }
                        else
                        {
                            if (info.Key == first.TriggerKey)
                            {
                                if (first.OnEnter_Handler != null)
                                {
                                    first.OnEnter_Handler.Invoke(first, new EventArgs());
                                }
                            }
                            else
                            {
                                if (first.KeyPresses.Count > 0)
                                {
                                    KeyPress p = GetKeyPress(first.KeyPresses, info.Key, info.Modifiers);
                                    if (p != null)
                                    {
                                        if (p.OnPress_Handler != null)
                                        {
                                            p.OnPress_Handler.Invoke(Extensions.ConvertToString(info.Key), new ConsoleKeyInfo(info));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else if (frst.controlType == ControlType.TextBox)
                {
                    TextBox first = (TextBox)frst;
                    pointer = first.pos;
                    Focus(first);                  
                    if (first.pos == 0)
                    {
                        driver.DoubleBuffer_DrawFillRectangle((uint)x, (uint)y + 13, 8, 2, (uint)first.ForeColor.ToArgb());
                        driver.DoubleBuffer_Update();
                        if (first.txt.Length == 0)
                            DrawPlaceholder(first);
                    }
                    else
                    {
                        if (first.pos == first.cLength)
                        {
                            //Draw vertical cursor:
                            x = x + (first.txt.Length * 8);
                            driver.DoubleBuffer_DrawFillRectangle((uint)x, (uint)y, 2, 15, (uint)first.ForeColor.ToArgb());
                            driver.DoubleBuffer_Update();
                        }
                        else
                        {
                            x = x + (first.pos * 8);
                            driver.DoubleBuffer_DrawFillRectangle((uint)x, (uint)y + 13, 8, 2, (uint)first.ForeColor.ToArgb());
                            driver.DoubleBuffer_Update();
                        }
                    }
                    bool rkey = true;
                    while (rkey)
                    {
                        System.ConsoleKeyInfo info = Console.ReadKey(true);
                        switch (info.KeyChar)
                        {
                            #region Letters&Numbers
                            case 'a':
                            case 'A':
                            case 'b':
                            case 'B':
                            case 'c':
                            case 'C':
                            case 'd':
                            case 'D':
                            case 'e':
                            case 'E':
                            case 'f':
                            case 'F':
                            case 'g':
                            case 'G':
                            case 'h':
                            case 'H':
                            case 'i':
                            case 'I':
                            case 'j':
                            case 'J':
                            case 'k':
                            case 'K':
                            case 'l':
                            case 'L':
                            case 'm':
                            case 'M':
                            case 'n':
                            case 'N':
                            case 'o':
                            case 'O':
                            case 'p':
                            case 'P':
                            case 'q':
                            case 'Q':
                            case 'r':
                            case 'R':
                            case 's':
                            case 'S':
                            case 't':
                            case 'T':
                            case 'u':
                            case 'U':
                            case 'v':
                            case 'V':
                            case 'w':
                            case 'W':
                            case 'x':
                            case 'X':
                            case 'y':
                            case 'Y':
                            case 'z':
                            case 'Z':
                            case '0':
                            case '1':
                            case '2':
                            case '3':
                            case '4':
                            case '5':
                            case '6':
                            case '7':
                            case '8':
                            case '9':
                            case ' ':
                            case ';':
                            case ':':
                            case '<':
                            case ',':
                            case '>':
                            case '.':
                            case '?':
                            case '/':
                            case '"':
                            case '\'':
                            case '{':
                            case '[':
                            case '}':
                            case ']':
                            case '|':
                            case '\\':
                            case '+':
                            case '=':
                            case '-':
                            case '_':
                            case ')':
                            case '(':
                            case '*':
                            case '&':
                            case '^':
                            case '%':
                            case '$':
                            case '#':
                            case '@':
                            case '!':
                            case '~':
                            case '`':
                                #endregion
                                if (info.Key != ConsoleKey.Tab && info.Key != ConsoleKey.Backspace && info.Key != ConsoleKey.RightArrow && info.Key != ConsoleKey.LeftArrow)
                                {
                                    if (first.KeyPresses.Count > 0)
                                    {
                                        KeyPress p = GetKeyPress(first.KeyPresses, info.Key, info.Modifiers);
                                        if (p != null)
                                        {
                                            if (p.OnPress_Handler != null)
                                            {
                                                p.OnPress_Handler.Invoke(Extensions.ConvertToString(info.Key), new ConsoleKeyInfo(info));
                                            }
                                        }
                                    }
                                }

                                if (first.txt.Length < first.cLength)
                                {
                                    if (first.Filter != "")
                                    {
                                        if (first.Filter.Contains(info.KeyChar.ToString()))
                                        {
                                            // Get current text:
                                            string prevText = "";
                                            if (first.Mask != '~')
                                            {
                                                for (int i2 = 0; i2 < first.txt.Length; i2++)
                                                {
                                                    prevText += first.Mask;
                                                }
                                            }
                                            else
                                            {
                                                prevText = first.txt.ToString();
                                            }
                                            // Erase placeholder:
                                            if (prevText == "")
                                                ErasePlaceholder(first);
                                            first.txt = first.txt.Insert(pointer, info.KeyChar.ToString());
                                            // Clear text in textbox:
                                            driver._DrawACSIIString(prevText, (uint)first.BackColor.ToArgb(), (uint)first.X + 1, (uint)first.Y);
                                            driver.DoubleBuffer_Update();
                                            //Erase cursor:
                                            driver.DoubleBuffer_DrawFillRectangle((uint)x, (uint)y + 13, 8, 2, (uint)first.BackColor.ToArgb());
                                            driver.DoubleBuffer_Update();
                                            x += 8;
                                            // Draw new string:
                                            if (first.Mask != '~')
                                            {
                                                uint i = (uint)first.X + 1;
                                                for (int i2 = 0; i2 < first.txt.Length; i2++)
                                                {
                                                    driver._DrawACSIIString(first.Mask.ToString(), (uint)first.ForeColor.ToArgb(), i, (uint)first.Y);
                                                    i += 8;
                                                }
                                            }
                                            else
                                            {
                                                driver._DrawACSIIString(first.txt.ToString(), (uint)first.ForeColor.ToArgb(), (uint)first.X + 1, (uint)first.Y);
                                            }
                                            //Draw cursor one character up:
                                            driver.DoubleBuffer_DrawFillRectangle((uint)x, (uint)y + 13, 8, 2, (uint)first.ForeColor.ToArgb());
                                            driver.DoubleBuffer_Update();
                                            if (first.KeyPress != null)
                                                first.KeyPress.Invoke(first, info);
                                            pointer++;
                                        }
                                    }
                                    else
                                    {
                                        // Get current text:
                                        string prevText = "";
                                        if (first.Mask != '~')
                                        {
                                            for (int i2 = 0; i2 < first.txt.Length; i2++)
                                            {
                                                prevText += first.Mask;
                                            }
                                        }
                                        else
                                        {
                                            prevText = first.txt.ToString();
                                        }
                                        // Erase placeholder:
                                        if (prevText == "")
                                            ErasePlaceholder(first);
                                        first.txt = first.txt.Insert(pointer, info.KeyChar.ToString());
                                        // Clear text in textbox:
                                        driver._DrawACSIIString(prevText, (uint)first.BackColor.ToArgb(), (uint)first.X + 1, (uint)first.Y);
                                        driver.DoubleBuffer_Update();
                                        //Erase cursor:
                                        driver.DoubleBuffer_DrawFillRectangle((uint)x, (uint)y + 13, 8, 2, (uint)first.BackColor.ToArgb());
                                        driver.DoubleBuffer_Update();
                                        x += 8;
                                        // Draw new string:
                                        if (first.Mask != '~')
                                        {
                                            uint i = (uint)first.X + 1;
                                            for (int i2 = 0; i2 < first.txt.Length; i2++)
                                            {
                                                driver._DrawACSIIString(first.Mask.ToString(), (uint)first.ForeColor.ToArgb(), i, (uint)first.Y);
                                                i += 8;
                                            }
                                        }
                                        else
                                        {
                                            driver._DrawACSIIString(first.txt.ToString(), (uint)first.ForeColor.ToArgb(), (uint)first.X + 1, (uint)first.Y);
                                        }
                                        if (pointer == (first.cLength - 1))
                                            //Draw cursor up one character in vertical state:
                                            driver.DoubleBuffer_DrawFillRectangle((uint)x, (uint)y, 2, 15, (uint)first.ForeColor.ToArgb());
                                        else
                                            //Draw cursor one character up:
                                            driver.DoubleBuffer_DrawFillRectangle((uint)x, (uint)y + 13, 8, 2, (uint)first.ForeColor.ToArgb());
                                        driver.DoubleBuffer_Update();
                                        if (first.KeyPress != null)
                                            first.KeyPress.Invoke(first, info);
                                        pointer++;
                                    }
                                }
                                else
                                {
                                    if (first.Limit != null)
                                        first.Limit.Invoke(first, new EventArgs());
                                    if (first.BeepOnLimit)
                                        Console.Beep();
                                }
                                break;
                            default:
                                switch (info.Key)
                                {
                                    case ConsoleKey.Backspace:
                                        if (first.txt.Length > 0 && pointer > 0)
                                        {
                                            if (pointer == first.cLength)
                                            {
                                                //Erase last character:
                                                if (first.Mask != '~')
                                                    driver._DrawACSIIString(first.Mask.ToString(), (uint)first.BackColor.ToArgb(), (uint)x - 8, (uint)y);
                                                else
                                                    driver._DrawACSIIString(first.txt[first.txt.Length - 1].ToString(), (uint)first.BackColor.ToArgb(), (uint)x - 8, (uint)y);
                                                driver.DoubleBuffer_Update();
                                                //Erase vertical cursor:
                                                driver.DoubleBuffer_DrawFillRectangle((uint)x, (uint)y, 2, 15, (uint)first.BackColor.ToArgb());
                                                driver.DoubleBuffer_Update();
                                                x -= 8;
                                                //Draw cursor one character back:
                                                driver.DoubleBuffer_DrawFillRectangle((uint)x, (uint)y + 13, 8, 2, (uint)first.ForeColor.ToArgb());
                                                driver.DoubleBuffer_Update();
                                                first.txt.Remove(first.txt.Length - 1, 1);
                                            }
                                            else
                                            {
                                                if (pointer == first.txt.Length)
                                                {
                                                    //Erase last character:
                                                    if (first.Mask != '~')
                                                        driver._DrawACSIIString(first.Mask.ToString(), (uint)first.BackColor.ToArgb(), (uint)x - 8, (uint)y);
                                                    else
                                                        driver._DrawACSIIString(first.txt[first.txt.Length - 1].ToString(), (uint)first.BackColor.ToArgb(), (uint)x - 8, (uint)y);
                                                    driver.DoubleBuffer_Update();
                                                    //Erase cursor:
                                                    driver.DoubleBuffer_DrawFillRectangle((uint)x, (uint)y + 13, 8, 2, (uint)first.BackColor.ToArgb());
                                                    driver.DoubleBuffer_Update();
                                                    x -= 8;
                                                    //Draw cursor one character back:
                                                    driver.DoubleBuffer_DrawFillRectangle((uint)x, (uint)y + 13, 8, 2, (uint)first.ForeColor.ToArgb());
                                                    driver.DoubleBuffer_Update();
                                                    first.txt.Remove(first.txt.Length - 1, 1);
                                                    // Draw placeholder:
                                                    if ((pointer - 1) == 0)
                                                        DrawPlaceholder(first);
                                                }
                                                else
                                                {
                                                    // Get current text:
                                                    string prevText = "";
                                                    if (first.Mask != '~')
                                                    {
                                                        for (int i2 = 0; i2 < first.txt.Length; i2++)
                                                        {
                                                            prevText += first.Mask;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        prevText = first.txt.ToString();
                                                    }
                                                    // Remove character from internal string:
                                                    first.txt = first.txt.Remove(pointer - 1, 1);
                                                    // Clear text in textbox:
                                                    driver._DrawACSIIString(prevText, (uint)first.BackColor.ToArgb(), (uint)first.X + 1, (uint)first.Y);
                                                    driver.DoubleBuffer_Update();
                                                    //Erase cursor:
                                                    driver.DoubleBuffer_DrawFillRectangle((uint)x, (uint)y + 13, 8, 2, (uint)first.BackColor.ToArgb());
                                                    driver.DoubleBuffer_Update();
                                                    x -= 8;
                                                    // Draw new string:
                                                    if (first.Mask != '~')
                                                    {
                                                        uint i = (uint)first.X + 1;
                                                        for (int i2 = 0; i2 < first.txt.Length; i2++)
                                                        {
                                                            driver._DrawACSIIString(first.Mask.ToString(), (uint)first.ForeColor.ToArgb(), i, (uint)first.Y);
                                                            i += 8;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        driver._DrawACSIIString(first.txt.ToString(), (uint)first.ForeColor.ToArgb(), (uint)first.X + 1, (uint)first.Y);
                                                    }
                                                    //Draw cursor one character back:
                                                    driver.DoubleBuffer_DrawFillRectangle((uint)x, (uint)y + 13, 8, 2, (uint)first.ForeColor.ToArgb());
                                                    driver.DoubleBuffer_Update();
                                                }
                                            }
                                            pointer--;
                                            if (first.Delete != null)
                                            {
                                                first.Delete.Invoke(first, new EventArgs());
                                            }
                                        }
                                        else
                                        {
                                            if (first.Limit != null)
                                                first.Limit.Invoke(first, new EventArgs());
                                            if (first.BeepOnLimit)
                                                Console.Beep();
                                        }
                                        break;
                                    case ConsoleKey.Delete:
                                        if (pointer < first.txt.Length)
                                        {
                                            // Get current text:
                                            string prevText = "";
                                            if (first.Mask != '~')
                                            {
                                                for (int i2 = 0; i2 < first.txt.Length; i2++)
                                                {
                                                    prevText += first.Mask;
                                                }
                                            }
                                            else
                                            {
                                                prevText = first.txt.ToString();
                                            }
                                            // Remove character from internal string:
                                            first.txt = first.txt.Remove(pointer, 1);
                                            // Clear text in textbox:
                                            driver._DrawACSIIString(prevText, (uint)first.BackColor.ToArgb(), (uint)first.X + 1, (uint)first.Y);
                                            driver.DoubleBuffer_Update();
                                            // Draw new string:
                                            if (first.Mask != '~')
                                            {
                                                uint i = (uint)first.X + 1;
                                                for (int i2 = 0; i2 < first.txt.Length; i2++)
                                                {
                                                    driver._DrawACSIIString(first.Mask.ToString(), (uint)first.ForeColor.ToArgb(), i, (uint)first.Y);
                                                    driver.DoubleBuffer_Update();
                                                    i += 8;
                                                }
                                            }
                                            else
                                            {
                                                driver._DrawACSIIString(first.txt.ToString(), (uint)first.ForeColor.ToArgb(), (uint)first.X + 1, (uint)first.Y);
                                                driver.DoubleBuffer_Update();
                                            }
                                            // Draw placeholder:
                                            if (pointer == 0 && first.txt.Length == 0)
                                                DrawPlaceholder(first);

                                            if (first.Delete != null)
                                                first.Delete.Invoke(first, new EventArgs());
                                        }
                                        else
                                        {
                                            if (first.Limit != null)
                                                first.Limit.Invoke(first, new EventArgs());
                                            if (first.BeepOnLimit)
                                                Console.Beep();
                                        }
                                        break;
                                    case ConsoleKey.Tab:
                                        //Move to next textbox (if one exists):
                                        //Erase cursor:
                                        if (first.txt.Length == first.cLength)
                                            //Erase vertical cursor:
                                            driver.DoubleBuffer_DrawFillRectangle((uint)x + 1, (uint)y, 2, 15, (uint)first.BackColor.ToArgb());
                                        else
                                            //Erase cursor:
                                            driver.DoubleBuffer_DrawFillRectangle((uint)x, (uint)y + 13, 8, 2, (uint)first.BackColor.ToArgb());
                                        driver.DoubleBuffer_Update();
                                        Next();
                                        break;
                                    case ConsoleKey.LeftArrow:
                                        if (pointer > 0)
                                        {
                                            if (pointer == first.cLength)
                                            {
                                                //Erase vertical cursor:
                                                driver.DoubleBuffer_DrawFillRectangle((uint)x, (uint)y, 2, 15, (uint)first.BackColor.ToArgb());
                                                driver.DoubleBuffer_Update();
                                            }
                                            else
                                            {
                                                //Erase cursor:
                                                driver.DoubleBuffer_DrawFillRectangle((uint)x, (uint)y + 13, 8, 2, (uint)first.BackColor.ToArgb());
                                                driver.DoubleBuffer_Update();
                                                if (pointer < first.txt.Length)
                                                {
                                                    //Redraw previously selected character:
                                                    driver._DrawACSIIString(first.txt[pointer].ToString(), (uint)first.ForeColor.ToArgb(), (uint)x, (uint)y);
                                                    driver.DoubleBuffer_Update();
                                                }
                                            }
                                            //Draw cursor one character back:
                                            x -= 8;
                                            driver.DoubleBuffer_DrawFillRectangle((uint)x, (uint)y + 13, 8, 2, (uint)first.ForeColor.ToArgb());
                                            driver.DoubleBuffer_Update();
                                            pointer--;
                                        }
                                        break;
                                    case ConsoleKey.RightArrow:
                                        if (pointer < first.txt.Length)
                                        {
                                            //Erase cursor:
                                            driver.DoubleBuffer_DrawFillRectangle((uint)x, (uint)y + 13, 8, 2, (uint)first.BackColor.ToArgb());
                                            driver.DoubleBuffer_Update();
                                            //Redraw previously selected character:
                                            driver._DrawACSIIString(first.txt[pointer].ToString(), (uint)first.ForeColor.ToArgb(), (uint)x, (uint)y);
                                            driver.DoubleBuffer_Update();
                                            x += 8;
                                            if (pointer == (first.cLength - 1))
                                                // Draw cursor:
                                                driver.DoubleBuffer_DrawFillRectangle((uint)x, (uint)y, 2, 15, (uint)first.ForeColor.ToArgb());
                                            else
                                                // Draw vertical cursor:
                                                driver.DoubleBuffer_DrawFillRectangle((uint)x, (uint)y + 13, 8, 2, (uint)first.ForeColor.ToArgb());
                                            driver.DoubleBuffer_Update();
                                            pointer++;
                                        }
                                        break;
                                    default:
                                        break;
                                }
                                break;
                        }
                    }
                }
            }
        }
        internal static KeyPress GetKeyPress(List<KeyPress> list, ConsoleKey key, ConsoleModifiers modifiers)
        {
            foreach (KeyPress p in list)
            {
                if (p.ConsoleKeyInfo.Key == key && p.ConsoleKeyInfo.Modifiers == modifiers)
                {
                    return p;
                }
            }
            return null;
        }
        internal static void FocusControl(TextBox tbox)
        {
            // unfocus current control:
            if (currentControl.controlType == ControlType.Button)
            {
                Unfocus(currentControl);
            }
            else if (currentControl.controlType == ControlType.TextBox)
            {
                int x = tbox.X + 1 + (tbox.txt.Length * 8);
                //Erase cursor:
                if (tbox.txt.Length == tbox.cLength)
                {
                    //Erase vertical cursor:
                    driver.DoubleBuffer_DrawFillRectangle((uint)x + 1, (uint)tbox.Y, 2, 15, (uint)tbox.BackColor.ToArgb());
                    driver.DoubleBuffer_Update();
                }
                else
                {
                    //Erase cursor:     
                    driver.DoubleBuffer_DrawFillRectangle((uint)x, (uint)tbox.Y + 13, 8, 2, (uint)tbox.BackColor.ToArgb());
                    driver.DoubleBuffer_Update();
                }
                Unfocus(currentControl);
            }

            // focus specified control:
            //index = GetIndex(currentScreen, tbox);
            Focus(tbox, tbox.X + 1, tbox.Y, tcount);
        }
        internal static void FocusControl(Button btn)
        {
            // unfocus current control:
            if (currentControl.controlType == ControlType.Button)
            {
                Unfocus(currentControl);
            }
            else if (currentControl.controlType == ControlType.TextBox)
            {
                Unfocus(currentControl);
            }

            // focus specified control:
            Focus(btn, btn.X + 1, btn.Y, tcount);
        }
        internal static void DeleteControl(Control control, Control next = null)
        {
            if (currentScreen != null)
            {
                // Erase visual parts of the control:
                switch (control.controlType)
                {
                    case ControlType.Button:
                        Button btn = (Button)control;
                        driver.DoubleBuffer_DrawFillRectangle((uint)btn.X, (uint)btn.Y, ((uint)btn.txt.Length * 8) + 20, 13 + 10, (uint)currentScreen.backColor.ToArgb());
                        break;
                    case ControlType.Label:
                        Label label = (Label)control;
                        driver._DrawACSIIString(label.Text, (uint)currentScreen.backColor.ToArgb(), (uint)label.X, (uint)label.Y);
                        break;
                    case ControlType.Line:
                        Line line = (Line)control;
                        driver.DoubleBuffer_DrawLine((uint)currentScreen.backColor.ToArgb(), line.X, line.Y, line.EndX, line.EndY);
                        break;
                    case ControlType.Picture:
                        Picture picture = (Picture)control;
                        driver.DoubleBuffer_DrawFillRectangle((uint)picture.X, (uint)picture.Y, picture.Image.Width, picture.Image.Height, (uint)currentScreen.backColor.ToArgb());
                        break;
                    case ControlType.Rectangle:
                        Shapes.Rectangle rectangle = (Shapes.Rectangle)control;
                        if (rectangle.Fill)
                            driver.DoubleBuffer_DrawFillRectangle((uint)rectangle.X, (uint)rectangle.Y, (uint)rectangle.Width, (uint)rectangle.Height, (uint)currentScreen.backColor.ToArgb());
                        else
                            driver.DoubleBuffer_DrawRectangle((uint)currentScreen.backColor.ToArgb(), rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
                        break;
                    case ControlType.TextBox:
                        TextBox tbox = (TextBox)control;
                        driver.DoubleBuffer_DrawFillRectangle((uint)tbox.X, (uint)tbox.Y, (uint)(tbox.cLength * 8) + 3, 15, (uint)currentScreen.backColor.ToArgb());
                        break;
                }
                driver.DoubleBuffer_Update();
                // Change internal variables:
                tabControls.RemoveAt(GetControlIndex(control));
                tcount = tabControls.Count;                
                // Validate current control or focus a specified control:
                if (next != null)
                {
                    Focus(next);
                    Focus(next, next.X + 1, next.Y, tcount);
                }
                else
                {
                    if (currentControl == control)
                    {
                        // Move to another control:
                        int cindex = Internal.GetFirstTabControl(currentScreen.Controls);
                        if (cindex > -1)
                        {
                            // Set focus to another control:
                            Focus(currentScreen.Controls[cindex]);
                            Focus(currentScreen.Controls[cindex], currentScreen.Controls[cindex].X + 1, currentScreen.Controls[cindex].Y, tcount);
                        }
                    }
                    else
                    {
                        Focus(currentControl, currentControl.X + 1, currentControl.Y, tcount);
                    }
                }   
            }                                
        }
        internal static void DrawControl(Control control)
        {
            if (currentScreen != null)
            {
                // Draw visual parts of the control:
                switch (control.controlType)
                {
                    case ControlType.Button:
                        tcount++;
                        Button btn = (Button)control;
                        tabControls.Add(btn);
                        driver.DoubleBuffer_DrawFillRectangle((uint)btn.X, (uint)btn.Y, ((uint)btn.txt.Length * 8) + 20, 13 + 10, (uint)btn.UnfocusBackColor.ToArgb());
                        driver.DoubleBuffer_Update();
                        driver._DrawACSIIString(btn.txt, (uint)btn.UnfocusTextColor.ToArgb(), (uint)btn.X + 10, (uint)btn.Y + 5);
                        break;
                    case ControlType.Label:
                        Label label = (Label)control;
                        if (label.Text.Contains("\n"))
                        {
                            string[] lines = label.Text.Split("\n");
                            int y = label.Y;
                            for (int i2 = 0; i2 < lines.Length; i2++)
                            {
                                driver._DrawACSIIString(lines[i2], (uint)label.foreColor.ToArgb(), (uint)label.X, (uint)y);
                                driver.DoubleBuffer_Update();
                                y += 12;
                            }
                        }
                        else
                        {
                            driver._DrawACSIIString(label.Text, (uint)label.foreColor.ToArgb(), (uint)label.X, (uint)label.Y);
                        }
                        break;
                    case ControlType.Line:
                        Line line = (Line)control;
                        driver.DoubleBuffer_DrawLine((uint)line.Color.ToArgb(), line.X, line.Y, line.EndX, line.EndY);
                        break;
                    case ControlType.Picture:
                        Picture picture = (Picture)control;
                        picture.Update();
                        break;
                    case ControlType.Rectangle:
                        Shapes.Rectangle rectangle = (Shapes.Rectangle)control;
                        if (rectangle.Fill)
                            driver.DoubleBuffer_DrawFillRectangle((uint)rectangle.X, (uint)rectangle.Y, (uint)rectangle.Width, (uint)rectangle.Height, (uint)rectangle.Color.ToArgb());
                        else
                            driver.DoubleBuffer_DrawRectangle((uint)rectangle.Color.ToArgb(), rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
                        break;
                    case ControlType.TextBox:
                        tcount++;
                        TextBox tbox = (TextBox)control;
                        tabControls.Add(tbox);
                        driver.DoubleBuffer_DrawFillRectangle((uint)tbox.X, (uint)tbox.Y, (uint)(tbox.cLength * 8) + 4, 15, (uint)tbox.UnfocusBackColor.ToArgb());
                        DrawPlaceholder(tbox);
                        break;
                }
                driver.DoubleBuffer_Update();
            }
        }
        internal static void Next()
        {
            index++;
            if (tcount <= index)
                index = 0;
            Unfocus(currentControl);
            if (currentControl.controlType == ControlType.TextBox)
                ((TextBox)currentControl).pos = pointer;

            Focus(null, tabControls[index].X + 1, tabControls[index].Y, tcount);
        }
    }
}
