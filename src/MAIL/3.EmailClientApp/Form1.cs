using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using OpenPop.Pop3;
using OpenPop.Mime;
using System.IO;
//using Message = OpenPop.Mime.Message;
using AE.Net.Mail;

namespace EmailClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private List<Message> myLetters;

        private void Log (string s)
        {
            label5.Text = s;
            label5.Refresh();
        }

        private List<Message> GetMessages(string hostname, int port, string username, string password)
        {
            List<Message> retList = new List<Message>();

            // Connect to the IMAP server. The 'true' parameter specifies to use SSL
            // which is important (for Gmail at least)
            using (ImapClient ic = new ImapClient(hostname, username, password, AuthMethods.Login, port, false))
            {
                // Select a mailbox. Case-insensitive
                ic.SelectMailbox("INBOX");
                Console.WriteLine(ic.GetMessageCount());

                // Get all messages. 0 is the first message;
                // MailMessage represents a message in your mailbox
                // You can get All messages with: ic.GetMessages(0, ic.GetMessageCount() - 1)
                foreach (MailMessage m in ic.GetMessages(0, 10, false).OrderByDescending(m => m.Date))
                {
                    Console.WriteLine(m.Subject);
                    var msg = new Message()
                    {
                        DateSent = m.Date.ToString(),
                        ReturnPath = m.To.First().ToString(),
                        Subject = m.Subject,
                        OrigMsg = m
                    };
                    retList.Add(msg);
                }
            }

            return retList;
        }

        private void LoadTable()
        {
            dataGridView1.Rows.Clear();
            foreach (Message m in myLetters)
            {
                dataGridView1.Rows.Add(new object[] { m.DateSent, m.ReturnPath, m.Subject });
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox6.Text.Length == 0 || textBox5.Text.Length == 0 || textBox2.Text.Length == 0 || textBox1.Text.Length == 0)
            {
                MessageBox.Show(this, "Заполнены не все поля");
            }
            else
            {
                Log("Получение писем ...");
                label5.Refresh();
                myLetters = GetMessages(textBox1.Text, Convert.ToInt32(textBox2.Text), textBox6.Text, textBox5.Text);
                Log("Получено писем: " + myLetters.Count);
                LoadTable();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox6.Text.Length == 0 || textBox5.Text.Length == 0 || textBox4.Text.Length == 0 || textBox3.Text.Length == 0)
            {
                MessageBox.Show(this, "Заполнены не все поля");
            }
            else
            {
                Form3 sendform = new Form3(textBox6.Text, textBox5.Text, textBox4.Text, textBox3.Text, null);
                sendform.ShowDialog();
            }        
        }

        public static bool isanswer;

        private void dataGridView1_DoubleClick(object sender, EventArgs e)
        {
            Form2 letter = new Form2(myLetters[dataGridView1.CurrentRow.Index]);
            letter.ShowDialog();
            if (isanswer)
            {
                string to = myLetters[dataGridView1.CurrentRow.Index].ReturnPath.ToString();
                Form3 sendform = new Form3(textBox6.Text, textBox5.Text, textBox4.Text, textBox3.Text, to);
                sendform.ShowDialog();
            }
        }
    }

    public class Message
    {
        public string DateSent { get; set; }
        public string ReturnPath { get; set; }
        public string Subject { get; set; }
        public MailMessage OrigMsg { get; set; }
    }
}
