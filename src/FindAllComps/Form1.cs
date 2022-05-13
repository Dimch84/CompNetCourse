using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace FindAllComps
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int timeout = 100;
            IPAddress myip;
            Ping ping = new Ping();
            string data = "Hello";
            byte[] buffer = Encoding.ASCII.GetBytes(data);
            PingReply reply;
            MAC mac = new MAC();
            string myname = Dns.GetHostName();
            IPAddress[] web = Dns.GetHostByName(myname).AddressList;
            myip = web[0];

            progressBar1.Value = 0;
            dataGridView1.Rows.Add();
            dataGridView1.Rows[0].Cells[0].Value = "Текущий ПК";
            dataGridView1.Rows.Add();
            dataGridView1.Rows[1].Cells[0].Value = myip.ToString();
            dataGridView1.Rows[1].Cells[2].Value = myname; 
            dataGridView1.Rows[1].Cells[1].Value = mac.GetMAC(myip);
            dataGridView1.Rows.Add();
            dataGridView1.Rows[2].Cells[0].Value = "Сеть";
            dataGridView1.Refresh();

      //      foreach (IPAddress ip in web)
      //      {                
                
                string myipstr = myip.ToString().Remove(myip.ToString().LastIndexOf('.')) + ".";          
                for (int i = 1; i < 255; i++)
                {
                    progressBar1.Value = (int)(i / 2.54);
                    reply = ping.Send(myipstr + i, timeout, buffer);
                    if (reply.Status == IPStatus.Success)
                    {
                        dataGridView1.Rows.Add();
                        myip = reply.Address;
                        dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[0].Value = myip.ToString();
                        dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[1].Value = mac.GetMAC(myip);
                        try
                        {
                            IPHostEntry local = Dns.GetHostByAddress(myip);
                            dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[2].Value = local.HostName;
                        }
                        catch (SocketException)
                        {
                            dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[2].Value = " - ";
                        }
    
                        dataGridView1.Refresh();
                    }
       //         }
            }
        }
    }

    public class MAC
    {
        [DllImport("iphlpapi.dll", ExactSpelling = true)]
        public static extern int SendARP(int DestIP, int SrcIP, [Out] byte[] pMacAddr, ref int PhyAddrLen);

        public string GetMAC(IPAddress ip)
        {
            byte[] ab = new byte[6];
            int len = ab.Length;
            int r = SendARP(ip.GetHashCode(), 0, ab, ref len);
            return BitConverter.ToString(ab, 0, 6);
        }
    }
}
