using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SocketSamples
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("*** New socket");
            NewTcpSocket();

            Console.WriteLine("*** Socket Connect, Send");
            TcpSocketConnect();

            Console.WriteLine("*** Multi Send");
            UDPMultiSend();
        }

        static void NewTcpSocket()
        {
            IPAddress ia = IPAddress.Parse("127.0.0.1");
            IPEndPoint ie = new IPEndPoint(ia, 8000);

            Socket test = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Console.WriteLine($"Socket data AddressFamily:{test.AddressFamily} SocketType:{test.SocketType} ProtocolType:{test.ProtocolType} Blocking:{test.Blocking}");

            test.Blocking = false;
            Console.WriteLine($"Set blocking = false. Blocking:{test.Blocking}, Connected:{test.Connected}");

            test.Bind(ie);
            IPEndPoint iep = (IPEndPoint)test.LocalEndPoint;
            Console.WriteLine($"LocalEndPoint: {iep}");
            test.Close();
        }

        static void TcpSocketConnect()
        {
            IPAddress host = IPAddress.Parse("192.168.0.104");
            IPEndPoint hostep = new IPEndPoint(host, 8080);
            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                sock.Connect(hostep);
            }
            catch (SocketException e)
            {
                Console.WriteLine("Problem connecting to host");
                Console.WriteLine(e.ToString());
                sock.Close();
                return;
            }

            try
            {
                sock.Send(Encoding.ASCII.GetBytes("testing"));
            }
            catch (SocketException e)
            {
                Console.WriteLine("Problem sending data");
                Console.WriteLine(e.ToString());
                sock.Close();
                return;
            }
            sock.Close();
        }

        static void UDPMultiSend()
        {
            Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint iep = new IPEndPoint(IPAddress.Any, 9051);
            IPEndPoint iep2 = new IPEndPoint(IPAddress.Parse("224.100.0.1"), 9050);
            server.Bind(iep);

            byte[] data = Encoding.ASCII.GetBytes("This is a test message");
            server.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(IPAddress.Parse("224.100.0.1")));
            server.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, 50);
            server.SendTo(data, iep2);
            server.Close();
        }
    }
}
