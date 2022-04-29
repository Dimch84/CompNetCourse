using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace Server
{
    public partial class frmStart : Form
    {
        public frmStart()
        {
            InitializeComponent();

            Program.startForm = this;
        }

        private void btnEstablishConnection_Click(object sender, EventArgs e)
        {
            IPAddress serverIP;
            if (!IPAddress.TryParse(fieldIP.Text, out serverIP))
            {
                setServerMessage("Некорректный IP адрес сервера!");
                return;
            }

            int serverPort = 0;
            if (!Int32.TryParse(fieldPort.Text, out serverPort))
            {
                setServerMessage("Некорректный номер порта!");
                return;
            }

            var server = new TCPServer(serverIP, serverPort);
            Task.Run(() => server.Start());
        }

        public static void setConnectionStatus(string msg)
        {
            lblStatus.Text = msg;
        }

        public static void setServerMessage(string msg)
        {
            lblServerMessage.Text = msg;
        }

    }
}
