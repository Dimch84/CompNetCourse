using System.Net.Sockets;
using System.Net;

namespace FtpServerExample
{
    public class User
    {
        public UserSession commandSession { get; set; }
        public UserSession dataSession { get; set; }
        public TcpListener dataListener { get; set; }
        public IPEndPoint remoteEndPoint { get; set; }
        public string userName { get; set; }
        public string workDir { get; set; }
        public string CurrentDir { get; set; }
        public int LoginOK { get; set; }
        public bool isBinary { get; set; }
        public bool isPassive { get; set; }
    }
}
