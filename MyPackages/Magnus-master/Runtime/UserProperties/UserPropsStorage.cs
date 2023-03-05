using System.Collections.Generic;
using MagnusSdk.Core.Utils;
using Newtonsoft.Json;

namespace MagnusSdk.Core.UserProperties
{
    public static class UserPropsStorage
    {
        private const string kUserProperties = "USER_PROPERTIES";
        private const string kEventsCounters = "EVENT_COUNTERS";

        private static DebounceDispatcher _upUpdatesDispatcher = new DebounceDispatcher(200);
        private static DebounceDispatcher _ecUpdatesDispatcher = new DebounceDispatcher(200);

        public static Dictionary<string, object> GetUserProperties()
        {
            return JsonConvert.DeserializeObject<Dictionary<string, object>>(Storage.GetString(kUserProperties)) ??
                   new Dictionary<string, object>();
        }

        public static Dictionary<string, int> GetEventsCounters()
        {
            return JsonConvert.DeserializeObject<Dictionary<string, int>>(Storage.GetString(kEventsCounters)) ??
                   new Dictionary<string, int>();
        }

        public static void SaveUserProperties(Dictionary<string, object> up)
        {
            _upUpdatesDispatcher.Dispatch(() =>
            {
                Storage.SetString(kUserProperties, JsonConvert.SerializeObject(up));
            });
        }

        public static void SaveEventsCounters(Dictionary<string, int> ec)
        {
            _ecUpdatesDispatcher.Dispatch(() =>
            {
                Storage.SetString(kEventsCounters, JsonConvert.SerializeObject(ec));
            });
        }
    }
}