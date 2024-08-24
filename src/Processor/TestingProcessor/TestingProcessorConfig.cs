public class TestingProcessorConfig
{
    public bool EnableMetrics { get; set; }
    public List<Attribute>? ResourceAttributes;
    public List<Attribute>? SpanAttributes;
}

public class Attribute
{
    public string Key { get; set; } = "";
    public string Value { get; set; } = "";
}