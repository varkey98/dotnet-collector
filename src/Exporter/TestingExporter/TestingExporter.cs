using Google.Protobuf.Collections;
using OpenTelemetry;
using OpenTelemetry.Proto.Common.V1;
using OpenTelemetry.Proto.Resource.V1;
using OpenTelemetry.Proto.Trace.V1;

namespace OpentelemetryDotnetCollector.Exporter.TestingExporter;

class TestingExporter(TestingExporterConfig cfg) : BaseExporter<ResourceSpans>
{
    private readonly TestingExporterConfig cfg = cfg;

    // this is expected to have only a single resource
    public override ExportResult Export(in Batch<ResourceSpans> batch)
    {
        do
        {
            var spans = batch.GetEnumerator().Current;
            Resource resource = spans.Resource;
            if(!CompareAttributes(cfg.ResourceAttributes, resource.Attributes))
            {
                return ExportResult.Failure;
            }

            // only a single scope is expected as of now
            foreach(var span in spans.ScopeSpans.First().Spans)
            {
                var expectedAttributes = cfg.SpanAttributes.GetEnumerator().Current;
                if(!CompareAttributes(expectedAttributes, span.Attributes))
                {
                    return ExportResult.Failure;
                }

                // successfully evaluated all given spans
                if(!cfg.SpanAttributes.GetEnumerator().MoveNext())
                {
                    return ExportResult.Success;
                }
            }

        } while (batch.GetEnumerator().MoveNext());
        return ExportResult.Success;
    }

    private static Dictionary<string, string> GetAttributes(RepeatedField<KeyValue> attrs)
    {
        Dictionary<string, string> ret = [];

        foreach (var attr in attrs)
        {
            ret.Add(attr.Key, attr.Value.HasStringValue ? attr.Value.StringValue : "");
        }
        return ret;
    }

    private static bool CompareAttributes(Dictionary<string, string> expectedAttributes, RepeatedField<KeyValue> actualAttributes)
    {
        Dictionary<string, string> attrs = GetAttributes(actualAttributes);

        foreach (var attr in expectedAttributes)
        {

            if (attrs.TryGetValue(attr.Key, out var value))
            {
                if (!attr.Value.Equals(value))
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        return true;
    }
}