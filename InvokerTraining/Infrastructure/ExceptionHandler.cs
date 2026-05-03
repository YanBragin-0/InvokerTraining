using Microsoft.AspNetCore.Diagnostics;

namespace InvokerTraining.Infrastructure
{
    public class ExceptionHandler : IExceptionHandler
    {
        private readonly ILogger _logger;
        public ExceptionHandler(ILogger<ExceptionHandler> logger)
        {
            _logger = logger;
        }
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            _logger.LogError(exception,"Exception occured: {Message}", exception.Message);
            var response = new
            {
                Title = "Server Error",
                Status = StatusCodes.Status500InternalServerError,
                Detail = exception.Message
            };
            httpContext.Response.StatusCode = response.Status;
            await httpContext.Response.WriteAsJsonAsync(response,cancellationToken);
            return true;
        }
    }
}
