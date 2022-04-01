using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Text;

public class ClientHandler
{
	public TcpClient clientSocket;
	
	public void runClient()
	{
		// creating the stream classes
		StreamReader readerStream = new StreamReader(clientSocket.GetStream());
		NetworkStream writerStream = clientSocket.GetStream();
		
		string returnData = readerStream.ReadLine();
		string userName = returnData;
		
		Console.WriteLine("Welcome " + userName + " to the Server");
				
		while(true)
		{				
			returnData = readerStream.ReadLine();
			
			if (returnData.IndexOf("<EOF>") > -1)
			{
				Console.WriteLine("Bye Bye " + userName );
				break;
			}
			
			Console.WriteLine(userName + ": " + returnData);
			
			returnData += "\r\n";
				
			byte [] dataWrite = Encoding.ASCII.GetBytes(returnData);
			
			writerStream.Write(dataWrite, 0, dataWrite.Length);
		}
	
		clientSocket.Close();
	}
}

public class EchoServer
{
	const int ECHO_PORT = 8080;
	public static int nClients = 0;
	
	public static void Main(string [] arg)
	{
		try
		{
			IPAddress ip=IPAddress.Parse("127.0.0.1");

			// binding the server to the local port
			TcpListener clientListener = new TcpListener(ip, ECHO_PORT);
		
			// starting to listen
			clientListener.Start();
			
			Console.WriteLine("Waiting for connections...");	
			
			while(true)
			{
				// acepting the connection
				TcpClient client = clientListener.AcceptTcpClient();
													
				ClientHandler cHandler = new ClientHandler();
				// passing value to the thread class
				cHandler.clientSocket = client;				
				
				// creating a new thread for the client
				Thread clientThread = new Thread(new ThreadStart(cHandler.runClient));
				clientThread.Start();
			}
		
			clientListener.Stop();
		}
		catch(Exception exp)
		{
			Console.WriteLine("Exception: " + exp);
		}		
	}
}