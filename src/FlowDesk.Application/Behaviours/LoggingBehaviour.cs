using Microsoft.Extensions.Logging;

namespace FlowDesk.Application.Behaviours;

internal sealed class LoggingBehaviour<TRequest, TResponse>(
    ILogger<LoggingBehaviour<TRequest, TResponse>> logger,
    TimeProvider timeProvider)
 : IPipelineBehavior<TRequest, TResponse>
 where TRequest : IRequest<TResponse> 
{  
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var startedAt = timeProvider.GetTimestamp();

        logger.LogInformation("Starting request {RequestName}", requestName);

        try
        {
            var result = await next();

            var elapsed = timeProvider.GetElapsedTime(startedAt);

            logger.LogInformation(
                "Completed request {RequestName} in {ElapsedMilliseconds}ms",
                requestName, elapsed.TotalMilliseconds);

                return result;
        }
        catch(Exception ex)
        {
            var elapsed = timeProvider.GetElapsedTime(startedAt);

            logger.LogError(ex,
                "Request {RequestName} failed after {ElapsedMilliseconds}ms",
                requestName, elapsed.TotalMilliseconds);
            
            throw;
        }
    }
}
