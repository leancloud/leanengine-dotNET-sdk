using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace LeanCloud.Engine
{
    /// <summary>
    /// Enforce https middleware.
    /// </summary>
    public class EnforceHttpsMiddleware
    {
        private readonly RequestDelegate _next;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:LeanCloud.Engine.EnforceHttpsMiddleware"/> class.
        /// </summary>
        /// <param name="next">Next.</param>
        public EnforceHttpsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// Invoke the specified context.
        /// </summary>
        /// <returns>The invoke.</returns>
        /// <param name="context">Context.</param>
        public async Task Invoke(HttpContext context)
        {
            Console.WriteLine($"{context.Request.Scheme}://{context.Request.Host}{context.Request.Path} {context.Request.IsHttps}");
            if ((EngineAspNetMiddleware.hostingCloud.IsProduction || context.Request.Host.Value.EndsWith(".leanapp.cn", StringComparison.Ordinal)) && !context.Request.IsHttps)
            {
                var url = $"https://{context.Request.Host}{context.Request.Path}";
                context.Response.StatusCode = 302;
                context.Response.Headers.Add("Location", url);

                await context.Response.WriteAsync($"Found. Redirecting to ${url}");
            }
            else
            {
                await _next(context);
            }
        }
    }
}
