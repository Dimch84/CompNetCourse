using GetMyLocalAddrAndMask;

var ipAddr = NetUtility.GetMyAddress(out var mask);
Console.WriteLine($"My local address: {ipAddr}, mask: {mask}");
