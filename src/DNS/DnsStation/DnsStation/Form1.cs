using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
namespace DnsStation
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            IPHostEntry me = Dns.GetHostEntry(textBox1.Text);
            listBox1.Items.Clear();
            listBox1.Items.Add("Host name：" + me.HostName);

        }

        private void button2_Click(object sender, EventArgs e)
        {
            IPHostEntry me = Dns.GetHostEntry(textBox1.Text);
            listBox1.Items.Clear();
            listBox1.Items.Add("List of IP addresses：");
            foreach (IPAddress ip in me.AddressList)
            {
                listBox1.Items.Add(ip);
            }
        }
    }
}
