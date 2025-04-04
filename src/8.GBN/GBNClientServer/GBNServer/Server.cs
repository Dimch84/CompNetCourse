using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Diagnostics;

// https://www.codeproject.com/Articles/14531/Go-Back-N-Simulator

namespace Go_Back_N_Server
{
    public partial class Server : Form
    {
        private Segment[] segments = new Segment[40];//Array for 40 Segments
        private int index=-1; //Index for segments rray
        private bool corrupt_ACK = false; //Acknowledge corrupt
        private double probability = 1; //Probability of Acknowledge corrupt in channel
        private int corrupted_ACKs = 0; //number of corrupted ACKs
        private Probability[] corruptedACKs_index; //Array for Segments' indexes whose ACK will be corrupet

        public Server()
        {
            InitializeComponent();//Initialize Controls
        }
 
        private void Serve()
        {
             try
             {
                 //string response;//The msg taken from sender side
                 int recv;//Number
                 int packet_number;//Packet Number
                 bool work = true;//Boolean variable for while loop
                 IPEndPoint ipep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9999);//IPEndPoint with 127.0.0.1 IP and 9999 port number
                 Socket newsock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//TCP connection
                 newsock.Bind(ipep);//Combine IP and Socket
                 newsock.Listen(40);//Maximum lenght of the pending connection queue is set to 40
                 byte[] data;//Byte array for segment

                   foreach (Control var in this.Controls)//Check Controls in Form
                {
                    for (int i = 1; i < 41; i++)
                    {
                        if (var is Label && (var.Name == "label" + i.ToString()))// Checks control is label && its name starts with "label or not"
                        {
                            var.BackColor = Color.Gold;//Change Color to Gold
                        }
                    }
                }

                while (work)//Infinite Loop until work is set to false
                {
                    data = new byte[259];//new 259 byte long array
                    Socket client = newsock.Accept();//Server waits for client.If it finds then it opens a new socket for connection

                    data = Encoding.ASCII.GetBytes("Welcome");//Welcome msg is encoded to byte format
                    client.Send(data, data.Length, SocketFlags.None);//msg is sent

                    data = new byte[259];//new 259 byte long array

                    recv = client.Receive(data);//Receive Segment from sender side

                    byte[] packet_tag = new byte[3];//3 Byte long Byte Array

                    packet_tag[0] = data[256];//Take Byte from Segment
                    packet_tag[1] = data[257];//Take Byte from Segment
                    packet_tag[2] = data[258];//Take Byte from Segment

                    packet_number = Convert.ToInt16(Encoding.ASCII.GetString(packet_tag, 0, 2));//256th and 257th Byte shows the packet number.
                    int corrupted = Convert.ToInt16(Encoding.ASCII.GetString(packet_tag, 2, 1));//258th byte determines whether segment is corrupted or not.

                    index = packet_number;//Refresh index

                    if (packet_number >= 45)//Checks packet number is bigger than 45 or not
                    {
                        work = false;//while loop will end

                    }

                    for (int i = 0; i < 256; i++)
                    {
                        segments[packet_number - 1].data[i] = data[i];//Adding received segment to Segments' Arraay
                    }


                   // response = Encoding.ASCII.GetString(segments[packet_number - 1].data, 0, 256);

                    corrupt_ACK = false;//Corrupt ACK is default to set false
                    if (corrupted == 0)//Check corrupted. 0-> Segment NOT Corrupted, 1->Segment Corrupted 
                    {
                        segments[packet_number - 1].reached_dest = true;//Segment Received
                        segments[packet_number - 1].packet_pos = packet_number;//Segment packet position is set

                        if (CheckOther.Checked)//Check whether ACK corrupt probability is set or not
                        {                            
                           for (int i = 0; i < corruptedACKs_index.Length; i++)
                            {
                                if (corruptedACKs_index[i].Index == packet_number && !corruptedACKs_index[i].Used)//Check packet_number, corrupted indexes and index is used before 
                                {

                                    data = new byte[4];//4 byte long byte array
                                    data = Encoding.ASCII.GetBytes("NACK");//NACK is encoded to byte format
                                    client.Send(data, data.Length, SocketFlags.None);//NACK is sent to Sender Side

                                    corruptedACKs_index[i].Used = true;//Index will not be used  
                                    corrupt_ACK = true;//ACK is corrupted
                                    
                                    foreach (Control var in this.Controls)//Checks all controls in Form
                                    {
                                        if (var is Label && (var.Name == "label" + segments[packet_number - 1].packet_pos.ToString()))
                                        {
                                            var.BackColor = Color.DeepSkyBlue;//Packet Received but ACK is corrupted
                                        }
                                    }
                                }
                            }
                        }

                        if (!corrupt_ACK)//Check corrupt ACK is false or true
                        {
                            //if it is false
                            data = new byte[3];//3 Byte long Array
                            data = Encoding.ASCII.GetBytes("ACK");//ACK is encoded to byte
                            client.Send(data, data.Length, SocketFlags.None);//Send ACK to Sender Side

                            foreach (Control var in this.Controls)//Checks all controls in Form
                            {
                                if (var is Label && (var.Name == "label" + segments[packet_number - 1].packet_pos.ToString()))
                                {
                                    var.BackColor = Color.Red;//Segment Received and ACK sent :)
                                }
                            }
                        }
                    }
                    else
                    {
                        segments[packet_number - 1].reached_dest = false; //Segment Not Received
                        segments[packet_number - 1].packet_pos = packet_number;//Packet Position is Set

                        data = new byte[4];//4 bytes for Data
                        data = Encoding.ASCII.GetBytes("NACK");//NACK is encoded
                        client.Send(data, data.Length, SocketFlags.None);//NACK is sent to Server Side
                        for (int i = 0; i < corruptedACKs_index.Length; i++)
                        {
                            if (corruptedACKs_index[i].Index == packet_number && !corruptedACKs_index[i].Used)
                            {
                                corruptedACKs_index[i].Used = true;//Index is used 
                            }
                        }
                    }
                    client.Close();//Client Connection is closed
                }
                newsock.Close();//Server Connection is closed
                lblSrvState.Text = "Server is NOT Working";//Server's State is shown to User
            }
            catch (Exception ex)
            {
               // MessageBox.Show(ex.Message,"Exception",MessageBoxButtons.OK,MessageBoxIcon.Error);//Exception Message is shown
            }
        }
      
