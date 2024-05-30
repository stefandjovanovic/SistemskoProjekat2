using System;
using System.Net.Http;
using System.Threading;



namespace ClientApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // Lista fajlova
            List<string> fileNames = new List<string> { 
                "lorem.txt",
                "Lorem.bin",
                "nijeFajl",
                "nePostoji.txt",
                "lorem.txt",
                "lorem.bin",
                "nePostoji.txt",
                "proba.bin",
                "proba.txt",
                "proba1.txt",
                "test.txt",
                "nesto.txt" ,
                "nekifajl.bin",
                "proba.bin"
            };

            Thread[] threads = new Thread[fileNames.Count];

            int i = 0;
            foreach (string fileName in fileNames)
            {

                threads[i] = new Thread(() => SendRequest(fileName));
                threads[i].Start();
                i++;
            }


            foreach(Thread thread in threads)
            {
                thread.Join();
            }
           
        }

        static void SendRequest(string fileName)
        {
            string url = $"http://localhost:5050/{fileName}";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = client.GetAsync(url).Result;
                    response.EnsureSuccessStatusCode();

                    string responseBody = response.Content.ReadAsStringAsync().Result;
                    Console.WriteLine($"Odgovor servera za fajl {fileName}:");
                    Console.WriteLine(responseBody);
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine($"Došlo je do greške prilikom slanja zahteva za fajl {fileName}: {ex.Message}");
                }
            }
        }

    }
}