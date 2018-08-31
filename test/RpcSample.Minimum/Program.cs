using System;
using LeanCloud;
using LeanCloud.Engine;
using LeanCloud.Engine.Rpc;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace RpcSample.Minimum
{
    class Program
    {
        static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                   .UseCloud(ConfigCloud, ConfigAppCloud)
                   .Build();

        public static void ConfigCloud(Cloud cloud)
        {
            AVClient.Initialize("JRsk29cDwQNGHaIM2PM0VBWt-9Nh9j0Va", "uerPuKWcaSqHGBYVbPBYcv6V");
            AVClient.HttpLog(Console.WriteLine);
            cloud.UseLog();
            cloud.UseHttpsRedirect();
        }

        public static void ConfigAppCloud(IApplicationBuilder app, Cloud cloud)
        {
            var rpcServer = new StorageServer();
            app.UseRpcServer(cloud, rpcServer, "/rpc");
        }
    }
}
