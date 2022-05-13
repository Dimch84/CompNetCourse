using System;
using System.Windows.Forms;
using System.Net;
using System.IO;

namespace NetTraffic.Headers
{
    public class TCPHeader
    {
        private ushort usSourcePort;
        private ushort usDestinationPort;
        private uint uiSequenceNumber = 555;
        private uint uiAcknowledgementNumber = 555;
        private ushort usDataOffsetAndFlags = 555;
        private ushort usWindow = 555;
        private short sChecksum = 555;
        private ushort usUrgentPointer;
        private byte byHeaderLength;
        private ushort usMessageLength;
        private byte[] byTCPData = new byte[4096];

        public TCPHeader(byte[] byBuffer, int nReceived)
        {
            try
            {
                MemoryStream memoryStream = new MemoryStream(byBuffer, 0, nReceived);
                BinaryReader binaryReader = new BinaryReader(memoryStream);

                usSourcePort = (ushort)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());
                usDestinationPort = (ushort)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());
                uiSequenceNumber = (uint)IPAddress.NetworkToHostOrder(binaryReader.ReadInt32());
                uiAcknowledgementNumber = (uint)IPAddress.NetworkToHostOrder(binaryReader.ReadInt32());
                usDataOffsetAndFlags = (ushort)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());
                usWindow = (ushort)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());
                sChecksum = (short)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());
                usUrgentPointer = (ushort)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());
                byHeaderLength = (byte)(usDataOffsetAndFlags >> 12);
                byHeaderLength *= 4;
                usMessageLength = (ushort)(nReceived - byHeaderLength);

                Array.Copy(byBuffer, byHeaderLength, byTCPData, 0, nReceived - byHeaderLength);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public string SourcePort
        {
            get
            {
                return usSourcePort.ToString();
            }
        }

        public string DestinationPort
        {
            get
            {
                return usDestinationPort.ToString();
            }
        }

        public byte[] Data
        {
            get
            {
                return byTCPData;
            }
        }
    }
}
