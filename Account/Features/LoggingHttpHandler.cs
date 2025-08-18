using System.Diagnostics;

namespace AccountServices.Features
{
    public class LoggingHttpHandler : DelegatingHandler
    {
        private readonly ILogger<LoggingHttpHandler> _logger;

        public LoggingHttpHandler(ILogger<LoggingHttpHandler> logger)
        {
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var sw = Stopwatch.StartNew();

            _logger.LogInformation("Outgoing HTTP {Method} {Url}", request.Method, request.RequestUri);

            var response = await base.SendAsync(request, cancellationToken);

            sw.Stop();
            _logger.LogInformation(
                "HTTP response {StatusCode} from {Url} in {Elapsed} ms",
                (int)response.StatusCode,
                request.RequestUri,
                sw.ElapsedMilliseconds);

            return response;
        }
    }

}
