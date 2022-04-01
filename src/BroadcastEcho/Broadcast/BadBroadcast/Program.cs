using System.Net;
using System.Net.Sockets;
using System.Text;

Socket sock = null;
try
{
    sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
    IPEndPoint iep = new IPEndPoint(IPAddress.Broadcast, 9050);

    byte[] data = Encoding.ASCII.GetBytes("This is a test message");
    // no socket options nor bind are set
    sock.SendTo(data, iep);
}
catch (Exception e)
{
    Console.WriteLine(e.Message);
}
finally
{
    if (sock != null)
        sock.Close();
}
