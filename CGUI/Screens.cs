using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace CGUI
{
    /// <summary>
    /// Class containing pre-made screens.
    /// </summary>
    public class Screens
    {
        /// <summary>
        /// Represents a simple startup screen containing a message.
        /// </summary>
        /// <param name="text">The text to display.</param>
        /// <param name="x">The texts' X position.</param>
        /// <param name="y">The texts' Y position.</param>
        /// <returns>A Screen instance that can be displayed with VGADriver.RenderScreen()</returns>
        public static Screen StartupScreen(string text = "Starting up...", int x = 0, int y = 0)
        {
            return new Screen()
            {
                Controls = new ControlList()
                {
                    {new Label(text, x, y)}
                }
            };
        }
    }
}
