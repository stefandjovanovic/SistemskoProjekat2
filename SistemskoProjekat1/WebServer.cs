using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SistemskoProjekat2
{
    public class WebServer
    {
        private readonly HttpListener listener = new HttpListener();
        private readonly Func<HttpListenerRequest, Cache.Cache, Task<string>> responderMethod;
        private Cache.Cache cache;


        public WebServer(Func<HttpListenerRequest, Cache.Cache, Task<string>> responderMethod, int CacheCap, params string[] prefixes)
        {
            if (prefixes == null || prefixes.Length == 0)
            {
                throw new ArgumentException("URI ne sadrzi adekvatan broj parametara");
            }

            if (responderMethod == null)
            {
                throw new ArgumentException("Potreban je odgovarajuci responderMethod");
            }

            foreach (string prefix in prefixes)
            {
                listener.Prefixes.Add(prefix);
            }

            cache = new Cache.Cache(CacheCap);

            this.responderMethod = responderMethod;

            listener.Start();
        }

        public void Run()
        {
            Task.Run( async () =>
            {
                Console.WriteLine("Webserver je pokrenut...");
                try
                {
                    while (listener.IsListening)
                    {
                            HttpListenerContext context = await listener.GetContextAsync();
                            try
                            {
                                if (context == null)
                                {
                                    return;
                                }

                                HttpListenerRequest request = context.Request;
                                HttpListenerResponse response = context.Response;


                                if (request.RawUrl == "/favicon.ico")
                                {
                                    return;
                                }

                                Stopwatch stopwatch = new Stopwatch();

                                // Begin timing
                                stopwatch.Start();

                                string rstr = await responderMethod(request, this.cache);
                                stopwatch.Stop();

                                // Write result
                                Console.WriteLine("Proteklo vreme: " + stopwatch.Elapsed.TotalMilliseconds);

                                byte[] buf = Encoding.UTF8.GetBytes(rstr);
                                response.ContentLength64 = buf.Length;
                                response.OutputStream.Write(buf, 0, buf.Length);


                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error: {ex}");
                            }
                            finally
                            {
                                if (context != null)
                                {
                                    context.Response.OutputStream.Close();
                                }
                            }

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex}");
                }
            });
        }

        public void Stop()
        {
            listener.Stop();
            listener.Close();
        }


    }
}
