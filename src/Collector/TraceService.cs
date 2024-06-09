using Grpc.Core;
using OpenTelemetry.Proto.Collector.Trace.V1;
using static OpenTelemetry.Proto.Collector.Trace.V1.TraceService;

namespace OpentelemetryDotnetCollector.Service;

class TraceService : TraceServiceBase
{
    public override Task<ExportTraceServiceResponse> Export(ExportTraceServiceRequest request, ServerCallContext context)
    {

        foreach(var span in request.ResourceSpans)
        {
            Console.WriteLine(span.ToString());
        }
        return Task.FromResult(new ExportTraceServiceResponse());
    }
}