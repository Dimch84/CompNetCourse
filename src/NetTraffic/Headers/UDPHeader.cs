using System;
using System.Net;
using System.IO;

namespace NetTraffic.Headers
{
    public class UDPHeader
    {
        private ushort usSourcePort;
        private ushort usDestinationPort;
        private ushort usLength;
        private short sChecksum;
        private byte[] byUDPData = new byte[4096];

        public UDPHeader(byte[] byBuffer, int nReceived)
        {
            MemoryStream memoryStream = new MemoryStream(byBuffer, 0, nReceived);
            BinaryReader binaryReader = new BinaryReader(memoryStream);

            usSourcePort = (ushort)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());
            usDestinationPort = (ushort)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());
            usLength = (ushort)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());
            sChecksum = IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());
            
            Array.Copy(byBuffer, 8, byUDPData, 0, nReceived - 8);
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
                return byUDPData;
            }
        }
    }
}
