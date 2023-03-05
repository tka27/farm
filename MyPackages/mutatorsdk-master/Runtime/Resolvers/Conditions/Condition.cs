using MagnusSdk.Core.DeviceProperties;
using MagnusSdk.Core.UserProperties;
using MagnusSdk.Mutator.Conditions;
using Newtonsoft.Json.Linq;

namespace MagnusSdk.Mutator.Resolvers.Conditions
{
    public class Condition
    {
        public readonly int Id;
        public readonly string Name;
        public readonly int Priority;

        private readonly ConditionsChecker _conditionsChecker = new ConditionsChecker();

        public Condition(JObject data)
        {
            Id = (int)data["id"];
            Name = data["name"].ToString();
            Priority = (int)data["priority"];
            
            _conditionsChecker.SetupSegmentsCheckers(data["segments"].ToObject<JObject[]>());
            _conditionsChecker.SetupPerformedEventCheckers(data["performed_events"].ToObject<JObject[]>());
        }

        public bool IsFit(DeviceProps deviceProps, UserProps userProps) => _conditionsChecker.IsFit(deviceProps, userProps);
    }
}