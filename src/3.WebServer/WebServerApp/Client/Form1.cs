using System;
using System.Globalization;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;
using Timer = System.Windows.Forms.Timer;

namespace Client
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            openFileDialog.ShowDialog();
            textBoxFile.Text = openFileDialog.FileName;
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            int length = int.Parse(textBox3.Text);
            int offset = int.Parse(textBox2.Text);
            Stream fileStream = File.OpenRead(textBoxFile.Text);
            if (length > fileStream.Length | length < 0)
                length = (int)fileStream.Length;

            byte[] fileBuffer = new byte[length];
            fileStream.Seek(offset, SeekOrigin.Begin);
            fileStream.Read(fileBuffer, 0, length);
            label7.Text = "Block: " + Encoding.ASCII.GetString(fileBuffer);

            fileBuffer = Encoding.ASCII.GetBytes(Base64Encode(Encoding.ASCII.GetString(fileBuffer)));

            TcpClient clientSocket = new TcpClient(textBoxServer.Text, 8764);
            NetworkStream networkStream = clientSocket.GetStream();
            networkStream.Write(fileBuffer, 0, fileBuffer.GetLength(0));
            networkStream.Close();
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        private void buttonCount_Click(object sender, EventArgs e)
        {
            string retString;
            //string text = File.ReadAllText(openFileDialog.FileName);
            string text = textBox1.Text;
            int result = 0;
            int _result = 0;

            label.Text = "Number of words: ";
            _label.Text = "Number of lines: ";

            string[] textArray = text.Split(new char[] {' '});
            string[] _textArray = text.Split(new char[] { '\n' });
            result = textArray.Length;
            _result = _textArray.Length;
            retString = result.ToString();
            label.Text += retString;
            retString = _result.ToString();
            _label.Text += retString;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Tick.Start();
            DateTime localTime = DateTime.Now;
            label3.Text = localTime.ToString();

        }

        private void Tick_Tick(object sender, EventArgs e)
        {
            DateTime localTime = DateTime.Now;
            label3.Text = localTime.ToString();
            Tick.Start();
        }
    }
}
