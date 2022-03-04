using Helper;
using System;
using System.IO;
using System.Net;

namespace WebClientSample
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //
            Printer.Header("Basic WebClient");
            BasicWebClient();
            Console.ReadLine();

            //
            Printer.Header("Http WebRequest Sample");
            HttpWebRequestSample();

            // 
            Printer.Header("Response data");
            ResponseData();
        }

        public static void BasicWebClient()
        {
            WebClient Client = new WebClient();
            Stream strm = Client.OpenRead("http://yandex.ru");
            StreamReader sr = new StreamReader(strm);
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                Console.WriteLine(line);
            }
            strm.Close();
        }

        public static void HttpWebRequestSample()
        {
            String page = "http://yandex.ru";
            HttpWebRequest requestSite = (HttpWebRequest)WebRequest.Create(page);

            HttpWebResponse response = (HttpWebResponse)requestSite.GetResponse();

            // read data from response
            Stream dataStream = response.GetResponseStream();
            StreamReader read = new StreamReader(dataStream);
            String data = read.ReadToEnd();
            Console.WriteLine(data);
        }

        static void ResponseData()
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create("http://www.microsoft.com");

            using (HttpWebResponse resp = (HttpWebResponse)req.GetResponse())
            {
                Console.WriteLine($"Last modified: {resp.LastModified}\n");

                // Display the header name/value pairs. 
                string[] names = resp.Headers.AllKeys;
                Console.WriteLine("{0,-20}{1}", "Name", "Value");
                foreach (string n in names)
                    Console.WriteLine("{0,-20}{1}", n, resp.Headers[n]);
            }
        }
    }
}
