namespace FirstSample
{
    public class Program
    {
        public static void Main()
        {
            DnsTest.Run(hostname: "www.microsoft.com");
            DnsTest.Run(hostname: "math-cs.spbu.ru");

            UriTest.Run();
            UriTest.Run2();

            WebRequestTest.Run();

            NetworkAnalysisDemo.Run();
        }
    }
}
