using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Interfaces;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Providers;
using Orleans.Runtime;

namespace Grains
{
    [StorageProvider]
    public class HelloGrain: Grain<GreetingArchive>, IHello
    {
        private readonly ILogger<HelloGrain> _logger;
        public HelloGrain(ILogger<HelloGrain> logger)
        {
            _logger = logger;
        }
        public override Task OnActivateAsync()
        {
            Console.WriteLine($"On Activate is called");

            return base.OnActivateAsync();
        }

        public override Task OnDeactivateAsync()
        {
            Console.WriteLine($"On Deactivate is called");
            return base.OnDeactivateAsync();
        }

        public override void Participate(IGrainLifecycle lifecycle)
        {
            base.Participate(lifecycle);
            lifecycle.Subscribe(this.GetType().FullName, GrainLifecycleStage.SetupState, token => 
            { 
                Console.WriteLine("Setup State");
                return Task.CompletedTask;
            });
            lifecycle.Subscribe(this.GetType().FullName, GrainLifecycleStage.Activate, token =>
            {
                Console.WriteLine("Activate State");
                return Task.CompletedTask;
            });
            lifecycle.Subscribe(this.GetType().FullName, GrainLifecycleStage.First, token =>
            {
                Console.WriteLine("First State");
                return Task.CompletedTask;
            });
            lifecycle.Subscribe(this.GetType().FullName, GrainLifecycleStage.Last, token =>
            {
                Console.WriteLine("Last State");
                return Task.CompletedTask;
            });
        }

        public async Task<string> SayHello(string greeting)
        {
            State.Greetings.Add(greeting);

            await WriteStateAsync();

            var primaryKey = this.GetPrimaryKeyLong(out string keyExtension);

            this.DeactivateOnIdle();

            Console.WriteLine($"This is primary key: {primaryKey}: | keyExtension:{keyExtension}");

            var traceId = RequestContext.Get("traceId");

            return $"You said: {greeting}, I said 'Hello' !";
        }
    }

    public class GreetingArchive
    {
        public List<string> Greetings { get; private set; } = new List<string>();
    }
}