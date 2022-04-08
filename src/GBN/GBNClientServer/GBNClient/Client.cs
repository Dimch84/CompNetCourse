using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;

// https://www.codeproject.com/Articles/14531/Go-Back-N-Simulator

namespace Go_Back_N_Client
{
    public partial class Client : Form
    {
        private byte[] file = new byte[10240];//Byte Array for file
        private Segment[] segments = new Segment[40];//Segments' Array
        private int index = 0;//Index for Segments' Array
        private bool corrupt_segment = false;//Corrupt Segment
        private double probability = 1;//Probability for Segment Corruption
        private int corrupted_packets = 0;//number of segments which will be corrupted
        private Probability[] corrupt_index;//Array for Segments' indexes whose Segment will be corrupted
        private object lockvar = string.Empty;//Object to Synchronize Threads
        private int window_size = 0;//Window Size 

        public Client()
        {
            InitializeComponent();//Initialize Controls
        }

        private void SendSegment()
        {
            try
            {
                Segment seg = segments[index];//Segment
                seg.packet_pos = ++index;//Increase index and set packet position 
                seg.packet_ack = false;//packet ack is not received

                byte[] data = new byte[259];//259 Byte long Array
                string response = string.Empty;//Response from Receiver Side

                IPEndPoint ipep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9999);//Server IP Address and Port Number
                Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//TCP connection
                server.Connect(ipep);//Connect to Server(Receiver Side)
                int recv = server.Receive(data);//Receive "Welcome" msg
                //response = Encoding.ASCII.GetString(data, 0, recv);//Decode msg 

                data = new byte[259];//259 Byte long Array for segment
                for (int i = 0; i < 256; i++)
                {
                    data[i] = seg.data[i];//Fill segment
                }

                //adding sequence number
                if (seg.packet_pos > 9)
                {
                    int tens = seg.packet_pos / 10;
                    int ones = seg.packet_pos % 10;
                    data[256] = Encoding.ASCII.GetBytes(tens.ToString())[0];
                    data[257] = Encoding.ASCII.GetBytes(ones.ToString())[0];

                }
                else
                {
                    data[256] = Encoding.ASCII.GetBytes("0")[0];
                    data[257] = Encoding.ASCII.GetBytes(seg.packet_pos.ToString())[0];
                }
                corrupt_segment = false;//corrupt segment is set to false

                if (CheckOther.Checked)//Check corrupt probability is set or not
                {
                    for (int i = 0; i < corrupt_index.Length; i++)
                    {
                        if (seg.packet_pos == corrupt_index[i].Index && !corrupt_index[i].Used)//Check packet_number, corrupted indexes and index is used before 
                        {
                            corrupt_segment = true;//Corrupt segment is enable
                            corrupt_index[i].Used = true;//Used before
                        }
                    }
                }

                if (corrupt_segment)//Check corrupt_segment
                {
                    data[258] = Encoding.ASCII.GetBytes("1")[0];// 0-> not corrupted 1-> corrupted
                }
                else
                {
                    data[258] = Encoding.ASCII.GetBytes("0")[0];// 0-> not corrupted 1-> corrupted
                }

                int send = server.Send(data, data.Length, SocketFlags.None);//Send Segment
                data = new byte[4];//4 Byte long array
                recv = server.Receive(data);//Receive response
                response = Encoding.ASCII.GetString(data, 0, recv);//Decode response msg
                if (response.Trim() == "ACK")//Check Segment ACK is received
                {
                    seg.packet_ack = true;//Segment ACK is received
                    foreach (Control var in this.Controls)
                    {
                        if (var is Label && (var.Name == "label" + seg.packet_pos.ToString()))
                        {
                            var.BackColor = Color.Yellow;//Segment ACK is received
                        }
                    }
                }
                if (response.Trim() == "NACK")//Check Segment ACK is Not received
                {
                    seg.packet_ack = false;//Segment ACK is not received
                    foreach (Control var in this.Controls)
                    {
                        if (var is Label && (var.Name == "label" + seg.packet_pos.ToString()))
                        {
                            var.BackColor = Color.DodgerBlue;//Segment ACK is NOT received
                        }
                    }
                }

                server.Shutdown(SocketShutdown.Both);//Server Socket is not allowed
                server.Close();//server connection is closed.
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Failure",MessageBoxButtons.OK,MessageBoxIcon.Error);//Exception Message is shown
            }
        }
        
