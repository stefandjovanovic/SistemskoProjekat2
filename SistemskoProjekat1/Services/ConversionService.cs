using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SistemskoProjekat2.Services
{
    internal class ConversionService
    {
        private string rootFolder;
        public ConversionService(string rootFolder)
        {
            this.rootFolder = rootFolder;
        }

        public async Task<string> GetRequest(HttpListenerRequest request, Cache.Cache cache)
        {

            string url = request.RawUrl ?? "";

            Console.WriteLine("\n--------------Primljen je zahtev: " + url + "\n");

            if (url == "/" || url == "")
            {
                Console.WriteLine("Nije zahtevan fajl");
                return "Niste zahtevali fajl";
            }
            string fileName = url.Split('/')[1];
            string path = Path.Combine(rootFolder, fileName);


            string? cacheData = cache.ReadCache(fileName);
            string data;

            if (cacheData != null)
            {
                Console.WriteLine($"Citanje podataka iz kesa za fajl {fileName}");
                return cacheData;
            }
            else
            {
                if (File.Exists(path))
                {
                    if (path.EndsWith(".txt"))
                    {
                        Console.WriteLine("Konverzija txt u bin");

                        data = await this.TransformTxtToBinAsync(path);
                        cache.WriteToCache(fileName, data);
                        return data;
                    }
                    else if (path.EndsWith(".bin"))
                    {
                        Console.WriteLine("Konverzija bin u txt");
                        data = await this.TransformBinToTxtAsync(path);
                        cache.WriteToCache(fileName, data);
                        return data;
                    }
                    else
                    {
                        Console.WriteLine("Nije unet format fajla");
                        data = "Nije unet format fajla";
                        cache.WriteToCache(fileName, data);
                        return data;
                    }
                }
                else
                {
                    Console.WriteLine("Fajl se ne nalazi u folderu");
                    data = "Ne postoji fajl u root folderu";
                    cache.WriteToCache(fileName, data);
                    return data;
                }
            }



        }

        private async Task<string> TransformTxtToBinAsync(string path)
        {
            byte[] bytes = await File.ReadAllBytesAsync(path);
            //upisemo u bin fajl
            string writePath = path.Replace(".txt", ".bin");
            await WriteToBin(writePath, bytes);

            string data = BitConverter.ToString(bytes).Replace("-", " ");
            return data;
        }
        private async Task<string> TransformBinToTxtAsync(string path)
        {
            byte[] bytes = await File.ReadAllBytesAsync(path);
            string data = System.Text.Encoding.Default.GetString(bytes);
            //upisemo u txt fajl
            string writePath = path.Replace(".bin", ".txt");
            await WriteToTxt(writePath, data);

            return data;
        }

        private async Task WriteToBin(string path, byte[] bytes)
        {

            if (!File.Exists(path))
            {
                await File.WriteAllBytesAsync(path, bytes);
            }

        }

        private async Task WriteToTxt(string path, string data)
        {
            if (!File.Exists(path))
            {
                await File.WriteAllTextAsync(path, data);
            }
        }
    }
}
