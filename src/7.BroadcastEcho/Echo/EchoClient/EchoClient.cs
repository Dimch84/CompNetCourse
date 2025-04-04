using System;
using System.Net;
using System.IO;
using System.Net.Sockets;
using System.Text;

public class EchoClient
{
	const int ECHO_PORT = 8080;
	
	public static void Main(string [] arg)
	{
		Console.Write("Your UserName:");
		
		string userName = Console.ReadLine();
		
		Console.WriteLine("-----Logged In----->");
		
		try
		{
			// creating a connection with the ChatServer	
			TcpClient EchoClient = new TcpClient("127.0.0.1", ECHO_PORT);
			
			// creating the stream classes
			StreamReader readerStream = new StreamReader(EchoClient.GetStream());
			NetworkStream writerStream = EchoClient.GetStream();
			
			string dataToSend;
			
			dataToSend = userName;
			dataToSend += "\r\n";
			
			// sending username to the server			
			byte [] data = Encoding.ASCII.GetBytes(dataToSend);				
			writerStream.Write(data, 0, data.Length);
			
			while(true)
			{
				Console.Write(userName + ":");
				
				dataToSend = Console.ReadLine();
				dataToSend += "\r\n";
				
				data = Encoding.ASCII.GetBytes(dataToSend);
				
				writerStream.Write(data, 0, data.Length);
				//writerStream.Flush();
				
				if (dataToSend.IndexOf("<EOF>") > -1)
				{
					break;
				}
				
				string returnData;
				
				returnData = readerStream.ReadLine();
				
				Console.WriteLine("Server: " + returnData);
			}
			
			EchoClient.Close();
		}
		catch(Exception exp)
		{
			Console.WriteLine("Exception: " + exp);
		}
	}
}