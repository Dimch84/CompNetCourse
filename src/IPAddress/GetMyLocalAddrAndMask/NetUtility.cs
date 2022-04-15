using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace GetMyLocalAddrAndMask
{
    public static class NetUtility
    {
        /// <summary>
        /// Gets my local IP address (not necessarily external) and subnet mask
        /// </summary>
        public static IPAddress GetMyAddress(out IPAddress mask)
        {
            NetworkInterface ni = GetNetworkInterface();
            if (ni == null)
            {
                mask = null;
                return null;
            }

            IPInterfaceProperties properties = ni.GetIPProperties();
            foreach (UnicastIPAddressInformation unicastAddress in properties.UnicastAddresses)
            {
                if (unicastAddress != null && unicastAddress.Address != null && unicastAddress.Address.AddressFamily == AddressFamily.InterNetwork)
                {
                    mask = unicastAddress.IPv4Mask;
                    return unicastAddress.Address;
                }
            }

            mask = null;
            return null;
        }

        private static NetworkInterface GetNetworkInterface()
        {
            IPGlobalProperties computerProperties = IPGlobalProperties.GetIPGlobalProperties();
            if (computerProperties == null)
                return null;

            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            if (nics == null || nics.Length < 1)
                return null;

            NetworkInterface best = null;
            foreach (NetworkInterface adapter in nics)
            {
                if (adapter.NetworkInterfaceType == NetworkInterfaceType.Loopback || adapter.NetworkInterfaceType == NetworkInterfaceType.Unknown)
                    continue;
                if (!adapter.Supports(NetworkInterfaceComponent.IPv4))
                    continue;
                if (best == null)
                    best = adapter;
                if (adapter.OperationalStatus != OperationalStatus.Up)
                    continue;

                // A computer could have several adapters (more than one network card)
                // here but just return the first one for now...
                return adapter;
            }
            return best;
        }
    }
}