        /// <summary>
        /// Send Segment according to its position
        /// </summary>
        /// <param name="position"></param>
        private void SendSegments(int position)
        {
            lock (lockvar)//Synchronize threads 
            {
                try
                {
                    Segment seg = segments[position - 1];//Segment
                    seg.packet_pos = position;//set packet position 
                    seg.packet_ack = false;//packet ack is not received


                    byte[] data = new byte[259];//259 Byte long Array
                    string response = string.Empty;//Response from Receiver Side
                    IPEndPoint ipep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9999);//Server IP Address and Port Number
                    Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//TCP connection

                    server.Connect(ipep);//Connect to Server(Receiver Side)
                    int recv = server.Receive(data);//Receive "Welcome" msg
                    response = Encoding.ASCII.GetString(data, 0, recv);//Decode msg 

                    data = new byte[259];//259 Byte long Array for segment
                    for (int i = 0; i < 256; i++)
                    {
                        data[i] = seg.data[i];//Fill segment
                    }

                    //adding sequence number
                    if (seg.packet_pos > 9)
                    {
                        int tens = seg.packet_pos / 10;
                        int ones = seg.packet_pos % 10;
                        data[256] = Encoding.ASCII.GetBytes(tens.ToString())[0];
                        data[257] = Encoding.ASCII.GetBytes(ones.ToString())[0];
                    }
                    else
                    {
                        data[256] = Encoding.ASCII.GetBytes("0")[0];
                        data[257] = Encoding.ASCII.GetBytes(seg.packet_pos.ToString())[0];
                    }
                   
                    data[258] = Encoding.ASCII.GetBytes("0")[0];// 0-> not corrupted 1-> corrupted

                    int send = server.Send(data, data.Length, SocketFlags.None);//Send Segment
                    data = new byte[4];//4 Byte long array
                    recv = server.Receive(data);//Receive response
                    response = Encoding.ASCII.GetString(data, 0, recv);//Decode response msg
                    if (response.Trim() == "ACK")//Check Segment ACK is received or not
                    {
                        // MessageBox.Show("packet received"+Encoding.ASCII.GetString(seg.data, 0, seg.data.Length));
                        seg.packet_ack = true;//Segment ACK is received
                        foreach (Control var in this.Controls)
                        {
                            if (var is Label && (var.Name == "label" + seg.packet_pos.ToString()))
                            {
                                var.BackColor = Color.Yellow;//Segment ACK is received
                            }
                        }
                    }
                    if (response.Trim() == "NACK")//Check Segment ACK is Not received
                    {
                        seg.packet_ack = false;//Segment ACK is not received
                        foreach (Control var in this.Controls)
                        {
                            if (var is Label && (var.Name == "label" + seg.packet_pos.ToString()))
                            {
                                var.BackColor = Color.DodgerBlue;
                            }
                        }
                    }
                    server.Shutdown(SocketShutdown.Both);//Server Socket is not allowed
                    server.Close();//server connection is closed.
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString(), "Failure", MessageBoxButtons.OK, MessageBoxIcon.Error);//Exception Message is shown
                }
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            bool firstSend = true;//First time Segment Sending

            if (index > 0 && index <= 40)//Check index
            {
                if (index == window_size)
                {
                    for (int i = 0; i < index; i++)
                    {
                        if (segments[i].packet_ack == false)//Check previous packets' ACKS
                        {
                            for (int j = 0; j < index; j++)
                            {
                                SendSegments(j + 1);//Resend Segments
                            }
                            firstSend = false;//first send
                            break;
                        }
                    }
                }
                else
                {
                    for (int i = index - window_size; i < index; i++)
                    {
                        if (segments[i].packet_ack == false)//Check previous packets' ACKS
                        {
                            for (int j = index - window_size; j < index; j++)
                            {
                                SendSegments(j + 1);//Resend Segments
                            }
                            firstSend = false;//first send
                            break;
                        }
                    }
                }

                if (firstSend)
                {
                    window_size = (int)numWindowSize.Value;//window size is set
                    int delay = (int)numDelay.Value;//delay value in microsecs
                    Thread[] threads = new Thread[window_size];//Multiple threads
                    for (int i = 0; i < window_size; i++)
                    {
                        threads[i] = new Thread(new ThreadStart(SendSegment));//Construct threads
                        threads[i].Start();//Thread starts
                        Thread.Sleep(delay);//Threads sleep
                    }
                }
            }

