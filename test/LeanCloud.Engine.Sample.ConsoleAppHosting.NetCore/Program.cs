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
            Cloud cloud = new Cloud().BeforeSave("Todo", todo =>
            {
                var title = todo.Get<string>("title");
                // reset value for title
                if (!string.IsNullOrEmpty(title))
                    if (title.Length > 20) todo["title"] = title.Substring(0, 20);
                // returning any value will be ok.
                return Task.FromResult(true);
            }).UseLog();

            var webHostBuilder = WebHost.CreateDefaultBuilder(args)
                                        .UseStartup<Startup>()
                                        .UseCloud(cloud);

            return webHostBuilder.Build();
        }

    }
}
