using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Mail;
using System.Net;

namespace EmailClient
{
    public partial class Form3 : Form
    {
        public Form3(string log, string pass, string s, string p, string to)
        {
            InitializeComponent();
            login = log;
            password = pass;
            smtp = s;
            port = Convert.ToInt32(p);
            if (to != null) textBox1.Text = to;         
        }

        string login;
        string password;
        string smtp;
        int port;

        private void button1_Click(object sender, EventArgs e)
        {
            SmtpClient Smtp = new SmtpClient(smtp, port);
            Smtp.EnableSsl = true;
            Smtp.Credentials = new NetworkCredential(login, password);
            Smtp.Timeout = 1000;
            
            MailMessage Message = new MailMessage();
            Message.From = new MailAddress(login);
            Message.To.Add(new MailAddress(textBox1.Text));
            Message.Subject = textBox2.Text;
            Message.Body = textBox3.Text;

            try
            {
                label3.Text = "Идет отправка";
                label3.Refresh();
                Smtp.Send(Message);
                label3.Text = "Письмо отправлено";
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.ToString());
            }
        }
    }
}
