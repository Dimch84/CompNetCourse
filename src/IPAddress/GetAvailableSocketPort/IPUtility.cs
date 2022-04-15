using System.Net;
using System.Net.NetworkInformation;

namespace GetAvailableSocketPort
{
    public static class IPUtility
    {
        /// <summary>
        /// Returns first available port on the specified IP address. The port scan excludes ports that are open on ANY loopback adapter. 
        /// If the address upon which a port is requested is an 'ANY' address all ports that are open on ANY IP are excluded.
        /// </summary>
        /// <param name="rangeStart"></param>
        /// <param name="rangeEnd"></param>
        /// <param name="ip">The IP address upon which to search for available port.</param>
        /// <param name="includeIdlePorts">If true includes ports in TIME_WAIT state in results. TIME_WAIT state is typically cool down period for recently released ports.</param>
        /// <returns></returns>
        public static List<ushort> GetAvailablePort(UInt16 rangeStart, UInt16 rangeEnd, IPAddress ip, bool includeIdlePorts)
        {
            IPGlobalProperties ipProps = IPGlobalProperties.GetIPGlobalProperties();

            // if the ip we want a port on is an 'any' or loopback port we need to exclude all ports that are active on any IP
            Func<IPAddress, bool> isIpAnyOrLoopBack = i => IPAddress.Any.Equals(i) ||
                                                           IPAddress.IPv6Any.Equals(i) ||
                                                           IPAddress.Loopback.Equals(i) ||
                                                           IPAddress.IPv6Loopback.Equals(i);

            // get all active ports on specified IP. 
            List<ushort> excludedPorts = new List<ushort>();

            // if a port is open on an 'any' or 'loopback' interface then include it in the excludedPorts
            excludedPorts.AddRange(from n in ipProps.GetActiveTcpConnections()
                                   where
                                       n.LocalEndPoint.Port >= rangeStart
                                       && n.LocalEndPoint.Port <= rangeEnd
                                       &&
                                       (isIpAnyOrLoopBack(ip) || n.LocalEndPoint.Address.Equals(ip) ||
                                        isIpAnyOrLoopBack(n.LocalEndPoint.Address))
                                       && (!includeIdlePorts || n.State != TcpState.TimeWait)
                                   select (ushort)n.LocalEndPoint.Port);

            excludedPorts.AddRange(from n in ipProps.GetActiveTcpListeners()
                                   where n.Port >= rangeStart && n.Port <= rangeEnd
                                         &&
                                         (isIpAnyOrLoopBack(ip) || n.Address.Equals(ip) || isIpAnyOrLoopBack(n.Address))
                                   select (ushort)n.Port);

            excludedPorts.AddRange(from n in ipProps.GetActiveUdpListeners()
                                   where n.Port >= rangeStart && n.Port <= rangeEnd
                                         &&
                                         (isIpAnyOrLoopBack(ip) || n.Address.Equals(ip) || isIpAnyOrLoopBack(n.Address))
                                   select (ushort)n.Port);

            excludedPorts.Sort();

            var ret = new List<ushort>();
            for (ushort port = rangeStart; port <= rangeEnd; port++)
            {
                if (!excludedPorts.Contains(port))
                {
                    ret.Add(port);
                }
            }

            return ret;
        }
    }
}
