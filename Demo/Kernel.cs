using System;
using System.Collections.Generic;
using System.Text;
using Sys = Cosmos.System;
using CGUI;
using System.Drawing;

namespace Demo
{
    public class Kernel : Sys.Kernel
    {
        VGADriver driver;
        protected override void BeforeRun()
        {
            driver = new VGADriver();
        }
        Label l;
        TextBox t2;
        Button btn;
        Screen s;
        protected override void Run()
        {
            s = new Screen(Color.LightBlue);
            Label l1 = new Label("Welcome!", 10, 10);
            TextBox t = new TextBox(15, 10, 30);
            t2 = new TextBox(15, 10, 120);
            t2.Placeholder = "Username";
            btn = new Button("enter", 10, 150);
            btn.OnEnter += Btn_OnEnter;
            KeyPress p = new KeyPress(ConsoleKey.D1, ConsoleModifiers.Alt);
            p.OnPress += Kernel_OnPress;
            t.KeyPresses.Add(p);
            t.OnDelete += T_OnDelete;
            t.OnKeyPress += T_OnKeyPress;
            l = new Label("--", 10, 80);
            s.Controls.Add(l1);
            s.Controls.Add(t);
            s.Controls.Add(t2);
            s.Controls.Add(btn);
            s.Controls.Add(l);
            driver.RenderScreen(s);
        }

        private void T_OnKeyPress(object sender, System.ConsoleKeyInfo e)
        {
            l.Update(((TextBox)sender).Text);
        }

        private void T_OnDelete(object sender, System.EventArgs e)
        {
            l.Update(((TextBox)sender).Text);
        }

        private void Btn_OnEnter(object sender, System.EventArgs e)
        {
            //t2.SetText("NewText8901234567890");
            TextBox tbox = new TextBox(15, 10, 200);
            s.Controls.Add(tbox);
            s.Controls.Remove(btn);
        }

        private void Kernel_OnPress(object sender, CGUI.ConsoleKeyInfo info)
        {
            
        }
    }
}
