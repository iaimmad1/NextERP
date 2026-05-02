using MediatR;

namespace NextERP.Common.Behaviours
{
    /// <summary>
    /// MediatR Pipeline Behaviour for logging.
    /// Logs request execution and performance metrics.
    /// </summary>
    public class LoggingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<LoggingBehaviour<TRequest, TResponse>> _logger;

        public LoggingBehaviour(ILogger<LoggingBehaviour<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;
            var startTime = DateTime.UtcNow;

            _logger.LogInformation($"Starting request: {requestName}");

            try
            {
                var response = await next();
                var duration = DateTime.UtcNow - startTime;

                _logger.LogInformation(
                    $"Completed request: {requestName} in {duration.TotalMilliseconds}ms");

                return response;
            }
            catch (Exception ex)
            {
                var duration = DateTime.UtcNow - startTime;
                _logger.LogError(ex, $"Failed request: {requestName} after {duration.TotalMilliseconds}ms");
                throw;
            }
        }
    }
}
