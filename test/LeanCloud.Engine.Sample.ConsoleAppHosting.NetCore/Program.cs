using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace LeanCloud.Engine.Sample.ConsoleAppHosting.NetCore
{
    class Program
    {
        static void Main(string[] args)
        {
            AVClient.Configuration configuration = new AVClient.Configuration()
            {
                ApplicationId = "315XFAYyIGPbd98vHPCBnLre-9Nh9j0Va",
                ApplicationKey = "Y04sM6TzhMSBmCMkwfI3FpHc",
                MasterKey = "Ep3IHWFqi41DMm44T49lKy07"
            };

            AVClient.Initialize(configuration);

            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            Cloud cloud = new Cloud().UseHookClass<TodoHook>().UseLog();

            var webHostBuilder = WebHost.CreateDefaultBuilder(args)
                                        .UseStartup<Startup>()
                                        .UseCloud(cloud);

            return webHostBuilder.Build();
        }

    }
}
