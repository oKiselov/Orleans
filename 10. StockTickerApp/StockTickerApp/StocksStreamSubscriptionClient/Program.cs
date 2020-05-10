using System;
using System.Threading.Tasks;
using Grains;
using Orleans;
using Orleans.Configuration;

namespace StocksStreamSubscriptionClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = InitClient().Result;
        }

        private static async Task<IClusterClient> InitClient()
        {
            var client = new ClientBuilder()
                .UseLocalhostClustering()
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "stock-stream-cluster";
                    options.ServiceId = "StockTickerApp";
                })
                .ConfigureApplicationParts(parts =>
                    parts.AddApplicationPart(typeof(StocksStreamingGrain).Assembly).WithReferences())
                .Build();

            await client.Connect();

            Console.WriteLine("Client successfully connected to SiloHost!!!");

            return client;
        }
    }
}
