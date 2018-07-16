using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using LeanCloud.Core.Internal;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.Extensions.Configuration;
using LeanCloud.Storage.Internal;

namespace LeanCloud.Engine
{
    /// <summary>
    /// Middleware options.
    /// </summary>
    public struct MiddlewareOptions
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:LeanCloud.Engine.MiddlewareOptions"/> trust proxy.
        /// </summary>
        /// <value><c>true</c> if trust proxy; otherwise, <c>false</c>.</value>
        public bool TrustProxy { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:LeanCloud.Engine.MiddlewareOptions"/> https redirect.
        /// </summary>
        /// <value><c>true</c> if https redirect; otherwise, <c>false</c>.</value>
        public bool HttpsRedirect { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:LeanCloud.Engine.MiddlewareOptions"/> is log.
        /// </summary>
        /// <value><c>true</c> if log; otherwise, <c>false</c>.</value>
        public bool Log { get; set; }
    }

    /// <summary>
    /// Engine ASP net middleware.
    /// </summary>
    public static class EngineAspNetMiddleware
    {
        /// <summary>
        /// Gets or sets the hosting environment.
        /// </summary>
        /// <value>The hosting environment.</value>
        public static IHostingEnvironment HostingEnvironment { get; set; }


        public static IWebHostBuilder BuildWebHost(IWebHostBuilder webHostBuilder, Cloud cloud, Action<IApplicationBuilder> configApp = null)
        {
            webHostBuilder = webHostBuilder.ConfigureServices(services =>
                      {
                          services.AddRouting();
                      }).ConfigureAppConfiguration((hostingContext, config) =>
                      {
                          var env = hostingContext.HostingEnvironment;
                          HostingEnvironment = env;
                          config.AddEnvironmentVariables();

                      }).Configure(app =>
                      {
                          if (toggleLog)
                          {
                              app.UseLog();
                          }
                          app.UseCloud(cloud);
                          app.UseDefaultHomepage();
                          cloud.Start();
                          configApp?.Invoke(app);
                      }).ConfigureLogging((context, builder) =>
                      {
                          builder.AddFilter("Microsoft", LogLevel.Warning)
                                           .AddFilter("System", LogLevel.Warning)
                                           .AddFilter("NToastNotify", LogLevel.Warning)
                                           .AddConsole();
                      }).UseKestrel();

            if (cloud.IsDevelopment)
            {
                webHostBuilder = webHostBuilder.UseLocalHost();
            }
            return webHostBuilder;
        }

        /// <summary>
        /// Uses the local host with port.
        /// </summary>
        /// <returns>The local host.</returns>
        /// <param name="webHostBuilder">Web host builder.</param>
        /// <param name="port">Port.</param>
        public static IWebHostBuilder UseLocalHost(this IWebHostBuilder webHostBuilder)
        {
            webHostBuilder.UseUrls(GetLocalHostUrl());
            return webHostBuilder;
        }

        public static string GetLocalHostUrl(UInt16 port = 3000)
        {
            return $"http://localhost:{port}";
        }

        /// <summary>
        /// Start the cloud with args.
        /// </summary>
        /// <returns>The start.</returns>
        /// <param name="cloud">Cloud.</param>
        /// <param name="args">Arguments.</param>
        public static Cloud Start(this Cloud cloud, string[] args)
        {
            var webHostBuilder = WebHost.CreateDefaultBuilder(args).ConfigureAppConfiguration((hostingContext, config) =>
            {
                if (args != null)
                {
                    config.AddCommandLine(args);
                }
            });
            return cloud.Start(webHostBuilder);
        }

        #region webhost builder extensions

        /// <summary>
        /// Start the specified cloud and webHostBuilder.
        /// </summary>
        /// <returns>The start.</returns>
        /// <param name="cloud">Cloud.</param>
        /// <param name="webHostBuilder">Web host builder.</param>
        public static Cloud Start(this Cloud cloud, IWebHostBuilder webHostBuilder)
        {
            webHostBuilder = BuildWebHost(webHostBuilder, cloud);
            webHostBuilder.Build().Run();
            return cloud;
        }

        public static IWebHostBuilder UseCloud(this IWebHostBuilder webHostBuilder, Cloud cloud, Action<IApplicationBuilder, Cloud> configureAppCloud)
        {
            webHostBuilder = BuildWebHost(webHostBuilder, cloud, (app) =>
            {
                configureAppCloud(app, cloud);
            });
            return webHostBuilder;
        }

        public static IWebHostBuilder UseCloud(this IWebHostBuilder webHostBuilder, Action<IApplicationBuilder, Cloud> configureAppCloud)
        {
            return webHostBuilder.UseCloud(new Cloud(), configureAppCloud);
        }

        public static IWebHostBuilder UseCloud(this IWebHostBuilder webHostBuilder,  Action<IApplicationBuilder> configureApp)
        {
            return webHostBuilder.UseCloud(new Cloud(), configureApp); ;
        }

        public static IWebHostBuilder UseCloud(this IWebHostBuilder webHostBuilder, Cloud cloud, Action<IApplicationBuilder> configureApp)
        {
            webHostBuilder = BuildWebHost(webHostBuilder, cloud, configureApp);
            return webHostBuilder;
        }

        /// <summary>
        /// Configs the cloud.
        /// </summary>
        /// <returns>The cloud.</returns>
        /// <param name="webHostBuilder">Web host builder.</param>
        /// <param name="cloud">Cloud.</param>
        public static IWebHostBuilder UseCloud(this IWebHostBuilder webHostBuilder, Cloud cloud)
        {
            return webHostBuilder.UseCloud(cloud, (app) => { });
        }

        /// <summary>
        /// Uses the cloud.
        /// </summary>
        /// <returns>The cloud.</returns>
        /// <param name="webHostBuilder">Web host builder.</param>
        public static IWebHostBuilder UseCloud(this IWebHostBuilder webHostBuilder)
        {
            return webHostBuilder.UseCloud(new Cloud());
        }

        #endregion

        internal static Cloud hostingCloud;

        #region application builder extensions

        /// <summary>
        /// Uses the default homepage.
        /// </summary>
        /// <returns>The default homepage.</returns>
        /// <param name="app">App.</param>
        public static IApplicationBuilder UseDefaultHomepage(this IApplicationBuilder app)
        {
            var routeBuilder = new RouteBuilder(app);

            routeBuilder.MapGet("/", async context =>
            {
                await context.Response.WriteAsync("Hello, dotnet is running on LeanEngine now.");
            });
            var routes = routeBuilder.Build();
            app.UseRouter(routes);
            return app;
        }

        /// <summary>
        /// Uses the cloud.
        /// </summary>
        /// <returns>The cloud.</returns>
        /// <param name="app">App.</param>
        /// <param name="cloud">Cloud.</param>
        public static IApplicationBuilder UseCloud(this IApplicationBuilder app, Cloud cloud)
        {
            hostingCloud = cloud;

            var routeBuilder = new RouteBuilder(app);

            app.Use(async (context, next) =>
            {
                if (context.IsHookRequest())
                {
                    var valid = context.ValidFromApi();
                    if (!valid)
                    {
                        var exception = new EngineException($"Hook key check failed, request from ${ context.Request.HttpContext.Connection.RemoteIpAddress}")
                        {
                            ErrorCode = 401,
                        };
                        await context.Response.WriteAsync(exception);
                    }
                    else
                    {
                        await next();
                    }
                }
                else
                {
                    await next();
                }
            });

            routeBuilder.MapGet("/__engine/{version}/ping", Pong);
            routeBuilder.MapPost("/{version}/functions/{className}/{hookName}", InvokeClassHook);
            routeBuilder.MapPost("/{version}/functions/onVerified/{verifiedField}", InvokeVerifyHook);
            routeBuilder.MapPost("/{version}/functions/_User/{action}", InvokeUserHook);
            routeBuilder.MapGet("/{version}/functions/_ops/metadatas", ResponseFunctionList);
            routeBuilder.MapPost("/{version}/{funcOrRpc}/{funcName}", ResponseCloudFunction);

            var routes = routeBuilder.Build();
            app.UseRouter(routes);

            return app;
        }

        /// <summary>
        /// Uses the cloud.
        /// </summary>
        /// <returns>The cloud.</returns>
        /// <param name="app">App.</param>
        public static IApplicationBuilder UseCloud(this IApplicationBuilder app)
        {
            var cloud = new Cloud();
            app.UseCloud(cloud);
            cloud.Start();
            return app;
        }

        /// <summary>
        /// Uses the log.
        /// </summary>
        /// <returns>The log.</returns>
        /// <param name="app">App.</param>
        public static IApplicationBuilder UseLog(this IApplicationBuilder app)
        {
            app.UseMiddleware<RequestResponseLoggingMiddleware>();
            return app;
        }

        internal static bool UsedHttpsRedirect { get; set; }
        /// <summary>
        /// Uses the https redirect.
        /// </summary>
        /// <returns>The https redirect.</returns>
        /// <param name="app">App.</param>
        public static IApplicationBuilder UseHttpsRedirect(this IApplicationBuilder app)
        {
            app.TrustProxy();
            UsedHttpsRedirect = true;
            app.UseMiddleware<EnforceHttpsMiddleware>();
            return app;
        }

        internal static bool ProxyTrusted { get; set; }
        /// <summary>
        /// trust internal proxy http request.
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder TrustProxy(this IApplicationBuilder app)
        {
            ProxyTrusted = true;
            return app;
        }

        #endregion

        private static bool toggleLog { get; set; }

        /// <summary>
        /// Uses the log.
        /// </summary>
        /// <returns>The log.</returns>
        /// <param name="cloud">Cloud.</param>
        public static Cloud UseLog(this Cloud cloud)
        {
            toggleLog = true;
            return cloud;
        }

        #region get hosting sub-domain with schema
        /// <summary>
        /// get hosting url,eg https://dotnetweb.leanapp.cn
        /// </summary>
        /// <param name="cloud"></param>
        /// <returns></returns>
        public static string GetHostingUrl(this Cloud cloud)
        {
            if (cloud.IsDevelopment)
            {
                return GetLocalHostUrl();
            }
            var regionMainDomianKv = new Dictionary<string, string>()
            {
                { "cn", ".leanapp.cn" },
                { "us", "avosapps.us" }
            };
            var shcema = UsedHttpsRedirect ? "https" : "http";
            var region = cloud.GetLeanEnv(Cloud.LeanEnvKey.LEANCLOUD_REGION);
            var mainDomain = regionMainDomianKv[region.ToLower()];
            var subDomain = cloud.GetLeanEnv(Cloud.LeanEnvKey.LEANCLOUD_APP_DOMAIN);

            return $"{shcema}://{subDomain}{mainDomain}";
        }
        #endregion

        private static string HookKey
        {
            get => Environment.GetEnvironmentVariable("LEANCLOUD_APP_HOOK_KEY");
        }

        internal static string GetRequestHeader(this HttpContext context, string key)
        {
            foreach (var kv in context.Request.Headers)
            {
                if (kv.Key.ToLower().Equals(key.ToLower()))
                {
                    return kv.Value;
                }
            }
            return null;
        }

        private static bool ValidFromApi(this HttpContext context)
        {
            if (hostingCloud.IsDevelopment) return true;
            var hookKeyKeyInLower = "x-lc-hook-key";
            foreach (var kv in context.Request.Headers)
            {
                if (kv.Key.ToLower().Equals(hookKeyKeyInLower))
                {
                    if (kv.Value == HookKey)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static bool Match(string routeTemplate, string requestPath)
        {
            var template = TemplateParser.Parse(routeTemplate);
            var matcher = new TemplateMatcher(template, GetDefaults(template));
            var values = new RouteValueDictionary();
            var moduleMatch = matcher.TryMatch(requestPath, values);
            return moduleMatch;
        }

        // This method extracts the default argument values from the template.
        private static RouteValueDictionary GetDefaults(RouteTemplate parsedTemplate)
        {
            var result = new RouteValueDictionary();

            foreach (var parameter in parsedTemplate.Parameters)
            {
                if (parameter.DefaultValue != null)
                {
                    result.Add(parameter.Name, parameter.DefaultValue);
                }
            }

            return result;
        }

        private static bool IsHookRequest(this HttpContext context)
        {
            var isMetaData = Match("/{version}/functions/_ops/metadatas", context.Request.Path);
            if (isMetaData) return false;
            var isClassHook = Match("/{version}/functions/{className}/{hookName}", context.Request.Path);
            var isVerifyHook = Match("/{version}/functions/onVerified/{verifiedField}", context.Request.Path);
            var isUserHook = Match("/{version}/functions/_User/{action}", context.Request.Path);
            return isClassHook || isVerifyHook || isUserHook;
        }

        /// <summary>
        /// Writes the async.
        /// </summary>
        /// <returns>The async.</returns>
        /// <param name="response">Response.</param>
        /// <param name="json">Json.</param>
        public static Task WriteAsync(this HttpResponse response, IDictionary<string, object> json)
        {
            var encodedStr = JsonConvert.SerializeObject(json);
            return response.ContentTypeJson().WriteAsync(encodedStr);
        }

        /// <summary>
        /// Writes the async.
        /// </summary>
        /// <returns>The async.</returns>
        /// <param name="response">Response.</param>
        /// <param name="engineException">Engine exception.</param>
        public static Task WriteAsync(this HttpResponse response, EngineException engineException)
        {
            var data = new Dictionary<string, object>()
            {
                { "code" , engineException.ErrorCode },
                { "error", engineException.Message }
            };

            var encodedStr = JsonConvert.SerializeObject(data);

            if (engineException.ErrorCode > 400)
            {
                response.StatusCode = engineException.ErrorCode;
            }
            else
            {
                response.StatusCode = 400;
            }

            return response.ContentTypeJson().WriteAsync(encodedStr);
        }
        /// <summary>
        /// Writes the async.
        /// </summary>
        /// <returns>The async.</returns>
        /// <param name="response">Response.</param>
        /// <param name="theObject">The object.</param>
        public static Task WriteAsync(this HttpResponse response, AVObject theObject)
        {
            var encodedObj = theObject.Encode();
            var encodedStr = JsonConvert.SerializeObject(encodedObj);
            return response.ContentTypeJson().WriteAsync(encodedStr);
        }

        /// <summary>
        /// Writes the async.
        /// </summary>
        /// <returns>The async.</returns>
        /// <param name="response">Response.</param>
        /// <param name="theObjectList">The object list.</param>
        public static Task WriteAsync(this HttpResponse response, IEnumerable<AVObject> theObjectList)
        {
            var encodedObjs = theObjectList.Select(theObject => theObject.Encode());
            var encodedStr = JsonConvert.SerializeObject(encodedObjs);
            return response.ContentTypeJson().WriteAsync(encodedStr);
        }

        /// <summary>
        /// Pong the specified context.
        /// </summary>
        /// <returns>The pong.</returns>
        /// <param name="context">Context.</param>
        public static async Task Pong(HttpContext context)
        {
            var response = new Dictionary<string, object>()
            {
                { "runtime", "dotnet-core" },
                { "version", "0.1.0" }
            };
            await context.Response.WriteAsync(response);
        }

        /// <summary>
        /// Responses the cloud function.
        /// </summary>
        /// <returns>The cloud function.</returns>
        /// <param name="context">Context.</param>
        public static async Task ResponseCloudFunction(HttpContext context)
        {
            var funcOrRpc = context.GetRouteValue("funcOrRpc").ToString();
            var funcName = context.GetRouteValue("funcName").ToString();

            using (var reader = new StreamReader(context.Request.Body))
            {
                var body = reader.ReadToEnd();
                var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(body);

                var engineContext = new EngineFunctionContext
                {
                    FunctionName = funcName,
                    CallType = funcOrRpc,
                    FunctionParameters = data
                };
                try
                {
                    await hostingCloud.Invoke(funcName, funcOrRpc, engineContext);
                    var resultWrapper = new Dictionary<string, object>()
                    {
                        { "result", engineContext.Result }
                    };
                    var encodedStr = JsonConvert.SerializeObject(resultWrapper);
                    await context.Response.WriteAsync(encodedStr);
                }
                catch (Exception ex)
                {
                    if (ex is EngineException engineException)
                    {
                        await context.Response.WriteAsync(engineException);
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Responses the function list.
        /// </summary>
        /// <returns>The function list.</returns>
        /// <param name="context">Context.</param>
        public static async Task ResponseFunctionList(HttpContext context)
        {
            var functionNames = await hostingCloud.QueryCloudFunctionsMetaDataAsync();
            var resultWrapper = new Dictionary<string, object>()
             {
                { "result", functionNames }
             };
            await context.Response.WriteAsync(resultWrapper);
        }

        /// <summary>
        /// Invokes the class hook.
        /// </summary>
        /// <returns>The class hook.</returns>
        /// <param name="context">Context.</param>
        public static async Task InvokeClassHook(HttpContext context)
        {
            var className = context.GetRouteValue("className").ToString();
            var hookName = context.GetRouteValue("hookName").ToString();
            using (var reader = new StreamReader(context.Request.Body))
            {
                var body = reader.ReadToEnd();
                var data = Json.Parse(body) as IDictionary<string, object>;

                if (data["object"] is IDictionary<string, object> objectMetaData)
                {
                    objectMetaData.RemoveKeys(new string[]
                    {
                        "__before","__after"
                    });

                    List<string> updatedKeys = null;
                    if (objectMetaData.ContainsKey("_updatedKeys"))
                    {
                        if (objectMetaData["_updatedKeys"] is List<object> upKys)
                        {
                            updatedKeys = upKys.Select(k => k.ToString()).ToList();
                        }
                    }
                    var objectState = AVObjectCoder.Instance.Decode(objectMetaData, AVDecoder.Instance);
                    AVObject theObject = AVObject.FromState<AVObject>(objectState, className);

                    AVUser by = null;
                    if (data["user"] is IDictionary<string, object> userMetaData)
                    {
                        var userState = AVObjectCoder.Instance.Decode(userMetaData, AVDecoder.Instance);
                        by = AVObject.FromState<AVUser>(userState, "_User");
                    }

                    var engineContext = new EngineObjectHookContext
                    {
                        TheObject = theObject,
                        MetaBody = data,
                        By = by,
                        UpdatedKeys = updatedKeys
                    };
                    await hostingCloud.InvokeClassHook(className, hookName, engineContext);
                    await context.Response.WriteAsync(engineContext.TheObject);
                }
            }
        }

        /// <summary>
        /// Invokes the verify hook.
        /// </summary>
        /// <returns>The verify hook.</returns>
        /// <param name="context">Context.</param>
        public static async Task InvokeVerifyHook(HttpContext context)
        {
            var verifiedField = context.GetRouteValue("verifiedField").ToString();
            using (var reader = new StreamReader(context.Request.Body))
            {
                var body = reader.ReadToEnd();
                var data = Json.Parse(body) as IDictionary<string, object>;
                var objectState = AVObjectCoder.Instance.Decode(data, AVDecoder.Instance);
                var engineContext = new EngineUserVerifyHookContext()
                {
                    TheUser = AVObject.FromState<AVUser>(objectState, "_User")
                };
                var t = hostingCloud.Invoke(verifiedField, engineContext);
                await HandleResult(context, t);
            }
        }

        /// <summary>
        /// Invokes the user hook.
        /// </summary>
        /// <returns>The user hook.</returns>
        /// <param name="context">Context.</param>
        public static async Task InvokeUserHook(HttpContext context)
        {
            var action = context.GetRouteValue("action").ToString();
            using (var reader = new StreamReader(context.Request.Body))
            {
                var body = reader.ReadToEnd();
                var data = Json.Parse(body) as IDictionary<string, object>;
                var objectState = AVObjectCoder.Instance.Decode(data, AVDecoder.Instance);
                var engineContext = new EngineUserActionHookContext()
                {
                    TheUser = AVObject.FromState<AVUser>(objectState, "_User"),
                    Action = action
                };
                var t = hostingCloud.Invoke(action, engineContext);
                await HandleResult(context, t);
            }
        }

        /// <summary>
        /// Restores the user.
        /// </summary>
        /// <returns>The user.</returns>
        /// <param name="context">Context.</param>
        public static async Task<AVUser> RestoreUser(HttpContext context)
        {
            string sessionToken = string.Empty;
            string[] sessionKeys = new string[] { "X-LC-Session" };
            sessionKeys.ToList().ForEach(key =>
            {
                if (context.Request.Headers.ContainsKey(key))
                {
                    sessionToken = context.Request.Headers[key];
                }
            });
            if (!string.IsNullOrEmpty(sessionToken))
            {
                return await AVUser.BecomeAsync(sessionToken);
            }
            return null;
        }

        /// <summary>
        /// Contents the type json.
        /// </summary>
        /// <returns>The type json.</returns>
        /// <param name="response">Response.</param>
        public static HttpResponse ContentTypeJson(this HttpResponse response)
        {
            response.ContentType = "application/json; charset=UTF-8";
            return response;
        }

        private static IDictionary<K, V> RemoveKeys<K, V>(this IDictionary<K, V> source, IEnumerable<K> toRemoved)
        {
            foreach (var key in toRemoved)
            {
                if (source.ContainsKey(key))
                {
                    source.Remove(key);
                }
            }
            return source;
        }

        /// <summary>
        /// Handles the result.
        /// </summary>
        /// <returns>The result.</returns>
        /// <param name="context">Context.</param>
        /// <param name="task">Task.</param>
        public static async Task HandleResult(HttpContext context, Task task)
        {
            var message = string.Empty;
            var statusCode = 500;
            try
            {
                await task;
                await context.Response.WriteAsync("{}");
            }
            catch (Exception exception)
            {
                message = exception.Message;
                if (exception is EngineException engineException1)
                {
                    await context.Response.WriteAsync(engineException1);
                }
                else if (exception is AggregateException aggregateException)
                {
                    if (aggregateException != null)
                    {
                        if (aggregateException.InnerException != null)
                        {
                            if (aggregateException.InnerException is EngineException engineException2)
                            {
                                await context.Response.WriteAsync(engineException2);
                            }
                        }
                        if (aggregateException.InnerExceptions != null)
                        {
                            if (aggregateException.InnerExceptions.Count == 1)
                            {
                                if (aggregateException.InnerExceptions[0] != null)
                                {
                                    message = aggregateException.InnerExceptions[0].Message;
                                    if (aggregateException.InnerExceptions[0] is EngineException engineException3)
                                    {
                                        await context.Response.WriteAsync(engineException3);
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    context.Response.StatusCode = statusCode;
                    await context.Response.WriteAsync(message);
                }
            }
        }
    }
}
