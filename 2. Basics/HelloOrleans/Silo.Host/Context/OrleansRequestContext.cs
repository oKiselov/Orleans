using System;
using Orleans.Runtime;

namespace Silo.Host.Context
{
    public class OrleansRequestContext: IOrleansRequestContext
    {
        public Guid TraceId => RequestContext.Get("traceId") == null ? Guid.Empty : (Guid)RequestContext.Get("traceId");
    }

    public interface IOrleansRequestContext
    {
        Guid TraceId { get; }
    }
}