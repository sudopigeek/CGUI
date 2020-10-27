using Cosmos.System.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using Cosmos.System;

namespace CGUI
{
    /// <summary>
    /// Double-Buffered SVGAII Video Driver
    /// </summary>
    public class VGADriver
    {
        internal static DoubleBufferedVMWareSVGAII driver;
        internal Screen currentScreen;
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
        /// Draws a reactangle.
        /// </summary>
        /// <param name="color">The rectangle color.</param>
        /// <param name="x">The X coordinate on the screen.</param>
        /// <param name="y">The Y coordinate on the screen.</param>
        /// <param name="width">The rectangle's width.</param>
        /// <param name="height">The rectangle's height.</param>
        public void DrawRectangle(Color color, int x, int y, int width, int height)
        {
            driver.DoubleBuffer_DrawRectangle((uint)color.ToArgb(), x, y, width, height);
            driver.DoubleBuffer_Update();
        }
        /// <summary>
        /// Draws a filled or normal rectangle.
        /// </summary>
        /// <param name="color">The rectangle color.</param>
        /// <param name="x">The X coordinate on the screen.</param>
        /// <param name="y">The Y coordinate on the screen.</param>
        /// <param name="width">The rectangle's width.</param>
        /// <param name="height">The rectangle's height.</param>
        /// <param name="fill">Determines whether the rectangle should be filled or outlined (normal).</param>
        public void DrawRectangle(Color color, int x, int y, int width, int height, bool fill)
        {
            if (fill)
                driver.DoubleBuffer_DrawFillRectangle((uint)x, (uint)y, (uint)width, (uint)height, (uint)color.ToArgb());
            else
                driver.DoubleBuffer_DrawRectangle((uint)color.ToArgb(), x, y, width, height);

            driver.DoubleBuffer_Update();
        }
        /// <summary>
        /// Draws a rectangle.
        /// </summary>
        /// <param name="color">The rectangle color.</param>
        /// <param name="area">The Area instance containing the location for the rectangle.</param>
        public void DrawRectangle(Color color, Area area)
        {
            driver.DoubleBuffer_DrawRectangle((uint)color.ToArgb(), area.X, area.Y, area.Width, area.Height);
            driver.DoubleBuffer_Update();
        }
        /// <summary>
        /// Draws a filled or normal rectangle.
        /// </summary>
        /// <param name="color">The rectangle color.</param>
        /// <param name="area">The Area instance containing the location for the rectangle.</param>
        /// <param name="fill">Determines whether the rectangle should be filled or outlined (normal).</param>
        public void DrawRectangle(Color color, Area area, bool fill)
        {
            if (fill)
                driver.DoubleBuffer_DrawFillRectangle((uint)color.ToArgb(), (uint)area.X, (uint)area.Y, (uint)area.Width, (uint)area.Height);
            else
                driver.DoubleBuffer_DrawRectangle((uint)color.ToArgb(), area.X, area.Y, area.Width, area.Height);

            driver.DoubleBuffer_Update();
        }
        /// <summary>
        /// Draws a line.
        /// </summary>
        /// <param name="color">The color of the line.</param>
        /// <param name="startx">The X starting point.</param>
        /// <param name="starty">The Y starting point.</param>
        /// <param name="endx">The X endpoint.</param>
        /// <param name="endy">The Y endpoint.</param>
        public void DrawLine(Color color, int startx, int starty, int endx, int endy)
        {
            driver.DoubleBuffer_DrawLine((uint)color.ToArgb(), startx, starty, endx, endy);
            driver.DoubleBuffer_Update();
        }
        /// <summary>
        /// Draws a line based on two points.
        /// </summary>
        /// <param name="color">The line color.</param>
        /// <param name="startPoint">The starting point of the line.</param>
        /// <param name="endPoint">The ending point of the line.</param>
        public void DrawLine(Color color, Cosmos.System.Graphics.Point startPoint, Cosmos.System.Graphics.Point endPoint)
        {
            if (startPoint.X == -1 || endPoint.X == -1)
                throw new Exception("The points cannot be blank.");

            driver.DoubleBuffer_DrawLine((uint)color.ToArgb(), startPoint.X, startPoint.Y, endPoint.X, endPoint.Y);
            driver.DoubleBuffer_Update();
        }
        internal void DrawImage(Image image, int x, int y)
        {
            driver.DoubleBuffer_DrawImage(image, (uint)x, (uint)y);
            driver.DoubleBuffer_Update();
        }
        internal void DrawImage(Image image, Cosmos.System.Graphics.Point point)
        {
            driver.DoubleBuffer_DrawImage(image, (uint)point.X, (uint)point.Y);
            driver.DoubleBuffer_Update();
        }
        /// <summary>
        /// Runs the mouse.
        /// </summary>
        public void RunMouse()
        {
            if (Internal.mouseActivated)
            {
                Internal.RunMouse();
                UpdateScreen();
                Mouse.DrawCursor(MouseManager.X, MouseManager.Y);
                driver.DoubleBuffer_Update();
            }    
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
                else if (screen.Controls[i].controlType == ControlType.TextBox)
                {
                    tcount++;
                    TextBox tbox = ((TextBox)screen.Controls[i]);
                    tboxes.Add(tbox);
                    driver.DoubleBuffer_DrawFillRectangle((uint)tbox.X, (uint)tbox.Y, (uint)(tbox.cLength * 8) + 4, 15, (uint)tbox.backColor.ToArgb());  
                }
            }
            driver.DoubleBuffer_Update();
            
