using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace Microsoft.Samples.IPv6Sockets
{
    static class IPv6Server
    {
        static void Main()
        {
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.IPv6Any, 5150);

            Socket serverSocket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, true);

            try
            {
                serverSocket.Bind(localEndPoint);
                serverSocket.Listen(int.MaxValue);
                Console.WriteLine("Server started.");

                while (true)
                {
                    try
                    {
                        Socket clientSocket = serverSocket.Accept();
                        Console.WriteLine(
                            "Accepted connection from: {0}",
                            clientSocket.RemoteEndPoint.ToString());

                        StreamReader reader = null;
                        StreamWriter writer = null;
                        try
                        {
                            NetworkStream networkStream = 
                                new NetworkStream(clientSocket);
                            reader = new StreamReader(networkStream);
                            string clientMessage = reader.ReadLine();
                            Console.WriteLine(
                                "Server received message: {0}", clientMessage);

                            writer = new StreamWriter(networkStream);
                            string serverMessage = "Hello!";
                            writer.WriteLine(serverMessage);
                            writer.Flush();
                            Console.WriteLine(
                                "Server sent message: {0}", serverMessage);
                        }
                        catch (SocketException ex)
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
                    }
                    catch (SocketException ex)
                    {
                        Console.WriteLine(
                            "Server could not accept connection: {0}",
                            ex.Message);
                    }
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine("Failed to start server: {0}", ex.Message);
            }
            finally
            {
                if (serverSocket != null)
                    serverSocket.Close();
            }
        }
    }
}
