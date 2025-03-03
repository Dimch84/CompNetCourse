using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace WebServer
{
	public class MyHttpRequest
	{
		const String CRLF = "\r\n";
		Socket socket;

		public MyHttpRequest(Socket socket)
		{
			this.socket = socket;
		}

		public void Run()
		{
			try
			{
				processRequest();
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}
		}

		private void processRequest()
		{
			// Получаем ссылки на входной и выходной потоки сокета.
			NetworkStream stream = new NetworkStream(socket);
			StreamReader reader = new StreamReader(stream);

			bool fileExists = false;

			// Получаем строку запроса из HTTP-сообщения.
			String requestLine = reader.ReadLine();

			// Отладочная информация для внутреннего пользования
			Console.WriteLine("Incoming!");
			Console.WriteLine(requestLine);

			// Извлекаем имя файла из строки запроса.
			var tokens = requestLine.Split(' ');
			// пропускаем слово названия метода (строку "GET")
			String fileName = tokens[1];

			// Добавляем  "." к имени для указания на текущий каталог.
			if (!string.IsNullOrEmpty(fileName))
				fileName = fileName.Substring(1);
	
			// Открываем запрошенный файл.
			if (File.Exists(fileName))
				fileExists = true;

			// Создаем ответное сообщение.
			String statusLine = null;
			String contentTypeLine = null;
			String entityBody = null;
			if (fileExists)
			{
				statusLine = "HTTP/1.0 200 OK" + CRLF;
				contentTypeLine = "Content-Type: " + contentType(fileName) + CRLF;

				using (StreamReader fis = new StreamReader(fileName))
				{
					String headerLine = fis.ReadLine();
					while (!string.IsNullOrEmpty(headerLine))
					{
						Console.WriteLine(headerLine);
						headerLine = fis.ReadLine();
					}
				}
			}
			else
			{
				statusLine = "HTTP/1.0 404 Not Found" + CRLF;
				contentTypeLine = "Content-Type: text/html" + CRLF;
				entityBody = "<HTML>" +
				"<HEAD><TITLE>Not Found</TITLE></HEAD>" +
				"<BODY>Not Found</BODY></HTML>";
			}

			StringBuilder ret = new StringBuilder();
			// Отправляем строку состояния.
			ret.AppendLine(statusLine);

			// Отправляем строку типа содержимого.
			ret.AppendLine(contentTypeLine);

			// Отправляем пустую строку для указания конца заголовков.
			ret.AppendLine(CRLF);

			// Отправляем тело объекта.
			if (fileExists)
			{
				StreamReader fis = new StreamReader(fileName);
				ret.AppendLine(fis.ReadToEnd());
			}
			else
			{
				ret.AppendLine(entityBody);
			}

			byte[] data = Encoding.ASCII.GetBytes(ret.ToString());
			socket.Send(data);

			// Закрываем потоки и сокет.
			stream.Close();
			reader.Close();
			socket.Close();
		}

		private static String contentType(String fileName)
		{
			if (fileName.EndsWith(".htm") || fileName.EndsWith(".html"))
			{
				return "text/html";
			}

			if (fileName.EndsWith(".ram") || fileName.EndsWith(".ra"))
			{
				return "audio/x-pn-realaudio";
			}

			return "application/octet-stream";
		}
	}
}
