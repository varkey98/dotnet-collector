namespace OpentelemetryDotnetCollector.Exporter.TestingExporter;

class TestingExporterConfig
{
    public Dictionary<string, string> ResourceAttributes { get; set; } = [];
    public List<Dictionary<string, string>> SpanAttributes { get; set; } = [];
}