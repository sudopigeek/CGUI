using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace CGUI.CommandLine
{
    public class CommandArgs : EventArgs
    {
        public List<string> Arguments { get; internal set; }
        internal CommandArgs(List<string> args)
        {
            Arguments = args;
        }
    }
    public class CommandLine
    {
        public List<Command> Commands { get; set; }
        public string currentDirectory { get; set; } = @"0:\";
        public string Text
        {
            get
            {
                return txt.ToString();
            }
        }
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
        public CommandLine(VGADriver driver)
        {
            charLength = Internal.screenWidth / 8;
            driver.ClearScreen();
            tColor = (uint)Color.White.ToArgb();
            bColor = (uint)Color.Black.ToArgb();
            Commands.Add(new Command("cd ..", "Moves up one directory."));
            Commands.Add(new Command("cd", "Moves down into a directory."));
            PromptLocX = 0;
            PromptLocY = 0;
        }
        public CommandLine(VGADriver driver, Color backColor, Color textColor)
        {
            charLength = Internal.screenWidth / 8;
            driver.ClearScreen(backColor);
            tColor = (uint)textColor.ToArgb();
            bColor = (uint)backColor.ToArgb();
            Commands.Add(new Command("cd ..", "Moves up one directory."));
            Commands.Add(new Command("cd", "Moves down into a directory."));
            PromptLocX = 0;
            PromptLocY = 0;
        }
        public void Run()
        {
            if (Prompt_Handler != null)
            {
                Prompt_Handler.Invoke("PROMPT", new EventArgs());
            }
            Prompt();
            SaveLineToMemory(currentDirectory + ">" + txt.ToString());
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
                        //Erase cursor:
                        VGADriver.driver.DoubleBuffer_DrawFillRectangle((uint)LocX, (uint)LocY + 13, 8, 2, bColor);
                        VGADriver.driver.DoubleBuffer_Update();
                        if ((currentDirectory + ">" + txt.ToString()).Length == 78)
                        {
                            //Draw character entered:
                            VGADriver.driver._DrawACSIIString(info.KeyChar.ToString(), tColor, (uint)LocX, (uint)LocY);
                            VGADriver.driver.DoubleBuffer_Update();
                            // Go to next line:
                            LocX = 0;
                            LocY += 12;
                            //Draw cursor up one character:
                            VGADriver.driver.DoubleBuffer_DrawFillRectangle((uint)LocX, (uint)LocY + 13, 8, 2, tColor);
                            VGADriver.driver.DoubleBuffer_Update();
                        }
                        else
                        {
                            LocX += 8;
                            //Draw cursor up one character:
                            VGADriver.driver.DoubleBuffer_DrawFillRectangle((uint)LocX, (uint)LocY + 13, 8, 2, tColor);
                            VGADriver.driver.DoubleBuffer_Update();
                            //Draw character entered:
                            VGADriver.driver._DrawACSIIString(info.KeyChar.ToString(), tColor, (uint)LocX - 8, (uint)LocY);
                            VGADriver.driver.DoubleBuffer_Update();
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
                                if (txt.Length > 0)
                                {
                                    string[] cmds = txt.ToString().Split(' ');
                                    int cmdindex = GetCommand(cmds);
                                    if (cmdindex == -1)
                                    {
                                        if (InvalidCMD_Handler != null)
                                        {
                                            InvalidCMD_Handler.Invoke(Commands[cmdindex], new EventArgs());
                                        }
                                    }
                                    else
                                    {
                                        if (Commands[cmdindex].Execute != null)
                                        {
                                            List<string> args = new List<string>();
                                            for (int i = 1; i < cmds.Length; i++)
                                            {
                                                args.Add(cmds[i]);
                                            }
                                            Commands[cmdindex].Execute.Invoke("EXECUTE", new CommandArgs(args));
                                        }
                                    }
                                }
                                SaveLineToMemory(currentDirectory + ">" + txt.ToString());
                                //Erase cursor:
                                VGADriver.driver.DoubleBuffer_DrawFillRectangle((uint)LocX, (uint)LocY + 13, 8, 2, bColor);
                                VGADriver.driver.DoubleBuffer_Update();
                                Prompt();
                                break;
                        }
                        break;
                }
            }
            
        }
        internal int GetCommand(string[] c)
        {
            foreach (Command cmd in Commands)
            {
                if (cmd.CommandText == c[0])
                {
                    return Commands.IndexOf(cmd);
                }
            }
            return -1;
        }
        internal void SaveLineToMemory(string line)
        {
            ScreenInput.Add(line);
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
            VGADriver.driver.DoubleBuffer_DrawFillRectangle((uint)LocX, (uint)LocY + 13, 8, 2, tColor);
            VGADriver.driver.DoubleBuffer_Update();
        }
        internal List<string> ScreenInput = new List<string>();
        internal StringBuilder txt = new StringBuilder("");
        internal uint tColor;
        internal uint bColor;
        internal EventHandler Prompt_Handler;
        internal EventHandler InvalidCMD_Handler;
        internal EventHandler Backspace_Handler;
        internal EventHandler KeyPress_Handler;
        internal int charLength; 
        internal uint PromptLocX;
        internal uint PromptLocY;
        internal int LocX;
        internal int LocY = -12;
    }

    public class Command
    {
        public string CommandText { get; set; }
        public string About { get; set; }
        public Dictionary<string, string> Flags { get; set; }
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
        public Command(string command, string about)
        {
            CommandText = command;
            About = about;
        }
        internal EventHandler<CommandArgs> Execute;
    }
}
