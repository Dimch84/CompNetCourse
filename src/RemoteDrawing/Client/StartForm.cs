using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;

namespace Client
{
    public partial class frmStart : Form
    {
        public frmStart()
        {
            InitializeComponent();
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

            frmMain.client = new TCPClient(serverIP, serverPort);
            frmMain.client.Connect();
        }

        public static void setConnectionStatus(string msg)
        {
            lblStatus.Text = msg;
        }

        public static void setServerMessage(string msg)
        {
            lblServerMessage.Text = msg;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            frmMain.buffer[8] = 2;
            frmMain.ClearDrawField();
            frmMain.client.Send(frmMain.buffer);
        }
    }
}
