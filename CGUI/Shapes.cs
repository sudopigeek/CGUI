using System.Drawing;

namespace CGUI.Shapes
{
    /// <summary>
    /// The Line control.
    /// </summary>
    public class Line : Control
    {
        internal int EndX { get; set; }
        internal int EndY { get; set; }
        /// <summary>
        /// The line's color.
        /// </summary>
        public Color Color { get; set; }
        /// <summary>
        /// Starts a new instance of the Line class.
        /// </summary>
        /// <param name="color">The color of the line.</param>
        /// <param name="startx">The start X coordinate.</param>
        /// <param name="starty">The start Y coordinate.</param>
        /// <param name="endx">The ending X coordinate.</param>
        /// <param name="endy">The ending Y coordinate.</param>
        public Line(Color color, int startx, int starty, int endx, int endy)
        {
            controlType = ControlType.Line;
            Color = color;
            X = startx;
            Y = starty;
            EndX = endx;
            EndY = endy;
        }
        /// <summary>
        /// Updates the line.
        /// </summary>
        public void Update()
        {
            VGADriver.driver.DoubleBuffer_DrawLine((uint)Color.ToArgb(), X, Y, EndX, EndY);
            VGADriver.driver.DoubleBuffer_Update();
        }
    }
    /// <summary>
    /// The Rectangle control.
    /// </summary>
    public class Rectangle : Control
    {
        /// <summary>
        /// The rectangle's color.
        /// </summary>
        public Color Color { get; set; }
        internal int Width;
        internal int Height;
        internal bool Fill;
        /// <summary>
        /// Starts a new instance of the Rectangle class, representing a filled rectangle.
        /// </summary>
        /// <param name="x">The rectangle's X coordinate.</param>
        /// <param name="y">The rectangle's Y coordinate.</param>
        /// <param name="width">The rectangle's width.</param>
        /// <param name="height">The rectangle's height.</param>
        /// <param name="color">The rectangle's color.</param>
        public Rectangle(int x, int y, int width, int height, Color color)
        {
            controlType = ControlType.Rectangle;
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Color = color;
            Fill = true;
        }
        /// <summary>
        /// Starts a new instance of the Rectangle class, representing a filled or empty rectangle.
        /// </summary>
        /// <param name="x">The rectangle's X coordinate.</param>
        /// <param name="y">The rectangle's Y coordinate.</param>
        /// <param name="width">The rectangle's width.</param>
        /// <param name="height">The rectangle's height.</param>
        /// <param name="color">The rectangle's color.</param>
        /// <param name="fill">Set to true if the rectangle should be filled; otherwise, false.</param>
        public Rectangle(int x, int y, int width, int height, Color color, bool fill)
        {
            controlType = ControlType.Rectangle;
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Color = color;
            Fill = fill;
        }
        /// <summary>
        /// Updates the rectangle.
        /// </summary>
        public void Update()
        {
            if (Fill)
            {
                VGADriver.driver.DoubleBuffer_DrawFillRectangle((uint)X, (uint)Y, (uint)Width, (uint)Height, (uint)Color.ToArgb());
            }
            else
            {
                VGADriver.driver.DoubleBuffer_DrawRectangle((uint)Color.ToArgb(), X, Y, Width, Height);
            }
            VGADriver.driver.DoubleBuffer_Update();
        }
    }
}
