using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MagnusSdk.Core;
using MagnusSdk.Mutator.Parameters;
using MagnusSdk.Mutator.Resolvers.Conditions;
using MagnusSdk.Mutator.Resolvers.Experiments;
using MagnusSdk.Mutator.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace MagnusSdk.Mutator
{
    public static class Mutator
    {

        private static MutatorApi _api;
        
        public static async void Initialize()
        {
            if (!Magnus.IsInitialized)
            {
                Debug.LogError("Mutator require MagnusSdk initialization");
                return;
            }
            
            MutatorStorage.Initialize();
            _api = new MutatorApi(Magnus.Token);
        }

        public static async Task<JObject> FetchConfig(int cacheDuration = 3600)
        {
            DateTime lastFetchTime = MutatorStorage.GetFetchTime();

            int timeSinceLastFetch = (int)(DateTime.Now - lastFetchTime).TotalSeconds;

            if (timeSinceLastFetch > cacheDuration)
            {
                try
                {
                    JObject config = await _api.FetchConfig();
                    MutatorStorage.SaveFetchCache(config);
                    MutatorStorage.SaveFetchTime(DateTime.Now);

                    return config;
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                }
            }
            
            JObject cachedConfig = MutatorStorage.GetFetchCache();
            return cachedConfig;
        }

        public static async Task<MutatorConfig> Activate()
        {
            JObject config = MutatorStorage.GetFetchCache();
            JObject remoteConfig = config["remote_config"].ToObject<JObject>();

            List<Parameter> parameters = new List<Parameter>();
            foreach (JObject parameterData in remoteConfig["parameters"].ToObject<JObject[]>())
            {
                parameters.Add(new Parameter(parameterData));
            }
            
            ConditionsResolver cr = new ConditionsResolver(remoteConfig["conditions"].ToObject<JObject[]>());
            ExperimentsResolver er = new ExperimentsResolver(config["experiments"].ToObject<JObject[]>());

            cr.ResolveConditions(ref parameters);

            Dictionary<int, int> selectedVariantsMap = er.ResolveExperiments(ref parameters);

            if (selectedVariantsMap.Count > 0)
            {
                try
                {
                    await _api.PushSelectedVariants(selectedVariantsMap);

                    foreach (KeyValuePair<int, int> sv in selectedVariantsMap)
                    {
                        MutatorStorage.SaveVariant(sv.Key, sv.Value);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                }
            }

            return new MutatorConfig(parameters);
        }
    }
}