using GetPost;

// GET
Console.WriteLine("GET DATA");
var resGET = Utils.SubmitRequestAndGetResponse("https://www.google.com/search", "q=риа+новости", false);
Console.WriteLine(resGET);

// POST
Console.WriteLine("\n\n\nPOST DATA");
var resPUT = Utils.SubmitRequestAndGetResponse("https://httpbin.org/post", "my test message to service", true);
Console.WriteLine(resPUT);
