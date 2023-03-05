using Newtonsoft.Json.Linq;

namespace MagnusSdk.Mutator.Parameters
{
    public struct ParameterValueByCondition
    {
        public readonly object Value;
        public readonly int ConditionId;

        public ParameterValueByCondition(JObject data)
        {
            Value = data["value"];
            ConditionId = (int)data["condition_id"];
        }
    }
}
