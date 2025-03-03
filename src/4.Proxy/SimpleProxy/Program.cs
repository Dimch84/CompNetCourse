namespace SimpleProxy{
    using System;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Threading;
    using System.Configuration;

    class Program
    {
        static string port;
        static string host;

        static void Main(){
            host = "*";
            port = "9992";

            //http server listener
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("http://" + host + ":" + port + "/");
            listener.Start();
            Console.WriteLine("Listening on " + host + ":" + port);

            //main server loop
            while (true){
			
                //as soon as there is a connection request
                HttpListenerContext ctx = listener.GetContext();
                new Thread(new Worker(ctx, port).ProcessRequest).Start();
            }
        }
    }

    class Worker
    {
        HttpListenerContext context;
        WebProxy parent;
        string port;

        public Worker(HttpListenerContext context, string port){
            this.context = context;

            //host = "*";
            this.port = port;
        }

        private byte[] GetBytesFromStream(Stream stream)
        {
            byte[] result;
            byte[] buffer = new byte[256];

            BinaryReader reader = new BinaryReader(stream);
            MemoryStream memoryStream = new MemoryStream();

            int count = 0;
            while (true)
            {
                count = reader.Read(buffer, 0, buffer.Length);
                memoryStream.Write(buffer, 0, count);

                if (count == 0)
                    break;
            }

            result = memoryStream.ToArray();
            memoryStream.Close();
            reader.Close();
            stream.Close();

            return result;
        }

        public void ProcessRequest()
        {
            //request console log
            string url = context.Request.Url.ToString().Replace(":" + port, "");
            string msg = DateTime.Now.ToString("hh:mm:ss") + " " + context.Request.HttpMethod + " " + context.Request.Url.Host.ToString();
            Console.WriteLine(msg);

            var req = context.Request.Url.Host.ToString();
            var blacklist = ConfigurationManager.AppSettings;
            foreach (var key in blacklist.AllKeys)
            {
                if (parser(req).Equals(key))
                {
                    //response to client without call to server
                    string htmlBody = "<html><body><h1>Error. URL is in black list</h1></body></html>";
                    byte[] errorBytes = Encoding.ASCII.GetBytes(htmlBody);

                    context.Response.ContentType = "text/html";
                    context.Response.ContentLength64 = errorBytes.Length;
                    context.Response.OutputStream.Write(errorBytes, 0, errorBytes.Length);
                    context.Response.OutputStream.Close();

                    return;
                }
            }

            byte[] result;
            try
            {
                WebRequest request = WebRequest.Create(url);

                request.Method = context.Request.HttpMethod;
                request.ContentType = context.Request.ContentType;
                request.ContentLength = context.Request.ContentLength64;
                if (context.Request.ContentLength64 > 0 && context.Request.HasEntityBody)
                {
                    using (System.IO.Stream body = context.Request.InputStream)
                    {
                        byte[] requestdata = GetBytesFromStream(body);
                        request.ContentLength = requestdata.Length;
                        Stream s = request.GetRequestStream();
                        s.Write(requestdata, 0, requestdata.Length);
                        s.Close();
                    }
                }               

                //request processing
                WebResponse response = request.GetResponse();
                result = GetBytesFromStream(response.GetResponseStream());
                context.Response.ContentType = response.ContentType;

                response.Close();
            }
            catch (WebException wex)
            {
                //exception handler (404,407...)
                result = Encoding.UTF8.GetBytes(wex.Message);
                HttpWebResponse resp = (HttpWebResponse)wex.Response;
                context.Response.StatusCode = (int)resp.StatusCode;
                context.Response.StatusDescription = resp.StatusDescription;
                Console.WriteLine("ERROR:" + wex.Message);
            }
            catch (Exception ex)
            {
                result = Encoding.UTF8.GetBytes(ex.Message);
                Console.WriteLine("ERROR:" + ex.Message);
            }

            //response to client
            byte[] b = result;
            context.Response.ContentLength64 = b.Length;
            context.Response.OutputStream.Write(b, 0, b.Length);
            context.Response.OutputStream.Close();
        }

        private static string parser(string name)
        {
            if (name.Contains("www."))
            {
                return name.Replace("www.", string.Empty);
            }
            if (name.Contains("https://www."))
            {
                return name.Replace("https://www.", string.Empty);
            }
            if (name.Contains("https://"))
            {
                return name.Replace("https://", string.Empty);
            }
            return name;
        }
    }
}

