using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Cosmos.System.Graphics;

namespace CGUI
{
    /// <summary>
    /// The Label control.
    /// </summary>
    public class Label : Control
    {
        /// <summary>
        /// The label's text.
        /// </summary>
        public string Text { get; internal set; }
        /// <summary>
        /// The text color of the label.
        /// </summary>
        public Color foreColor { get; set; }
        internal Color backColor { get; set; } = Color.Empty;
        private string prevText;
        /// <summary>
        /// Starts a new instance of the Label class.
        /// </summary>
        /// <param name="text">The label text.</param>
        /// <param name="foreColor">The text color.</param>
        /// <param name="x">The label's X coordinate.</param>
        /// <param name="y">The label's Y coordinate.</param>
        public Label(string text, Color foreColor, int x, int y)
        {
            controlType = ControlType.Label;
            X = x;
            Y = y;
            Text = text;
            prevText = text;
            this.foreColor = foreColor;
        }
        /// <summary>
        /// Starts a new instance of the Label class.
        /// </summary>
        /// <param name="text">The label text in the default color (white).</param>
        /// <param name="x">The label's X coordinate.</param>
        /// <param name="y">The label's Y coordinate.</param>
        public Label(string text, int x, int y)
        {
            controlType = ControlType.Label;
            X = x;
            Y = y;
            Text = text;
            prevText = text;
            foreColor = Color.White;
        }
        /// <summary>
        /// Starts a new instance of the Label class.
        /// </summary>
        /// <param name="text">The label text in the default color (white).</param>
        /// <param name="point">The point to place the label.</param>
        public Label(string text, Cosmos.System.Graphics.Point point)
        {
            controlType = ControlType.Label;
            X = point.X;
            Y = point.Y;
            Text = text;
            prevText = text;
            foreColor = Color.White;
        }
        /// <summary>
        /// Starts a new instance of the Label class.
        /// </summary>
        /// <param name="text">The label text in the default color (white).</param>
        /// <param name="foreColor">The text color.</param>
        /// <param name="point">The point to place the label.</param>
        public Label(string text, Color foreColor, Cosmos.System.Graphics.Point point)
        {
            controlType = ControlType.Label;
            X = point.X;
            Y = point.Y;
            Text = text;
            prevText = text;
            this.foreColor = foreColor;
        }
        /// <summary>
        /// Updates/changes the label's text.
        /// </summary>
        /// <param name="newText">The new text to display.</param>
        public void Update(string newText)
        {
            if (Text.Contains("\n"))
            {
                string[] lines = Text.Split("\n");
                int y = Y;
                for (int i2 = 0; i2 < lines.Length; i2++)
                {
                    VGADriver.driver._DrawACSIIString(lines[i2], (uint)backColor.ToArgb(), (uint)X, (uint)y);
                    VGADriver.driver.DoubleBuffer_Update();
                    y += 12;
                }
                
            }
            else
            {
                VGADriver.driver._DrawACSIIString(Text, (uint)backColor.ToArgb(), (uint)X, (uint)Y);
                VGADriver.driver.DoubleBuffer_Update();              
            }

            if (newText.Contains("\n"))
            {
                string[] lines = newText.Split("\n");
                int y = Y;
                for (int i = 0; i < lines.Length; i++)
                {
                    VGADriver.driver._DrawACSIIString(lines[i], (uint)foreColor.ToArgb(), (uint)X, (uint)y);
                    VGADriver.driver.DoubleBuffer_Update();
                    y += 12;
                }
            }
            else
            {
                VGADriver.driver._DrawACSIIString(newText, (uint)foreColor.ToArgb(), (uint)X, (uint)Y);
                VGADriver.driver.DoubleBuffer_Update();
            }
            prevText = Text;
            Text = newText; 
        }
    }
    /// <summary>
    /// The Picture control.
    /// </summary>
    public class Picture : Control
    {
        /// <summary>
        /// The control's image.
        /// </summary>
        public Image Image { get; set; }
        /// <summary>
        /// The border color around the image. This is off by default.
        /// </summary>
        public Color borderColor { get; set; } = Color.Empty;
        internal Image prevImage;
        /// <summary>
        /// Starts a new instance of the Picture class.
        /// </summary>
        /// <param name="image">The image to use.</param>
        /// <param name="x">The picture's X coordinate.</param>
        /// <param name="y">The picture's Y coordinate.</param>
        public Picture(Image image, int x, int y)
        {
            controlType = ControlType.Picture;
            Image = image;
            prevImage = image;
            X = x;
            Y = y;
        }
        /// <summary>
        /// Updates the picture.
        /// </summary>
        public void Update()
        {
            VGADriver.driver.DoubleBuffer_DrawFillRectangle((uint)X, (uint)Y, prevImage.Width, prevImage.Height, (uint)Internal.screenColor.ToArgb());
            VGADriver.driver.DoubleBuffer_Update();
            VGADriver.driver.DoubleBuffer_DrawImage(Image, (uint)X, (uint)Y);
            prevImage = Image;
            VGADriver.driver.DoubleBuffer_Update();

            if (borderColor != Color.Empty)
                VGADriver.driver.DoubleBuffer_DrawLine((uint)borderColor.ToArgb(), X - 1, Y - 1, X + (int)Image.Width + 1, Y - 1);
                VGADriver.driver.DoubleBuffer_DrawLine((uint)borderColor.ToArgb(), X - 1, Y - 1, X - 1, Y + (int)Image.Height + 1);
                VGADriver.driver.DoubleBuffer_DrawLine((uint)borderColor.ToArgb(), X - 1, Y + (int)Image.Height + 1, X + (int)Image.Width + 1, Y + (int)Image.Height + 1);
                VGADriver.driver.DoubleBuffer_DrawLine((uint)borderColor.ToArgb(), X + (int)Image.Width + 1, Y - 1, X + (int)Image.Width + 1, Y + (int)Image.Height + 1);
                VGADriver.driver.DoubleBuffer_Update();
        }
    }
    /// <summary>
    /// The TextBox control.
    /// </summary>
    public class TextBox : Control
    {
        /// <summary>
        /// The list of KeyPress instances representing extra/custom keys to listen for when the textbox has focus.
        /// </summary>
        public List<KeyPress> KeyPresses { get; set; }
        /// <summary>
        /// The foreground color of the textbox when it is not in focus.
        /// </summary>
        public Color UnfocusForeColor { get; set; } = Color.White;
        /// <summary>
        /// The background color of the textbox when it is not in focus.
        /// </summary>
        public Color UnfocusBackColor { get; set; } = Color.DarkBlue;
        /// <summary>
        /// The text color for the textbox (including the cursor).
        /// </summary>
        public Color ForeColor { get; set; } = Color.White;
        /// <summary>
        /// The background color for the textbox.
        /// </summary>
        public Color BackColor { get; set; } = Color.Blue;
        /// <summary>
        /// The character(s) that are allowed in the textbox. Set this to blank ("") to disable and allow all characters.
        /// </summary>
        public string Filter { get; set; } = "";
        /// <summary>
        /// The character to draw on the screen instead of the actual character. Set this to '~' to disable.
        /// </summary>
        public char Mask { get; set; } = '~';
        /// <summary>
        /// Determines whether or not to beep when attempting to type outside of the textbox's character limit.
        /// </summary>
        public bool BeepOnLimit { get; set; } = false;
        /// <summary>
        /// Gets the current text in the textbox.
        /// </summary>
        public string Text
        {
            get
            {
                return txt.ToString();
            }
        }
        internal StringBuilder txt = new StringBuilder("");
        internal int FocusOrder;
        internal int cLength;
        internal int pos = 0;
        internal EventHandler Limit;
        internal EventHandler<System.ConsoleKeyInfo> KeyPress;
        internal EventHandler Delete;
        /// <summary>
        /// Raised when a character is deleted from the textbox.
        /// </summary>
        public event EventHandler OnDelete
        {
            add
            {
                Delete = value;
            }
            remove
            {
                Delete -= value;
            }
        }
        /// <summary>
        /// Raised when a character is entered into the textbox, and returns the ConsoleKeyInfo instance.
        /// </summary>
        public event EventHandler<System.ConsoleKeyInfo> OnKeyPress
        {
            add
            {
                KeyPress = value;
            }
            remove
            {
                KeyPress -= value;
            }
        }
        /// <summary>
        /// Raised when the user attempts to enter or delete characters outside of the textbox's character range limit.
        /// </summary>
        public event EventHandler OnLimit
        {
            add
            {
                Limit = value;
            }
            remove
            {
                Limit -= value;
            }
        }
        /// <summary>
        /// Starts a new instance of the TextBox class.
        /// </summary>
        /// <param name="charLength">The number of characters allowed in the textbox.</param>
        /// <param name="foreColor">The text color.</param>
        /// <param name="x">The textbox's X coordinate.</param>
        /// <param name="y">The textbox's Y coordinate.</param>
        public TextBox(int charLength, Color foreColor, int x, int y)
        {
            controlType = ControlType.TextBox;
            KeyPresses = new List<KeyPress>();
            cLength = charLength;
            this.ForeColor = foreColor;
            X = x;
            Y = y;
        }
        /// <summary>
        /// Starts a new instance of the TextBox class.
        /// </summary>
        /// <param name="charLength">The number of characters allowed in the textbox.</param>
        /// <param name="x">The textbox's X coordinate.</param>
        /// <param name="y">The textbox's Y coordinate.</param>
        public TextBox(int charLength, int x, int y)
        {
            controlType = ControlType.TextBox;
            KeyPresses = new List<KeyPress>();
            cLength = charLength;
            X = x;
            Y = y;
        }
        /// <summary>
        /// Sets focus to the textbox.
        /// </summary>
        public void Focus()
        {
            if (VGADriver.driver != null)
            {
                VGADriver.FocusControl(this);
            }
        }
        /// <summary>
        /// Sets the specified text into the textbox.
        /// </summary>
        /// <param name="text">The text to put in the textbox.</param>
        public void SetText(string text)
        {
            VGADriver.SetText(this, text);
        }
    }
    /// <summary>
    /// The Button control.
    /// </summary>
    public class Button : Control
    {
        internal EventHandler OnEnter_Handler;
        /// <summary>
        /// The list of KeyPress instances representing extra/custom keys to listen for when the button has focus.
        /// </summary>
        public List<KeyPress> KeyPresses { get; set; }
        /// <summary>
        /// Raised when the TriggerKey is pressed.
        /// </summary>
        public event EventHandler OnEnter
        {
            add
            {
                OnEnter_Handler = value;
            }
            remove
            {
                OnEnter_Handler -= value;
            }
        }
        /// <summary>
        /// The text color when the button is not in focus.
        /// </summary>
        public Color UnfocusTextColor { get; set; } = Color.Gray;
        /// <summary>
        /// The background color when the button is not in focus.
        /// </summary>
        public Color UnfocusBackColor { get; set; } = Color.LightGray;
        /// <summary>
        /// The key that the button listens for (default is Enter/Return). When the key is pressed, the OnEnter event is raised.
        /// </summary>
        public ConsoleKey TriggerKey { get; set; } = ConsoleKey.Enter;
        /// <summary>
        /// The text color.
        /// </summary>
        public Color TextColor { get; set; } = Color.Black;
        /// <summary>
        /// The background color.
        /// </summary>
        public Color BackColor { get; set; } = Color.White;
        internal string txt = "";
        /// <summary>
        /// Starts a new instance of the Button class.
        /// </summary>
        /// <param name="text">The button text.</param>
        /// <param name="x">The button's X coordinate.</param>
        /// <param name="y">The button's Y coordinate.</param>
        public Button(string text, int x, int y)
        {
            controlType = ControlType.Button;
            KeyPresses = new List<KeyPress>();
            txt = text;
            X = x;
            Y = y;            
        }
        /// <summary>
        /// Starts a new instance of the Button class.
        /// </summary>
        /// <param name="text">The button text.</param>
        /// <param name="triggerKey">The key that the button listens for (default is Enter/Return). When the key is pressed, the OnEnter event is raised.</param>
        /// <param name="x">The button's X coordinate.</param>
        /// <param name="y">The button's Y coordinate.</param>
        public Button(string text, ConsoleKey triggerKey, int x, int y)
        {
            controlType = ControlType.Button;
            KeyPresses = new List<KeyPress>();
            txt = text;
            TriggerKey = triggerKey;
            X = x;
            Y = y;
        }
        /// <summary>
        /// Sets focus to the button.
        /// </summary>
        public void Focus()
        {
            if (VGADriver.driver != null)
            {
                VGADriver.FocusControl(this);
            }
        }
    }
    /// <summary>
    /// Represents a keypress.
    /// </summary>
    public class KeyPress
    {
        internal EventHandler<ConsoleKeyInfo> OnPress_Handler;
        /// <summary>
        /// The ConsoleKeyInfo instance representing the key with its modifiers, if any.
        /// </summary>
        public ConsoleKeyInfo ConsoleKeyInfo { get; }
        /// <summary>
        /// Raised when the key and any modifiers associated with it is pressed.
        /// </summary>
        public event EventHandler<ConsoleKeyInfo> OnPress
        {
            add
            {
                OnPress_Handler = value;
            }
            remove
            {
                OnPress_Handler -= value;
            }
        }
        /// <summary>
        /// Starts a new instance of the KeyPress class using the specified key and modifiers.
        /// </summary>
        /// <param name="key">The ConsoleKey to use.</param>
        /// <param name="modifiers">The key's modifiers.</param>
        public KeyPress(ConsoleKey key, ConsoleModifiers modifiers)
        {
            ConsoleKeyInfo = new ConsoleKeyInfo(key, modifiers);
        }
        /// <summary>
        /// Starts a new instance of the KeyPress class using the specified key.
        /// </summary>
        /// <param name="key">The ConsoleKey to use.</param>
        public KeyPress(ConsoleKey key)
        {
            ConsoleKeyInfo = new ConsoleKeyInfo(key);
        }
    }
    /// <summary>
    /// Represents info about a key and its modifiers.
    /// </summary>
    public class ConsoleKeyInfo
    {
        /// <summary>
        /// The ConsoleKey represented in this instance.
        /// </summary>
        public ConsoleKey Key { get; }
        /// <summary>
        /// The key's modifiers, if any. If there is none, this equals zero.
        /// </summary>
        public ConsoleModifiers Modifiers { get; }
        /// <summary>
        /// The character represented by the ConsoleKey. If the ConsoleKey is longer than one character, then this is null and KeyString is used.
        /// </summary>
        public char ?KeyChar { get; }
        /// <summary>
        /// The string represented by the ConsoleKey. If the ConsoleKey is only one character long, then both KeyString and KeyChar are used.
        /// </summary>
        public string KeyString { get; }
        /// <summary>
        /// Starts a new instance of the ConsoleKeyInfo class using the specified key and modifiers.
        /// </summary>
        /// <param name="key">The ConsoleKey to use.</param>
        /// <param name="modifiers">The modifiers to use.</param>
        public ConsoleKeyInfo(ConsoleKey key, ConsoleModifiers modifiers)
        {
            Key = key;
            Modifiers = modifiers;
            string value = Extensions.ConvertToString(key);
            if (value.Length > 1)
            {
                // use KeyString only:
                KeyChar = null;
                KeyString = value;
            }
            else
            {
                // use both:
                KeyChar = Convert.ToChar(value);
                KeyString = value;
            }
        }
        /// <summary>
        /// Starts a new instance of the ConsolekeyInfo class using the specified key.
        /// </summary>
        /// <param name="key">The ConsoleKey to use.</param>
        public ConsoleKeyInfo(ConsoleKey key)
        {
            Key = key;
            Modifiers = 0;
            string value = Extensions.ConvertToString(key);
            if (value.Length > 1)
            {
                // use KeyString only:
                KeyChar = null;
                KeyString = value;
            }
            else
            {
                // use both:
                KeyChar = Convert.ToChar(value);
                KeyString = value;
            }
        }
        internal ConsoleKeyInfo(System.ConsoleKeyInfo info)
        {
            Key = info.Key;
            Modifiers = info.Modifiers;
            KeyChar = info.KeyChar;
            KeyString = Extensions.ConvertToString(info.Key);
        }
    }

