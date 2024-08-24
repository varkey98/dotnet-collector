#nullable enable

using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;

namespace DotnetCollector.Exporter.MetricsExporter;

public class MetricsExporterBuilder
{
    private readonly MetricsExporterConfig Cfg;
    private readonly MeterProviderBuilder? MpBuilder;
    private MeterProvider? Mp;

    private static readonly Version Version = typeof(MetricsExporterBuilder)!.Assembly!.GetName()!.Version;
    public MetricsExporterBuilder(MetricsExporterConfig cfg)
    {
        Cfg = cfg;

        if(cfg.Enabled)
        {
            MpBuilder = Sdk.CreateMeterProviderBuilder()
                .SetResourceBuilder(
                    ResourceBuilder
                        .CreateDefault()
                        .AddService(Cfg.ServiceName, Version.ToString())
                )
            .AddOtlpExporter((opts, metricReaderOpts) => {
                        opts.Endpoint = new Uri(cfg.Endpoint);
                        opts.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
                        metricReaderOpts
                            .PeriodicExportingMetricReaderOptions
                            .ExportIntervalMilliseconds = 60000;
            });
        }
    }

    public MetricsExporterBuilder AddSource(string name)
    {
        MpBuilder?.AddMeter(name);
        return this;
    }

    public MetricsExporterBuilder AddSources(string[] names)
    {
        foreach(var name in names)
        {
            MpBuilder?.AddMeter(name);
        }
                return this;
    }

    public void Build()
    {
        Mp = MpBuilder?.Build();
    }
}