            if (index == 0 && firstSend)
            {
                window_size = (int)numWindowSize.Value;//window size is set
                int delay = (int)numDelay.Value;//delay value in microsecs
                Thread[] threads = new Thread[window_size];//Multiple threads
                for (int i = 0; i < window_size; i++)
                {
                    threads[i] = new Thread(new ThreadStart(SendSegment));//Construct threads
                    threads[i].Start();//Thread starts
                    Thread.Sleep(delay);//Threads sleep
                }
            }
        }

        private void Client_Load(object sender, EventArgs e)
        {
            lblClientState.Text = "Please Upload 10240 Byte Long File...";
            btnConnect.Enabled = false;//bntConnect is disabled default
            foreach (Control var in this.Controls)//Check controls in form
            {
                if (var is Label && var.Name.StartsWith("label"))
                {
                    var.Text = var.Text.Replace("label", "Segment#");
                }
            }

            probability = 1 / ((double)numProbability.Value);//probability value is calculated
            corrupted_packets = (int)(probability * 40);//number of corrupted ACKs calculated
            corrupt_index = new Probability[corrupted_packets];//Probability array is created

            Random random = new Random();//Random number for Corrupted ACKS Index
            for (int i = 0; i < corrupt_index.Length; i++)
            {
                corrupt_index[i] = new Probability();//Member is constructed
                corrupt_index[i].Used = false;//Not used
                int rand = random.Next(0, 40);
                for (int j = 0; j < i - 1; j++)
                {
                    if (corrupt_index[j].Index == rand)
                    {
                        rand = random.Next(0, 40);//a random number is selected between 0 to 40
                       for (int k = 0; k < j - 1; k++)
                        {
                            if (corrupt_index[k].Index == rand)//check other indexes
                            {
                                rand = random.Next(0, 40);//a random number is selected between 0 to 40
                                for (int m = 0; m < k - 1; m++)
                                {
                                    if (corrupt_index[m].Index == rand)//check other indexes
                                    {
                                        rand = random.Next(0, 40);//a random number is selected between 0 to 40
                                    }
                                }
                            }
                        }
                    }
                }
                corrupt_index[i].Index = rand;//add random number in index
            }

            ToolTip tipUpload = new ToolTip();//Tooltip for btnFileUpload
            tipUpload.IsBalloon = true;//Show in balloon form
            tipUpload.ToolTipTitle = "GBN Simulation";
            tipUpload.ToolTipIcon = ToolTipIcon.Info;
            tipUpload.SetToolTip(btnFileUpload, "Click to Upload A File");//Add tooltip to btnFileUpload

            ToolTip tipRun = new ToolTip();//Tooltip for btnConnect
            tipRun.IsBalloon = true;//Show in balloon form
            tipRun.ToolTipTitle = "GBN Simulation";
            tipRun.ToolTipIcon = ToolTipIcon.Info;
            tipRun.SetToolTip(btnConnect, "Click to Run");//Add tooltip to btnConnect
        }

        private void btnFileUpload_Click(object sender, EventArgs e)
        {
            index = 0;//index is set to zero
            for (int i = 0; i < 40; i++)
            {
                segments[i] = new Segment();//Segment member created
                segments[i].data = new byte[256];//Byte array created
            }

            OpenFileDialog ofd = new OpenFileDialog();//Open File Dialog 
            ofd.Title = "Select a text file to Send";
            ofd.Filter = "Text Files (*.txt)|*.txt";//File Type txt
            if (ofd.ShowDialog(this) == DialogResult.OK)//Check file is selected or not
            {
                FileStream fs = new FileStream(ofd.FileName, FileMode.Open);//FileStream is created
                file = ReadFully(fs, 10240);//10240 Byte is read
                for (int i = 0; i < 40; i++)
                {
                    for (int j = 0; j < 256; j++)
                    {
                        segments[i].data[j] = file[i * 256 + j];//Fragmenting File into segments
                    }
                }
                lblClientState.Text = "40 Segments with 256Bytes  are ReaDy to Send!..";
                btnConnect.Enabled = true;//Enable bntConnect
                foreach (Control var in this.Controls)
                {
                    if (var is Label && var.Name.StartsWith("label"))
                    {
                        var.BackColor = Color.PaleGreen;//Segments are ready to send.
                    }
                }
                fs.Close();//Filestream is closed.
            }
        }

        public static byte[] ReadFully(Stream stream, int dimension)
        {
            byte[] buffer = new byte[dimension];//A byte array with "dimension" long
            using (MemoryStream ms = new MemoryStream())//Memory stream
            {
                while (true)
                {
                    int read = stream.Read(buffer, 0, buffer.Length);
                    if (read <= 0)//check number of bytes read
                        return ms.ToArray();//if number of bytes read is zero or negative, return byte array
                    ms.Write(buffer, 0, read);//writes a block of bytes to the current stream using data read from buffer
                }
            }
        }

        private void numProbability_ValueChanged(object sender, EventArgs e)
        {
            probability = 1 / ((double)numProbability.Value);//probability value is calculated
            corrupted_packets = (int)(probability * 40);//number of corrupted ACKs calculated
            corrupt_index = new Probability[corrupted_packets];//Probability array is created

            Random random = new Random();//Random number for Corrupted ACKS Index
            for (int i = 0; i < corrupt_index.Length; i++)
            {
                corrupt_index[i] = new Probability();//Member is constructed
                corrupt_index[i].Used = false;//Not used
                int rand = random.Next(0, 40);
                for (int j = 0; j < i - 1; j++)
                {
                    if (corrupt_index[j].Index == rand)
                    {
                        rand = random.Next(0, 40);//a random number is selected between 0 to 40
                        for (int k = 0; k < j - 1; k++)
                        {
                            if (corrupt_index[k].Index == rand)//check other indexes
                            {
                                rand = random.Next(0, 40);//a random number is selected between 0 to 40
                                for (int m = 0; m < k - 1; m++)
                                {
                                    if (corrupt_index[m].Index == rand)//check other indexes
                                    {
                                        rand = random.Next(0, 40);//a random number is selected between 0 to 40
                                    }
                                }
                            }
                        }
                    }
                }
                corrupt_index[i].Index = rand;//add random number in index
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

        private void numWindowSize_ValueChanged(object sender, EventArgs e)
        {
            window_size = (int)numWindowSize.Value;//Window Size is set
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
       
        }
        private bool Packet_ack;

        public bool packet_ack
        {
            get { return Packet_ack; }
            set { Packet_ack = value; }
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