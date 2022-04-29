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

namespace Client
{
    public partial class frmMain : Form
    {
        bool isMouseDown = false;

        static Graphics graphics;
        static Pen pen;

        int x, y;
        public static  byte[] buffer = new byte[9];
        public static byte[] buf = new byte[4];

        public static TCPClient client;


        public frmMain()
        {
            InitializeComponent();

            pen = new Pen(Color.Black, 2);
            var picture = new Bitmap(cDrawField.Width, cDrawField.Height);
            cDrawField.Image = picture;

            graphics = Graphics.FromImage(cDrawField.Image);
            graphics.Clear(Color.White);
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

            buf = BitConverter.GetBytes(x);
            buf.CopyTo(buffer, 0);
            buf = BitConverter.GetBytes(y);
            buf.CopyTo(buffer, 4);
            buffer[8] = 0;

            client.Send(buffer);

            cDrawField.Refresh();
        }

        private void cDrawField_MouseDown(object sender, MouseEventArgs e)
        {
            isMouseDown = true;
            x = e.X;
            y = e.Y;

            buf = BitConverter.GetBytes(x);
            buf.CopyTo(buffer, 0);
            buf = BitConverter.GetBytes(y);
            buf.CopyTo(buffer, 4);
            buffer[8] = 1;

            client.Send(buffer);
        }

        private void cDrawField_MouseUp(object sender, MouseEventArgs e)
        {
            isMouseDown = false;
        }

        public static void ClearDrawField()
        {
            graphics.Clear(Color.White);
            cDrawField.Refresh();
        }
    }
}
