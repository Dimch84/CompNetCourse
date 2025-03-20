using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Globalization;

namespace FtpServerExample
{
    public partial class MainForm : Form
    {      
        TcpListener myTcpListener;
        Dictionary<string, string> users;

        public MainForm()
        {
            InitializeComponent();

            users = new Dictionary<string, string>();
            users.Add("test", "test");
            textBox1.Text = @"C:\FTP\";
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            this.listBoxStatus.Items.Add("FTP Started");

            Thread t = new Thread(ListenClientConnect);
            t.IsBackground = true;
            t.Start();

            buttonStart.Enabled = false;
            buttonStop.Enabled = true;
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ListenClientConnect()
        {
            myTcpListener = new TcpListener(IPAddress.Any, 21);
            myTcpListener.Start();

            while (true)
            {
                try
                {
                    TcpClient client = myTcpListener.AcceptTcpClient();
                    AddInfo(string.Format("{0} ({1})", client.Client.RemoteEndPoint, myTcpListener.LocalEndpoint));

                    User user = new User();
                    user.commandSession = new UserSession(client);
                    user.workDir = textBox1.Text;

                    Thread t = new Thread(UserProcessing);
                    t.IsBackground = true;
                    t.Start(user);
                }
                catch
                {
                    break;
                }
            }
        }

        #region Commands

        private void CommandUser(User user, string command, string param)
        {
            string sendString = string.Empty;
            if (command == "USER")
            {
                sendString = "331 USER command OK, password required.";
                user.userName = param;
                user.LoginOK = 1;
            }
            else
            {
                sendString = "501 USER command syntax error.";
            }
            ReplyCommandToUser(user, sendString);
        }  

        private void CommandPassword(User user, string command, string param)
        {
            string sendString = string.Empty;
            if (command == "PASS")
            {
                string password = null;
                if (users.TryGetValue(user.userName, out password))
                {
                    if (password == param)
                    {
                        sendString = "230 User logged in success";
                        user.LoginOK = 2;
                    }
                    else
                        sendString = "530 Password incorrect.";
                }
                else
                {
                    sendString = "530 User name or password incorrect.";
                }
            }
            else
            {
                sendString = "501 PASS command Syntax error.";
            }
            ReplyCommandToUser(user, sendString);

            user.CurrentDir = user.workDir;
        }

        private void CommandCWD(User user, string temp)
        {     
            string sendString = string.Empty;
            try
            {
                string dir = user.workDir.TrimEnd('/') + temp;

                if (Directory.Exists(dir))
                {
                    user.CurrentDir = dir;
                    sendString = "250 Directory changed to '" + dir + "' successfully";
                }
                else
                {
                    sendString = "550 Directory '" + dir + "' does not exist";
                }
            }
            catch
            {
                sendString = "502 Directory changed unsuccessfully";
            }
            ReplyCommandToUser(user, sendString);
        }

        private void CommandPWD(User user)
        {
            string sendString = string.Empty;
            sendString = "257 '" + user.CurrentDir + "' is the current directory";
            ReplyCommandToUser(user, sendString);
        }

        private void CommandPASV(User user)
        {
            string sendString = string.Empty;
            var localIP = "127.0.0.1"; //Dns.GetHostEntry("").AddressList[1];
            var dns = Dns.GetHostEntry("");

            Random random = new Random();
            int randNum1, randNum2, port;
            while (true)
            {
                randNum1 = random.Next(5, 200);
                randNum2 = random.Next(0, 200);
                port = (randNum1 << 8) | randNum2;
                try
                {
                    user.dataListener = new TcpListener(IPAddress.Parse(localIP), port);
                    AddInfo(localIP.ToString() + ":" + port);
                }
                catch
                {
                    continue;
                }
                user.isPassive = true;
                string tmp = localIP.ToString().Replace('.', ',');
                sendString = "227 Entering Passive Mode (" + tmp + "," + randNum1 + "," + randNum2 + ")";
                ReplyCommandToUser(user, sendString);
                user.dataListener.Start();
                break;
            }
        }

        private void CommandPORT(User user, string portString)
        {
            string sendString = string.Empty;
            String[] tmp = portString.Split(',');
            String ipString = "" + tmp[0] + "." + tmp[1] + "." + tmp[2] + "." + tmp[3];
            int portNum = (int.Parse(tmp[4]) << 8) | int.Parse(tmp[5]);
            user.remoteEndPoint = new IPEndPoint(IPAddress.Parse(ipString), portNum);
            sendString = "200 PORT command successful.";
            ReplyCommandToUser(user, sendString);
        }

        private void CommandLIST(User user, string parameter)
        {
            string sendString = string.Empty;
            DateTimeFormatInfo m = new CultureInfo("en-US", true).DateTimeFormat;
            string[] dir = Directory.GetDirectories(user.CurrentDir);
            if (string.IsNullOrEmpty(parameter) == false)
            {
                if (Directory.Exists(user.CurrentDir + parameter))
                {
                    dir = Directory.GetDirectories(user.CurrentDir + parameter);
                }
                else
                {
                    string s = user.CurrentDir.TrimEnd('/');
                    user.CurrentDir = s.Substring(0, s.LastIndexOf("/") + 1);
                }
            }
            for (int i = 0; i < dir.Length; i++)
            {
                string folderName = Path.GetFileName(dir[i]);
                DirectoryInfo d = new DirectoryInfo(dir[i]);
                sendString += @"dwr-\t" + Dns.GetHostName() + "\t" +
                    m.GetAbbreviatedMonthName(d.CreationTime.Month) +
                    d.CreationTime.ToString(" dd yyyy") + "\t" +
                    folderName + Environment.NewLine;
            }

            string[] files = Directory.GetFiles(user.CurrentDir);
            if (string.IsNullOrEmpty(parameter) == false)
            {
                if (Directory.Exists(user.CurrentDir + parameter + "/"))
                {
                    files = Directory.GetFiles(user.CurrentDir + parameter + "/");
                }
            }
            for (int i = 0; i < files.Length; i++)
            {
                FileInfo f = new FileInfo(files[i]);
                string fileName = Path.GetFileName(files[i]);

                sendString += "-wr-\t" + Dns.GetHostName() + "\t" + f.Length +
                    " " + m.GetAbbreviatedMonthName(f.CreationTime.Month) +
                    f.CreationTime.ToString(" dd yyyy") + "\t" +
                    fileName + Environment.NewLine;
            }
            bool isBinary = user.isBinary;
            user.isBinary = false;
            ReplyCommandToUser(user, "150 Opening ASCII mode data connection");
            InitDataSession(user);
            SendByUserSession(user, sendString);
            ReplyCommandToUser(user, "226 Transfer complete.");
            user.isBinary = isBinary;
        }

        private void CommandRETR(User user, string fileName)
        {
            string sendString = "";
            string path = user.CurrentDir + fileName;
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            if (user.isBinary)
            {
                sendString = "150 Opening BINARY mode data connection for  download";
            }
            else
            {
                sendString = "150 Opening ASCII mode data connection for download";
            }
            ReplyCommandToUser(user, sendString);
            InitDataSession(user);
            SendFileByUserSession(user, fs);
            ReplyCommandToUser(user, "226 Transfer complete.");
        }

        private void CommandSTOR(User user, string fileName)
        {
            string sendString = "";
            string path = user.CurrentDir + fileName;
            FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write);
            if (user.isBinary)
            {
                sendString = "150 Opening BINARY mode data connection for upload";
            }
            else
            {
                sendString = "150 Opening ASCII mode data connection for upload";
            }

            ReplyCommandToUser(user, sendString);
            InitDataSession(user);
            ReadFileByUserSession(user, fs);
            ReplyCommandToUser(user, "226 Transfer complete.");
        } 

