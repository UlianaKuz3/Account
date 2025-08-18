using System.Diagnostics;

namespace AccountServices.Features
{
    public class LoggingHttpHandler(ILogger<LoggingHttpHandler> logger) : DelegatingHandler
    {

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var sw = Stopwatch.StartNew();

            logger.LogInformation("Outgoing HTTP {Method} {Url}", request.Method, request.RequestUri);

            var response = await base.SendAsync(request, cancellationToken);

            sw.Stop();
            logger.LogInformation(
                "HTTP response {StatusCode} from {Url} in {Elapsed} ms",
                (int)response.StatusCode,
                request.RequestUri,
                sw.ElapsedMilliseconds);

            return response;
        }
    }

}
