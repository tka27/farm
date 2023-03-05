using System.Collections.Generic;
using MagnusSdk.Core;
using MagnusSdk.Mutator.Parameters;
using Newtonsoft.Json.Linq;

namespace MagnusSdk.Mutator.Resolvers.Conditions
{
    public class ConditionsResolver
    {
        private readonly List<Condition> _conditions = new List<Condition>();

        public ConditionsResolver(JObject[] conditionsData)
        {
            foreach (JObject conditionData in conditionsData)
            {
                Condition condition = new Condition(conditionData);
                _conditions.Add(condition);
            }
            
            _conditions.Sort((condition0, condition1) => condition0.Priority - condition1.Priority);
        }

        public void ResolveConditions(ref List<Parameter> parameters)
        {
            foreach (Parameter parameter in parameters)
            {
                foreach (Condition condition in _conditions)
                {
                    if (
                        parameter.ValuesByConditions.ContainsKey(condition.Id) &&
                        condition.IsFit(Magnus.DeviceProps, Magnus.UserProps)
                    )
                    {
                        ParameterValueByCondition pvbc = parameter.ValuesByConditions[condition.Id];
                        parameter.SelectValueVariant(pvbc.Value);
                        break;
                    }
                }
            }
        }
    }
}