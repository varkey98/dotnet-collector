using Grpc.Net.Client;
using OpenTelemetry.Proto.Collector.Trace.V1;

namespace DotnetCollector.Exporter.TraceExporter;
public class TraceExporter
{
    private readonly TraceExporterConfig Cfg;
    private readonly TraceService.TraceServiceClient Client;
    public TraceExporter(TraceExporterConfig cfg)
    {
        Cfg = cfg;
        GrpcChannelOptions channelOpts = new()
        {
            Credentials = Grpc.Core.ChannelCredentials.Insecure
        };
        GrpcChannel channel = GrpcChannel.ForAddress(Cfg.Endpoint, channelOpts);

        Client = new TraceService.TraceServiceClient(channel);
    }

    public async Task<ExportTraceServiceResponse> Export(ExportTraceServiceRequest request)
    {
        var cancellationToken = new CancellationTokenSource(TimeSpan.FromMilliseconds(Cfg.TimeoutInMs));
        return await Client.ExportAsync(request, cancellationToken: cancellationToken.Token);
    }
}