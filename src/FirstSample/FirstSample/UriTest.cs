namespace FirstSample
{
    public class UriTest
    {
        public static Uri GetSimpleUri() {
            var builder = new UriBuilder();
            builder.Scheme = "https";
            builder.Host = "math-cs.spbu.ru";
            return builder.Uri;
        }

        public static Uri GetSimpleUri_Constructor() {
            var builder = new UriBuilder("https", "math-cs.spbu.ru");
            return builder.Uri;
        }

        public static void Run()
        {
            var simpleUri = GetSimpleUri();
            Console.WriteLine(simpleUri.ToString());

            var constructorUri = GetSimpleUri_Constructor();
            Console.WriteLine(constructorUri.ToString());

            Console.ReadLine();
        }

        public static void Run2()
        {
		    Uri resource1 = new Uri("http://www.gotdotnet.com/userarea/default.aspx");
		    Uri resource2 = new Uri("http://www.gotdotnet.com/team/libraries/");
		    Console.WriteLine(resource1.MakeRelative(resource2));
		    Console.WriteLine(resource2.MakeRelative(resource1));

		    Uri resource3 = new Uri("http://msdn.microsoft.com/vstudio/default.asp");
		    Console.WriteLine(resource2.MakeRelative(resource3));

            Console.ReadLine();
        }
    }
}
