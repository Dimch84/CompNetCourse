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

namespace SenderUDP2
{
    public partial class Form1 : Form
    {
        public int remotePort;
        public IPAddress remoteIP;
        public IPEndPoint remoteEP;
        public DateTime timeStartUDP;
        Socket listeningSocket;
        public Form1()
        {
            InitializeComponent();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            int numSendBytes = Convert.ToInt32(txtNumb.Text);
            SendDataByUDP(numSendBytes);
            SendDataAboutPackets();
            SendDataAboutTime();
        }

        public void SendDataByUDP(int NumByte)
        {
            try
            {
                listeningSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                // отправка сообщений на разные порты
                byte[] data = new byte[NumByte];
                GetNumbers(NumByte, data);
                remotePort = Convert.ToInt32(txtPort.Text);
                EndPoint remotePoint = new IPEndPoint(IPAddress.Parse(txtIP.Text), remotePort);
                timeStartUDP = DateTime.Now;
                listeningSocket.SendTo(data, remotePoint);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                CloseConnection();
            }
        }

        private void SendDataAboutPackets()
        {
            remoteIP = IPAddress.Parse(txtIP.Text);
            remoteEP = new IPEndPoint(remoteIP, remotePort);
            int packetsCount = Convert.ToInt32(txtNumb.Text);
            // Отправляе число пакетов по Tcp
            using (Socket socket = new Socket(remoteIP.AddressFamily, SocketType.Stream, ProtocolType.Tcp))
            {
                socket.Connect(remoteEP);

                socket.Send(BitConverter.GetBytes(packetsCount));

                socket.Shutdown(SocketShutdown.Both);
            }
        }

        private void SendDataAboutTime()
        {
            remoteIP = IPAddress.Parse(txtIP.Text);
            remoteEP = new IPEndPoint(remoteIP, remotePort);
            Encoding u8 = Encoding.UTF8;
            string info = timeStartUDP.TimeOfDay.ToString();
            // Отправляем число пакетов по Tcp
            using (Socket socket = new Socket(remoteIP.AddressFamily, SocketType.Stream, ProtocolType.Tcp))
            {
                socket.Connect(remoteEP);

                socket.Send(u8.GetBytes(info));

                socket.Shutdown(SocketShutdown.Both);
            }
        }

        private void CloseConnection()
        {
            if (listeningSocket != null)
            {
                listeningSocket.Shutdown(SocketShutdown.Both);
                listeningSocket.Close();
                listeningSocket = null;
            }
        }

        public void GetNumbers(int count, byte[] numbers)
        {
            for (int i = 0; i < count; i++)
                numbers[i] = Convert.ToByte(i % 254 + 1);
        }
    }
}
