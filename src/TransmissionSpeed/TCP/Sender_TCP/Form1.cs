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
using System.Net.Sockets;
using System.Threading;

namespace Sender_TCP
{
    public partial class Form1 : Form
    {
        public int numsendbytes;
        public const int numbytes = 256;
        public IPAddress remoteIp;
        public IPEndPoint remoteEP;
        public DateTime timeStartTCP;
        public DateTime timeEndTCP;
        public int packetsCount;
        public int port;

        public Form1()
        {
            InitializeComponent();
        }


        public void ConnectToTcpServer(int numbytes)
        {
            try
            {
                Socket socket;
                socket = TcpConnectToServer();
                SendDataByTCP(socket, numbytes);
                TerminateConnection(socket);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void TerminateConnection(Socket socket)
        {
            try
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void SendDataByTCP(Socket socket, int numbytes)
        {
            if (numbytes != -1)
            {
                byte[] data = new byte[numbytes];
                GetNumbers(numbytes, data);
                TcpSendData(socket, data, numbytes);
            }
            else
            {
                byte[] data = new byte[1];
                TcpSendData(socket, data, numbytes);
            }
        }

        private string PrepToSendInfoAboutConnection()
        {
            string result = "";
            result += timeStartTCP.TimeOfDay.ToString();
            result += numsendbytes.ToString();
            return result;
        }

        public void GetNumbers(int count, byte[] numbers)
        {
            for (int i = 0; i< count; i++)
            {
                numbers[i] = Convert.ToByte(i % 254 + 1);
            }
        }

        public void TcpSendData(Socket socket, byte[] data, int numbytes)
        {
            if (numbytes != -1)
            {
                timeStartTCP = DateTime.Now;
                socket.Send(data);
            }
            else
            {
                Encoding u8 = Encoding.UTF8;
                string info = PrepToSendInfoAboutConnection();
                socket.Send(u8.GetBytes(info));
            }
        }

        public Socket TcpConnectToServer()
        {
            try
            {
                port = int.Parse(txtPort.Text);
                IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse(txtIp.Text), port);
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(remoteEP);
                return socket;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return null;
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            numsendbytes = Convert.ToInt32(txtNumPack.Text);
            ConnectToTcpServer(numsendbytes);
            Thread.Sleep(2500);
            ConnectToTcpServer(-1);     
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
