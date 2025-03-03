using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace WebServer
{
	class WebServer
	{
		// In Browser run: http://127.0.0.1:8080/test.html
		public static void Main(String[] argv)
		{
			// Получаем номер порта из командной строки.
			int port = int.Parse(argv[0]);

			IPAddress ia = IPAddress.Parse("127.0.0.1");
			IPEndPoint ie = new IPEndPoint(ia, port);

			Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			socket.Bind(ie);
			socket.Listen();

			// Обрабатываем HTTP-запросы в бесконечном цикле.
			while (true) 
			{
				// Слушаем входящие запросы по TCP.
				Socket connection = socket.Accept();

				// Создаем объект для обрабатываемого HTTP-запроса.
				MyHttpRequest request = new MyHttpRequest(connection);

				// Создаем новый поток для обработки запроса.
				Thread thread = new Thread(() => request.Run());

				// Запускаем поток.
				thread.Start();
			}
		}
	}
}
