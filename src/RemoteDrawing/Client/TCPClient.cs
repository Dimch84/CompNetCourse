using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;


namespace Client
{
    public class TCPClient
    {
        private Socket client;
        private IPEndPoint serverIP;

        public TCPClient(IPAddress ip, int port)
        {
            this.serverIP = new IPEndPoint(ip, port);
            this.client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            frmStart.setServerMessage("");
        }

        public void Connect()
        {
            try
            {
                this.client.BeginConnect(this.serverIP, new AsyncCallback(ConnectCallBack), this.client);
            }
            catch (Exception e)
            {
                frmStart.setServerMessage("Не удалось установить соединение..");
            }
        }

        private void ConnectCallBack(IAsyncResult ar)
        {
            var handler = (Socket)ar.AsyncState;
            this.client.EndConnect(ar);
            frmStart.setConnectionStatus("подключен к серверу");
            frmStart.setServerMessage("");
        }

        public void Disconnect()
        {
            this.client.BeginDisconnect(false, new AsyncCallback(DisconnectCallBack), this.client);
        }

        private void DisconnectCallBack(IAsyncResult ar)
        {
            Socket handler = ar.AsyncState as Socket;
            handler.EndDisconnect(ar);
            frmStart.setConnectionStatus("не подключен");
            frmStart.setServerMessage("Соединение разорвано...");
        }

        public void Send(byte[] buffer)
        {
            try
            {
                this.client.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(SendCallBack), this.client);
            }
            catch (Exception e)
            {
                frmStart.setServerMessage("Не удалось отправить данные..");
            }
        }

        private void SendCallBack(IAsyncResult ar)
        {
            Socket handler = ar.AsyncState as Socket;
            handler.EndSend(ar);
            frmStart.setServerMessage("");
        }
    }
}
