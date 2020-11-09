using Cosmos.System;
using System.Drawing;

namespace CGUI
{
    /// <summary>
    /// Class for initiating a mouse.
    /// </summary>
    public class Mouse
    {
        /// <summary>
        /// Equals true when the left button on the mouse is pressed.
        /// </summary>
        public bool LeftClick
        {
            get
            {
                return Internal.L_Pressed;
            }
        }
        /// <summary>
        /// Equals true when the right button on the mouse is pressed.
        /// </summary>
        public bool RightClick
        {
            get
            {
                return Internal.R_Pressed;
            }
        }
        /// <summary>
        /// The mouse's pointer type.
        /// </summary>
        public PointerType Pointer { get; }
        internal static PointerType pointer;
        /// <summary>
        /// Starts a new instance of the Mouse class using the default pointer (normal).
        /// </summary>
        public Mouse()
        {
            MouseManager.ScreenWidth = (uint)Internal.screenWidth;
            MouseManager.ScreenHeight = (uint)Internal.screenHeight;
            MouseManager.X = (uint)Internal.screenWidth / 2;
            MouseManager.Y = (uint)Internal.screenHeight / 2;
            Internal.mouseActivated = true;
            Pointer = PointerType.Normal;
            pointer = PointerType.Normal;
        }
        /// <summary>
        /// Changes how the pointer/mouse looks.
        /// </summary>
        /// <param name="type">The pointer type to change to.</param>
        public void SetPointer(PointerType type)
        {
            pointer = type;
        }

        private static int[] mouse_Normal = new int[]
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
        private static int[] mouse_Text = new int[]
            {
                0,0,2,2,2,0,2,2,2,0,0,
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
                0,0,2,2,2,0,2,2,2,0,0,
        };
        internal static void DrawCursor(uint x, uint y)
        {
            switch (pointer)
            {
                case PointerType.Normal:
                    for (uint h = 0; h < 19; h++)
                    {
                        for (uint w = 0; w < 12; w++)
                        {
                            if (mouse_Normal[h * 12 + w] == 1)
                            {
                                VGADriver.driver.DoubleBuffer_SetPixel(w + x, h + y, (uint)Color.Black.ToArgb());
                            }
                            if (mouse_Normal[h * 12 + w] == 2)
                            {
                                VGADriver.driver.DoubleBuffer_SetPixel(w + x, h + y, (uint)Color.White.ToArgb());
                            }
                        }
                    }
                    break;
                case PointerType.Text:
                    for (uint h = 0; h < 19; h++)
                    {
                        for (uint w = 0; w < 11; w++)
                        {
                            if (mouse_Text[h * 11 + w] == 1)
                            {
                                VGADriver.driver.DoubleBuffer_SetPixel(w + x, h + y, (uint)Color.Black.ToArgb());
                            }
                            if (mouse_Text[h * 11 + w] == 2)
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
    /// Represents a type of mouse pointer.
    /// </summary>
    public enum PointerType
    {
        /// <summary>
        /// Normal mouse pointer (arrow).
        /// </summary>
        Normal,
        /// <summary>
        /// I-Beam shaped pointer normally shown when using a text editor.
        /// </summary>
        Text
    }
}
