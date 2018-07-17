using System;
using LeanCloud.Engine;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;


namespace WebHosting.UseCloud.Minimum
{
    class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
        WebHost.CreateDefaultBuilder(args)
            .UseCloud((cloud) => 
            {
                cloud.UseLog();
                cloud.UseHttpsRedirect();
            }).Build();

        public static void ConfigAppCloud(IApplicationBuilder app, Cloud cloud)
        {
            app.UseLog();
            app.UseHttpsRedirect();
        }
    }
}
