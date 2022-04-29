using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace Microsoft.Samples.IPv6Sockets
{
    static class IPv6Client
    {
        static void Main(string[] args)
        {
            string serverDnsName = args[0]; // "localhost"
            try
            {
                IPHostEntry resolvedServer = Dns.GetHostEntry(serverDnsName);
                for (int i = 0; i < resolvedServer.AddressList.Length; i++)
                {
                    IPAddress address = resolvedServer.AddressList[i];
                    if (address.AddressFamily != AddressFamily.InterNetworkV6)
                        continue;

                    IPEndPoint serverEndPoint = new IPEndPoint(address, 5150);
                    Socket tcpSocket = 
                        new Socket(
                            address.AddressFamily, 
                            SocketType.Stream, 
                            ProtocolType.Tcp);
                    try
                    {
                        tcpSocket.Connect(serverEndPoint);
                        StreamWriter writer = null;
                        StreamReader reader = null;
                        try
                        {
                            NetworkStream networkStream = 
                                new NetworkStream(tcpSocket);
                            writer = new StreamWriter(networkStream);
                            string clientMessage = "Hi there!";
                            writer.WriteLine(clientMessage);
                            writer.Flush();
                            Console.WriteLine(
                                "Client sent message: {0}", clientMessage);

                            reader = new StreamReader(networkStream);
                            string serverMessage = reader.ReadLine();
                            Console.WriteLine(
                                "Client received message: {0}", serverMessage);
                        }
                        catch (SocketException ex)
                        {
                            Console.WriteLine(
                                "Message exchange failed: {0}", ex.Message);
                        }
                        catch (IOException ex)
                        {
                            Console.WriteLine(
                                "Message exchange failed: {0}", ex.Message);
                        }
                        finally
                        {
                            if (reader != null)
                                reader.Close();
                            if (writer != null)
                                writer.Close();
                        }
                        break;
                    }
                    catch (SocketException)
                    {
                        if (tcpSocket != null)
                            tcpSocket.Close();
                        if (i == resolvedServer.AddressList.Length - 1)
                            Console.WriteLine(
                                "Failed to connect to the server.");
                    }
                }

            }
            catch (SocketException ex)
            {
                Console.WriteLine(
                    "Could not resolve server DNS name: {0}", ex.Message);
            }
        }
    }
}