        private void CommandTYPE(User user, string param)
        {
            string sendString = "";
            if (param == "I")
            {
                user.isBinary = true;
                sendString = "200 Type set to I(Binary)";
            }
            else
            {
                user.isBinary = false;
                sendString = "200 Type set to A(ASCII)";
            }
            ReplyCommandToUser(user, sendString);
        }

        #endregion

        private void UserProcessing(object obj)
        {
            User user = (User)obj;
            string sendString = "220 FTP Server v1.0";
            string oldFileName = "";
            ReplyCommandToUser(user, sendString);

            while (true)
            {
                string receiveString = null;
                try
                {
                    receiveString = user.commandSession.sr.ReadLine();
                }
                catch (Exception ex)
                {
                    if (user.commandSession.client.Connected == false)
                    {
                        AddInfo("Not connected");
                    }
                    else
                    {
                        AddInfo("Exception:" + ex.Message);
                    }
                    break;
                }
                if (receiveString == null)
                {
                    AddInfo("Received string is null");
                    break;
                }
                AddInfo(string.Format("[{0}]--{1}", user.commandSession.client.Client.RemoteEndPoint, receiveString));

                string command = receiveString;
                string param = string.Empty;
                int index = receiveString.IndexOf(' ');
                if (index != -1)
                {
                    command = receiveString.Substring(0, index).ToUpper();
                    param = receiveString.Substring(command.Length).Trim();
                }

                if (command == "QUIT")
                {
                    user.commandSession.Close();
                    return;
                }
                else
                {
                    switch (user.LoginOK)
                    {
                        case 0:
                            CommandUser(user, command, param);
                            break;
                        case 1:
                            CommandPassword(user, command, param);
                            break;

                        case 2:
                            {
                                switch (command)
                                {
                                    case "CWD":
                                        CommandCWD(user, param);
                                        break;
                                    case "PWD":
                                        CommandPWD(user);
                                        break;
                                    case "PASV":
                                        CommandPASV(user);
                                        break;
                                    case "PORT":
                                        CommandPORT(user, param);
                                        break;
                                    case "LIST":
                                    case "NLST":
                                        CommandLIST(user, param);
                                        break;
                                    case "RETR":
                                        CommandRETR(user, param);
                                        break;
                                    case "STOR":
                                        CommandSTOR(user, param);
                                        break;                                  
                                    case "TYPE":
                                        CommandTYPE(user, param);
                                        break;
                                    default:
                                        sendString = "502 command is not implemented.";
                                        ReplyCommandToUser(user, sendString);
                                        break;
                                }
                            }
                            break;
                    }
                }
            }
        }

