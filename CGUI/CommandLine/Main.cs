using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace CGUI.CommandLine
{
    /// <summary>
    /// Custom event class used for passing command info to the user. [IN DEVELOPMENT]
    /// </summary>
    public class CommandArgs : EventArgs
    {
        /// <summary>
        /// The command's arguments.
        /// </summary>
        public List<string> Arguments { get; internal set; }
        internal CommandArgs(List<string> args)
        {
            Arguments = args;
        }
    }
    /// <summary>
    /// The main class for the CGUI command line interface. [IN DEVELOPMENT]
    /// </summary>
    public class CommandLine
    {
        /// <summary>
        /// The current directory that is displayed at the prompt (default is 0:\>)
        /// </summary>
        public string currentDirectory { get; set; } = @"0:\";
        /// <summary>
        /// The text entered at the prompt.
        /// </summary>
        public string Text
        {
            get
            {
                return txt.ToString();
            }
        }
        /// <summary>
        /// Raised every time the prompt renders.
        /// </summary>
        public event EventHandler OnPrompt
        {
            add
            {
                Prompt_Handler = value;
            }
            remove
            {
                Prompt_Handler -= value;
            }
        }
        /// <summary>
        /// Raised when the user enters a command that is not registered.
        /// </summary>
        public event EventHandler UnknownCommand
        {
            add
            {
                InvalidCMD_Handler = value;
            }
            remove
            {
                InvalidCMD_Handler -= value;
            }
        }
        /// <summary>
        /// Raised when the user hits the Backspace key.
        /// </summary>
        public event EventHandler OnBackspace
        {
            add
            {
                Backspace_Handler = value;
            }
            remove
            {
                Backspace_Handler -= value;
            }
        }
        /// <summary>
        /// Raised when the user presses a key.
        /// </summary>
        public event EventHandler OnKeyPress
        {
            add
            {
                KeyPress_Handler = value;
            }
            remove
            {
                KeyPress_Handler -= value;
            }
        }
        /// <summary>
        /// Starts a new instance of the CommandLine class using the default colors (black background and white foreground).
        /// </summary>
        /// <param name="driver">The underlying VGADriver class instance.</param>
        public CommandLine(VGADriver driver)
        {
            charLength = Internal.screenWidth / 8;
            charHeight = Internal.screenHeight / 12;
            driver.ClearScreen();
            tColor = (uint)Color.White.ToArgb();
            bColor = (uint)Color.Black.ToArgb();
            PromptLocX = 0;
            PromptLocY = 0;
        }
        /// <summary>
        /// Starts a new instance of the CommandLine class using the specified colors.
        /// </summary>
        /// <param name="driver">The underlying VGADriver class instance.</param>
        /// <param name="backColor">The prompt's background color.</param>
        /// <param name="textColor">The prompt's foreground color.</param>
        public CommandLine(VGADriver driver, Color backColor, Color textColor)
        {
            charLength = Internal.screenWidth / 8;
            charHeight = Internal.screenHeight / 12;
            driver.ClearScreen(backColor);
            tColor = (uint)textColor.ToArgb();
            bColor = (uint)backColor.ToArgb();
            PromptLocX = 0;
            PromptLocY = 0;
        }
        /// <summary>
        /// Registers a command.
        /// </summary>
        /// <param name="command">The Command class instance representing the command to register.</param>
        public void RegisterCommand(Command command)
        {
            CMDs.Add(command.CommandText);
            CMDS.Add(command);
        }
        /// <summary>
        /// Writes the specified text to the prompt.
        /// </summary>
        /// <param name="text">The text to write.</param>
        public void WriteToPrompt(string text)
        {           
            if ((currentDirectory.Length + text.Length) < charLength)
            {
                //Erase cursor:
                VGADriver.driver.DoubleBuffer_DrawFillRectangle((uint)LocX, (uint)LocY + 13, 8, 2, bColor);
                VGADriver.driver.DoubleBuffer_Update();
                // Draw string:
                VGADriver.driver._DrawACSIIString(text, (uint)Color.White.ToArgb(), (uint)LocX, (uint)LocY);
                VGADriver.driver.DoubleBuffer_Update();
                txt.Append(text);
                LocX = (currentDirectory.Length + text.Length) * 8;
                DrawCursor(LocX, LocY);
            }
            //else
            //{
            //    //Erase cursor:
            //    VGADriver.driver.DoubleBuffer_DrawFillRectangle((uint)LocX, (uint)LocY + 13, 8, 2, bColor);
            //    VGADriver.driver.DoubleBuffer_Update();
            //    string first = text.Substring(0, charLength - currentDirectory.Length - 1);
            //    // Draw string:
            //    VGADriver.driver._DrawACSIIString(first, (uint)Color.White.ToArgb(), (uint)LocX, (uint)LocY);
            //    VGADriver.driver.DoubleBuffer_Update();
            //    string rest = text.Substring(charLength - currentDirectory.Length - 1);
            //    //Save last coordinates:
            //    LLocX = LocX + ((first.Length) * 8) - 8;
            //    LLocY = LocY;
            //    // Go to next line:
            //    LocX = 0;
            //    LocY += 12;
            //    // Draw string:
            //    VGADriver.driver._DrawACSIIString(rest, (uint)Color.White.ToArgb(), (uint)LocX, (uint)LocY);
            //    VGADriver.driver.DoubleBuffer_Update();
            //    txt.Append(text);
            //    LocX = (rest.Length) * 8;
            //    DrawCursor(LocX, LocY);
            //}
        }
        /// <summary>
        /// Writes the specified string and moves the cursor to the next line.
        /// </summary>
        /// <param name="text">The string to write.</param>
        public void WriteLine(string text)
        {
            if (text.Length < charLength)
            {
                //Erase cursor:
                VGADriver.driver.DoubleBuffer_DrawFillRectangle((uint)LocX, (uint)LocY + 13, 8, 2, bColor);
                VGADriver.driver.DoubleBuffer_Update();
                // Go to next line:
                LocX = 0;
                LocY += 12;
                txt.Append(text);
                ScreenInput.Add(text);
                EndLine += 1;
                // Draw string:
                VGADriver.driver._DrawACSIIString(EndLine.ToString() + ", " + charHeight.ToString() + ", " + ScreenInput.Count.ToString(), (uint)Color.White.ToArgb(), (uint)LocX, (uint)LocY);
                VGADriver.driver.DoubleBuffer_Update();        
                // Go to next line:
                //LocX = 0;
                //LocY += 14;
                //DrawCursor(LocX, LocY);
                PromptLocX = 0;
                PromptLocY += 12;
            }
            //else
            //{
            //    //Erase cursor:
            //    VGADriver.driver.DoubleBuffer_DrawFillRectangle((uint)LocX, (uint)LocY + 13, 8, 2, bColor);
            //    VGADriver.driver.DoubleBuffer_Update();
            //    string first = text.Substring(0, charLength - 1);
            //    // Go to next line:
            //    LocX = 0;
            //    LocY += 12;
            //    // Draw string:
            //    VGADriver.driver._DrawACSIIString(first, (uint)Color.White.ToArgb(), (uint)LocX, (uint)LocY);
            //    VGADriver.driver.DoubleBuffer_Update();
            //    ScreenInput.Add(first);
            //    string rest = text.Substring(charLength - 1);
            //    //Save last coordinates:
            //    LLocX = LocX + ((first.Length) * 8) - 8;
            //    LLocY = LocY;
            //    // Go to next line:
            //    LocX = 0;
            //    LocY += 12;
            //    // Draw string:
            //    VGADriver.driver._DrawACSIIString(rest, (uint)Color.White.ToArgb(), (uint)LocX, (uint)LocY);
            //    VGADriver.driver.DoubleBuffer_Update();
            //    txt.Append(text);
            //    ScreenInput.Add(rest);
            //    LocX = (rest.Length) * 8;
            //    EndLine += 2;
            //    //// Go to next line:
            //    //LocX = 0;
            //    //LocY += 12;
            //    PromptLocX = 0;
            //    PromptLocY += 24;

            //}
        }

        /// <summary>
        /// Runs the prompt.
        /// </summary>
        public void Run()
        {
            Prompt();
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
                        //Erase cursor:
                        VGADriver.driver.DoubleBuffer_DrawFillRectangle((uint)LocX, (uint)LocY + 13, 8, 2, bColor);
                        VGADriver.driver.DoubleBuffer_Update();
                        if ((currentDirectory + ">" + txt.ToString()).Length % 79 == 0)
                        {
                            ////Draw character entered:
                            //VGADriver.driver._DrawACSIIString(info.KeyChar.ToString(), tColor, (uint)LocX, (uint)LocY);
                            //VGADriver.driver.DoubleBuffer_Update();
                            ////Save last coordinates:
                            //LLocX = LocX;
                            //LLocY = LocY;
                            //// Go to next line:
                            //LocX = 0;
                            //LocY += 12;
                            ////Draw cursor up one character:
                            //VGADriver.driver.DoubleBuffer_DrawFillRectangle((uint)LocX, (uint)LocY + 13, 8, 2, tColor);
                            //VGADriver.driver.DoubleBuffer_Update();
                        }
                        else
                        {
                            if ((LocX + 8) <= charLength)
                            {
                                LocX += 8;
                                //Draw cursor up one character:
                                VGADriver.driver.DoubleBuffer_DrawFillRectangle((uint)LocX, (uint)LocY + 13, 8, 2, tColor);
                                VGADriver.driver.DoubleBuffer_Update();
                                //Draw character entered:
                                VGADriver.driver._DrawACSIIString(info.KeyChar.ToString(), tColor, (uint)LocX - 8, (uint)LocY);
                                VGADriver.driver.DoubleBuffer_Update();
                            }
                            
                        }

                        if (KeyPress_Handler != null)
                        {
                            KeyPress_Handler.Invoke(info.KeyChar, new EventArgs());
                        }
                        txt.Append(info.KeyChar);
                        break;
                    default:
                        switch (info.Key)
                        {
                            case ConsoleKey.Backspace:
                                if (txt.Length > 0)
                                {
                                    if (LocX == 0)
                                    {
                                        //// Erase last character:
                                        //VGADriver.driver._DrawACSIIString(txt[txt.Length - 1].ToString(), bColor, (uint)LLocX, (uint)LLocY);                                        
                                        ////Erase cursor:
                                        //VGADriver.driver.DoubleBuffer_DrawFillRectangle((uint)LocX, (uint)LocY + 13, 8, 2, bColor);
                                        //VGADriver.driver.DoubleBuffer_Update();
                                        //LocX = LLocX;
                                        //LocY = LLocY;
                                        ////LocX -= 8;
                                        ////Draw cursor one character back:
                                        //VGADriver.driver.DoubleBuffer_DrawFillRectangle((uint)LocX, (uint)LocY + 13, 8, 2, tColor);
                                        //VGADriver.driver.DoubleBuffer_Update();
                                        //txt.Remove(txt.Length - 1, 1);
                                    }
                                    else
                                    {
                                        // Erase last character:
                                        VGADriver.driver._DrawACSIIString(txt[txt.Length - 1].ToString(), bColor, (uint)LocX - 8, (uint)LocY);
                                        //Erase cursor:
                                        VGADriver.driver.DoubleBuffer_DrawFillRectangle((uint)LocX, (uint)LocY + 13, 8, 2, bColor);
                                        VGADriver.driver.DoubleBuffer_Update();
                                        LocX -= 8;
                                        //Draw cursor one character back:
                                        VGADriver.driver.DoubleBuffer_DrawFillRectangle((uint)LocX, (uint)LocY + 13, 8, 2, tColor);
                                        VGADriver.driver.DoubleBuffer_Update();
                                        txt.Remove(txt.Length - 1, 1);
                                    }
                                    
                                    if (Backspace_Handler != null)
                                    {
                                        Backspace_Handler.Invoke("BACKSPACE", new EventArgs());
                                    }
                                }
                                else
                                {
                                    if (Backspace_Handler != null)
                                    {
                                        Backspace_Handler.Invoke("BACKSPACE", new EventArgs());
                                    }
                                }
                                break;
                            case ConsoleKey.Enter:
                                EndLine += 1;
                                if (EndLine == (charHeight - 2))
                                {
                                    Console.Beep();
                                    // move all lines up one:
                                    ScrollDown();
                                }

                                if (txt.Length > 0)
                                {
                                    if (txt.ToString().Contains(" "))
                                    {
                                        string[] cmds = txt.ToString().Split(' ');
                                        if (CMDs.Contains(cmds[0]))
                                        {
                                            int cmdindex = CMDs.IndexOf(cmds[0]);
                                            if (CMDS[cmdindex].Execute != null)
                                            {
                                                // use array
                                                List<string> args = new List<string>();
                                                for (int i = 1; i < cmds.Length; i++)
                                                {
                                                    args.Add(cmds[i]);
                                                }
                                                CMDS[cmdindex].Execute.Invoke("EXECUTE", new CommandArgs(args));
                                                SaveToMemory();
                                            }
                                        }
                                        else
                                        {
                                            if (InvalidCMD_Handler != null)
                                            {
                                                InvalidCMD_Handler.Invoke(cmds[0], new EventArgs());
                                            }
                                            SaveToMemory();
                                        }
                                    }
                                    else
                                    {
                                        if (CMDs.Contains(txt.ToString()))
                                        {
                                            // use string
                                            CMDS[CMDs.IndexOf(txt.ToString())].Execute.Invoke("EXECUTE", new CommandArgs(new List<string>()));
                                            SaveToMemory();
                                        }
                                        else
                                        {
                                            if (InvalidCMD_Handler != null)
                                            {
                                                InvalidCMD_Handler.Invoke(txt.ToString(), new EventArgs());
                                            }
                                            SaveToMemory();
                                        }
                                    }
                                }
                                else
                                {
                                    SaveToMemory();
                                }                            
                                break;
                        }
                        break;
                }
            }
            
        }
        internal void ScrollUp()
        {

        }
        internal void ScrollDown()
        {
            VGADriver.driver.DoubleBuffer_Clear(bColor);
            VGADriver.driver.DoubleBuffer_Update();
            for (int i = (ScreenInput.Count - 1) - EndLine; i < EndLine; i++)
            {
                WriteLine(ScreenInput[i]);
            }
            SaveToMemory();
        }
        internal void SaveToMemory()
        {
            ScreenInput.Add(currentDirectory + ">" + txt.ToString());
            txt.Clear();
            EndLine += 1;
            //Erase cursor:
            VGADriver.driver.DoubleBuffer_DrawFillRectangle((uint)LocX, (uint)LocY + 13, 8, 2, bColor);
            VGADriver.driver.DoubleBuffer_Update();
            Prompt();
        }
        internal void Prompt()
        {

            // Draw prompt string:
            VGADriver.driver._DrawACSIIString(currentDirectory + ">", tColor, PromptLocX, PromptLocY);
            VGADriver.driver.DoubleBuffer_Update();
            // Update locations:
            PromptLocY += 12;
            LocX = ((currentDirectory.Length + 1) * 8);
            LocY += 12;
            // Draw cursor:
            DrawCursor(LocX, LocY);
            if (Prompt_Handler != null)
            {
                Prompt_Handler.Invoke("PROMPT", new EventArgs());
            }
        }
        internal void DrawCursor(int x, int y)
        {
            VGADriver.driver.DoubleBuffer_DrawFillRectangle((uint)x, (uint)y + 13, 8, 2, tColor);
            VGADriver.driver.DoubleBuffer_Update();
        }
        internal List<string> ScreenInput = new List<string>();
        internal List<string> CMDs = new List<string>();
        internal List<Command> CMDS = new List<Command>();
        internal StringBuilder txt = new StringBuilder("");
        internal uint tColor;
        internal uint bColor;
        internal EventHandler Prompt_Handler;
        internal EventHandler InvalidCMD_Handler;
        internal EventHandler Backspace_Handler;
        internal EventHandler KeyPress_Handler;
        internal int charLength;
        internal int charHeight;
        internal uint PromptLocX;
        internal uint PromptLocY;
        internal int LocX;
        internal int LocY = -12;
        internal int LLocX;
        internal int LLocY;
        internal int EndLine;
    }
    /// <summary>
    /// Represents a command. [IN DEVELOPMENT]
    /// </summary>
    public class Command
    {
        /// <summary>
        /// The actual command name (e.g. cd).
        /// </summary>
        public string CommandText { get; set; }
        /// <summary>
        /// The command summary.
        /// </summary>
        public string About { get; set; }
        /// <summary>
        /// The command's flags; the keys are the actual flags (e.g. -a) and their corresponding values are the flags' summary (e.g. Summary sentence).
        /// </summary>
        public Dictionary<string, string> Flags { get; set; }
        /// <summary>
        /// Raised when the command is called/executed.
        /// </summary>
        public event EventHandler<CommandArgs> OnExecute 
        {
            add
            {
                Execute = value;
            }
            remove
            {
                Execute -= value;
            }
        }
        /// <summary>
        /// Starts a new instance of the Command class.
        /// </summary>
        /// <param name="command">The actual command (e.g. cd)</param>
        /// <param name="about">The string that summarizes/describes the command.</param>
        public Command(string command, string about)
        {
            CommandText = command;
            About = about;
        }
        internal EventHandler<CommandArgs> Execute;
    }
}
