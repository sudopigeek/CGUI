using System.Drawing;

namespace CGUI
{
    /// <summary>
    /// Double-Buffered SVGAII Video Driver
    /// </summary>
    public partial class VGADriver
    {        
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
            currentScreen = screen;
            currentControl = null;
            tabControls.Clear();
            tcount = 0;
            index = 0;
            ValidateControls();
            Internal.screenColor = screen.backColor;
            driver.DoubleBuffer_DrawFillRectangle(0, 0, (uint)Internal.screenWidth, (uint)Internal.screenHeight, (uint)screen.backColor.ToArgb());
            for (int i = 0; i < screen.Controls.Count; i++)
            {
                DrawControl(screen.Controls[i]);
            }
            driver.DoubleBuffer_Update();

            if (tcount >= 1)
            {
                int f = Internal.GetFirstTabControl(screen.Controls);
                if (f > -1)
                {
                    Control c = screen.Controls[f];
                    Focus(c, c.X, c.Y, tcount);
                }   
            }
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
        public ControlList Controls { get; set; }
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
            Controls = new ControlList();
        }
        /// <summary>
        /// Starts a new instance of the Screen class with the specified background color.
        /// </summary>
        /// <param name="backColor">The background color for the screen.</param>
        public Screen(Color backColor)
        {
            this.backColor = backColor;
            Controls = new ControlList();
        }
    }
    /// <summary>
    /// Represents types of currently defined controls.
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
    /// <summary>
    /// Represents horizontal alignments.
    /// </summary>
    public enum HorizontalAlign
    {
        /// <summary>
        /// Center align.
        /// </summary>
        Center,
        /// <summary>
        /// Left align.
        /// </summary>
        Left,
        /// <summary>
        /// Right align.
        /// </summary>
        Right
    }
    /// <summary>
    /// Represents vertical alignments.
    /// </summary>
    public enum VerticalAlign
    {
        /// <summary>
        /// Top align.
        /// </summary>
        Top,
        /// <summary>
        /// Middle align.
        /// </summary>
        Middle,
        /// <summary>
        /// Bottom align.
        /// </summary>
        Bottom
    }
      
    /// <summary>
    /// Internal base class for CGUI controls.
    /// </summary>
    public class Control
    {
        internal ControlType controlType { get; set; }
        internal int X { get; set; }
        internal int Y { get; set; }
        internal bool? Enabled { get; set; } = null;
    }
}