        private void btnServe_Click(object sender, EventArgs e)
        {
            Thread ta = new Thread(new ThreadStart(Serve));//Create a thread for Serve
            ta.Start();//thread starts
            lblSrvState.Text = "Server is Working";  //Server's State is shown to User       
        }

        private void Server_Load(object sender, EventArgs e)
        {
            lblSrvState.Text = "Server is NOT Working";//Server's State is shown to User

            for (int i = 0; i < 40; i++)
            {
                segments[i] = new Segment();//Segments' array's members are constructed
                segments[i].data = new byte[256];
            }
            foreach (Control var in this.Controls)
            {
                if (var is Label && var.Name.StartsWith("label"))
                {
                    var.Text = var.Text.Replace("label", "Segment#");//Labels' texts are changed to "Segment#"
                }
            }
            
            ToolTip tipServe = new ToolTip();//Tooltip for btnServe
            tipServe.IsBalloon = true;//Show in balloon form
            tipServe.ToolTipTitle = "GBN Simulation";
            tipServe.ToolTipIcon = ToolTipIcon.Info;
            tipServe.SetToolTip(btnServe, "Click to Start Simulation");//Add tooltip to btnServe

            ToolTip tipDownload = new ToolTip();//Tooltip for btnServe
            tipDownload.IsBalloon = true;//Show in balloon form
            tipDownload.ToolTipTitle = "GBN Simulation";
            tipDownload.ToolTipIcon = ToolTipIcon.Info;
            tipDownload.SetToolTip(btnDownload, "Click to Get File");//Add tooltip to btnServe

            probability = 1 / ((double)numProbability.Value);//probability value is calculated
            corrupted_ACKs = (int)(probability * 40);//number of corrupted ACKs calculated
            corruptedACKs_index = new Probability[corrupted_ACKs];//Probability array is created

            Random random = new Random();//Random number for Corrupted ACKS Index

            for (int i = 0; i < corruptedACKs_index.Length; i++)
            {
                corruptedACKs_index[i] = new Probability();//Member is constructed
                corruptedACKs_index[i].Used = false;//Not used
                int rand = random.Next(0, 40);//a random number is selected between 0 to 40
                
                for (int j = 0; j < i - 1; j++)
                {
                    if (corruptedACKs_index[j].Index == rand)//check other indexes
                    {
                        rand = random.Next(0, 40);

                        for (int k = 0; k < j - 1; k++)
                        {
                            if (corruptedACKs_index[k].Index == rand)//check other indexes
                            {
                                rand = random.Next(0, 40);

                                for (int m = 0; m < k - 1; m++)
                                {
                                    if (corruptedACKs_index[m].Index == rand)//check other indexes
                                    {
                                        rand = random.Next(0, 40);
                                    }
                                }
                            }
                        }
                    }
                }
                corruptedACKs_index[i].Index = rand;//add random number in index
            }
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            try
            {
                bool DownloadEnable = false;//DownloadEnable is set to false
                for (int i = 0; i < 40; i++)
                {
                    if (segments[i].reached_dest == false)//Check segments
                    {
                        DownloadEnable = false;//if any segments not received, DownloadEnable is set to false
                        break;//break loop
                    }
                    else
                    {
                        DownloadEnable = true;//Download is now avaiable
                    }
                }

                if (index >= 40 && DownloadEnable)//check index and DownloadEnable
                {
                    SaveFileDialog sfd = new SaveFileDialog();//Create SaveFileDialog
                    sfd.Title = "Select a text file";//Title
                    sfd.Filter = "Text Files (*.txt)|*.txt";//File Type

                    if (sfd.ShowDialog(this) == DialogResult.OK)//check file name is selected or not
                    {
                        FileStream fs = new FileStream(sfd.FileName, FileMode.Create);//create file stream
                        StreamWriter swtr = new StreamWriter(fs, Encoding.GetEncoding("ISO-8859-9"));//Stream Writer which uses Turkish Encoding

                        byte[] receivedData = new byte[10240];//Our segments are total 10240 byte data.

                        for (int i = 0; i < 40; i++)
                        {
                            for (int j = 0; j < 256; j++)
                            {
                                receivedData[i * 256 + j] = segments[i].data[j];//Combine all segments in receivedData
                            }
                        }
                        string words = Encoding.ASCII.GetString(receivedData, 0, 10240);//Decode receivedData to ASCII format
                        swtr.WriteLine(words);//Write string to file stream fs
                        fs.Close();//file stream is closed.

                        Process process = new Process();//Process for open text file
                        process.StartInfo.FileName = sfd.FileName;//File Name is set
                        process.StartInfo.Verb = "Open";//Command
                        process.StartInfo.CreateNoWindow = true;
                        process.Start();//Open text file
                    }
                }
                else
                {
                    MessageBox.Show("No File Received or File is Corrupted!...", "Failure");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);//Exception Message is shown
            }
        }

