using System;
using System.Windows.Forms;
using System.Net;
using System.IO;

namespace NetTraffic.Headers
{
    public class IPHeader
    {
        private byte byVersionAndHeaderLength;
        private byte byDifferentiatedServices;
        private ushort usTotalLength;
        private ushort usIdentification;
        private ushort usFlagsAndOffset;
        private byte byTTL;
        private byte byProtocol;
        private short sChecksum;
        private uint uiSourceIPAddress;
        private uint uiDestinationIPAddress;
        private byte byHeaderLength;
        private byte[] byIPData = new byte[4096];

        public IPHeader(byte[] byBuffer, int nReceived)
        {
            try
            {
                MemoryStream memoryStream = new MemoryStream(byBuffer, 0, nReceived);
                BinaryReader binaryReader = new BinaryReader(memoryStream);

                byVersionAndHeaderLength = binaryReader.ReadByte();
                byDifferentiatedServices = binaryReader.ReadByte();
                usTotalLength = (ushort)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());
                usIdentification = (ushort)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());
                usFlagsAndOffset = (ushort)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());
                byTTL = binaryReader.ReadByte();
                byProtocol = binaryReader.ReadByte();
                sChecksum = IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());
                uiSourceIPAddress = (uint)(binaryReader.ReadInt32());
                uiDestinationIPAddress = (uint)(binaryReader.ReadInt32());
                byHeaderLength = byVersionAndHeaderLength;
                byHeaderLength <<= 4;
                byHeaderLength >>= 4;
                byHeaderLength *= 4;
                Array.Copy(byBuffer, byHeaderLength, byIPData, 0, usTotalLength - byHeaderLength);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "MJsniffer", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        public string Version
        {
            get
            {
                if ((byVersionAndHeaderLength >> 4) == 4) return "IP v4";
                else if ((byVersionAndHeaderLength >> 4) == 6) return "IP v6";
                else return "Unknown";
            }
        }

        public ushort MessageLength
        {
            get
            {
                return (ushort)(usTotalLength - byHeaderLength);
            }
        }

        public string TTL
        {
            get
            {
                return byTTL.ToString();
            }
        }

        public Protocol ProtocolType
        {
            get
            {
                if (byProtocol == 6) return Protocol.TCP;
                else if (byProtocol == 17) return Protocol.UDP;
                else return Protocol.Unknown;
            }
        }

        public IPAddress SourceAddress
        {
            get
            {
                return new IPAddress(uiSourceIPAddress);
            }
        }

        public IPAddress DestinationAddress
        {
            get
            {
                return new IPAddress(uiDestinationIPAddress);
            }
        }

        public byte[] Data
        {
            get
            {
                return byIPData;
            }
        }
    }
}
