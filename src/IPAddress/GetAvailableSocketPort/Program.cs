using GetAvailableSocketPort;
using GetMyLocalAddrAndMask;

var myIpAddress = NetUtility.GetMyAddress(out var mask);
var availablePorts = IPUtility.GetAvailablePort(0, 8080, myIpAddress, true);
Console.WriteLine(String.Join(", ", availablePorts));
