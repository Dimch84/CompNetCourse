using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace Receiver_TCP
{
    public partial class Form1 : Form
    {
        public Socket server;
        public int numsendbytes;
        public const int numbytes = 256;
        public IPAddress remoteIp;
        public IPEndPoint remoteEP;
        public DateTime timeStartTCP;
        public DateTime timeEndTCP;
        public int packetsCount;
        public int port;
        public const int numbytesinbuffer = 100000000;

        public Form1()
        {
            InitializeComponent();
        }

        public Socket TcpCreateServer()
        {
            port = Convert.ToInt32(txtPort.Text);
            EndPoint remoteEP = new IPEndPoint(IPAddress.Parse(txtIp.Text), port);
            Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listenSocket.Bind(remoteEP);

            return listenSocket;
        }

        public byte[] ListeningForConnections(Socket server)
        {
            byte[] ReceiveBytes;
            Socket client;
            server.Listen(10);
            client = server.Accept();
            ReceiveBytes = TcpReceiveData(client);
            TerminateConnections(client);
            return ReceiveBytes;

        }

        public byte[] TcpReceiveData(Socket socket)
        {
            byte[] dataBytes = new byte[numbytesinbuffer];
            do
            {
                socket.Receive(dataBytes, dataBytes.Length, 0);
            }
            while (socket.Available > 0);
            timeEndTCP = DateTime.Now;

            return dataBytes;
        }

        public void TerminateConnections(Socket socket)
        {
            try
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnReceive_Click(object sender, EventArgs e)
        {
            server = CreateTCPServer();
            int numBytesTcp = GetNumBytes(ListeningForConnections(server));
            byte[] InfoAboutConnection = ListeningForConnections(server);
            PrintInfo(numBytesTcp, InfoAboutConnection);
            this.Refresh();
            Thread.Sleep(1000);
            server.Close();
        }

        public void PrintInfo(int NumReceiveBytesByTcp, byte[] InfoAboutConnection)
        {
            Encoding u8 = Encoding.UTF8;
            string Info = u8.GetString(InfoAboutConnection, 0, 50);
            string tmp = Info.Substring(0, 16);
            DateTime StartSendTCP = Convert.ToDateTime(tmp);
            tmp = Info.Substring(16,Info.Length-40);
            DateTime FinishSendTCP = timeEndTCP;
            double TCPTimeDif = (FinishSendTCP.TimeOfDay - StartSendTCP.TimeOfDay).TotalSeconds;
            string check = Convert.ToString(NumReceiveBytesByTcp) + " of " + Convert.ToString(Convert.ToInt32(tmp));
            string speed = Convert.ToString(Math.Round(Convert.ToInt32(tmp) / TCPTimeDif, 3)) + " B/S";
            txtNumPack.Text = check;
            txtSpeed.Text = speed;
        }

        public int GetNumBytes(byte[] data)
        {
            int i;
            for (i = numbytesinbuffer - 1; i >= 0; i--)
            {
                if (data[i] != 0)
                    break;
            }
            i++;
            return i;
        }

        public Socket CreateTCPServer()
        {
            try
            {
                Socket socket;
                socket = TcpCreateServer();
                return socket;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return null;
        }

    }
}
