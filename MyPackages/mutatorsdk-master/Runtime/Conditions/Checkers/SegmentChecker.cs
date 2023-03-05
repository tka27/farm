using System.Collections.Generic;
using System.Linq;
using MagnusSdk.Core.DeviceProperties;
using MagnusSdk.Core.UserProperties;
using Newtonsoft.Json.Linq;

namespace MagnusSdk.Mutator.Conditions.Checkers
{
    public class SegmentChecker : BaseConditionChecker
    {
        private readonly int _id;
        private readonly string _valueType;
        private readonly object[] _values;
        private readonly string _option;
        private readonly string _condition;

        public SegmentChecker(JObject data)
        {
            _id = (int)data["id"];
            _valueType = data["type"].ToString();
            _values = data["value"].ToObject<object[]>();
            _option = data["option"].ToString();
            _condition = data["condition"].ToString();
        }

        public bool IsFit(DeviceProps dp, UserProps up)
        {
            Dictionary<string, object> props = new Dictionary<string, object>();

            foreach (KeyValuePair<string, object> kvp in dp.GetPropsDictionary())
                props[kvp.Key] = kvp.Value;
            
            foreach (KeyValuePair<string, object> kvp in up.UserProperties)
                props[kvp.Key] = kvp.Value;
            

            if (_condition == "set")
                return props.ContainsKey(_option);

            if (_condition == "not_set")
                return !props.ContainsKey(_option);

            if (
                !string.IsNullOrEmpty(_condition) &&
                _values != null && _values.Length > 0
            )
            {
                if (!props.TryGetValue(_option, out object propValue))
                    return false;

                switch (_valueType)
                {
                    case "numeric":
                        return TestNumeric(
                            (float) propValue,
                            _condition,
                            _values.Select(v => (float) v).ToArray()
                        );
                    case "string":
                        return TestString(
                            propValue.ToString(),
                            _condition,
                            _values.Select(v => v.ToString()).ToArray()
                        );
                    case "version":
                        return TestVersion(
                            propValue.ToString(),
                            _condition,
                            _values.Select(v => v.ToString()).ToArray()
                        );
                }
            }
            
            return false;
        }
    }
}