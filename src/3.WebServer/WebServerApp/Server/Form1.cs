using System;
using System.Collections;
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

namespace Server
{
    public partial class Form1 : Form
    {
        private ArrayList nSockets;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            timer1.Start();
            DateTime localTime = DateTime.Now;
            label1.Text = localTime.ToString();

            IPHostEntry IPHost = Dns.GetHostByName(Dns.GetHostName());
            labelStatus.Text = "My IP address is " + IPHost.AddressList[0].ToString();

            nSockets = new ArrayList();

            Thread thdListener = new Thread(() => listenerThread());
            thdListener.Start();
        }

        public void listenerThread()
        {
            TcpListener tcpListener = new TcpListener(8764);
            tcpListener.Start();

            while (true)
            {
                Socket handlerSocket = tcpListener.AcceptSocket();
                if (handlerSocket.Connected)
                {
                    this.Invoke(new Action(() => lbConnections.Items.Add(handlerSocket.RemoteEndPoint.ToString() + " connected.")));
                    lock (this)
                    {
                        nSockets.Add(handlerSocket);
                    }

                    Thread thdHandler = new Thread(() => handlerThread());
                    thdHandler.Start();
                }
            }
        }

        public void handlerThread()
        {
            Socket handlerSocket = (Socket)nSockets[nSockets.Count - 1];
            NetworkStream networkStream = new NetworkStream(handlerSocket);

            int thisRead = 0;
            int blockSize = 1024;
            Byte[] dataByte = new Byte[blockSize];
            lock (this)
            {
                // Only one process can access
                // the same file at any given time
                using (FileStream fileStream = new FileStream(@"C:\\Temp\\test.txt", FileMode.Create))
                    while (true)
                    {
                        thisRead = networkStream.Read(dataByte, 0, blockSize);
                        fileStream.Write(dataByte, 0, thisRead);
                        if (thisRead == 0) break;
                    }
            }
            handlerSocket = null;
        }

        private void lbConnections_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            DateTime localTime = DateTime.Now;
            label1.Text = localTime.ToString();
            timer1.Start();
        }
    }
}
