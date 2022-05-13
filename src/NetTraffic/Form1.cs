using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using NetTraffic.Headers;

namespace NetTraffic
{  
    public partial class Form1 : Form
    {    
        public Form1()
        {
            InitializeComponent();
        }

        private Socket socket;
        private string myip;
        private byte[] data = new byte[4096];
        private bool isRunning = false;

        private static int insize;
        private static int outsize;
        private static Dictionary<int, int> inport;
        private static Dictionary<int, int> outport;

        private void Form1_Load(object sender, EventArgs e)
        {
            myip = Dns.GetHostByName(Dns.GetHostName()).AddressList[0].ToString();
            textBox1.Text = myip;
            insize = outsize = 0;
            inport = new Dictionary<int, int>();
            outport = new Dictionary<int, int>();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (isRunning) 
                socket.Close();
        }

        private void Log(Unit u)
        {
            if (listBox1.InvokeRequired) 
                listBox1.Invoke(new Action<Unit>((s) => listBox1.Items.Add(s)), u);
            else listBox1.Items.Add(u);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (!isRunning)
                {
                    button1.Text = "Стоп";
                    isRunning = true;
                    timer1.Start();
                    socket = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.IP);
                    socket.Bind(new IPEndPoint(IPAddress.Parse(textBox1.Text), 0));
                    socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.HeaderIncluded, true);                          
                    byte[] byTrue = new byte[4] { 1, 0, 0, 0 };
                    byte[] byOut = new byte[4] { 1, 0, 0, 0 }; 
                    socket.IOControl(IOControlCode.ReceiveAll, byTrue, byOut);
                    socket.BeginReceive(data, 0, data.Length, SocketFlags.None, new AsyncCallback(OnReceive), null);                    
                }
                else
                {
                    button1.Text = "Старт";
                    isRunning = false;
                    timer1.Stop();
                    socket.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void OnReceive(IAsyncResult ar)
        {
            try
            {
                int nReceived = socket.EndReceive(ar);

                ParseData(data, nReceived);

                if (isRunning)
                {
                    data = new byte[4096];
                    socket.BeginReceive(data, 0, data.Length, SocketFlags.None, new AsyncCallback(OnReceive), null);
                }
            }
            catch (ObjectDisposedException)
            {
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ParseData(byte[] byteData, int nReceived)
        {
            IPHeader ipHeader = new IPHeader(byteData, nReceived);

            if (radioButton1.Checked)
            {
                if (ipHeader.SourceAddress.ToString() == myip) 
                    outsize += ipHeader.MessageLength;
                else 
                    insize += ipHeader.MessageLength;

                string s = ipHeader.SourceAddress.ToString() + " -> " + ipHeader.DestinationAddress.ToString() + "  " + ipHeader.MessageLength;
                string f = ipHeader.SourceAddress.ToString() + " -> " + ipHeader.DestinationAddress.ToString() +
                    "\r\n" + ipHeader.Version +
                    "\r\nSize: " + ipHeader.MessageLength +
                    " b\r\nTime to live: " + ipHeader.TTL +
                    "\r\nSource: " + ipHeader.SourceAddress.ToString() +
                    "\r\nDestination: " + ipHeader.DestinationAddress.ToString();

                switch (ipHeader.ProtocolType)
                {
                    case Protocol.TCP:
                        TCPHeader tcpHeader = new TCPHeader(ipHeader.Data, ipHeader.MessageLength);
                        f += "\r\nProtocol: TCP\r\nSource Port: " + tcpHeader.SourcePort + "\r\nDestination Port: " + tcpHeader.DestinationPort;
                        break;
                    case Protocol.UDP:
                        UDPHeader udpHeader = new UDPHeader(ipHeader.Data, (int)ipHeader.MessageLength);
                        f += "\r\nProtocol: UDP\r\nSource Port: " + udpHeader.SourcePort + "\r\nDestination Port: " + udpHeader.DestinationPort;
                        break;
                    case Protocol.Unknown:
                        f += "\r\nProtocol: Unknown";
                        break;
                }

                Log(new Unit(s, f));
            }
            else 
            {
                int size = ipHeader.MessageLength;
                int port = 0;
                switch (ipHeader.ProtocolType)
                {
                    case Protocol.TCP:
                        port = Convert.ToInt32(new TCPHeader(ipHeader.Data, ipHeader.MessageLength).DestinationPort);
                        break;
                    case Protocol.UDP:
                        port = Convert.ToInt32(new UDPHeader(ipHeader.Data, (int)ipHeader.MessageLength).DestinationPort);
                        break;
                    case Protocol.Unknown:
                        return;
                }

                if (radioButton2.Checked)
                {
                    if (!inport.ContainsKey(port)) 
                        inport.Add(port, size);
                    else inport[port] += size;
                }
                else
                {
                    if (!outport.ContainsKey(port)) 
                        outport.Add(port, size);
                    else outport[port] += size;
                }
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {
            panel1.Visible = false;
        }

        private void listBox1_Click(object sender, EventArgs e)
        {
            panel1.Visible = false;
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
                label1.Text = ((Unit)listBox1.SelectedItem).info;
            else
                label1.Text = (string)listBox1.SelectedItem;

            panel1.Visible = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                label4.Text = insize.ToString();
                label5.Text = outsize.ToString();
            }
            else
            {
                listBox1.Items.Clear();
                if (radioButton2.Checked) 
                    foreach (KeyValuePair<int, int> kvp in outport) 
                        listBox1.Items.Add("Port: " + kvp.Key + "  size: " + kvp.Value);
                else 
                    foreach (KeyValuePair<int, int> kvp in inport) 
                        listBox1.Items.Add("Port: " + kvp.Key + "  size: " + kvp.Value);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
        }
    }

    //////////////////////////////////////////////
    
    public enum Protocol
    {
        TCP = 6,
        UDP = 17,
        Unknown = -1
    };

    public class Unit
    {
        string shortInfo;
        string fullInfo;

        public string info
        {
            get
            {
                return fullInfo;
            }
        }

        public Unit (string s1, string s2)
        {
            shortInfo = s1;
            fullInfo = s2;
        }

        public override string ToString()
        {
            return shortInfo;
        }
    }
}