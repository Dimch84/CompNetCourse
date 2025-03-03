using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;

namespace DNSSamples
{
    internal class Program
    {
        private static string strIpAddress = string.Empty;

        static void Main()
        {
            Console.WriteLine("*** DNS Name Resolution ***");
            IPHostEntry MyHost = Dns.Resolve("yandex.ru");
            foreach (IPAddress MyIP in MyHost.AddressList)
            {
                Console.WriteLine(MyIP.MapToIPv4());
            }
            strIpAddress = MyHost.AddressList[0].MapToIPv4().ToString();

            // 
            Console.WriteLine("*** Get Host By Name ***");
            string hostName = Dns.GetHostName();
            Console.WriteLine("Local hostname: {0}", hostName);
            IPHostEntry myself = Dns.GetHostByName(hostName);
            foreach (IPAddress address in myself.AddressList)
            {
                Console.WriteLine("IP Address: {0}", address.ToString());
            }

            // 
            Console.WriteLine("*** Get DNS Address Info ***");
            IPAddress test = IPAddress.Parse(strIpAddress);
            IPHostEntry iphe = Dns.GetHostByAddress(test);

            Console.WriteLine("Information for {0}", test.ToString());
            Console.WriteLine("Host name: {0}", iphe.HostName);
            foreach (string alias in iphe.Aliases)
            {
                Console.WriteLine("Alias: {0}", alias);
            }
            foreach (IPAddress address in iphe.AddressList)
            {
                Console.WriteLine("Address: {0}", address.ToString());
            }

            //
            Console.WriteLine("*** Get Dns Server ***");
            var dnsServers = GetDnsServers();
            Console.WriteLine(string.Join(", ", dnsServers));

            //Printer.Header("Find DNS Servers with Registry");
            // FindDNSServers();
        }

        public static List<string> GetDnsServers()
        {
            List<string> Servers = new List<string>();

            foreach (NetworkInterface Nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (Nic.OperationalStatus == OperationalStatus.Up && Nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                {
                    IPInterfaceProperties NicProperties = Nic.GetIPProperties();
                    foreach (IPAddress DnsAddress in NicProperties.DnsAddresses)
                    {
                        Servers.Add(DnsAddress.ToString());
                    }
                }
            }
            return Servers;
        }

        public static void FindDNSServers()
        {
            Microsoft.Win32.RegistryKey start = Microsoft.Win32.Registry.LocalMachine;
            string DNSservers = @"SYSTEM\CurrentControlSet\Services\Tcpip\Parameters";

            Microsoft.Win32.RegistryKey DNSserverKey = start.OpenSubKey(DNSservers);
            if (DNSserverKey == null)
            {
                Console.WriteLine("Unable to open DNS servers key");
                return;
            }

            string serverlist = (string)DNSserverKey.GetValue("NameServer");

            Console.WriteLine("DNS Servers: {0}", serverlist);
            DNSserverKey.Close();
            start.Close();

            char[] token = new char[1];
            token[0] = ' ';
            string[] servers = serverlist.Split(token);

            foreach (string server in servers)
            {
                Console.WriteLine("DNS server: {0}", server);
            }
        }
    }
}