    #region ControlChecks
    /// <summary>
    /// Represents a controls' status.
    /// </summary>
    public enum Status
    {
        /// <summary>
        /// For internal purposes only.
        /// </summary>
        Null,
        /// <summary>
        /// Status for a control that is all good (not out of bounds).
        /// </summary>
        OK,
        /// <summary>
        /// Status for a control whose origin (X,Y location) is out of bounds.
        /// </summary>
        OriginOutOfBounds,
        /// <summary>
        /// Status for a control that is partially or completely out of bounds.
        /// </summary>
        ControlOutOfBounds
    }
    /// <summary>
    /// Represents and contains info about that controls status.
    /// </summary>
    internal class ControlStatus
    {
        internal static Status GetStatus(Control c)
        {
            Status Status = Status.Null;
            if (c.controlType == ControlType.Button)
            {
                Button btn = (Button)c;
                // check if origin is out of bounds:
                if (btn.X >= Internal.screenWidth || btn.Y >= Internal.screenHeight || btn.X < 0 || btn.Y < 0)
                {
                    Status = Status.OriginOutOfBounds;
                }

                if (Status == Status.Null)
                {
                    // check if any part of the control is out of bounds:
                    if (((btn.txt.Length * 8) + 20 + btn.X) > Internal.screenWidth || (btn.Y + 23) > Internal.screenHeight)
                    {
                        Status = Status.ControlOutOfBounds;
                    }
                }

                if (Status == Status.Null)
                {
                    Status = Status.OK;
                }
            }
            else if (c.controlType == ControlType.Label)
            {
                Label lbl = (Label)c;
                // check if origin is out of bounds:
                if (lbl.X >= Internal.screenWidth || lbl.Y >= Internal.screenHeight || lbl.X < 0 || lbl.Y < 0)
                {
                    Status = Status.OriginOutOfBounds;
                }

                if (Status == Status.Null)
                {
                    // check if any part of the control is out of bounds:
                    if (((lbl.Text.Length * 8) + lbl.X) > Internal.screenWidth || (lbl.Y + 13) > Internal.screenHeight)
                    {
                        Status = Status.ControlOutOfBounds;
                    }
                }

                if (Status == Status.Null)
                {
                    Status = Status.OK;
                }
            }
            else if (c.controlType == ControlType.Line)
            {
                CGUI.Shapes.Line line = (CGUI.Shapes.Line)c;
                // check if origin is out of bounds:
                if (line.X >= Internal.screenWidth || line.Y >= Internal.screenHeight || line.X < 0 || line.Y < 0)
                {
                    Status = Status.OriginOutOfBounds;
                }

                if (Status == Status.Null)
                {
                    // check if any part of the control is out of bounds:
                    if (line.EndX > Internal.screenWidth || line.EndY > Internal.screenHeight)
                    {
                        Status = Status.ControlOutOfBounds;
                    }
                }

                if (Status == Status.Null)
                {
                    Status = Status.OK;
                }

            }
            else if (c.controlType == ControlType.Picture)
            {
                Picture picture = (Picture)c;
                // check if origin is out of bounds:
                if (picture.X >= Internal.screenWidth || picture.Y >= Internal.screenHeight || picture.X < 0 || picture.Y < 0)
                {
                    Status = Status.OriginOutOfBounds;
                }

                if (Status == Status.Null)
                {
                    // check if any part of the control is out of bounds:
                    if ((picture.Image.Width + picture.X) > Internal.screenWidth || (picture.Image.Height + picture.Y) > Internal.screenHeight)
                    {
                        Status = Status.ControlOutOfBounds;
                    }
                }

                if (Status == Status.Null)
                {
                    Status = Status.OK;
                }
            }
            else if (c.controlType == ControlType.Rectangle)
            {
                CGUI.Shapes.Rectangle rectangle = (CGUI.Shapes.Rectangle)c;
                // check if origin is out of bounds:
                if (rectangle.X >= Internal.screenWidth || rectangle.Y >= Internal.screenHeight || rectangle.X < 0 || rectangle.Y < 0)
                {
                    Status = Status.OriginOutOfBounds;
                }

                if (Status == Status.Null)
                {
                    // check if any part of the control is out of bounds:
                    if ((rectangle.X + rectangle.Width) > Internal.screenWidth || (rectangle.Y + rectangle.Height) > Internal.screenHeight)
                    {
                        Status = Status.ControlOutOfBounds;
                    }
                }

                if (Status == Status.Null)
                {
                    Status = Status.OK;
                }
            }
            else if (c.controlType == ControlType.TextBox)
            {
                TextBox tbox = (TextBox)c;
                // check if origin is out of bounds:
                if (tbox.X >= Internal.screenWidth || tbox.Y >= Internal.screenHeight || tbox.X < 0 || tbox.Y < 0)
                {
                    Status = Status.OriginOutOfBounds;
                }

                if (Status == Status.Null)
                {
                    // check if any part of the control is out of bounds:
                    if (((tbox.cLength * 8) + 4 + tbox.X) > Internal.screenWidth || (tbox.Y + 15) > Internal.screenHeight)
                    {
                        Status = Status.ControlOutOfBounds;
                    }
                }

                if (Status == Status.Null)
                {
                    Status = Status.OK;
                }
            }

            return Status;
        }
    }
    #endregion
}