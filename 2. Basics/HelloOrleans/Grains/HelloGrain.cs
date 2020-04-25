using System;
using System.Threading.Tasks;
using Interfaces;
using Orleans;
using Orleans.Runtime;

namespace Grains
{
    public class HelloGrain: Grain, IHello
    {
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

        public Task<string> SayHello(string greeting)
        {
            var primaryKey = this.GetPrimaryKeyLong(out string keyExtension);

            this.DeactivateOnIdle();

            Console.WriteLine($"This is primary key: {primaryKey}: | keyExtension:{keyExtension}");

            return Task.FromResult($"You said: {greeting}, I said 'Hello' !");
        }
    }
}