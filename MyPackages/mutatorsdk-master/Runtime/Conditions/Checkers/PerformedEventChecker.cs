using System.Collections.Generic;
using MagnusSdk.Core.UserProperties;
using Newtonsoft.Json.Linq;

namespace MagnusSdk.Mutator.Conditions.Checkers
{
    public class PerformedEventChecker : BaseConditionChecker
    {
        private readonly int _id;
        private readonly string _eventName;
        private readonly int _eventCountValue;
        private readonly string _condition;

        public PerformedEventChecker(JObject data)
        {
            _id = (int)data["id"];
            _eventName = data["name"].ToString();
            _eventCountValue = (int)data["count"];
            _condition = data["condition"].ToString();
        }
        
        public bool IsFit(UserProps up)
        {
            Dictionary<string, int> eventCounters = up.EventCounters;

            if (_condition == "not_set")
                return !eventCounters.ContainsKey(_eventName);

            if (!string.IsNullOrEmpty(_condition) && eventCounters.TryGetValue(_eventName, out int ec))
                return TestNumeric(
                    (float) ec,
                    _condition,
                    new [] { (float)_eventCountValue }
                );

            return false;
        }
    }
}