using System;
using System.Threading.Tasks;
using Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Runtime;
using Orleans.Runtime.Messaging;
using Polly;

namespace Client
{
    public class ClientProgram
    {
        static int Main(string[] args)
        {
            return RunMainAsync().Result;
        }

        private static async Task<int> RunMainAsync()
        {
            try
            {
                using (var client = StartClient())
                {
                    var grain = client.GetGrain<IGreetingsGrain>(0);
                    await grain.SendGreetings("Good Morning");

                    var grain1 = client.GetGrain<IGreetingsGrain>(0);
                    await grain1.SendGreetings("Good afternoon");

                    var grain2 = client.GetGrain<IGreetingsGrain>(0);
                    await grain2.SendGreetings("Good evening");

                    Console.ReadKey();
                }

                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return -1;
            }
        }

        static IClusterClient StartClient()
        {
            return Policy<IClusterClient>
                .Handle<SiloUnavailableException>()
                .Or<OrleansMessageRejectionException>()
                .Or<ConnectionFailedException>(exception =>
                {
                    Console.WriteLine(exception.Source + exception.Message);
                    return true;
                })
                .WaitAndRetry(new[] { TimeSpan.FromSeconds(5)})
                .Execute(() =>
                    {
                        var client = new ClientBuilder()
                            .UseLocalhostClustering()
                            .Configure<ClusterOptions>(options =>
                            {
                                options.ClusterId = "dev";
                                options.ServiceId = "HelloApp";
                            })
                            .Build();

                        client.Connect().GetAwaiter().GetResult();
                        Console.WriteLine("Client connected");

                        return client;
                    }
                );
        }
    }
}
