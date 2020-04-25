using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Orleans.Configuration;
using Orleans.Hosting;

namespace Silo.Host
{
    public class HostProgram
    {
        public static async Task<int> Main(string[] args)
        {
            Thread.Sleep(3000);
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

                // Endpoints
                .Configure<EndpointOptions>(options =>
                {
                    // silo to silo
                    options.SiloPort = 11111;
                    // silo to client at the same cluster 
                    options.GatewayPort = 30000;
                    options.AdvertisedIPAddress = IPAddress.Loopback;
                });

            var host = builder.Build();
            await host.StartAsync();
            return host;

        }
    }
}
