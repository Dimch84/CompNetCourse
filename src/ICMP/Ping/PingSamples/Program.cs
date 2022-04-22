using System.Net.NetworkInformation;
using PingSamples;

static string PingComp(string comp)
{
    using (Ping ping = new Ping())
    {
        try
        {
            Console.Write("Pinging {0}... ", comp);
            PingReply reply = ping.Send(comp, 100);
            if (reply.Status == IPStatus.Success)
            {
                Console.WriteLine("Success - IP Address:{0}, RoundtripTime: {1}", reply.Address, reply.RoundtripTime);
                return reply.Address.ToString();
            }
            else
            {
                Console.WriteLine(reply.Status);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error ({0})",
                ex.InnerException.Message);
        }

        return String.Empty;
    }
}

// main
var host = "yandex.ru";

Console.WriteLine("Casual ping");
var ipAddress = PingComp(host);

Console.WriteLine("\nCustom ping");
SimplePing.Ping(ipAddress);
