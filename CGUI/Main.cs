using System;
using System.Collections.Generic;
using System.Drawing;

namespace CGUI
{
    /// <summary>
    /// Double-Buffered SVGAII Video Driver
    /// </summary>
    public class VGADriver
    {
        internal static DoubleBufferedVMWareSVGAII driver;
        private Screen currentScreen;
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
                    driver._DrawACSIIString(label.Text, (uint)label.foreColor.ToArgb(), (uint)label.X, (uint)label.Y);
                }
                else if (screen.Controls[i].controlType == ControlType.Line)
                {
                    Line line = ((Line)screen.Controls[i]);
                    driver.DoubleBuffer_DrawLine((uint)line.Color.ToArgb(), line.X, line.Y, line.EndX, line.EndY);
                }
                else if (screen.Controls[i].controlType == ControlType.Rectangle)
                {
                    Rectangle rectangle = ((Rectangle)screen.Controls[i]);
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
                    driver.DoubleBuffer_DrawImage(picture.Image, (uint)picture.X, (uint)picture.Y);
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
                TextBox first = ((TextBox)screen.Controls[Internal.GetFirstTextBox(screen.Controls)]);
                Focus(tabControls, first.X + 1, first.Y, tcount);
            }
        }

        internal static int tcount = 0;
        internal static int index = 0;
        internal static List<Control> tabControls = new List<Control>();
        internal static void Unfocus(TextBox tbox)
        {
            driver.DoubleBuffer_DrawFillRectangle((uint)tbox.X, (uint)tbox.Y, (uint)(tbox.cLength * 8) + 4, 15, (uint)tbox.UnfocusBackColor.ToArgb());
            driver.DoubleBuffer_Update();
            driver._DrawACSIIString(tbox.txt.ToString(), (uint)tbox.UnfocusForeColor.ToArgb(), (uint)tbox.X + 1, (uint)tbox.Y);
            driver.DoubleBuffer_Update();
        }
        internal static void Focus(TextBox tbox)
        {
            driver.DoubleBuffer_DrawFillRectangle((uint)tbox.X, (uint)tbox.Y, (uint)(tbox.cLength * 8) + 4, 15, (uint)tbox.BackColor.ToArgb());
            driver.DoubleBuffer_Update();
            driver._DrawACSIIString(tbox.txt.ToString(), (uint)tbox.ForeColor.ToArgb(), (uint)tbox.X + 1, (uint)tbox.Y);
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
        internal static void Focus(List<Control> tControls, int x, int y, int tcount)
        {
            if (tcount >= 1)
            {
                Control frst = tControls[index];
                if (frst.controlType == ControlType.Button)
                {
                    Button first = (Button)frst;
                    Focus(first);
                    index++;
                    bool rkey = true;
                    while (rkey)
                    {
                        ConsoleKeyInfo info = Console.ReadKey(true);
                        switch (info.Key)
                        {
                            case ConsoleKey.Tab:
                                //Move to next control (if one exists):
                                if (tcount > index)
                                {
                                    Unfocus(first);
                                    Focus(tControls, tControls[index].X + 1, tControls[index].Y, tcount);
                                }
                                else
                                {
                                    index = 0;
                                    Unfocus(first);
                                    Focus(tControls, tControls[index].X + 1, tControls[index].Y, tcount);
                                }
                                rkey = false;
                                break;
                            case ConsoleKey.Enter:
                                first.OnEnter_Handler.Invoke(first, new EventArgs());
                                //rkey = false;
                                break;
                            default:
                                break;
                        }
                    }
                }
                else if (frst.controlType == ControlType.TextBox)
                {
                    TextBox first = (TextBox)frst;
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
                        ConsoleKeyInfo info = Console.ReadKey(true);
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
                                if (first.txt.Length < first.cLength)
                                {
                                    if (first.Filter != "")
                                    {
                                        if (first.Filter.Contains(info.KeyChar.ToString()))
                                        {
                                            //Erase cursor:
                                            driver.DoubleBuffer_DrawFillRectangle((uint)x, (uint)y + 13, 8, 2, (uint)first.BackColor.ToArgb());
                                            driver.DoubleBuffer_Update();
                                            x += 8;
                                            if (first.txt.Length == (first.cLength - 1))
                                            {
                                                //Draw cursor up one character in vertical state:
                                                driver.DoubleBuffer_DrawFillRectangle((uint)x + 1, (uint)y, 2, 15, (uint)first.ForeColor.ToArgb());
                                                driver.DoubleBuffer_Update();
                                            }
                                            else
                                            {
                                                //Draw cursor up one character:
                                                driver.DoubleBuffer_DrawFillRectangle((uint)x, (uint)y + 13, 8, 2, (uint)first.ForeColor.ToArgb());
                                                driver.DoubleBuffer_Update();
                                            }

                                            if (first.Mask == '~')
                                            {
                                                //Draw character entered:
                                                driver._DrawACSIIString(info.KeyChar.ToString(), (uint)first.ForeColor.ToArgb(), (uint)x - 8, (uint)y);
                                                driver.DoubleBuffer_Update();
                                            }
                                            else
                                            {
                                                //Draw mask character:
                                                driver._DrawACSIIString(first.Mask.ToString(), (uint)first.ForeColor.ToArgb(), (uint)x - 8, (uint)y);
                                                driver.DoubleBuffer_Update();
                                            }

                                            first.txt.Append(info.KeyChar);
                                        }
                                        else
                                        {
                                            if (first.BeepOnLimit)
                                            {
                                                Console.Beep();
                                            }          
                                        }
                                    }
                                    else
                                    {
                                        //Erase cursor:
                                        driver.DoubleBuffer_DrawFillRectangle((uint)x, (uint)y + 13, 8, 2, (uint)first.BackColor.ToArgb());
                                        driver.DoubleBuffer_Update();
                                        x += 8;
                                        if (first.txt.Length == (first.cLength - 1))
                                        {
                                            //Draw cursor up one character in vertical state:
                                            driver.DoubleBuffer_DrawFillRectangle((uint)x + 1, (uint)y, 2, 15, (uint)first.ForeColor.ToArgb());
                                            driver.DoubleBuffer_Update();
                                        }
                                        else
                                        {
                                            //Draw cursor up one character:
                                            driver.DoubleBuffer_DrawFillRectangle((uint)x, (uint)y + 13, 8, 2, (uint)first.ForeColor.ToArgb());
                                            driver.DoubleBuffer_Update();
                                        }

                                        if (first.Mask == '~')
                                        {
                                            //Draw character entered:
                                            driver._DrawACSIIString(info.KeyChar.ToString(), (uint)first.ForeColor.ToArgb(), (uint)x - 8, (uint)y);
                                            driver.DoubleBuffer_Update();
                                        }
                                        else
                                        {
                                            //Draw mask character:
                                            driver._DrawACSIIString(first.Mask.ToString(), (uint)first.ForeColor.ToArgb(), (uint)x - 8, (uint)y);
                                            driver.DoubleBuffer_Update();
                                        }
                                        
                                        first.txt.Append(info.KeyChar);
                                    }
                                }
                                else
                                {
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
                                        if (first.txt.Length > 0)
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
                                            if (first.txt.Length == first.cLength)
                                            {
                                                //Erase vertical cursor:
                                                driver.DoubleBuffer_DrawFillRectangle((uint)x + 1, (uint)y, 2, 15, (uint)first.BackColor.ToArgb());
                                                driver.DoubleBuffer_Update();
                                                x -= 8;
                                            }
                                            else
                                            {
                                                //Erase cursor:
                                                driver.DoubleBuffer_DrawFillRectangle((uint)x, (uint)y + 13, 8, 2, (uint)first.BackColor.ToArgb());
                                                driver.DoubleBuffer_Update();
                                                x -= 8;
                                            }
                                            //Draw cursor one character back:
                                            driver.DoubleBuffer_DrawFillRectangle((uint)x, (uint)y + 13, 8, 2, (uint)first.ForeColor.ToArgb());
                                            driver.DoubleBuffer_Update();
                                            first.txt.Remove(first.txt.Length - 1, 1);
                                        }
                                        else
                                        {
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
                                            Focus(tControls, tControls[index].X + 1, tControls[index].Y, tcount);
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
                                            Focus(tControls, tControls[index].X + 1, tControls[index].Y, tcount);
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

        /// <summary>
        /// Updates the screen.
        /// </summary>
        public void UpdateScreen()
        {
            driver.DoubleBuffer_Clear((uint)Color.Black.ToArgb());
            RenderScreen(currentScreen);
        }
        /// <summary>
        /// Clears the screen using the default color (black).
        /// </summary>
        public void ClearScreen()
        {
            currentScreen = null;
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
        public List<Control> Controls;
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
        public static int GetFirstTextBox(List<Control> controls)
        {
            for (int i = 0; i < controls.Count; i++)
            {
                if (controls[i].controlType == ControlType.TextBox)
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
