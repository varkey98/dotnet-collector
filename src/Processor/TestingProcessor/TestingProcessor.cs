using System.Diagnostics.Metrics;
using System.Reflection.Metadata;
using OpenTelemetry.Proto.Common.V1;
using OpenTelemetry.Proto.Trace.V1;

namespace DotnetCollector.Processor.TestingProcessor;
public class TestingProcessor(TestingProcessorConfig cfg) : BaseProcessor<ResourceSpans>
{
    private readonly TestingProcessorConfig Cfg = cfg;
    private static readonly Meter Meter = new("DotnetCollector.Processor.TestingProcessor");
    private static readonly Counter<long> TotalSpansProcessed = Meter.CreateCounter<long>("total.spans.processed");
    private static readonly Counter<long> TotalSpansSucceeded = Meter.CreateCounter<long>("total.spans.succeeded");

    public override void Process(ResourceSpans data)
    {
        bool evalFailed = false;
        if (Cfg.ResourceAttributes != null)
        {
            var resourceAttrs = data.Resource.Attributes;
            Dictionary<string, AnyValue> matchedAttrs = ProcessorUtils.GetMatchingKeys(resourceAttrs, Cfg.ResourceAttributes);
            if (matchedAttrs.Count != Cfg.ResourceAttributes.Count)
            {
                // failed
                evalFailed = true;
            }

            foreach (Attribute cfgAttr in Cfg.ResourceAttributes)
            {
                AnyValue value = matchedAttrs[cfgAttr.Key];
                if (!value.HasStringValue || !value.StringValue.Equals(cfgAttr.Value))
                {
                    // failed
                    evalFailed = true;
                    break;
                }
            }
        }

        if (Cfg.SpanAttributes != null)
        {
            foreach (var scopedSpans in data.ScopeSpans)
            {
                // update total spans processed counter
                TotalSpansProcessed.Add(scopedSpans.Spans.Count);

                // dont do span evaluation in case resource attributes didnt match
                if (evalFailed)
                {
                    continue;
                }

                foreach (var span in scopedSpans.Spans)
                {
                    var spanAttrs = span.Attributes;
                    Dictionary<string, AnyValue> matchedAttrs = ProcessorUtils.GetMatchingKeys(spanAttrs, Cfg.SpanAttributes);
                    if (matchedAttrs.Count != Cfg.SpanAttributes.Count)
                    {
                        // failed
                        continue;
                    }

                    foreach (Attribute cfgAttr in Cfg.SpanAttributes)
                    {
                        AnyValue value = matchedAttrs[cfgAttr.Key];
                        if (!value.HasStringValue || !value.StringValue.Equals(cfgAttr.Value))
                        {
                            // failed
                            continue;
                        }
                    }

                    // this span succeeded the evaluation
                    TotalSpansSucceeded.Add(1);
                }

            }
        }
    }
}