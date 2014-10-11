using Microsoft.Owin.Hosting;
using System;

namespace WaveDev.ModelR.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var url = "http://localhost:8082";

            using (WebApp.Start(url))
            {
                Console.WriteLine("\n[{0}] ModelR server listening on {1}.", DateTime.Now.ToString("dd-mm-yyyy hh:MM:ss"), url);
                Console.ReadKey();
            }
        }
    }
}
