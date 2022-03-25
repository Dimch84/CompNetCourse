using System;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;

namespace FtpClientExample
{
    public partial class FormMain : Form
    {
        private const int ftpPort = 21;
        private string ftpUriString;
        private NetworkCredential networkCredential;
        private string currentDir = "/";

        public FormMain()
        {
            InitializeComponent();
 
            //IPAddress[] ips = Dns.GetHostAddresses("");
            textBoxServer.Text = "127.0.0.1"; //ips[1].ToString();
            textBoxUserName.Text = "test";
            textBoxPassword.Text = "test";
        }

        private void buttonLogin_Click(object sender, EventArgs e)
        {
            if (textBoxServer.Text.Length == 0)
            {
                return;
            }

            ftpUriString = "ftp://" + textBoxServer.Text;
            networkCredential = new NetworkCredential(textBoxUserName.Text, textBoxPassword.Text);

            if (ShowFtpFileAndDirectory() == true)
            {
                buttonLogin.Enabled = false;
            }
        }

        private void buttonUpload_Click(object sender, EventArgs e)
        {
            OpenFileDialog f = new OpenFileDialog();
            if (f.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            FileInfo fileInfo = new FileInfo(f.FileName);
            string uri = GetUriString(fileInfo.Name);
            FtpWebRequest request = CreateFtpWebRequest(uri, WebRequestMethods.Ftp.UploadFile);
            request.ContentLength = fileInfo.Length;
            int buffLength = 8196;
            byte[] buff = new byte[buffLength];
            FileStream fs = fileInfo.OpenRead();
            try
            {
                Stream responseStream = request.GetRequestStream();
                int contentLen = fs.Read(buff, 0, buffLength);
                while (contentLen != 0)
                {
                    responseStream.Write(buff, 0, contentLen);
                    contentLen = fs.Read(buff, 0, buffLength);
                }
                responseStream.Close();
                fs.Close();

                FtpWebResponse response = GetFtpResponse(request);
                if (response == null)
                    return;

                ShowFtpFileAndDirectory();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void buttonDownload_Click(object sender, EventArgs e)
        {
            string fileName = GetSelectedFile();
            if (fileName.Length == 0)
            {
                return;
            }
            string filePath = Application.StartupPath + "\\DownLoad";
            if (Directory.Exists(filePath) == false)
            {
                Directory.CreateDirectory(filePath);
            }

            Stream responseStream = null;
            FileStream fileStream = null;
            StreamReader reader = null;
            try
            {
                string uri = GetUriString(fileName);
                FtpWebRequest request = CreateFtpWebRequest(uri, WebRequestMethods.Ftp.DownloadFile);
                FtpWebResponse response = GetFtpResponse(request);
                if (response == null)
                {
                    return;
                }
                responseStream = response.GetResponseStream();
                string path = filePath + "\\" + fileName;
                fileStream = File.Create(path);
                byte[] buffer = new byte[8196];
                int bytesRead;
                while (true)
                {
                    bytesRead = responseStream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0)
                    {
                        break;
                    }
                    fileStream.Write(buffer, 0, bytesRead);
                }
            }
            catch (UriFormatException err)
            {
                MessageBox.Show(err.Message);
            }
            catch (WebException err)
            {
                MessageBox.Show(err.Message);
            }
            catch (IOException err)
            {
                MessageBox.Show(err.Message);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                else if (responseStream != null)
                {
                    responseStream.Close();
                }
                if (fileStream != null)
                {
                    fileStream.Close();
                }
            }
        }       
        
        private void listBoxFtp_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxFtp.SelectedIndex > 0)
            {
                string fileName = GetSelectedFile();
                textBoxSelectedFile.Text = fileName;                
            }
        }
        
        private void listBoxFtp_DoubleClick(object sender, EventArgs e)
        {
            if (listBoxFtp.SelectedIndex == 0)
            {
                if (currentDir == "/")
                {
                    return;
                }
                int index = currentDir.LastIndexOf("/");
                if (index == 0)
                {
                    currentDir = "/";
                }
                else
                {
                    currentDir = currentDir.Substring(0, index);
                }
                ShowFtpFileAndDirectory();
            }
            else if (listBoxFtp.SelectedIndex > 0 && listBoxFtp.SelectedItem.ToString().Contains("[dir]"))
            {
                if (currentDir == "/")
                {
                    currentDir = "/" + listBoxFtp.SelectedItem.ToString().Substring(4);
                }
                else
                {
                    currentDir = currentDir + "/" + listBoxFtp.SelectedItem.ToString().Substring(4);
                }
                ShowFtpFileAndDirectory();
            }
        }

