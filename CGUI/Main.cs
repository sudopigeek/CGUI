using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using CGUI.Shapes;

namespace CGUI
{
    /// <summary>
    /// Double-Buffered SVGAII Video Driver
    /// </summary>
    public class VGADriver
    {
        internal static DoubleBufferedVMWareSVGAII driver;
        internal static Screen currentScreen;
        internal static Control currentControl;
        /// <summary>
        /// Starts a new instance of the VGADriver class using the default screen size (640x480).
        /// </summary>
        public VGADriver()
        {
            driver = new DoubleBufferedVMWareSVGAII();
            driver.SetMode(640, 480);
            Internal.screenWidth = 640;
            Internal.screenHeight = 480;
            Internal.screenColor = Color.Black;
            driver.DoubleBuffer_Update();
        }
        /// <summary>
        /// Starts a new instance of the VGADriver class using the specified screen width and height.
        /// </summary>
        /// <param name="screenWidth">The screen width, in pixels.</param>
        /// <param name="screenHeight">The screen height, in pixels.</param>
        public VGADriver(int screenWidth, int screenHeight)
        {
            driver = new DoubleBufferedVMWareSVGAII();
            driver.SetMode((uint)screenWidth, (uint)screenHeight);
            Internal.screenWidth = screenWidth;
            Internal.screenHeight = screenHeight;
            Internal.screenColor = Color.Black;
            driver.DoubleBuffer_Update();
        }
        /// <summary>
        /// Renders a Screen.
        /// </summary>
        /// <param name="screen">The Screen to render.</param>
        public void RenderScreen(Screen screen)
        {
            invalidControls.Clear();
            foreach (Control c in screen.Controls)
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

            int order = 0;
            for (int i = 0; i < screen.Controls.Count; i++)
            {
                if (screen.Controls[i].controlType == ControlType.TextBox)
                {
                    ((TextBox)screen.Controls[i]).FocusOrder = order;
                    order++;
                }
            }
            currentScreen = screen;
            currentControl = null;
            tabControls.Clear();
            tcount = 0;
            index = 0;
            Internal.screenColor = screen.backColor;
            driver.DoubleBuffer_DrawFillRectangle(0, 0, (uint)Internal.screenWidth, (uint)Internal.screenHeight, (uint)screen.backColor.ToArgb());
            for (int i = 0; i < screen.Controls.Count; i++)
            {
                if (screen.Controls[i].controlType == ControlType.Label)
                {
                    Label label = ((Label)screen.Controls[i]);
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
                }
                else if (screen.Controls[i].controlType == ControlType.Line)
                {
                    Line line = ((Line)screen.Controls[i]);
                    driver.DoubleBuffer_DrawLine((uint)line.Color.ToArgb(), line.X, line.Y, line.EndX, line.EndY);
                }
                else if (screen.Controls[i].controlType == ControlType.Rectangle)
                {
                    CGUI.Shapes.Rectangle rectangle = ((CGUI.Shapes.Rectangle)screen.Controls[i]);
                    if (rectangle.Fill)
                    {
                        driver.DoubleBuffer_DrawFillRectangle((uint)rectangle.X, (uint)rectangle.Y, (uint)rectangle.Width, (uint)rectangle.Height, (uint)rectangle.Color.ToArgb());
                    }
                    else
                    {
                        driver.DoubleBuffer_DrawRectangle((uint)rectangle.Color.ToArgb(), rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
                    }
                }
                else if (screen.Controls[i].controlType == ControlType.Picture)
                {
                    Picture picture = ((Picture)screen.Controls[i]);
                    picture.Update();
                }
                else if (screen.Controls[i].controlType == ControlType.TextBox)
                {
                    tcount++;
                    TextBox tbox = ((TextBox)screen.Controls[i]);
                    tabControls.Add(tbox);
                    driver.DoubleBuffer_DrawFillRectangle((uint)tbox.X, (uint)tbox.Y, (uint)(tbox.cLength * 8) + 4, 15, (uint)tbox.UnfocusBackColor.ToArgb());
                }
                else if (screen.Controls[i].controlType == ControlType.Button)
                {
                    tcount++;
                    Button btn = ((Button)screen.Controls[i]);
                    tabControls.Add(btn);
                    driver.DoubleBuffer_DrawFillRectangle((uint)btn.X, (uint)btn.Y, ((uint)btn.txt.Length * 8) + 20, 13 + 10, (uint)btn.UnfocusBackColor.ToArgb());
                    driver.DoubleBuffer_Update();
                    driver._DrawACSIIString(btn.txt, (uint)btn.UnfocusTextColor.ToArgb(), (uint)btn.X + 10, (uint)btn.Y + 5);
                }
            }
            driver.DoubleBuffer_Update();

            if (tcount >= 1)
            {
                int f = Internal.GetFirstTabControl(screen.Controls);
                if (f > -1)
                {
                    Control c = screen.Controls[f];
                    Focus(tabControls, c, c.X + 1, c.Y, tcount);
                }   
            }
            else
            {
                bool wait = true;
                while (wait)
                {
                    Console.ReadKey(true);
                }
            }
        }

        internal static int tcount = 0;
        internal static int index = 0;
        private static int pointer = 0;
        internal static List<Control> tabControls = new List<Control>();
        internal static List<Control> invalidControls = new List<Control>();
        internal void ErrorScreen()
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
        internal static void Unfocus(TextBox tbox)
        {
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
        }
        internal static void Focus(TextBox tbox)
        {
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
            driver.DoubleBuffer_Update();
        }
        internal static void Focus(Button button)
        {
            driver.DoubleBuffer_DrawFillRectangle((uint)button.X, (uint)button.Y, ((uint)button.txt.Length * 8) + 20, 13 + 10, (uint)button.BackColor.ToArgb());
            driver.DoubleBuffer_Update();
            driver._DrawACSIIString(button.txt, (uint)button.TextColor.ToArgb(), (uint)button.X + 10, (uint)button.Y + 5);
            driver.DoubleBuffer_Update();
        }
        internal static void Unfocus(Button button)
        {
            driver.DoubleBuffer_DrawFillRectangle((uint)button.X, (uint)button.Y, ((uint)button.txt.Length * 8) + 20, 13 + 10, (uint)button.UnfocusBackColor.ToArgb());
            driver.DoubleBuffer_Update();
            driver._DrawACSIIString(button.txt, (uint)button.UnfocusTextColor.ToArgb(), (uint)button.X + 10, (uint)button.Y + 5);
            driver.DoubleBuffer_Update();
        }
        internal static void Focus(List<Control> tControls, Control control, int x, int y, int tcount)
        {
            if (tcount >= 1)
            {
                Control frst;
                if (control == null)
                    frst = tControls[index];
                else
                    frst = control;
                
                if (frst.controlType == ControlType.Button)
                {
                    Button first = (Button)frst;
                    currentControl = first;
                    Focus(first);
                    index++;
                    bool rkey = true;
                    while (rkey)
                    {
                        System.ConsoleKeyInfo info = Console.ReadKey(true);
                        if (info.Key == ConsoleKey.Tab)
                        {
                            //Move to next control (if one exists):
                            if (tcount > index)
                            {
                                Unfocus(first);
                                Focus(tControls, null, tControls[index].X + 1, tControls[index].Y, tcount);
                            }
                            else
                            {
                                index = 0;
                                Unfocus(first);
                                Focus(tControls, null, tControls[index].X + 1, tControls[index].Y, tcount);
                            }
                            rkey = false;
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
                    currentControl = first;
                    Focus(first);
                    index++;
                    if (first.txt.ToString() == "")
                    {
                        driver.DoubleBuffer_DrawFillRectangle((uint)x, (uint)y + 13, 8, 2, (uint)first.ForeColor.ToArgb());
                        driver.DoubleBuffer_Update();
                    }
                    else
                    {
                        if (first.txt.Length == first.cLength)
                        {
                            //Draw vertical cursor:
                            driver.DoubleBuffer_DrawFillRectangle((uint)(x + (first.txt.Length * 8) + 1), (uint)y, 2, 15, (uint)first.ForeColor.ToArgb());
                            driver.DoubleBuffer_Update();
                            x = x + (first.txt.Length * 8);
                        }
                        else
                        {
                            x = (x + (first.txt.Length * 8));
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
                                            {
                                                first.KeyPress.Invoke(first, info);
                                            }
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
                                        {
                                            //Draw cursor up one character in vertical state:
                                            driver.DoubleBuffer_DrawFillRectangle((uint)x, (uint)y, 2, 15, (uint)first.ForeColor.ToArgb());
                                            driver.DoubleBuffer_Update();
                                        }
                                        else
                                        {
                                            //Draw cursor one character up:
                                            driver.DoubleBuffer_DrawFillRectangle((uint)x, (uint)y + 13, 8, 2, (uint)first.ForeColor.ToArgb());
                                            driver.DoubleBuffer_Update();
                                        }
                                        
                                        if (first.KeyPress != null)
                                        {
                                            first.KeyPress.Invoke(first, info);
                                        }
                                        pointer++;
                                    }                                   
                                }
                                else
                                {
                                    if (first.Limit != null)
                                    {
                                        first.Limit.Invoke(first, new EventArgs());
                                    }

                                    if (first.BeepOnLimit)
                                    {
                                        Console.Beep();
                                    }
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
                                                {
                                                    driver._DrawACSIIString(first.Mask.ToString(), (uint)first.BackColor.ToArgb(), (uint)x - 8, (uint)y);
                                                }
                                                else
                                                {
                                                    driver._DrawACSIIString(first.txt[first.txt.Length - 1].ToString(), (uint)first.BackColor.ToArgb(), (uint)x - 8, (uint)y);
                                                }
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
                                                    {
                                                        driver._DrawACSIIString(first.Mask.ToString(), (uint)first.BackColor.ToArgb(), (uint)x - 8, (uint)y);
                                                    }
                                                    else
                                                    {
                                                        driver._DrawACSIIString(first.txt[first.txt.Length - 1].ToString(), (uint)first.BackColor.ToArgb(), (uint)x - 8, (uint)y);
                                                    }
                                                    driver.DoubleBuffer_Update();
                                                    //Erase cursor:
                                                    driver.DoubleBuffer_DrawFillRectangle((uint)x, (uint)y + 13, 8, 2, (uint)first.BackColor.ToArgb());
                                                    driver.DoubleBuffer_Update();
                                                    x -= 8;
                                                    //Draw cursor one character back:
                                                    driver.DoubleBuffer_DrawFillRectangle((uint)x, (uint)y + 13, 8, 2, (uint)first.ForeColor.ToArgb());
                                                    driver.DoubleBuffer_Update();
                                                    first.txt.Remove(first.txt.Length - 1, 1);
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
                                            {
                                                first.Limit.Invoke(first, new EventArgs());
                                            }

                                            if (first.BeepOnLimit)
                                            {
                                                Console.Beep();
                                            }
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

                                            if (first.Delete != null)
                                            {
                                                first.Delete.Invoke(first, new EventArgs());
                                            }
                                        }
                                        else
                                        {
                                            if (first.Limit != null)
                                            {
                                                first.Limit.Invoke(first, new EventArgs());
                                            }

                                            if (first.BeepOnLimit)
                                            {
                                                Console.Beep();
                                            }
                                        }
                                        break;
                                    case ConsoleKey.Tab:
                                        //Move to next textbox (if one exists):
                                        if (tcount > index)
                                        {
                                            //Erase cursor:
                                            if (first.txt.Length == first.cLength)
                                            {
                                                //Erase vertical cursor:
                                                driver.DoubleBuffer_DrawFillRectangle((uint)x + 1, (uint)y, 2, 15, (uint)first.BackColor.ToArgb());
                                                driver.DoubleBuffer_Update();
                                            }
                                            else
                                            {
                                                //Erase cursor:
                                                driver.DoubleBuffer_DrawFillRectangle((uint)x, (uint)y + 13, 8, 2, (uint)first.BackColor.ToArgb());
                                                driver.DoubleBuffer_Update();
                                            }
                                            //Loop back through:
                                            Unfocus(first);
                                            Focus(tControls, null, tControls[index].X + 1, tControls[index].Y, tcount);
                                        }
                                        else
                                        {
                                            //Erase cursor:
                                            if (first.txt.Length == first.cLength)
                                            {
                                                //Erase vertical cursor:
                                                driver.DoubleBuffer_DrawFillRectangle((uint)x + 1, (uint)y, 2, 15, (uint)first.BackColor.ToArgb());
                                                driver.DoubleBuffer_Update();
                                            }
                                            else
                                            {
                                                //Erase cursor:
                                                driver.DoubleBuffer_DrawFillRectangle((uint)x, (uint)y + 13, 8, 2, (uint)first.BackColor.ToArgb());
                                                driver.DoubleBuffer_Update();
                                            }
                                            //Move back to the first one:
                                            index = 0;
                                            Unfocus(first);
                                            Focus(tControls, null, tControls[index].X + 1, tControls[index].Y, tcount);
                                        }
                                        break;
                                    case ConsoleKey.LeftArrow:
                                        if (pointer > 0)
                                        {
                                            if (pointer == first.cLength)
                                            {
                                                //Erase vertical cursor:
                                                driver.DoubleBuffer_DrawFillRectangle((uint)x, (uint)y, 2, 15, (uint)first.BackColor.ToArgb());
                                                driver.DoubleBuffer_Update();
                                                x -= 8;
                                                //Draw cursor one character back:
                                                driver.DoubleBuffer_DrawFillRectangle((uint)x, (uint)y + 13, 8, 2, (uint)first.ForeColor.ToArgb());
                                                driver.DoubleBuffer_Update();
                                                pointer--;
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
                                                //Draw cursor one character back:
                                                x -= 8;
                                                driver.DoubleBuffer_DrawFillRectangle((uint)x, (uint)y + 13, 8, 2, (uint)first.ForeColor.ToArgb());
                                                driver.DoubleBuffer_Update();
                                                pointer--;
                                            }                                        
                                        }
                                        break;
                                    case ConsoleKey.RightArrow:
                                        if (pointer < first.txt.Length)
                                        {
                                            if (pointer == (first.cLength - 1))
                                            {
                                                //Erase cursor:
                                                driver.DoubleBuffer_DrawFillRectangle((uint)x, (uint)y + 13, 8, 2, (uint)first.BackColor.ToArgb());
                                                driver.DoubleBuffer_Update();
                                                //Redraw previously selected character:
                                                driver._DrawACSIIString(first.txt[pointer].ToString(), (uint)first.ForeColor.ToArgb(), (uint)x, (uint)y);
                                                driver.DoubleBuffer_Update();
                                                //Draw cursor up one character in vertical state:
                                                x += 8;
                                                driver.DoubleBuffer_DrawFillRectangle((uint)x, (uint)y, 2, 15, (uint)first.ForeColor.ToArgb());
                                                driver.DoubleBuffer_Update();
                                                pointer++;
                                            }
                                            else
                                            {
                                                //Erase cursor:
                                                driver.DoubleBuffer_DrawFillRectangle((uint)x, (uint)y + 13, 8, 2, (uint)first.BackColor.ToArgb());
                                                driver.DoubleBuffer_Update();
                                                //Redraw previously selected character:
                                                driver._DrawACSIIString(first.txt[pointer].ToString(), (uint)first.ForeColor.ToArgb(), (uint)x, (uint)y);
                                                driver.DoubleBuffer_Update();
                                                //Draw cursor up one character:
                                                x += 8;
                                                driver.DoubleBuffer_DrawFillRectangle((uint)x, (uint)y + 13, 8, 2, (uint)first.ForeColor.ToArgb());
                                                driver.DoubleBuffer_Update();
                                                pointer++;
                                            }
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
                Unfocus((Button)currentControl);
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
                Unfocus((TextBox)currentControl);
            }

            // focus specified control:
            //index = GetIndex(currentScreen, tbox);
            Focus(tabControls, tbox, tbox.X + 1, tbox.Y, tcount);
        }
        internal static void FocusControl(Button btn)
        {
            // unfocus current control:
            if (currentControl.controlType == ControlType.Button)
            {
                Unfocus((Button)currentControl);
            }
            else if (currentControl.controlType == ControlType.TextBox) 
            {
                Unfocus((TextBox)currentControl);
            }

            // focus specified control:
            Focus(tabControls, btn, btn.X + 1, btn.Y, tcount);
        }

        /// <summary>
        /// Updates the screen.
        /// </summary>
        public void UpdateScreen()
        {
            driver.DoubleBuffer_Clear((uint)currentScreen.backColor.ToArgb());
            RenderScreen(currentScreen);
        }
        /// <summary>
        /// Clears the screen using the default color (black).
        /// </summary>
        public void ClearScreen()
        {
            currentScreen = null;
            currentControl = null;
            driver.DoubleBuffer_Clear((uint)Color.Black.ToArgb());
            driver.DoubleBuffer_Update();
        }
        /// <summary>
        /// Clears the screen using the specified color.
        /// </summary>
        /// <param name="color">The color to clear the screen with.</param>
        public void ClearScreen(Color color)
        {
            currentScreen = null;
            currentControl = null;
            driver.DoubleBuffer_Clear((uint)color.ToArgb());
            driver.DoubleBuffer_Update();
        }
    }
    /// <summary>
    /// Represents an area on the screen.
    /// </summary>
    public class Area
    {
        /// <summary>
        /// The X coordinate.
        /// </summary>
        public int X { get; set; }
        /// <summary>
        /// The Y coordinate.
        /// </summary>
        public int Y { get; set; }
        /// <summary>
        /// The area width.
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        /// The area height.
        /// </summary>
        public int Height { get; set; }
        /// <summary>
        /// Starts a new instance of the Area class.
        /// </summary>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        /// <param name="width">The area width.</param>
        /// <param name="height">The area height.</param>
        public Area(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
    }
    /// <summary>
    /// Represents a screen containing controls.
    /// </summary>
    public class Screen
    {
        /// <summary>
        /// The controls contained in this Screen instance.
        /// </summary>
        public List<Control> Controls { get; set; }
        /// <summary>
        /// The screen's background color.
        /// </summary>
        public Color backColor { get; set; }
        /// <summary>
        /// Starts a new instance of the Screen class with the default background color (black).
        /// </summary>
        public Screen()
        {
            backColor = Color.Black;
            Controls = new List<Control>();
        }
        /// <summary>
        /// Starts a new instance of the Screen class with the specified background color.
        /// </summary>
        /// <param name="backColor">The background color for the screen.</param>
        public Screen(Color backColor)
        {
            this.backColor = backColor;
            Controls = new List<Control>();
        }
    }
    /// <summary>
    /// Represents types of currently supported controls.
    /// </summary>
    public enum ControlType
    {
        /// <summary>
        /// Represents a label.
        /// </summary>
        Label,
        /// <summary>
        /// Represents a line.
        /// </summary>
        Line,
        /// <summary>
        /// Represents a rectangle.
        /// </summary>
        Rectangle,
        /// <summary>
        /// Represents a picture.
        /// </summary>
        Picture,
        /// <summary>
        /// Represents a textbox.
        /// </summary>
        TextBox,
        /// <summary>
        /// Represents a button.
        /// </summary>
        Button
    }
    internal class Internal
    {
        public static Color screenColor;
        public static int screenWidth;
        public static int screenHeight;
        public static int GetFirstTabControl(List<Control> controls)
        {
            for (int i = 0; i < controls.Count; i++)
            {
                if (controls[i].controlType == ControlType.TextBox)
                {
                    return i;
                }
                else if (controls[i].controlType == ControlType.Button)
                {
                    return i;
                }
            }
            return -1;
        }
    }   
    /// <summary>
    /// Internal base class for CGUI controls.
    /// </summary>
    public class Control
    {
        internal ControlType controlType { get; set; }
        internal int X { get; set; }
        internal int Y { get; set; }
    }
}
