using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FtpSampleApp {
    class Program {
        static async Task Main(string[] args) {
            Console.WriteLine(await GetDirectoryListing());
            Console.WriteLine(await PushFile());
            Console.WriteLine(await RequestFile());
        }

        public static async Task<string> GetDirectoryListing() 
        {
            Console.WriteLine("GetDirectoryListing...");

            StringBuilder strBuilder = new StringBuilder();
            FtpWebRequest req = (FtpWebRequest)WebRequest.Create("ftp://127.0.0.1:21");
            req.Method = WebRequestMethods.Ftp.ListDirectoryDetails;

            req.Credentials = new NetworkCredential("TestUser", "1234567");
            req.EnableSsl = false;

            FtpWebResponse resp = (FtpWebResponse)await req.GetResponseAsync();

            using (var respStream = resp.GetResponseStream()) {
                using (var reader = new StreamReader(respStream)) {
                    strBuilder.Append(reader.ReadToEnd());
                    strBuilder.Append(resp.WelcomeMessage);
                    strBuilder.Append($"Request returned status:  {resp.StatusDescription}");
                }
            }

            return strBuilder.ToString();
        }

        public static async Task<string> PushFile()
        {
            Console.WriteLine("PushFile...");

            StringBuilder strBuilder = new StringBuilder();

            try
            {
                FtpWebRequest req = (FtpWebRequest)WebRequest.Create("ftp://127.0.0.1:21/aa.csv");
                req.Method = WebRequestMethods.Ftp.UploadFile;

                req.Credentials = new NetworkCredential("TestUser", "1234567");
                req.UsePassive = true;

                byte[] fileBytes;
                using (var reader = new StreamReader(@"C:\Temp\aa.csv"))
                {
                    fileBytes = Encoding.ASCII.GetBytes(reader.ReadToEnd());
                }

                req.ContentLength = fileBytes.Length;
                using (var reqStream = await req.GetRequestStreamAsync())
                {
                    await reqStream.WriteAsync(fileBytes, 0, fileBytes.Length);
                }

                using (FtpWebResponse resp = (FtpWebResponse)req.GetResponse())
                {
                    strBuilder.Append(resp.StatusDescription);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return strBuilder.ToString();
        }

        public static async Task<string> RequestFile() {
            Console.WriteLine("RequestFile...");

            StringBuilder strBuilder = new StringBuilder();
            FtpWebRequest req = (FtpWebRequest)WebRequest.Create("ftp://127.0.0.1:21/aa.csv");
            req.Method = WebRequestMethods.Ftp.DownloadFile;

            req.Credentials = new NetworkCredential("TestUser", "1234567");
            req.UsePassive = true;

            using (FtpWebResponse resp = (FtpWebResponse)req.GetResponse())
            {
                strBuilder.Append(resp.StatusDescription);
                using (var respStream = resp.GetResponseStream())
                {
                    string fileName = Path.GetFileName(req.RequestUri.AbsolutePath);
                    using (var fileStream = File.Create(fileName))
                    {
                        byte[] buffer = new byte[1024];
                        int bytesRead;
                        while (true)
                        {
                            bytesRead = respStream.Read(buffer, 0, buffer.Length);
                            if (bytesRead == 0)
                                break;
                            fileStream.Write(buffer, 0, bytesRead);
                        }
                    }
                }
            }

            return strBuilder.ToString();
        }
    }
}
