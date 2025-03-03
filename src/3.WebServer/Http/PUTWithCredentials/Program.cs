using System.Net;

string url = "http://gaia.cs.umass.edu/wireshark-labs/protected_pages/HTTP-wireshark-file5.html";
string username = "wireshark-students";
string password = "network";

HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
request.Method = "GET";
request.Credentials = new NetworkCredential(username, password);
request.ContentType = "text/plain";

WebResponse response = request.GetResponse();

using (StreamReader reader = new StreamReader(response.GetResponseStream()))
{
    while (reader.Peek() != -1)
    {
        Console.WriteLine(reader.ReadLine());
    }
}
