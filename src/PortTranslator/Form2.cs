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

namespace PortTranslator
{
    public partial class Form2 : Form
    {
        public Form2(Rule r)
        {
            InitializeComponent();
            if (r.name != null) textBox1.Text = r.name;
            if (r.inip != null) textBox2.Text = r.inip.ToString();
            if (r.inport > 0) textBox3.Text = r.inport.ToString();
            if (r.extip != null) textBox4.Text = r.extip.ToString();
            if (r.extport > 0) textBox5.Text = r.extport.ToString();
            isAdd = false;
        }

        private bool isAdd;

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Form1.tRule.name = textBox1.Text;
                Form1.tRule.inip = IPAddress.Parse(textBox2.Text.Replace(",", "."));
                Form1.tRule.inport = Convert.ToInt32(textBox3.Text);
                Form1.tRule.extip = IPAddress.Parse(textBox4.Text.Replace(",", "."));
                Form1.tRule.extport = Convert.ToInt32(textBox5.Text);
                if (Form1.tRule.inport > 0 && Form1.tRule.extport > 0) isAdd = true;
                this.Close();
            }
            catch (Exception)
            {
                MessageBox.Show(this, "Проверьте правильность полей");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form1.tRule.success = false;
            this.Close();
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (isAdd) Form1.tRule.success = true;
            else Form1.tRule.success = false;
        }
    }
}
