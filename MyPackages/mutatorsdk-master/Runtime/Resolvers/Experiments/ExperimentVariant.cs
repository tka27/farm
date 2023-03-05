using System.Collections.Generic;
using MagnusSdk.Mutator.Parameters;
using Newtonsoft.Json.Linq;

namespace MagnusSdk.Mutator.Resolvers.Experiments
{
    public class ExperimentVariant
    {
        public readonly int Id;
        public readonly string Name;
        public readonly float Percentage = 0;
        public readonly List<ExperimentVariantParameter> Parameters
            = new List<ExperimentVariantParameter>();

        public ExperimentVariant(JObject data)
        {
            Id = (int) data["id"];
            Name = data["name"].ToString();
            
            JToken pToken = data["percentage"];
            if (pToken.Type != JTokenType.Null)
            {
                Percentage = (float) pToken;
            }

            foreach (JObject parameterData in data["parameters"].ToObject<JObject[]>())
            {
                Parameters.Add(new ExperimentVariantParameter(parameterData));
            }
        }

        public void Apply(ref List<Parameter> parameters)
        {
            foreach (ExperimentVariantParameter parameterVariant in Parameters)
            {
                foreach (Parameter parameter in parameters)
                {
                    if (parameterVariant.Key == parameter.Key)
                    {
                        if (parameterVariant.UseDefaultValue) parameter.SelectValueVariant(parameter.DefaultValue);
                        else parameter.SelectValueVariant(parameterVariant.Value);
                        break;
                    }
                }
            }
        }
    }
}