            if (tcount >= 1)
            {
                TextBox first = ((TextBox)screen.Controls[Internal.GetFirstTextBox(screen.Controls)]);
                TextBoxFocus(tboxes, first.X + 1, first.Y, tcount);
            }
        }

        private int tcount = 0;
        private int index = 0;
        private List<TextBox> tboxes = new List<TextBox>();
        private int s = 0;

        private void TextBoxFocus(List<TextBox> tboxes, int x, int y, int tcount)
        {
            if (tcount >= 1)
            {
                TextBox first = tboxes[index];
                index++;
                if (first.txt.ToString() == "")
                {
                    driver.DoubleBuffer_DrawFillRectangle((uint)x, (uint)y + 13, 8, 2, (uint)first.foreColor.ToArgb());
                    driver.DoubleBuffer_Update();
                }
                else
                {
                    if (first.txt.Length == first.cLength)
                    {
                        //Draw vertical cursor:
                        driver.DoubleBuffer_DrawFillRectangle((uint)(x + (first.txt.Length * 8) + 1), (uint)y, 2, 15, (uint)first.foreColor.ToArgb());
                        driver.DoubleBuffer_Update();
                        x = x + (first.txt.Length * 8);
                    }
                    else
                    {
                        x = (x + (first.txt.Length * 8));
                        driver.DoubleBuffer_DrawFillRectangle((uint)x, (uint)y + 13, 8, 2, (uint)first.foreColor.ToArgb());
                        driver.DoubleBuffer_Update();
                    } 
                }
                bool rkey = true;
                while (rkey)
                {
                    ConsoleKeyInfo info = System.Console.ReadKey(true);
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
                                        driver.DoubleBuffer_DrawFillRectangle((uint)x, (uint)y + 13, 8, 2, (uint)first.backColor.ToArgb());
                                        driver.DoubleBuffer_Update();
                                        x += 8;
                                        if (first.txt.Length == (first.cLength - 1))
                                        {
                                            //Draw cursor up one character in vertical state:
                                            driver.DoubleBuffer_DrawFillRectangle((uint)x + 1, (uint)y, 2, 15, (uint)first.foreColor.ToArgb());
                                            driver.DoubleBuffer_Update();
                                        }
                                        else
                                        {
                                            //Draw cursor up one character:
                                            driver.DoubleBuffer_DrawFillRectangle((uint)x, (uint)y + 13, 8, 2, (uint)first.foreColor.ToArgb());
                                            driver.DoubleBuffer_Update();
                                        }

                                        if (first.Mask == '~')
                                        {
                                            //Draw character entered:
                                            driver._DrawACSIIString(info.KeyChar.ToString(), (uint)first.foreColor.ToArgb(), (uint)x - 8, (uint)y);
                                            driver.DoubleBuffer_Update();
                                        }
                                        else
                                        {
                                            //Draw mask character:
                                            driver._DrawACSIIString(first.Mask.ToString(), (uint)first.foreColor.ToArgb(), (uint)x - 8, (uint)y);
                                            driver.DoubleBuffer_Update();
                                        }

                                        first.txt.Append(info.KeyChar);
                                    }
                                    else
                                    {
                                        System.Console.Beep();
                                    }
                                }
                                else
                                {
                                    //Erase cursor:
                                    driver.DoubleBuffer_DrawFillRectangle((uint)x, (uint)y + 13, 8, 2, (uint)first.backColor.ToArgb());
                                    driver.DoubleBuffer_Update();
                                    x += 8;
                                    if (first.txt.Length == (first.cLength - 1))
                                    {
                                        //Draw cursor up one character in vertical state:
                                        driver.DoubleBuffer_DrawFillRectangle((uint)x + 1, (uint)y, 2, 15, (uint)first.foreColor.ToArgb());
                                        driver.DoubleBuffer_Update();
                                    }
                                    else
                                    {
                                        //Draw cursor up one character:
                                        driver.DoubleBuffer_DrawFillRectangle((uint)x, (uint)y + 13, 8, 2, (uint)first.foreColor.ToArgb());
                                        driver.DoubleBuffer_Update();
                                    }

                                    if (first.Mask == '~')
                                    {
                                        //Draw character entered:
                                        driver._DrawACSIIString(info.KeyChar.ToString(), (uint)first.foreColor.ToArgb(), (uint)x - 8, (uint)y);
                                        driver.DoubleBuffer_Update();
                                    }
                                    else
                                    {
                                        //Draw mask character:
                                        driver._DrawACSIIString(first.Mask.ToString(), (uint)first.foreColor.ToArgb(), (uint)x - 8, (uint)y);
                                        driver.DoubleBuffer_Update();
                                    }

                                    first.txt.Append(info.KeyChar);
                                }                     
                            }
                            else
                            {
                                System.Console.Beep();
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
                                            driver._DrawACSIIString(first.Mask.ToString(), (uint)first.backColor.ToArgb(), (uint)x - 8, (uint)y);
                                        }
                                        else
                                        {
                                            driver._DrawACSIIString(first.txt[first.txt.Length - 1].ToString(), (uint)first.backColor.ToArgb(), (uint)x - 8, (uint)y);
                                        }           
                                        driver.DoubleBuffer_Update();
                                        if (first.txt.Length == first.cLength)
                                        {
                                            //Erase vertical cursor:
                                            driver.DoubleBuffer_DrawFillRectangle((uint)x + 1, (uint)y, 2, 15, (uint)first.backColor.ToArgb());
                                            driver.DoubleBuffer_Update();
                                            x -= 8;
                                        }
                                        else
                                        {
                                            //Erase cursor:
                                            driver.DoubleBuffer_DrawFillRectangle((uint)x, (uint)y + 13, 8, 2, (uint)first.backColor.ToArgb());
                                            driver.DoubleBuffer_Update();
                                            x -= 8;
                                        }   
                                        //Draw cursor one character back:
                                        driver.DoubleBuffer_DrawFillRectangle((uint)x, (uint)y + 13, 8, 2, (uint)first.foreColor.ToArgb());
                                        driver.DoubleBuffer_Update();
                                        first.txt.Remove(first.txt.Length - 1, 1);
                                    }
                                    else
                                    {
                                        System.Console.Beep();
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
                                            driver.DoubleBuffer_DrawFillRectangle((uint)x + 1, (uint)y, 2, 15, (uint)first.backColor.ToArgb());
                                            driver.DoubleBuffer_Update();
                                        }
                                        else
                                        {
                                            //Erase cursor:
                                            driver.DoubleBuffer_DrawFillRectangle((uint)x, (uint)y + 13, 8, 2, (uint)first.backColor.ToArgb());
                                            driver.DoubleBuffer_Update();
                                        }
                                        //Loop back through:
                                        TextBoxFocus(tboxes, tboxes[index].X + 1, tboxes[index].Y, tcount);
                                    }
                                    else
                                    {
                                        //Erase cursor:
                                        if (first.txt.Length == first.cLength)
                                        {
                                            //Erase vertical cursor:
                                            driver.DoubleBuffer_DrawFillRectangle((uint)x + 1, (uint)y, 2, 15, (uint)first.backColor.ToArgb());
                                            driver.DoubleBuffer_Update();
                                        }
                                        else
                                        {
                                            //Erase cursor:
                                            driver.DoubleBuffer_DrawFillRectangle((uint)x, (uint)y + 13, 8, 2, (uint)first.backColor.ToArgb());
                                            driver.DoubleBuffer_Update();
                                        }
                                        //Move back to the first one:
                                        index = 0;
                                        TextBoxFocus(tboxes, tboxes[index].X + 1, tboxes[index].Y, tcount);
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
        /// Represents a textbox.
        /// </summary>
        TextBox,
        /// <summary>
        /// Represents a masked textbox.
        /// </summary>
        MaskedTextBox
    }
    internal class Internal
    {
        public static bool mouseActivated = false;
        public static bool L_Pressed = false;
        public static bool R_Pressed = false;
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
        public static void RunMouse()
        {
            switch (MouseManager.MouseState)
            {
                case MouseState.Left:
                    R_Pressed = false;
                    L_Pressed = true;
                    break;
                case MouseState.Right:
                    L_Pressed = false;
                    R_Pressed = true;
                    break;
                case MouseState.None:
                    L_Pressed = false;
                    R_Pressed = false;
                    break;
            }
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
