using Newtonsoft.Json.Linq;

namespace MagnusSdk.Mutator.Resolvers.Experiments
{
    public class ExperimentVariantParameter
    {
        public readonly string Key;
        public readonly object Value;
        public readonly bool UseDefaultValue;

        public ExperimentVariantParameter(JObject data)
        {
            Key = data["key"].ToString();
            Value = data["value"];
            UseDefaultValue = (bool) data["use_default_value"];
        }
    }
}