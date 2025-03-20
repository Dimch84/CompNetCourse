using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Configuration;

public class MyEmailSender
{
    private readonly Socket socket;
    private readonly NetworkStream stream;
    private readonly StreamReader reader;

    public MyEmailSender(string hostName, int port)
    {
        // Устанавливаем TCP-соединение с почтовым сервером.
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        var hostEntry = Dns.GetHostByName(hostName);
        IPEndPoint ie = new IPEndPoint(hostEntry.AddressList[0], port);
        socket.Connect(ie);

        stream = new NetworkStream(socket);
        reader = new StreamReader(stream);
    }

    public string GetResponse()
    {
         return reader.ReadLine();
    }

    public void Close()
    {
        reader.Close();
        stream.Close();
        socket.Close();
    }

    private static string EncodeTo64(string toEncode)
    {
        byte[] toEncodeAsBytes
              = System.Text.ASCIIEncoding.ASCII.GetBytes(toEncode);

        string returnValue
              = System.Convert.ToBase64String(toEncodeAsBytes);

        return returnValue;
    }

    public void SendMessageAndCheckReply(string message, int code = -1, bool noResponse = false)
    {
        Console.WriteLine($"\nCommand: {message}");
        // request to SMTP server
        byte[] data = Encoding.ASCII.GetBytes(message);
        socket.Send(data);

        if (noResponse)
            return;

        string response = reader.ReadLine();
        Console.WriteLine($"Response: {response}");

        if (code != -1 && response != null && !response.ToString().StartsWith(code.ToString()))
        {
            throw new Exception($"код ответа {code} не получен от сервера. Response: {response}");
        }
    }

    /* Example of typical email:
    HELO dear server
    AUTH Login
    $myname
    $mypass
    MAIL From:sa @mail.local
    RCPT To:test @mail.local
    DATA
    Hello!
    How are you?
    .
    QUIT
    */
    public static void Main()
    {
        var appSettings = ConfigurationManager.AppSettings;
        string smtpUsername = appSettings["SmtpUser"]; // "st007122@spbu.ru"
        var smtpPassword = appSettings["SmtpPwd"];

        MyEmailSender sender = new MyEmailSender("mail.spbu.ru", 25);

        // Читаем первоначальный ответ сервера.
        string response = sender.GetResponse();
        Console.WriteLine(response);
        if (!response.StartsWith("220")) 
            throw new Exception("код ответа 220 не получен от сервера.");

        // Отправляем команду HELO и получаем ответ сервера.
        sender.SendMessageAndCheckReply($"HELO {smtpUsername}\r\n", 250);

        // Аутентификация пользователя
        sender.SendMessageAndCheckReply("AUTH LOGIN\r\n", 334);
        sender.SendMessageAndCheckReply($"{EncodeTo64(smtpUsername)}\r\n", 334);
        sender.SendMessageAndCheckReply($"{EncodeTo64(smtpPassword)}\r\n", 235);

        // Отправляем команду  MAIL FROM.
        sender.SendMessageAndCheckReply($"MAIL FROM: {smtpUsername}\r\n", 250);

        // Отправляем команду RCPT TO.
        sender.SendMessageAndCheckReply("RCPT TO: dmitry.shalymov@gmail.com\r\n", 250);

        // Отправляем команду DATA.
        sender.SendMessageAndCheckReply("DATA\r\n", 354);

        // Отправляем данные сообщения.
        StringBuilder sb = new StringBuilder();
        sb.Append($"From: {smtpUsername}\r\n");
        sb.Append($"To: {smtpUsername}\r\n");
        sb.Append($"Subject: Test message with Hello\r\n");

        sb.Append("Hi Dear,\r\n");
        sb.Append("This lab is too hard.\r\n");
        sender.SendMessageAndCheckReply(sb.ToString(), -1, true);

        // Завершаем строкой с одной точкой.
        sender.SendMessageAndCheckReply(".\r\n", 250);

        // Отправляем команду QUIT.
        sender.SendMessageAndCheckReply("QUIT\r\n");
        Console.WriteLine(sender.GetResponse());

        sender.Close();
   }
}
