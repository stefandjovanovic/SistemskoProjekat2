using SistemskoProjekat2.Services;
using System.Net;

namespace SistemskoProjekat2
{
    public class Program
    {
        private static void Main(string[] args)
        {
            string rootPath = "C:\\Documents\\Fax\\6 semestar\\Sistemsko programiranje\\root";

            ConversionService conversionService = new ConversionService(rootPath);

            WebServer ws = new WebServer(conversionService.GetRequest, 256, "http://localhost:5050/");

            //primer sa ogranicenom velicinom kesa - gde se izbacuju elmenti
            //WebServer ws = new WebServer(conversionService.GetRequest, 5, "http://localhost:5050/");

            ws.Run();
            Console.WriteLine("Pritiskom bilo kog tastera prekinite rad");
            Console.ReadKey();
            ws.Stop();
        }
    }
}