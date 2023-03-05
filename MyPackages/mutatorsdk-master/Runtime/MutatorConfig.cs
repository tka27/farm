using System.Collections.Generic;
using System.Linq;
using MagnusSdk.Mutator.Parameters;

namespace MagnusSdk.Mutator
{
    public class MutatorConfig
    {

        private readonly Dictionary<string, Parameter> _parametersMap;
        
        public MutatorConfig(List<Parameter> parameters)
        {
            _parametersMap = parameters.ToDictionary(p => p.Key, p => p);
        }

        public ParameterVariant GetValue(string key)
        {
            if (!_parametersMap.ContainsKey(key))
                return new ParameterVariant(0);

            return _parametersMap[key].SelectedVariant;
        }

        public bool HasValue(string key)
        {
            return _parametersMap.ContainsKey(key);
        }
    }
}