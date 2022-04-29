using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Server
{
    public class TCPServer
    {
        IPEndPoint serverIP;
        private const int MaxConnections = 10;
        public static ManualResetEvent allDone;
        public Socket server;
        public ServerHandler handler;

        public static StateObject state;
        public static bool isReceive = false;

        public TCPServer(IPAddress ip, int port)
        {
            // получаем адреса для запуска сокета
            this.serverIP = new IPEndPoint(ip, port);
            allDone = new ManualResetEvent(false);
        }

        public void Start()
        {
            frmStart.setServerMessage("");

            // создаем сокет
            this.server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and listen for incoming connections.  
            try
            {
                this.server.Bind(serverIP);
                this.server.Listen(10);

                frmStart.setConnectionStatus("Ожидание подключений...");

                // Set the event to nonsignaled state.  
                allDone.Reset();
                    
                this.server.BeginAccept(new AsyncCallback(AcceptCallback), this.server);

                // Wait until a connection is made before continuing.  
                allDone.WaitOne();

                while (handler.clientSocket.Connected)
                {
                    allDone.Reset();
                    handler.Start();
                    allDone.WaitOne();
                }
                frmStart.setConnectionStatus("не подключен");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            try
            {
                // Get the socket that handles the client request.  
                Socket listener = (Socket)ar.AsyncState;
                Socket accept_socket = listener.EndAccept(ar);
                frmStart.setConnectionStatus("подключен");
                frmStart.setServerMessage("");

                handler = new ServerHandler(accept_socket);
                handler.Start();

                // Signal the main thread to continue.  
                allDone.Set();         
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
    }
}
