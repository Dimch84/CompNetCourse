using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Threading;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.IO;

namespace PortTranslator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public static Rule tRule;

        IPAddress myip;
        List<Rule> RulesList;
        bool isRunnig;
        Thread[] tlist;

        private void Form1_Load(object sender, EventArgs e)
        {
            myip = Dns.GetHostByName(Dns.GetHostName()).AddressList[0];
            textBox1.Text = myip.ToString();
            RulesList = new List<Rule>();
            string path = Application.StartupPath + "\\TranslatorTable.cfg";
            try
            {
                LoadCfg(path);
            }
            catch (Exception)
            {
                MessageBox.Show(this, "Конфигурационный файл не найден");
            }
            isRunnig = false;
        }

        private void button3_Click(object sender, EventArgs e) //получить имя хоста
        {
            IPHostEntry host = Dns.GetHostEntry(textBox3.Text);
            textBox4.Text = host.AddressList[0].ToString();            
        }

        private void button5_Click(object sender, EventArgs e) //добавить правило
        {
            IPAddress ip = null;
            int port = 0;
            try
            {
                ip = IPAddress.Parse(textBox4.Text);
                port = 80;
            }
            catch (Exception)
            {
            }
            
            Rule r = new Rule(null, null, 0, ip, port, true);
            Form2 add = new Form2(r);
            add.ShowDialog();
            if (tRule.success)
            {
                RulesList.Add(tRule);
                dataGridView1.Rows.Add(new object [] { tRule.name, tRule.inip, tRule.inport, tRule.extip, tRule.extport });
            }
        }

        private void button6_Click(object sender, EventArgs e) //удалить правило
        {
            if (dataGridView1.CurrentRow == null || dataGridView1.CurrentRow.Index == dataGridView1.Rows.Count - 1) return;
            DataGridViewRow row = dataGridView1.CurrentRow;
            
            Rule r = new Rule((string)row.Cells[0].Value, (IPAddress)row.Cells[1].Value, (int)row.Cells[2].Value, (IPAddress)row.Cells[3].Value, (int)row.Cells[4].Value, true);
            RulesList.Remove(r);
            dataGridView1.Rows.Remove(row);
        }

        private void button4_Click(object sender, EventArgs e) //редактировать правило
        {
            if (dataGridView1.CurrentRow == null || dataGridView1.CurrentRow.Index == dataGridView1.Rows.Count - 1) return;
            DataGridViewRow row = dataGridView1.CurrentRow;
            Rule r = new Rule((string)row.Cells[0].Value, (IPAddress)row.Cells[1].Value, (int)row.Cells[2].Value, (IPAddress)row.Cells[3].Value, (int)row.Cells[4].Value, true);
            RulesList.Remove(r);
            Form2 edit = new Form2(r);
            edit.ShowDialog();
            int i = dataGridView1.CurrentRow.Index;
            dataGridView1.Rows[i].Cells[0].Value = tRule.name;
            dataGridView1.Rows[i].Cells[1].Value = tRule.inip;
            dataGridView1.Rows[i].Cells[2].Value = tRule.inport;
            dataGridView1.Rows[i].Cells[3].Value = tRule.extip;
            dataGridView1.Rows[i].Cells[4].Value = tRule.extport;
            RulesList.Add(tRule);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            FileStream fs = File.Open("TranslatorTable.cfg", FileMode.Create, FileAccess.Write);
            StreamWriter stream = new StreamWriter(fs);
            try
            {               
                foreach (Rule r in RulesList)
                {
                    string s = r.name + " " + r.inip.ToString() + " " + r.inport.ToString() + " " + r.extip.ToString() + " " + r.extport.ToString();
                    stream.WriteLine(s);
                }
            }
            finally
			{
                stream.Close();
                fs.Close();
			}
        }

        private void LoadCfg (string path)
        {
            using (StreamReader stream = new StreamReader(path))
            {
                dataGridView1.Rows.Clear();
                while (stream.Peek() > -1)
                {
                    string[] data;
                    data = stream.ReadLine().Split(' ');
                    tRule = new Rule(data[0], IPAddress.Parse(data[1]), Convert.ToInt32(data[2]), IPAddress.Parse(data[3]), Convert.ToInt32(data[4]), true);
                    RulesList.Add(tRule);
                    dataGridView1.Rows.Add(new object[] { tRule.name, tRule.inip, tRule.inport, tRule.extip, tRule.extport });
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                LoadCfg(openFileDialog1.FileName);
            }
        }

        private void Log(string str)
        {
            if (textBox2.InvokeRequired) textBox2.Invoke(new Action<string>((s) => textBox2.Text += " - " + DateTime.Now.ToString() + "   " + str + "\r\n"), str);
            else textBox2.Text += " - " + DateTime.Now.ToString() + "   " + str;
        }

        private void button1_Click(object sender, EventArgs e) //запуск
        {   
            if (!isRunnig)
            {
                tlist = new Thread[RulesList.Count];
                Log("Транслятор запущен\r\n");
                isRunnig = true;
                button1.Text = "Остановить транслятор";
                for (int i = 0; i < RulesList.Count; i++)
                {
                    tlist[i] = new Thread(Listening);
                    tlist[i].IsBackground = true;
                    tlist[i].Start(RulesList[i]);
                }
            }
            else
            {
                Log("Транслятор остановлен\r\n");
                isRunnig = false;
                button1.Text = "Запустить транслятор";
                for (int i = 0; i < RulesList.Count; i++) tlist[i].Abort();
            }
        }

        private void Listening(object o)
        {
            Rule rule = (Rule)o;
            TcpListener TCP = new TcpListener(rule.inip, rule.inport);
            ThreadData td;
            TCP.Start();

            while (true)
            {
                if (TCP.Pending())
                {
                    Thread t = new Thread(Execute);
                    t.IsBackground = true;
                    td = new ThreadData(TCP.AcceptSocket(), rule.extip, rule.extport);
                    t.Start(td);
                }
            }

            TCP.Stop();
        }

        private void Execute(object o)
        {
            ThreadData td = (ThreadData)o;            

            if (td.socket.Connected)
            {
                byte[] httpRequest = Receive(td.socket);
                Regex myReg = new Regex(@"Host: (((?<host>.+?):(?<port>\d+?))|(?<host>.+?))\s+", RegexOptions.Multiline | RegexOptions.IgnoreCase);
                Match m = myReg.Match(System.Text.Encoding.ASCII.GetString(httpRequest));
                string host = m.Groups["host"].Value;
                int port = 0;
                if (checkBox1.Checked)
                {
                    if (!int.TryParse(m.Groups["port"].Value, out port)) { port = 80; }
                }
                else port = td.port;

                //IPHostEntry entry = Dns.GetHostEntry(host);
                //IPEndPoint point = new IPEndPoint(entry.AddressList[0], port);
                
                IPEndPoint point = new IPEndPoint(td.ip, port);
                using (Socket rerouting = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                {
                    try
                    {
                        rerouting.Connect(point);
                    }
                    catch (Exception)
                    {
                        return;
                    }
                    if (rerouting.Send(httpRequest, httpRequest.Length, SocketFlags.None) != httpRequest.Length)
                    {
                        Log("Ошибка при отправке данных\r\n");
                    }
                    else
                    {
                        Log("Отправлено данных: " + httpRequest.Length);
                        byte[] httpResponse = Receive(rerouting);

                        if (httpResponse != null && httpResponse.Length > 0)
                        {
                            td.socket.Send(httpResponse, httpResponse.Length, SocketFlags.None);
                            Log("Получено данных: " + httpResponse.Length);
                        }
                    }
                }
                td.socket.Close();
            }
        }

        private static byte[] Receive(Socket socket)
        {
            byte[] b = new byte[socket.ReceiveBufferSize];
            int len = 0;
            using (MemoryStream m = new MemoryStream())
            {
                try
                {
                    while (socket.Poll(1000000, SelectMode.SelectRead) && (len = socket.Receive(b, socket.ReceiveBufferSize, SocketFlags.None)) > 0)
                    {
                        m.Write(b, 0, len);
                    }
                    return m.ToArray();
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
    }    

    public struct Rule
    {
        public string name;
        public IPAddress inip, extip;
        public int inport, extport;
        public bool success;

        public Rule(string s, IPAddress ip1, int p1, IPAddress ip2, int p2, bool suc)
        {
            name = s;
            inip = ip1;
            inport = p1;
            extip = ip2;
            extport = p2;
            success = suc;
        }
    }

    public struct ThreadData
    {
        public Socket socket;
        public IPAddress ip;
        public int port;

        public ThreadData (Socket s, IPAddress extip, int i)
        {
            socket = s;
            ip = extip;
            port = i;
        }
    }
}
