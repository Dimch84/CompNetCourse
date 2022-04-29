using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Server
{
    // State object for reading client data asynchronously  
    public class StateObject
    {
        // Size of receive buffer.  
        public const int bufferSize = 1024;

        // Receive buffer.  
        public byte[] buffer = new byte[bufferSize];

        // Received data string.
        public StringBuilder sb = new StringBuilder();

        // Client socket.
        public Socket workSocket = null;
    }

    public class ServerHandler
    {
        public Socket clientSocket;
        public ServerHandler(Socket clientSocket)
        {
            this.clientSocket = clientSocket;
        }

        public void Start()
        {
            Receive();
        }

        private void Receive()
        {
            StateObject state = new StateObject();
            state.workSocket = this.clientSocket;
            state.workSocket.BeginReceive(state.buffer, 0, StateObject.bufferSize, SocketFlags.None, new AsyncCallback(ReceiveCallBack), state);
        }

        private void ReceiveCallBack(IAsyncResult ar)
        {
            try
            {
                string cmd = String.Empty;
                StateObject state = ar.AsyncState as StateObject;
                Socket handler = state.workSocket;

                if (handler.Connected)
                {
                    //количество принятых байт
                    int bytes = handler.EndReceive(ar);
                    if (bytes > 0)
                    {
                        //сохраняем принятые данные в классе
                        if (state.buffer.Length == 0)
                            //если приняли пустую строку, то принимаем дальше
                            handler.BeginReceive(state.buffer, 0, StateObject.bufferSize, SocketFlags.None, new AsyncCallback(ReceiveCallBack), state);
                    }

                    frmMain.DrawPoints(state.buffer);
                    TCPServer.allDone.Set();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
    }
}
