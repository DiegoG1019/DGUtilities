using System;
using System.Drawing;
using System.Windows.Forms;

namespace DiegoG.Geometry
{
    public class MainForm : Form
    {
        private readonly System.ComponentModel.Container components;
        private Graphics Graphics;

        public MainForm()
        {
            InitializeComponent();
            CenterToScreen();
            SetStyle(ControlStyles.ResizeRedraw, true);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            Resize += Form1_Resize;
            Paint += MainForm_Paint;
            Graphics = CreateGraphics();
        }

        public void DrawTo(Action<Graphics> DrawAction)
            => Invoke(DrawAction, Graphics);

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Form1_Resize(object sender, System.EventArgs e)
        {

        }
    }
}
