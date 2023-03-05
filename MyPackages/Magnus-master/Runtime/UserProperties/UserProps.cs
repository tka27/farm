using System.Collections.Generic;

namespace MagnusSdk.Core.UserProperties
{
    public class UserProps
    {
        private Dictionary<string, object> _userProperties;
        private Dictionary<string, int> _eventCounters;

        public Dictionary<string, object> UserProperties => _userProperties;
        public Dictionary<string, int> EventCounters => _eventCounters;

        public void Initialize()
        {
            _userProperties = UserPropsStorage.GetUserProperties();
            _eventCounters = UserPropsStorage.GetEventsCounters();
        }

        public void SetUserProperty(string name, object value)
        {
            _userProperties[name] = value;
            UserPropsStorage.SaveUserProperties(_userProperties);
        }

        public void TrackEvent(string eventName)
        {
            if (!_eventCounters.ContainsKey(eventName))
                _eventCounters[eventName] = 0;
            _eventCounters[eventName]++;
            UserPropsStorage.SaveEventsCounters(_eventCounters);
        }
    }
}
