using System.Net;

class Program
{
    static void Output(IPHostEntry ipHostEntry)
    {
        Console.WriteLine("Host: {0}", ipHostEntry.HostName);
        Console.WriteLine("\nAliases:");
        foreach (string alias in ipHostEntry.Aliases)
        {
            Console.WriteLine(alias);
        }

        Console.WriteLine("\nAddress(es):");
        foreach (IPAddress address in ipHostEntry.AddressList)
        {
            Console.WriteLine("Address: {0}", address.ToString());
        }
        Console.ReadLine();
    }

    static void Lookup(string hostname)
    {
        IPHostEntry ipHostEntry = Dns.GetHostByName(hostname);

        Output(ipHostEntry);
    }

    static void ReverseLookup(string address)
    {
        IPHostEntry ipHostEntry = Dns.GetHostByAddress(address);
        Output(ipHostEntry);
    }

    static void Main(string[] args)
    {
        //Lookup("www.microsoft.com");
        ReverseLookup("195.70.219.101");
    }
}