        private void InitDataSession(User user)
        {
            TcpClient client = null;
            if (user.isPassive)
            {
                client = user.dataListener.AcceptTcpClient();
            }
            else
            {
                client = new TcpClient();
                client.Connect(user.remoteEndPoint);
            }
            user.dataSession = new UserSession(client);
        }

        private void SendByUserSession(User user, string sendString)
        {
            try
            {
                user.dataSession.sw.WriteLine(sendString);
            }
            finally
            {
                user.dataSession.Close();
            }
        }

        private void ReadFileByUserSession(User user, FileStream fs)
        {
            try
            {
                if (user.isBinary)
                {
                    byte[] bytes = new byte[1024];
                    BinaryWriter bw = new BinaryWriter(fs);
                    int count = user.dataSession.br.Read(bytes, 0, bytes.Length);
                    while (count > 0)
                    {
                        bw.Write(bytes, 0, count);
                        bw.Flush();
                        count = user.dataSession.br.Read(bytes, 0, bytes.Length);
                    }
                }
                else
                {
                    StreamWriter sw = new StreamWriter(fs);
                    while (user.dataSession.sr.Peek() > -1)
                    {
                        sw.WriteLine(user.dataSession.sr.ReadLine());
                        sw.Flush();
                    }
                }
            }
            finally
            {
                user.dataSession.Close();
                fs.Close();
            }
        }

        private void SendFileByUserSession(User user, FileStream fs)
        {
            try
            {
                if (user.isBinary)
                {
                    byte[] bytes = new byte[1024];
                    BinaryReader br = new BinaryReader(fs);
                    int count = br.Read(bytes, 0, bytes.Length);
                    while (count > 0)
                    {
                        user.dataSession.bw.Write(bytes, 0, count);
                        user.dataSession.bw.Flush();
                        count = br.Read(bytes, 0, bytes.Length);
                    }
                }
                else
                {
                    StreamReader sr = new StreamReader(fs);
                    while (sr.Peek() > -1)
                    {
                        user.dataSession.sw.WriteLine(sr.ReadLine());
                    }
                }
            }
            finally
            {
                user.dataSession.Close();
                fs.Close();
            }
        }

        private void ReplyCommandToUser(User user, string str)
        {
            try
            {
                user.commandSession.sw.WriteLine(str);
                AddInfo(string.Format("{0}：{1}", user.commandSession.client.Client.RemoteEndPoint, str));
            }
            catch
            {
                AddInfo(string.Format("{0}", user.commandSession.client.Client.RemoteEndPoint));
            }
        }

        private delegate void AddInfoDelegate(string str);
        private void AddInfo(string str)
        {
            if (listBoxStatus.InvokeRequired == true)
            {
                AddInfoDelegate d = new AddInfoDelegate(AddInfo);
                this.Invoke(d, str);
            }
            else
            {
                listBoxStatus.Items.Add(str);
                listBoxStatus.SelectedIndex = listBoxStatus.Items.Count - 1;
                listBoxStatus.ClearSelected();
            }
        }
    }
}