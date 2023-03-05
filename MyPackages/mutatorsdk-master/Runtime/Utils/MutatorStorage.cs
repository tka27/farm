using System;
using System.Collections.Generic;
using MagnusSdk.Core.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MagnusSdk.Mutator.Utils
{
    public static class MutatorStorage
    {
        private const string kIsNotFirstLaunch = "MUTATOR/INFL";
        private const string kFetchCache = "MUTATOR/FC";
        private const string kFetchTime = "MUTATOR/FT";
        private const string kSavedVariants = "MUTATOR/SV";
        private const string kSkippedExperiments = "MUTATOR/SE";

        private static Dictionary<int, int> _savedVariants;
        private static Dictionary<int, bool> _skippedExperiments;

        private static DebounceDispatcher _savedVariantsDispatcher = new DebounceDispatcher(500);
        private static DebounceDispatcher _skippedExperimentsDispatcher = new DebounceDispatcher(500);

        public static bool IsFirstLaunch { get; private set; }

        public static void Initialize()
        {
            if (!Storage.GetBool(kIsNotFirstLaunch))
            {
                IsFirstLaunch = true;
                Storage.SetBoot(kIsNotFirstLaunch, true);
            }

            _savedVariants = 
                JsonConvert.DeserializeObject<Dictionary<int, int>>(Storage.GetString(kSavedVariants))
                ?? new Dictionary<int, int>();
            _skippedExperiments =
                JsonConvert.DeserializeObject<Dictionary<int, bool>>(Storage.GetString(kSkippedExperiments))
                ?? new Dictionary<int, bool>();
        }

        public static JObject GetFetchCache()
        {
            string fetchCache = Storage.GetString(kFetchCache);
            if (String.IsNullOrEmpty(fetchCache))
                throw new Exception("Config cache is empty");
                    
            return JObject.Parse(fetchCache);
        }

        public static void SaveFetchCache(JObject fetchedConfig)
        {
            Storage.SetString(kFetchCache, fetchedConfig.ToString());
        }

        public static DateTime GetFetchTime()
        {
            string msString = Storage.GetString(kFetchTime);
            if (String.IsNullOrEmpty(msString))
            {
                return DateTimeOffset.FromUnixTimeMilliseconds(0).LocalDateTime;
            }

            long ms = Convert.ToInt64(msString);
            return DateTimeOffset.FromUnixTimeMilliseconds(ms).LocalDateTime;
        }

        public static void SaveFetchTime(DateTime fetchTime)
        {
            long ms = new DateTimeOffset(fetchTime).ToUnixTimeMilliseconds();
            Storage.SetString(kFetchTime, ms.ToString());
        }

        public static int GetSavedVariantId(int experimentId)
        {
            if (!_savedVariants.ContainsKey(experimentId))
                return 0;
            
            return _savedVariants[experimentId];
        }

        public static void SaveVariant(int experimentId, int variantId)
        {
            _savedVariants[experimentId] = variantId;
            SaveSavedVariants();
        }

        public static bool IsExperimentSkipped(int experimentId)
        {
            if (_skippedExperiments.ContainsKey(experimentId))
                return _skippedExperiments[experimentId];

            return false;
        }

        public static void SetSkippedExperiment(int experimentId, bool isSkipped = true)
        {
            _skippedExperiments[experimentId] = isSkipped;
            SaveSkippedExperiments();
        }
        
        private static void SaveSavedVariants()
        {
            _savedVariantsDispatcher.Dispatch(() =>
            {
                Storage.SetString(kSavedVariants, JsonConvert.SerializeObject(_savedVariants));
            });   
        }
        
        private static void SaveSkippedExperiments()
        {
            _skippedExperimentsDispatcher.Dispatch(() =>
            {
                Storage.SetString(kSkippedExperiments, JsonConvert.SerializeObject(_skippedExperiments));
            });   
        }
        
    }
}