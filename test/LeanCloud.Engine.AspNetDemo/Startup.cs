using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using LeanCloud.Engine;

namespace LeanCloud.Engine.AspNetDemo
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRouting();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            var routeBuilder = new RouteBuilder(app);
            routeBuilder.MapGet("/", async context =>
            {
                await context.Response.WriteAsync("hello, dotnet running on LeanEngine now.");
            });

            //

            //var myCloud = new MyCloud();
            //app.UseCloud((Cloud)myCloud);
            //myCloud.Start();
            var cloud = new Cloud();
            app.UseCloud(cloud);
            cloud.Start();
            var routes = routeBuilder.Build();
            app.UseRouter(routes);

            // PLEASE DO NOT call Cloud.UseClassHook any more, beacuse the hook has already regsterd by API server,
            // if you add new hook to the Cloud, it CAN NOT be found by API server.
        }
    }
}
