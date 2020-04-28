using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Orleans;
using Silo.Host.Context;

namespace Silo.Host.Filters
{
    public class LoggingFilter : IIncomingGrainCallFilter
    {
        private readonly GrainInfo _grainInfo;
        private readonly ILogger<LoggingFilter> _logger;
        private readonly JsonSerializerSettings _serializerSettings;
        private readonly IOrleansRequestContext _orleansRequestContext;

        public LoggingFilter(GrainInfo grainInfo, ILogger<LoggingFilter> logger, JsonSerializerSettings jsonSerializerSettings, IOrleansRequestContext orleansRequestContext)
        {
            _grainInfo = grainInfo;
            _logger = logger;
            _serializerSettings = jsonSerializerSettings;
            _orleansRequestContext = orleansRequestContext;
        }

        public async Task Invoke(IIncomingGrainCallContext context)
        {
            try
            {
                if (ShouldLog(context.InterfaceMethod.Name))
                {
                    var args = JsonConvert.SerializeObject(context.Arguments, _serializerSettings);
                    _logger.LogInformation($"LOGGINGFILTER TraceId: {_orleansRequestContext.TraceId} {context.Grain.GetType()}.{context.InterfaceMethod.Name}: arguments: {args} REQUEST");
                }


                // triggers called method
                await context.Invoke();

                if (ShouldLog(context.InterfaceMethod.Name))
                {
                    var result = JsonConvert.SerializeObject(context.Arguments, _serializerSettings);
                    _logger.LogInformation($"LOGGINGFILTER TraceId: {_orleansRequestContext.TraceId} {context.Grain.GetType()}.{context.InterfaceMethod.Name}: result: {result} RESULT");
                }
            }
            catch (Exception e)
            {
                var args = JsonConvert.SerializeObject(context.Arguments, _serializerSettings);
                var result = JsonConvert.SerializeObject(context.Arguments, _serializerSettings);

                _logger.LogError($"LOGGINGFILTER TraceId: {_orleansRequestContext.TraceId} {context.Grain.GetType()}.{context.InterfaceMethod.Name}: threw an exception: {nameof(e)} RESULT", e);
                throw;
            }
        }

        private bool ShouldLog(string methodName)
        {
            return _grainInfo.Methods.Contains(methodName);
        }
    }
}