using System;
using LeanCloud.Engine;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace WebHosting.Minimum
{
    class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
        WebHost.CreateDefaultBuilder(args)
               .UseCloud()
               .UseLog()
               .TrustProxy()
               .UseHttpsRedirect()
               .Build();
    }
}
