using Google.Protobuf.Collections;
using OpenTelemetry.Proto.Common.V1;

internal static class ProcessorUtils
{
    internal static Dictionary<string, AnyValue> GetMatchingKeys(RepeatedField<KeyValue> attrs, List<Attribute> configuredAttrs)
    {
        Dictionary<string, AnyValue> ret = [];
        foreach (var attr in attrs)
        {
            if (configuredAttrs.FindLastIndex((cfgAttr) =>
            {
                return cfgAttr.Key.Equals(attr.Key);
            }) != -1)
            {
                ret.Add(attr.Key, attr.Value);
            }
        }

        return ret;
    }
}