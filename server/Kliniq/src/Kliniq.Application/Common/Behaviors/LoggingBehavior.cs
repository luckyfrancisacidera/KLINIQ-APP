using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;
using Kliniq.Application.Common.Security;

namespace Kliniq.Application.Common.Behaviors
{
    public class LoggingBehavior<TRequest, TResponse>: IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle( TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;

            var stopwatch = Stopwatch.StartNew();

            _logger.LogInformation("Handling {RequestName}", requestName);

            // Do not serialize request objects into logs. Commands can contain passwords,
            // tokens, symptom descriptions, appointment reasons, or other private data.
            _logger.LogDebug(
                request is ISensitiveRequest
                    ? "Handling sensitive request {RequestName}; payload omitted."
                    : "Handling request {RequestName}; payload logging is disabled.",
                requestName);

            try
            {
                var response = await next();

                stopwatch.Stop();

                var elapsedMs = stopwatch.ElapsedMilliseconds;

                if (elapsedMs > 500)
                {
                    _logger.LogWarning("Handled {RequestName} in {ElapsedMs}ms (SLOW)",requestName, elapsedMs);
                }
                else
                {
                    _logger.LogInformation("Handled {RequestName} in {ElapsedMs}ms",requestName,elapsedMs);
                }

                return response;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                _logger.LogError(ex,"Unhandled exception in {RequestName} after {ElapsedMs}ms", requestName, stopwatch.ElapsedMilliseconds);

                throw;
            }
        }
    }
}