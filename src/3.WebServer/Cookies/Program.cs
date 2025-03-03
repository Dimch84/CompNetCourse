using System.Net;

HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://www.google.com/");
//HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://www.ozon.com/");
request.CookieContainer = new CookieContainer();

HttpWebResponse response = (HttpWebResponse)request.GetResponse();
foreach (Cookie cook in response.Cookies)
{
    Console.WriteLine("\n\n\nCookie:");
    Console.WriteLine("{0} = {1}", cook.Name, cook.Value);
    Console.WriteLine("Domain: {0}", cook.Domain);
    Console.WriteLine("Path: {0}", cook.Path);
    Console.WriteLine("Port: {0}", cook.Port);
    Console.WriteLine("Secure: {0}", cook.Secure);

    Console.WriteLine("When issued: {0}", cook.TimeStamp);
    Console.WriteLine("Expires: {0} (expired? {1})", cook.Expires, cook.Expired);
    Console.WriteLine("Don't save: {0}", cook.Discard);
    Console.WriteLine("Comment: {0}", cook.Comment);
    Console.WriteLine("Uri for comments: {0}", cook.CommentUri);
    Console.WriteLine("Version: RFC {0}", cook.Version == 1 ? "2109" : "2965");

    Console.WriteLine("String: {0}", cook.ToString());
}
