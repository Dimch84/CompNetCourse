using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ProxyServer
{
    class ProxyServer
    {
        const string CRLF = "\r\n";

        Socket clientSocket;
        Byte[] read = new byte[1024];
        Byte[] Buffer = null;
        Byte[] RecvBytes = new Byte[4096];

        public ProxyServer(Socket socket)
        {
            this.clientSocket = socket;
        }

        public void Run()
        {
            // http://localhost:8889/gaia.cs.umass.edu/wireshark-labs/HTTP-wireshark-file3.html
            string clientMessage = " ", sURL = " ";
            int bytes = ReadMessage(read, clientSocket, ref clientMessage);
            if (bytes == 0)
                return;

            var parts = clientMessage.Split();
            sURL = parts[1].Substring(1).Split("/")[0];

            Console.WriteLine("Connecting to Site: {0}", sURL);
            Console.WriteLine("Connection from {0}", clientSocket.RemoteEndPoint);
            try
            {
                IPHostEntry IPHost = Dns.Resolve(sURL);
                Console.WriteLine($"Request resolved: {IPHost.HostName}");

                string[] aliases = IPHost.Aliases;
                IPAddress[] address = IPHost.AddressList;
                Console.WriteLine($"Request IP address: {address[0]}");

                IPEndPoint sEndpoint = new IPEndPoint(address[0], 80);
                Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                serverSocket.Connect(sEndpoint);
                if (serverSocket.Connected)
                    Console.WriteLine("Socket connect OK");

                string GET = ModifyRequestForServer(clientMessage);
                Byte[] ByteGet = Encoding.ASCII.GetBytes(GET);
                serverSocket.Send(ByteGet, ByteGet.Length, 0);
                Int32 rBytes = serverSocket.Receive(RecvBytes, RecvBytes.Length, 0);
                Console.WriteLine($"Recieved {rBytes} bytes");

                String strRetPage = null;
                strRetPage = strRetPage + Encoding.ASCII.GetString(RecvBytes, 0, rBytes);
                while (rBytes > 0)
                {
                    rBytes = serverSocket.Receive(RecvBytes, RecvBytes.Length, 0);
                    strRetPage = strRetPage + Encoding.ASCII.GetString(RecvBytes, 0, rBytes);
                }

                serverSocket.Shutdown(SocketShutdown.Both);
                serverSocket.Close();

                SendMessage(clientSocket, strRetPage);
            }
            catch (Exception exc2)
            {
                Console.WriteLine(exc2.ToString());
            }
        }

        private string ModifyRequestForServer(string clientReq)
        {
            var parts = clientReq.Split(CRLF);
            var getLine = parts[0];
            int idx1 = getLine.IndexOf('/');
            int idx2 = getLine.Substring(idx1+1).IndexOf('/');
            var hostName = getLine.Substring(idx1+1, idx2);
            getLine = "GET " + getLine.Substring(idx1 + idx2 + 1);
            var hostLine = $"Host: {hostName}";
            parts[0] = getLine;
            parts[1] = hostLine;
            var modifiedRequest = String.Join(CRLF, parts);

            return modifiedRequest;
        }

        private int ReadMessage(byte[] ByteArray, Socket s, ref string clientMessage)
        {
            int bytes = s.Receive(ByteArray, 1024, 0);
            clientMessage = Encoding.ASCII.GetString(ByteArray);
            return bytes;
        }

        private void SendMessage(Socket s, string message)
        {
            Buffer = new Byte[message.Length + 1];
            int length = Encoding.ASCII.GetBytes(message, 0, message.Length, Buffer, 0);
            s.Send(Buffer, length, 0);
        }

        private static void TcpListenerProxyServer()
        {
            const int port = 8889;
            TcpListener tcplistener = new TcpListener(port);

            Console.WriteLine("Listening on port {0}", +port);
            tcplistener.Start();
            while (true)
            {
                Socket clientSocket = tcplistener.AcceptSocket();
                ProxyServer webproxy = new ProxyServer(clientSocket);
                webproxy.Run();
            }
        }

        private static void SocketProxyServer()
        {
            const int port = 8889;
            IPAddress ia = IPAddress.Parse("127.0.0.1");
            IPEndPoint ie = new IPEndPoint(ia, port);

            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(ie);

            Console.WriteLine($"Listening on port {port}");
            socket.Listen();

            while (true)
            {
                Socket clientSocket = socket.Accept();
                ProxyServer webproxy = new ProxyServer(clientSocket);
                webproxy.Run();
            }
        }

        static void Main(string[] args)
        {
            TcpListenerProxyServer();
            //SocketProxyServer();
        }
    }
}
