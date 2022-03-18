using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//using Message = OpenPop.Mime.Message;

namespace EmailClient
{
    public partial class Form2 : Form
    {
        public Form2(Message m)
        {
            InitializeComponent();
            M = m;
        }

        private Message M;

        private void Form2_Load(object sender, EventArgs e)
        {
            textBox1.Text = "Message-Id: " + M.OrigMsg.MessageID +
                "\r\nDate: " + M.OrigMsg.Date +
                "\r\nSender: " + M.OrigMsg.Sender +
                "\r\nContent-Type: " + M.OrigMsg.ContentType +
                "\r\nFrom: " + M.OrigMsg.From +
                "\r\nSubject: " + M.OrigMsg.Subject;

            textBox2.Text = M.OrigMsg.Body;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form1.isanswer = false;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form1.isanswer = true;
            this.Close();
        }
    }
}
