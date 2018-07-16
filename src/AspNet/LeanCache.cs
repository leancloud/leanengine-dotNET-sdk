using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace LeanCloud.Engine
{
    /// <summary>
    /// Lean cache.
    /// </summary>
    public class LeanCache
    {
        /// <summary>
        /// Gets or sets the name of the instance.
        /// </summary>
        /// <value>The name of the instance.</value>
        public string InstanceName { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:LeanCloud.Engine.LeanCache"/> class.
        /// </summary>
        /// <param name="instanceName">Instance name.</param>
        public LeanCache(string instanceName)
        {
            InstanceName = instanceName;
            currentConfigurations = GetLeanCacheConfigurations(InstanceName);
        }

        private Configurations currentConfigurations;
        /// <summary>
        /// Gets or sets the current configurations.
        /// </summary>
        /// <value>The current configurations.</value>
        public Configurations CurrentConfigurations
        {
            get => currentConfigurations;
            internal set => currentConfigurations = value;
        }

        /// <summary>
        /// Configurations.
        /// </summary>
        public struct Configurations
        {
            /// <summary>
            /// Connection string
            /// </summary>
            public string ConnectionString { get; set; }
            /// <summary>
            /// password 
            /// </summary>
            public string Password { get; set; }
            /// <summary>
            /// host
            /// </summary>
            public string Host { get; set; }
            /// <summary>
            /// port number
            /// </summary>
            public int Port { get; set; }
            /// <summary>
            /// instance name.
            /// </summary>
            public string InstanceName { get; set; }
        }

        /// <summary>
        /// Gets the lean cache redis connection string.
        /// </summary>
        /// <returns>The lean cache redis connection string.</returns>
        /// <param name="instanceName">Instance name.</param>
        public string GetLeanCacheRedisConnectionString(string instanceName)
        {
            return Environment.GetEnvironmentVariable($"REDIS_URL_{instanceName}");
        }

        /// <summary>
        /// Gets the lean cache configurations.
        /// </summary>
        /// <returns>The lean cache configurations.</returns>
        /// <param name="instanceName">Instance name.</param>
        public Configurations GetLeanCacheConfigurations(string instanceName)
        {
            var connectionString = GetLeanCacheRedisConnectionString(instanceName);
            var disassembleStrArray = connectionString.Split(':');
            var passwordAndEndPoint = disassembleStrArray[2].Split('@');
            var password = passwordAndEndPoint[0];
            var host = passwordAndEndPoint[1];
            var port = int.Parse(disassembleStrArray[3]);
            return new Configurations()
            {
                Host = host,
                Port = port,
                Password = password,
                ConnectionString = connectionString,
                InstanceName = this.InstanceName
            };
        }

        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <returns>The connection.</returns>
        public IConnectionMultiplexer GetConnection()
        {
            var redisConfiguration = this.CurrentConfigurations;

            ConfigurationOptions config = new ConfigurationOptions
            {
                ServiceName = this.InstanceName,
                ClientName = this.InstanceName,
                EndPoints =
                    {
                        {
                            redisConfiguration.Host, redisConfiguration.Port
                        },
                    },
                KeepAlive = 180,
                DefaultVersion = new Version(2, 8, 8),
                Password = redisConfiguration.Password,
                AbortOnConnectFail = false,
                ConnectRetry = 3,
            };

            var conn = ConnectionMultiplexer.Connect(config);
            return conn;
        }
    }

    /// <summary>
    /// Lean cache middleware.
    /// </summary>
    public static class LeanCacheMiddleware
    {
        /// <summary>
        /// Uses the lean cache.
        /// </summary>
        /// <returns>The lean cache.</returns>
        /// <param name="services">Services.</param>
        /// <param name="instanceName">Instance name.</param>
        public static IServiceCollection UseLeanCache(this IServiceCollection services, string instanceName)
        {
            return services.UseLeanCaches(new string[] { instanceName });
        }

        /// <summary>
        /// Uses the lean caches.
        /// </summary>
        /// <returns>The lean caches.</returns>
        /// <param name="services">Services.</param>
        /// <param name="instanceNames">Instance names.</param>
        public static IServiceCollection UseLeanCaches(this IServiceCollection services, IEnumerable<string> instanceNames)
        {
            if (instanceNames == null) return services;
            if (!instanceNames.Any()) return services;

            foreach (var instanceName in instanceNames)
            {
                var cache = new LeanCache(instanceName);

                services.AddTransient<LeanCache>(serviceProvider => cache);
            }

            services.AddTransient<Func<string, LeanCache>>(serviceProvider => name =>
            {
                var caches = serviceProvider.GetServices<LeanCache>();
                return caches.FirstOrDefault(cache => cache.InstanceName == name);
            });

            return services;
        }
    }
}
