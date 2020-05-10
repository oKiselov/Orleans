using Interfaces;
using Orleans;

namespace Grains
{
    public class StocksStreamingGrain: Grain, IStocksStreamingGrain
    {
    }
}
