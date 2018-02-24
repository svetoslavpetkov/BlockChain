using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Node
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            int port = 5555;

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "-p" && i+1 <args.Length)
                {
                    port = int.Parse(args[i + 1]);
                }
            }

            return WebHost.CreateDefaultBuilder(new string[] { })
                .UseStartup<Startup>()
                .UseUrls($"http://localhost:{port}/")
                .Build();
        }
    }
}
