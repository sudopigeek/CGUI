using Cosmos.System;
using System;
using System.Collections.Generic;
using System.Drawing;
using CGUI.Shapes;
using System.Text;

namespace CGUI
{
    /// <summary>
    /// Represents a mouse.
    /// </summary>
    public class Mouse
    {
        /// <summary>
        /// Raised when the mouse is clicked.
        /// </summary>
        public event EventHandler OnClick
        {
            add
            {
                OnClick_Handler = value;
            }
            remove
            {
                OnClick_Handler -= value;
            }
        }
        /// <summary>
        /// Raised when the mouse is right-clicked.
        /// </summary>
        public event EventHandler OnRightClick
        {
            add
            {
                OnRightClick_Handler = value;
            }
            remove
            {
                OnRightClick_Handler -= value;
            }
        }
        internal EventHandler OnClick_Handler;
        internal EventHandler OnRightClick_Handler;
        internal Mouse()
        {
            MouseManager.ScreenWidth = (uint)Internal.screenWidth;
            MouseManager.ScreenHeight = (uint)Internal.screenHeight;
            MouseManager.X = (uint)Internal.screenWidth / 2;
            MouseManager.Y = (uint)Internal.screenHeight / 2;
        }
        internal static int[] Cursor = new int[]
        {
            1,0,0,0,0,0,0,0,0,0,0,0,
            1,1,0,0,0,0,0,0,0,0,0,0,
            1,2,1,0,0,0,0,0,0,0,0,0,
            1,2,2,1,0,0,0,0,0,0,0,0,
            1,2,2,2,1,0,0,0,0,0,0,0,
            1,2,2,2,2,1,0,0,0,0,0,0,
            1,2,2,2,2,2,1,0,0,0,0,0,
            1,2,2,2,2,2,2,1,0,0,0,0,
            1,2,2,2,2,2,2,2,1,0,0,0,
            1,2,2,2,2,2,2,2,2,1,0,0,
            1,2,2,2,2,2,2,2,2,2,1,0,
            1,2,2,2,2,2,2,2,2,2,2,1,
            1,2,2,2,2,2,2,1,1,1,1,1,
            1,2,2,2,1,2,2,1,0,0,0,0,
            1,2,2,1,0,1,2,2,1,0,0,0,
            1,2,1,0,0,1,2,2,1,0,0,0,
            1,1,0,0,0,0,1,2,2,1,0,0,
            0,0,0,0,0,0,1,2,2,1,0,0,
            0,0,0,0,0,0,0,1,1,0,0,0
        };
        internal static int[] Text = new int[]
        {
            2,2,2,2,2,0,2,2,2,2,2,
            0,0,0,0,0,2,0,0,0,0,0,
            0,0,0,0,0,2,0,0,0,0,0,
            0,0,0,0,0,2,0,0,0,0,0,
            0,0,0,0,0,2,0,0,0,0,0,
            0,0,0,0,0,2,0,0,0,0,0,
            0,0,0,0,0,2,0,0,0,0,0,
            0,0,0,0,0,2,0,0,0,0,0,
            0,0,0,0,0,2,0,0,0,0,0,
            0,0,0,0,0,2,0,0,0,0,0,
            0,0,0,0,0,2,0,0,0,0,0,
            0,0,0,0,0,2,0,0,0,0,0,
            0,0,0,0,0,2,0,0,0,0,0,
            0,0,0,0,0,2,0,0,0,0,0,
            0,0,0,0,0,2,0,0,0,0,0,
            0,0,0,0,0,2,0,0,0,0,0,
            0,0,0,0,0,2,0,0,0,0,0,
            0,0,0,0,0,2,0,0,0,0,0,
            2,2,2,2,2,0,2,2,2,2,2,
        };
        internal static int[] Hand = new int[]
        {
            0,0,0,0,1,1,0,0,0,0,0,0,0,
            0,0,0,1,2,2,1,0,0,0,0,0,0,
            0,0,0,1,2,2,1,0,0,0,0,0,0,
            0,0,0,1,2,2,1,0,0,0,0,0,0,
            0,0,0,1,2,2,1,0,0,0,0,0,0,
            0,0,0,1,2,2,1,0,0,0,0,0,0,
            0,0,0,1,2,2,1,1,1,0,0,0,0,
            0,0,0,1,2,2,1,2,1,1,1,0,0,
            0,0,0,1,2,2,1,2,1,2,1,1,1,
            1,1,0,1,0,2,1,2,1,2,1,2,1,
            1,2,1,1,0,2,2,2,2,2,1,2,1,
            1,2,2,1,2,2,2,2,2,2,2,2,1,
            1,2,2,2,2,2,2,2,2,2,2,2,1,
            0,1,2,2,2,2,2,2,2,2,2,2,1,
            0,1,2,2,2,2,2,2,2,2,2,1,0,
            0,0,1,2,2,2,2,2,2,2,2,1,0,
            0,0,1,2,2,2,2,2,2,2,2,1,0,
            0,0,0,1,2,2,2,2,2,2,1,0,0,
            0,0,0,1,1,1,1,1,1,1,1,0,0
        };
        internal static Control DetectControl(Screen s)
        {
            foreach (Control control in s.Controls)
            {
                switch (control.controlType)
                {
                    case ControlType.Button:
                        if (MouseManager.X >= control.X && MouseManager.Y >= control.Y && MouseManager.X <= (control.X + (((Button)control).txt.Length * 8) + 20) && MouseManager.Y <= (control.Y + 13 + 10))
                        {
                            return control;
                        }
                        break;
                    case ControlType.Label:
                        if (MouseManager.X >= control.X && MouseManager.Y >= control.Y && MouseManager.X <= (control.X + (((Label)control).Text.Length * 8)) && MouseManager.Y <= (control.Y + 13))
                        {
                            return control;
                        }
                        break;
                    case ControlType.Line:
                        if (MouseManager.X >= control.X && MouseManager.Y >= control.Y && MouseManager.X <= ((Line)control).EndX && MouseManager.Y <= ((Line)control).EndY)
                        {
                            return control;
                        }
                        break;
                    case ControlType.Picture:
                        if (MouseManager.X >= control.X && MouseManager.Y >= control.Y && MouseManager.X <= (control.X + ((Picture)control).Image.Width) && MouseManager.Y <= (control.Y + ((Picture)control).Image.Height))
                        {
                            return control;
                        }
                        break;
                    case ControlType.Rectangle:
                        if (MouseManager.X >= control.X && MouseManager.Y >= control.Y && MouseManager.X <= (control.X + ((Shapes.Rectangle)control).Width) && MouseManager.Y <= ((Shapes.Rectangle)control).Height)
                        {
                            return control;
                        }
                        break;
                    case ControlType.TextBox:
                        if (MouseManager.X >= control.X && MouseManager.Y >= control.Y && MouseManager.X <= ((((TextBox)control).cLength * 8) + 4 + control.X) && MouseManager.Y <= (control.Y + 15))
                        {
                            return control;
                        }
                        break;
                }
            }
            return null;
        }
        internal static bool DetectControls(Screen s)
        {
            foreach (Control control in s.Controls)
            {
                switch (control.controlType)
                {
                    case ControlType.Button:
                        if (MouseManager.X >= control.X && MouseManager.Y >= control.Y && MouseManager.X <= (control.X + (((Button)control).txt.Length * 8) + 20) && MouseManager.Y <= (control.Y + 13 + 10))
                        {
                            return true;
                        }
                        break;
                    case ControlType.Label:
                        if (MouseManager.X >= control.X && MouseManager.Y >= control.Y && MouseManager.X <= (control.X + (((Label)control).Text.Length * 8)) && MouseManager.Y <= (control.Y + 13))
                        {
                            return true;
                        }
                        break;
                    case ControlType.Line:
                        if (MouseManager.X >= control.X && MouseManager.Y >= control.Y && MouseManager.X <= ((Line)control).EndX && MouseManager.Y <= ((Line)control).EndY)
                        {
                            return true;
                        }
                        break;
                    case ControlType.Picture:
                        if (MouseManager.X >= control.X && MouseManager.Y >= control.Y && MouseManager.X <= (control.X + ((Picture)control).Image.Width) && MouseManager.Y <= (control.Y + ((Picture)control).Image.Height))
                        {
                            return true;
                        }
                        break;
                    case ControlType.Rectangle:
                        if (MouseManager.X >= control.X && MouseManager.Y >= control.Y && MouseManager.X <= (control.X + ((Shapes.Rectangle)control).Width) && MouseManager.Y <= ((Shapes.Rectangle)control).Height)
                        {
                            return true;
                        }
                        break;
                    case ControlType.TextBox:
                        if (MouseManager.X >= control.X && MouseManager.Y >= control.Y && MouseManager.X <= ((((TextBox)control).cLength * 8) + 4 + control.X) && MouseManager.Y <= (control.Y + 15))
                        {
                            return true;
                        }
                        break;
                }
            }            
            return false;
        }
        internal static Control GetControl(Screen s)
        {
            foreach (Control control in s.Controls)
            {
                switch (control.controlType)
                {
                    case ControlType.Button:
                        if (MouseManager.X >= control.X && MouseManager.Y >= control.Y && MouseManager.X <= (control.X + (((Button)control).txt.Length * 8) + 20) && MouseManager.Y <= (control.Y + 13 + 10))
                        {
                            return control;
                        }
                        break;
                    case ControlType.Label:
                        if (MouseManager.X >= control.X && MouseManager.Y >= control.Y && MouseManager.X <= (control.X + (((Label)control).Text.Length * 8)) && MouseManager.Y <= (control.Y + 13))
                        {
                            return control;
                        }
                        break;
                    case ControlType.Line:
                        if (MouseManager.X >= control.X && MouseManager.Y >= control.Y && MouseManager.X <= ((Line)control).EndX && MouseManager.Y <= ((Line)control).EndY)
                        {
                            return control;
                        }
                        break;
                    case ControlType.Picture:
                        if (MouseManager.X >= control.X && MouseManager.Y >= control.Y && MouseManager.X <= (control.X + ((Picture)control).Image.Width) && MouseManager.Y <= (control.Y + ((Picture)control).Image.Height))
                        {
                            return control;
                        }
                        break;
                    case ControlType.Rectangle:
                        if (MouseManager.X >= control.X && MouseManager.Y >= control.Y && MouseManager.X <= (control.X + ((Shapes.Rectangle)control).Width) && MouseManager.Y <= ((Shapes.Rectangle)control).Height)
                        {
                            return control;
                        }
                        break;
                    case ControlType.TextBox:
                        if (MouseManager.X >= control.X && MouseManager.Y >= control.Y && MouseManager.X <= ((((TextBox)control).cLength * 8) + 4 + control.X) && MouseManager.Y <= (control.Y + 15))
                        {
                            return control;
                        }
                        break;
                }
            }
            return null;
        }
        internal static void DrawCursor(CursorType type, uint x, uint y)
        {
            switch (type)
            {
                case CursorType.Hand:
                    for (uint h = 0; h < 19; h++)
                    {
                        for (uint w = 0; w < 13; w++)
                        {
                            if (Hand[h * 13 + w] == 1)
                            {
                                VGADriver.driver.DoubleBuffer_SetPixel(w + x, h + y, (uint)Color.Black.ToArgb());
                            }
                            if (Hand[h * 13 + w] == 2)
                            {
                                VGADriver.driver.DoubleBuffer_SetPixel(w + x, h + y, (uint)Color.White.ToArgb());
                            }
                        }
                    }
                    break;
                case CursorType.Normal:
                    for (uint h = 0; h < 19; h++)
                    {
                        for (uint w = 0; w < 12; w++)
                        {
                            if (Cursor[h * 12 + w] == 1)
                            {
                                VGADriver.driver.DoubleBuffer_SetPixel(w + x, h + y, (uint)Color.Black.ToArgb());
                            }
                            if (Cursor[h * 12 + w] == 2)
                            {
                                VGADriver.driver.DoubleBuffer_SetPixel(w + x, h + y, (uint)Color.White.ToArgb());
                            }
                        }
                    }
                    break;
                case CursorType.Text:
                    for (uint h = 0; h < 19; h++)
                    {
                        for (uint w = 0; w < 11; w++)
                        {
                            if (Text[h * 11 + w] == 1)
                            {
                                VGADriver.driver.DoubleBuffer_SetPixel(w + x, h + y, (uint)Color.Black.ToArgb());
                            }
                            if (Text[h * 11 + w] == 2)
                            {
                                VGADriver.driver.DoubleBuffer_SetPixel(w + x, h + y, (uint)Color.White.ToArgb());
                            }
                        }
                    }
                    break;
            }
        }
    }
    /// <summary>
    /// Represents cursor types.
    /// </summary>
    public enum CursorType
    {
        /// <summary>
        /// The normal mouse cursor.
        /// </summary>
        Normal,
        /// <summary>
        /// The I-beam shaped cursor.
        /// </summary>
        Text,
        /// <summary>
        /// The hand-shaped cursor.
        /// </summary>
        Hand
    }
}
