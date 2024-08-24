namespace DotnetCollector.Exporter.MetricsExporter;

public class MetricsExporterConfig
{
    public bool Enabled { get; set; } = false;
    public string Endpoint { get; set; } = "";
    public string ServiceName { get; set; } = "dotnetcollector";
}