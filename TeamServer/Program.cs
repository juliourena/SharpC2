using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TeamServer.Controllers;
using TeamServer.Listeners;

namespace TeamServer
{
    public class Program
    {
        public static ServerController ServerController { get; private set; }

        public static void Main(string[] args)
        {
            var pass = args.Length > 0 ? args[0] : string.Empty;
            StartTeamServer(pass);
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        public static void StartTeamServer(string pass = "")
        {
            if (string.IsNullOrEmpty(pass))
            {
                throw new ArgumentException("Password cannot be null");
            }

            AuthenticationController.SetPassword(pass);

            ServerController = new ServerController();
        }
    }
}
