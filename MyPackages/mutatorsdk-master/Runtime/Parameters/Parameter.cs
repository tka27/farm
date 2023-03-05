using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace MagnusSdk.Mutator.Parameters
{
    public class Parameter
    {
        public readonly int Id;
        public readonly string Key;
        public readonly object DefaultValue;
        public readonly string Description;
        public readonly Dictionary<int, ParameterValueByCondition> ValuesByConditions 
            = new Dictionary<int, ParameterValueByCondition>();
        
        public ParameterVariant SelectedVariant { get; private set; }
        
        public Parameter(JObject data)
        {
            Id = (int)data["id"];
            Key = data["key"].ToString();
            DefaultValue = data["value"];
            Description = data["description"].ToString();
            
            foreach (JObject cd in data["conditions"].ToObject<JObject[]>())
            {
                ParameterValueByCondition pvbc = new ParameterValueByCondition(cd);
                ValuesByConditions.Add(pvbc.ConditionId, pvbc);
            }
            
            SelectValueVariant(DefaultValue);
        }

        public void SelectValueVariant(object value)
        {
            SelectedVariant = new ParameterVariant(value);
        }
    }
}
