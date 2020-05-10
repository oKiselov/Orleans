using System;
using System.Threading.Tasks;
using Grains;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;

namespace SiloHost
{
    class Program
    {
        static int Main(string[] args)
        {
            return RunSilo().Result;
        }

        private static async Task<int> RunSilo()
        {
            try
            {
                var host = await StartSilo();

                Console.WriteLine("Press enter to terminate Silo...");
                Console.ReadLine();

                await host.StopAsync();

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
            var silo = new SiloHostBuilder()
                .UseLocalhostClustering()
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "stock-stream-cluster";
                    options.ServiceId = "StockTickerApp";
                })
                .ConfigureEndpoints(siloPort: 11111, gatewayPort: 30000)
                .ConfigureApplicationParts(parts =>
                    parts.AddApplicationPart(typeof(StocksStreamingGrain).Assembly).WithReferences())
                .ConfigureLogging(logging =>
                    logging
                        .AddFilter("Microsoft", LogLevel.Warning)
                        .AddFilter("System", LogLevel.Warning)
                        .AddConsole())
                .Build();

            await silo.StartAsync();

            return silo;
        }
    }
}
