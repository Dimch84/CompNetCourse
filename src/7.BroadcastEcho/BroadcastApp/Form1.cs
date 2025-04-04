using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

namespace BroadcastApp
{
    // Приложение, подсчитывающее количество копий себя, запущенных в локальной сети
    // Приложение должно использовать набор сообщений, чтобы информировать другие приложения о своем состоянии.
    // После запуска приложение должно рассылать широковещательное сообщение о том что оно было запущено.
    // Получив сообщение о запуске другого приложения оно должно сообщать этому приложению о том, что оно работает.
    // Перед завершением работы приложение должно информировать все известные приложения о том что оно завершает работу.
    // На экран должен выводиться список IP адресов компьютеров на которых приложение запущено.
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private byte[] StartMsg, EndMsg;
        private Thread listener;
        private Socket _socket;
        private IPEndPoint _point;
        private IPAddress myIp;
        private List<string> list;

        private static UdpClient udpclient;
        private static Dictionary<string, int> data;

        private const int MAX_PORT = 11000;
        private static readonly int fixed_port = new Random().Next(1200, MAX_PORT - 1);

        private static int curr_port;

        private void Form1_Load(object sender, EventArgs e)
        {
            StartMsg = Encoding.ASCII.GetBytes("Hello");
            EndMsg = Encoding.ASCII.GetBytes("Bye");

            data = new Dictionary<string, int>();
            list = new List<string>();

            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);

            myIp = Dns.GetHostByName(Dns.GetHostName()).AddressList[0];
            try
            {
                udpclient = new UdpClient(MAX_PORT);
                _socket.Bind(new IPEndPoint(myIp, MAX_PORT));
                curr_port = MAX_PORT;
            }
            catch (Exception exc)
            {
                Console.WriteLine("Only one application can bind a port");
                udpclient = new UdpClient(fixed_port);
                _socket.Bind(new IPEndPoint(myIp, fixed_port));
                curr_port = fixed_port;
            }
            listBox1.Items.Add($"{myIp}:{curr_port}");

            _point = new IPEndPoint(IPAddress.Broadcast, MAX_PORT);
            _socket.SendTo(StartMsg, _point);

            timer1.Interval = Convert.ToInt32(textBox2.Text);

            listener = new Thread(() => Listener());
            listener.Start();
            timer1.Start();
        }

        private void Listener()
        {
            IPEndPoint point = new IPEndPoint(IPAddress.Any, curr_port);
            Byte[] temp;
            string pstr, recstr;
            string ip_port_key;

            while (true)
            {
                try
                {
                    temp = udpclient.Receive(ref point);
                    pstr = point.Address.ToString();
                    if (pstr == myIp.ToString() && curr_port == point.Port)
                        continue;
                   
                    ip_port_key = $"{pstr}:{point.Port}";
                    recstr = Encoding.ASCII.GetString(temp);
                    if (recstr == "Bye") 
                        data[ip_port_key] = -1;
                    else
                        if (data.ContainsKey(ip_port_key)) 
                            data[ip_port_key] = 6; // six timer ticks before response from host
                        else
                        {
                            data.Add(ip_port_key, 6);
                            SetIP(ip_port_key);
                        }
                }
                catch (Exception exc)
                {
                    Console.WriteLine("Exception while listening: " + exc.Message);
                    continue;
                }
            }
        }

        private void SetIP(string ip_port)
        {
            if (listBox1.InvokeRequired) 
                listBox1.Invoke(new Action<string>((i) => listBox1.Items.Add(i)), ip_port);
            else 
                listBox1.Items.Add(ip_port);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            _socket.SendTo(StartMsg, _point);

            for (int i = 1; i < listBox1.Items.Count; i++)
            {
                string tip = listBox1.Items[i].ToString();
                if (--data[tip] <= 0)
                {
                    data.Remove(tip);
                    list.Add(tip);
                }
            }

            foreach (var i in list) 
                listBox1.Items.Remove(i);

            list.Clear();

            textBox1.Text = " Копий запущенно: " + listBox1.Items.Count;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            listener.Abort();            
            udpclient.Close();

            _socket.SendTo(EndMsg, new IPEndPoint(IPAddress.Broadcast, MAX_PORT));

            _socket.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            timer1.Interval = Convert.ToInt32(textBox2.Text);
        }
    }
}
