using System.Net;

namespace FirstSample
{
    class DnsTest {
        public static void Run(string hostname) {
            IPHostEntry entry = Dns.Resolve(hostname);

            Console.WriteLine("IP Addresses for {0}: ", hostname);
            foreach (IPAddress address in entry.AddressList)
                Console.WriteLine(address.ToString());

            Console.WriteLine("\nAlias names:");
            foreach (string aliasName in entry.Aliases)
                Console.WriteLine(aliasName);

            Console.WriteLine("\nAnd the real hostname:");
            Console.WriteLine(entry.HostName);

            Console.ReadLine();
        }
    }
}
