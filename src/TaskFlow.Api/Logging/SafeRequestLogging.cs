using Microsoft.AspNetCore.Http;
using Serilog.AspNetCore;

namespace TaskFlow.Api.Logging;

public static class SafeRequestLogging
{
    public static void Configure(
        RequestLoggingOptions options)
    {
        options.MessageTemplate =
            "HTTP {RequestMethod} responded {StatusCode} in {Elapsed:0.0000} ms";

        options.EnrichDiagnosticContext =
            (diagnosticContext, httpContext) =>
            {
                diagnosticContext.Set(
                    "RequestMethod",
                    httpContext.Request.Method);

                diagnosticContext.Set(
                    "RequestPath",
                    httpContext.GetEndpoint()?.DisplayName
                        ?? "Unmatched endpoint");

                diagnosticContext.Set(
                    "StatusCode",
                    httpContext.Response.StatusCode);
            };
    }
}
