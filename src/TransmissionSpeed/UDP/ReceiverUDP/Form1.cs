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

namespace ReceiverUDP2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public DateTime timeEndUDP;
        public DateTime timeStartUDP;
        public IPAddress localIp;
        public IPEndPoint localEP;
        public int packetsCount;
        public int localPort;
        Socket listeningSocket;
        public const int numBytesInBuffer = 100000000;

        private void btnReceive_Click(object sender, EventArgs e)
        {
            localIp = IPAddress.Parse(txtIP.Text);
            localPort = Convert.ToInt32(txtPort.Text);
            localEP = new IPEndPoint(localIp,localPort);
            int numBytesUDP = RecieveDataByUDP();
            //timeStartUDP = DateTime.Now;
            RecieveNumPack();
            RecieveTime();
            PrintInfoAboutConnection(numBytesUDP);
            this.Refresh();
        }

        public void PrintInfoAboutConnection(int Numb)
        {
            double TimeS = timeStartUDP.TimeOfDay.TotalSeconds;
            double TimeE = timeEndUDP.TimeOfDay.TotalSeconds;
            double UDPTimeDif = (timeEndUDP.TimeOfDay - timeStartUDP.TimeOfDay).TotalSeconds;
            string speed = Convert.ToString(Math.Round(Numb * 0.001 / UDPTimeDif, 3)) + " KB/S";
            string packNum = Convert.ToString(Numb) + " of " + Convert.ToString(packetsCount);

            txtNumPack.Text = packNum;
            txtSpeed.Text = speed;
        }

        public int RecieveDataByUDP()
        {
            try
            {
                localPort = Convert.ToInt32(txtPort.Text);
                //Прослушиваем по адресу
                IPEndPoint localIP = new IPEndPoint(IPAddress.Parse(txtIP.Text), localPort);
                listeningSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                listeningSocket.Bind(localIP);

                while (true)
                {
                    int bytes = 0; // количество полученных байтов
                    byte[] data = new byte[numBytesInBuffer]; // буфер для получаемых данных

                    //адрес, с которого пришли данные
                    EndPoint remoteIp = new IPEndPoint(IPAddress.Any, 0);

                    do
                    {
                        bytes = listeningSocket.ReceiveFrom(data, ref remoteIp);
                        if (bytes != 0)
                        {
                            timeEndUDP = DateTime.Now;
                            break;
                        }
                    }
                    while (listeningSocket.Available > 0);
                    timeEndUDP = DateTime.Now;

                    return bytes;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return 1;
            }
            finally
            {
                CloseConnection();
            }
        }

        private void RecieveNumPack()
        {
            byte[] buffer = new byte[sizeof(int)];

            try
            {
                // По Tcp принимаем число пакетов
                using (Socket listener = new Socket(localIp.AddressFamily, SocketType.Stream, ProtocolType.Tcp))
                {
                    listener.Bind(localEP);
                    listener.Listen(10);
                    using (Socket socket = listener.Accept())
                    {
                        socket.Receive(buffer);
                        packetsCount = BitConverter.ToInt32(buffer, 0);
                        socket.Shutdown(SocketShutdown.Both);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void RecieveTime()
        {
            byte[] buffer;

            try
            {
                // По Tcp принимаем число пакетов
                using (Socket listener = new Socket(localIp.AddressFamily, SocketType.Stream, ProtocolType.Tcp))
                {
                    listener.Bind(localEP);
                    listener.Listen(10);
                    using (Socket socket = listener.Accept())
                    {
                        Encoding u8 = Encoding.UTF8;
                        buffer = TcpReceiveData(socket);
                        string time;
                        time = u8.GetString(buffer, 0, 16);
                        string tmp = time.Substring(0, 16);
                        timeStartUDP = Convert.ToDateTime(tmp);
                        socket.Shutdown(SocketShutdown.Both);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public byte[] TcpReceiveData(Socket socket)
        {
            byte[] dataBytes = new byte[numBytesInBuffer];
            do
            {
                socket.Receive(dataBytes, dataBytes.Length, 0);
            }
            while (socket.Available > 0);

            return dataBytes;
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
    }
}
