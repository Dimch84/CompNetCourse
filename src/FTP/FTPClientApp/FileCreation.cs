using System;
using System.Windows.Forms;

namespace NetworksLab2FTPClient
{
    public partial class FileCreation : Form
    {
        FtpClientUI parent;

        public FileCreation(String title, FtpClientUI p)
        {
            InitializeComponent();
            parent = p;
            titleTxt.Text = title;
        }

        public void setContent(String content)
        {
            contentRTxt.Text = content;
        }

        private void sendBtn_Click(object sender, EventArgs e)
        {
            parent.setFile(contentRTxt.Text);
            this.Close();
        }
    }
}
