using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

namespace Server
{
    public partial class frmMain : Form
    {
        bool isMouseDown = false;
        static bool isFirstSet = false;

        static Graphics graphics;
        static Pen pen;

        static int x, y;
        public static byte[] buffer = new byte[9];
        public static byte[] buf = new byte[4];


        public frmMain()
        {
            InitializeComponent();

            pen = new Pen(Color.Black, 2);
            var picture = new Bitmap(cDrawField.Width, cDrawField.Height);
            cDrawField.Image = picture;

            graphics = Graphics.FromImage(cDrawField.Image);
            graphics.Clear(Color.White);
        }

        public static void DrawPoints(byte[] buffer)
        {
            int bx, by;

            Array.Copy(buffer, 0, buf, 0, 4);
            bx = BitConverter.ToInt32(buf);

            Array.Copy(buffer, 4, buf, 0, 4);
            by = BitConverter.ToInt32(buf);

            if (buffer[8] == 1)
            {
                x = bx;
                y = by;
                return;
            }

            if (buffer[8] == 2)
            {
                graphics.Clear(Color.White);
                cDrawField.Refresh();
                return;
            }
            
            if (!isFirstSet)
            {
                isFirstSet = true;
                x = bx;
                y = by;
            }
            else
            {
                graphics.DrawLine(pen, x, y, bx, by);
                cDrawField.Refresh();
                x = bx;
                y = by;
            }
        }

        private void cDrawField_MouseDown(object sender, MouseEventArgs e)
        {
            //isMouseDown = true;
            //x = e.X;
            //y = e.Y;
        }

        private void cDrawField_MouseUp(object sender, MouseEventArgs e)
        {
            isMouseDown = false;
        }

        private void frmMain_Shown(object sender, EventArgs e)
        {
            var startForm = new frmStart();
            startForm.Show();
        }

        private void cDrawField_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isMouseDown)
                return;

            graphics.DrawLine(pen, x, y, e.X, e.Y);
            x = e.X;
            y = e.Y;

            cDrawField.Refresh();
        }

    }
}
