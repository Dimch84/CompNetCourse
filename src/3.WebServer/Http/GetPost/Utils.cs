using System.Text;
using System.Net;

namespace GetPost
{
    static class Utils
    {
        /// <summary>
        /// Submit a GET or POST request and returns the response as string
        /// </summary>
        /// <param name="uri">Standard URL</param>
        /// <param name="args">Arguments without the "?". Can be null</param>
        /// <param name="post">POST or not</param>
        /// <returns></returns>
        public static string SubmitRequestAndGetResponse(string uri, string args, bool post)
        {
            string userAgent = "Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US; rv:1.8.1.11) Gecko/20071127 Firefox/2.0.0.11"; // WINDOWS
            string referer = "http://www.google.com/";

            if (!post)
            {
                WebClient wc = new WebClient();
                wc.Headers[HttpRequestHeader.UserAgent] = userAgent;
                wc.Headers[HttpRequestHeader.Cookie] = "pass=deleted";

                try
                {
                    if (args == null)
                        return wc.DownloadString(uri);
                    else
                        return wc.DownloadString(uri + "?" + args);
                }
                catch (WebException)
                {
                    return null;
                }
            }
            else
            {
                try
                {
                    // Request
                    HttpWebRequest req = (HttpWebRequest)WebRequest.Create(uri);
                    req.Method = WebRequestMethods.Http.Post;
                    req.ContentType = "application/x-www-form-urlencoded";
                    req.Referer = referer;
                    req.UserAgent = userAgent;

                    byte[] bytes = Encoding.Default.GetBytes(args);
                    req.ContentLength = bytes.Length;

                    using (Stream reqStream = req.GetRequestStream())
                    {
                        reqStream.Write(bytes, 0, bytes.Length);
                    }

                    // Response
                    WebResponse resp = req.GetResponse();
                    using (Stream newStream = resp.GetResponseStream())
                    {
                        using (StreamReader sr = new StreamReader(newStream))
                        {
                            string result = sr.ReadToEnd();
                            return result;
                        }
                    }
                }
                catch (WebException)
                {
                    return null;
                }
            }
        }
    }
}
