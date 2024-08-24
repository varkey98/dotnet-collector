namespace DotnetCollector.Exporter.TraceExporter;

public class TraceExporterConfig
{    
    public bool Enabled {get; set; } = false;
    public string Endpoint {get; set;} = "";
    public long TimeoutInMs {get; set; } = 10000;
}