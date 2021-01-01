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
        /// <param name="x">The text's X position.</param>
        /// <param name="y">The text's Y position.</param>
        /// <returns>A Screen instance that can be displayed with VGADriver.RenderScreen().</returns>
        public static Screen StartupScreen(string text, int x, int y)
        {
            return new Screen()
            {
                Controls = new ControlList()
                {
                    {new Label(text, x, y)}
                }
            };
        }
        /// <summary>
        /// Represents a simple startup screen containing a message at the specified alignments.
        /// </summary>
        /// <param name="text">The text to display.</param>
        /// <param name="verticalAlign">The text's vertical alignment.</param>
        /// <param name="horizontalAlign">The text's horizontal alignment.</param>
        /// <returns></returns>
        public static Screen StartupScreen(string text, VerticalAlign verticalAlign, HorizontalAlign horizontalAlign)
        {
            return new Screen()
            {
                Controls = new ControlList()
                {
                    {new Label(text, horizontalAlign.GetValue(text), verticalAlign.GetValue(text))}
                }
            };
        }
    }
}
