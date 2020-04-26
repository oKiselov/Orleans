﻿using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Grains;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;

namespace Silo.Host
{
    public class HostProgram
    {
        public static async Task<int> Main(string[] args)
        {
            return await RunSilo();
        }

        private static async Task<int> RunSilo()
        {
            try
            {
                await StartSilo();
                Console.WriteLine("Silo Started");

                Console.WriteLine("Press enter to terminate");
                Console.ReadLine();

                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return -1;
            }
        }

        private static async Task<ISiloHost> StartSilo()
        {
            var config = LoadConfig();
            var orleansConfig = GetOrleansConfig(config);

            var builder = new SiloHostBuilder()

                // Clustering information
                .Configure<ClusterOptions>(options =>
                {
                    // unique ID for the orleans cluster
                    options.ClusterId = "dev";
                    // unique Id for our app
                    // not change across deployment
                    options.ServiceId = "HelloApp";
                })

                // Clustering provider 
                .UseLocalhostClustering()
                .UseDashboard()
                // Endpoints
                .Configure<EndpointOptions>(options =>
                {
                    // silo to silo
                    options.SiloPort = 11111;
                    // silo to client at the same cluster 
                    options.GatewayPort = 30000;
                    options.AdvertisedIPAddress = IPAddress.Loopback;
                })

                .AddAdoNetGrainStorageAsDefault(options =>
                {
                    options.Invariant = orleansConfig.Invariant;
                    options.ConnectionString =orleansConfig.ConnectionString;
                    options.UseJsonFormat = true;
                })
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(HelloGrain).Assembly).WithReferences())
                .ConfigureLogging(logging =>
                    logging
                        .AddFilter("Microsoft", LogLevel.Warning)
                        .AddFilter("System", LogLevel.Warning));

            var host = builder.Build();
            await host.StartAsync();
            return host;
        }

        private static IConfigurationRoot LoadConfig()
        {
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("appsettings.json");
            var config = configurationBuilder.Build();
            return config;
        }

        private static OrleansConfig GetOrleansConfig(IConfigurationRoot configurationRoot)
        {
            var orleansConfig = new OrleansConfig();
            var section = configurationRoot.GetSection("OrleansConfiguration");
            section.Bind(orleansConfig);
            return orleansConfig;
        }
    }

    public class OrleansConfig
    {
        public string Invariant { get; set; }
        public string ConnectionString { get; set; }
    }
}