        private bool ShowFtpFileAndDirectory()
        {
            listBoxFtp.Items.Clear();
            string uri;
            if (currentDir == "/")
            {
                uri = ftpUriString;
            }
            else
            {
                uri = ftpUriString + currentDir;
            }

            FtpWebRequest request = CreateFtpWebRequest(uri, WebRequestMethods.Ftp.ListDirectoryDetails);
            FtpWebResponse response = GetFtpResponse(request);
            if (response == null)
                return false;

            listBoxInfo.Items.Add("Response status：" + response.StatusDescription);
            string sData = null;
            using (StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.Default))
            {
                sData = sr.ReadToEnd();
            }
            string[] ftpDir = sData.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
 
            listBoxInfo.Items.AddRange(ftpDir);
            listBoxInfo.Items.Add("Response status: " + response.StatusDescription);

            int len = 0;
            for (int i = 0; i < ftpDir.Length; i++)
            {
                if (ftpDir[i].EndsWith("."))
                {
                    len = ftpDir[i].Length - 2;
                    break;
                }
            }

            for (int i = 0; i < ftpDir.Length; i++)
            {
                sData = ftpDir[i];
                int index = sData.LastIndexOf('\t');
                if (index == -1)
                {
                    if (len < sData.Length)
                        index = len;
                    else
                        continue;
                }
                string name = sData.Substring(index + 1);
                if (name == "." || name == "..")
                    continue;
                if (sData[0] == 'd' || (sData.ToLower()).Contains("<dir>"))
                {
                    listBoxFtp.Items.Add("[dir]" + name);
                }
            }

            for (int i = 0; i < ftpDir.Length; i++)
            {
                var s = ftpDir[i];
                int index = s.LastIndexOf('\t');
                if (index == -1)
                {
                    if (len < s.Length)
                        index = len;
                    else
                        continue;
                }
                string name = s.Substring(index + 1);
                if (name == "." || name == "..")
                    continue;
                if (!(s[0] == 'd' || (s.ToLower()).Contains("<dir>")))
                {
                    listBoxFtp.Items.Add("[file]" + name);
                }
            }

            return true;
        }

        private FtpWebRequest CreateFtpWebRequest(string uri, string requestMethod)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(uri);
            request.Credentials = networkCredential;
            request.KeepAlive = true;
            request.UseBinary = true;
            request.Method = requestMethod;

            return request;
        }

        private FtpWebResponse GetFtpResponse(FtpWebRequest request)
        {
            FtpWebResponse response = null;
            try
            {
                response = (FtpWebResponse)request.GetResponse();
                return response;
            }
            catch (WebException err)
            {
                listBoxInfo.Items.Add("Error：" + err.Status.ToString());
                FtpWebResponse e = (FtpWebResponse)err.Response;
                listBoxInfo.Items.Add("Status Code :" + e.StatusCode);
                listBoxInfo.Items.Add("Status Description :" + e.StatusDescription);
                return null;
            }
            catch (Exception err)
            {
                listBoxInfo.Items.Add(err.Message);
                return null;
            }
        }

        private string GetSelectedFile()
        {
            string fileName = "";
            if (listBoxFtp.SelectedIndex != -1 &&
                listBoxFtp.SelectedItem.ToString().Substring(0, 6) == "[file]")
            {
                fileName = listBoxFtp.SelectedItem.ToString().Substring(6).Trim();
            }  
            
            return fileName;
        }

        private string GetUriString(string fileName)
        {
            string uri = string.Empty;
            if (currentDir.EndsWith("/"))
            {
                uri = ftpUriString + currentDir + fileName;
            }
            else
            {
                uri = ftpUriString + currentDir + "/" + fileName;
            }

            return uri;
        }   
    }
}