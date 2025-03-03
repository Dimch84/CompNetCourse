using System;
using System.IO;
using System.Net;

namespace ClientApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var myProxy = CreateProxy("localhost", "9992");

            processProxyRequest("http://www.youtube.com", myProxy);
            Console.ReadLine();

            processProxyRequest("http://www.ya.ru", myProxy);
        }
        
        private static WebProxy CreateProxy(string host, string port)
        {
            WebProxy myProxy = new WebProxy();
            string proxyAddress = $"http://{host}:{port}";
            Uri newUri = new Uri(proxyAddress);
            myProxy.Address = newUri;
            //myProxy.BypassProxyOnLocal = false;
            //myProxy.Credentials = new NetworkCredential(username, password);

            return myProxy;
        }

        private static void processProxyRequest(string request, WebProxy myProxy)
        {
            try
            {
                // Create a new request to the mentioned URL.				
                WebRequest myWebRequest = WebRequest.Create(request);

                // set proxy to request
                myWebRequest.Proxy = myProxy;

                Console.WriteLine("\nThe Address of the  new Proxy settings are {0}", myProxy.Address);
                WebResponse myWebResponse = myWebRequest.GetResponse();

                // Print the  HTML contents of the page to the console.
                Stream streamResponse = myWebResponse.GetResponseStream();
                StreamReader streamRead = new StreamReader(streamResponse);
                Char[] readBuff = new Char[256];
                int count = streamRead.Read(readBuff, 0, 256);
                Console.WriteLine("\nThe contents of the Html pages are :");
                while (count > 0)
                {
                    String outputData = new String(readBuff, 0, count);
                    Console.Write(outputData);
                    count = streamRead.Read(readBuff, 0, 256);
                }

                Console.WriteLine();

                // Close the Stream object.
                streamResponse.Close();
                streamRead.Close();

                // Release the HttpWebResponse Resource.
                myWebResponse.Close();
            }
            catch (UriFormatException e)
            {
                Console.WriteLine("\nUriFormatException is thrown.Message is {0}", e.Message);
                Console.WriteLine("\nThe format of the myProxy address you entered is invalid");
            }
        }
    }
}
