﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace LeanCloud.Engine
{
    /// <summary>
    /// Request response logging middleware.
    /// </summary>
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:LeanCloud.Engine.RequestResponseLoggingMiddleware"/> class.
        /// </summary>
        /// <param name="next">Next.</param>
        public RequestResponseLoggingMiddleware(RequestDelegate next)
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
            Console.WriteLine(await FormatRequest(context.Request));

            Stream originalBody = context.Response.Body;

            try
            {
                using (var memStream = new MemoryStream())
                {
                    context.Response.Body = memStream;

                    await _next(context);
                    memStream.Position = 0;
                    string responseBody = new StreamReader(memStream).ReadToEnd();
                    Console.WriteLine($"{context.Response.StatusCode} {responseBody}");
                    memStream.Position = 0;
                    await memStream.CopyToAsync(originalBody);
                }
            }
            catch (Exception logEx)
            {

            }
            finally
            {
                context.Response.Body = originalBody;
            }
        }

        /// <summary>
        /// print request to Console.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task PrintRequest(HttpContext context)
        {
            Console.WriteLine(await FormatRequest(context.Request));
        }

        private async Task<string> FormatRequest(HttpRequest request)
        {
            using (var bodyStreamReader = new StreamReader(request.Body))
            {
                var bodyAsText = await bodyStreamReader.ReadToEndAsync();
                var requestData = Encoding.UTF8.GetBytes(bodyAsText);
                var stream = new MemoryStream(requestData);
                request.Body = stream;

                return $"{request.Method} {request.Scheme}://{request.Host}{request.Path} {request.QueryString} {FormatRequestHeaders(request)} {bodyAsText}";
            }
        }

        private async Task<string> FormatResponse(HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            var text = await new StreamReader(response.Body).ReadToEndAsync();
            response.Body.Seek(0, SeekOrigin.Begin);

            return $"{response.StatusCode} {text}";
        }

        private string FormatRequestHeaders(HttpRequest request)
        {
            var data = new Dictionary<string, object>();
            foreach (var header in request.Headers)
            {
                data.Add(header.Key, header.Value);
            }
            return JsonConvert.SerializeObject(data);
        }
    }
}