        private void numProbability_ValueChanged(object sender, EventArgs e)
        {
            probability = 1 / ((double)numProbability.Value);//probability value is calculated
            corrupted_ACKs = (int)(probability * 40);//number of corrupted ACKs calculated
            corruptedACKs_index = new Probability[corrupted_ACKs];//Probability array is created

            Random random = new Random();//Random number for Corrupted ACKS Index

            for (int i = 0; i < corruptedACKs_index.Length; i++)
            {
                corruptedACKs_index[i] = new Probability();//Member is constructed
                corruptedACKs_index[i].Used = false;//Not used
                int rand = random.Next(0, 40);//a random number is selected between 0 to 40

                for (int j = 0; j < i - 1; j++)
                {
                    if (corruptedACKs_index[j].Index == rand)//check other indexes
                    {
                        rand = random.Next(0, 40);

                        for (int k = 0; k < j - 1; k++)
                        {
                            if (corruptedACKs_index[k].Index == rand)//check other indexes
                            {
                                rand = random.Next(0, 40);

                                for (int m = 0; m < k - 1; m++)
                                {
                                    if (corruptedACKs_index[m].Index == rand)//check other indexes
                                    {
                                        rand = random.Next(0, 40);
                                    }
                                }
                            }
                        }
                    }
                }
                corruptedACKs_index[i].Index = rand;//add random number in index
            }
        }

        private void checkZero_CheckedChanged(object sender, EventArgs e)
        {
            CheckOther.Checked = !checkZero.Checked;//toogle check boxes
        }

        private void CheckOther_CheckedChanged(object sender, EventArgs e)
        {
            checkZero.Checked = !CheckOther.Checked;//toogle check boxes
        }
    }

    class Segment
    {
        private byte[] Data;//byte array for segment

        public byte[] data
        {
            get { return Data; }
            set { Data = value; }
        }
        private bool Reached_dest;//segment received

        public bool reached_dest
        {
            get { return Reached_dest; }
            set { Reached_dest = value; }
        }
        private int Packet_pos;//packet position

        public int packet_pos
        {
            get { return Packet_pos; }
            set { Packet_pos = value; }
        }

        public Segment()
        {
            packet_pos = 0;
            data = new byte[256];
        }
    }

    class Probability
    {
        private int index;//index
        private bool used;//used before

        public Probability()
        {
            used = false;
            index = -1;
        }

       
        public bool Used
        {
            get { return used; }
            set { used = value; }
        }

        public int Index
        {
            get { return index; }
            set { index = value; }
        }
    }
}
