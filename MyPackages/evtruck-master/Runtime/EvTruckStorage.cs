using System.Collections;
using System.Collections.Generic;
using MagnusSdk.Core.Utils;
using MagnusSdk.EvTruck.Data;
using Newtonsoft.Json;
using UnityEngine;

namespace MagnusSdk.EvTruck
{
    public static class EvTruckStorage
    {

        private const string kEventsDataStore = "EVTRUCK/ED";
        private const string kUserPropsStore = "EVTRUCK/UP";
        
        public static Dictionary<string, EventData> Events => _events;
        public static Dictionary<string, UserPropData> UserProps => _userProps;

        private static Dictionary<string, EventData> _events;
        private static Dictionary<string, UserPropData> _userProps;
        
        private static readonly DebounceDispatcher _eventsUpdatesDispatcher = new DebounceDispatcher(500);
        private static readonly DebounceDispatcher _userPropsUpdatesDispatcher = new DebounceDispatcher(500);

        public static void Initialize()
        {
            _events = JsonConvert.DeserializeObject<Dictionary<string, EventData>>(Storage.GetString(kEventsDataStore)) ??
                      new Dictionary<string, EventData>();
            _userProps = JsonConvert.DeserializeObject<Dictionary<string, UserPropData>>(Storage.GetString(kUserPropsStore)) ??
                         new Dictionary<string, UserPropData>();
        }
        
        public static void PushEvent(string key, EventData eventData)
        {
            if (_events.ContainsKey(key))
            {
                PushEvent($"0{key}", eventData);
                return;
            }
            _events[key] = eventData;
            SaveEvents();
        }

        public static void RemoveEvent(string key)
        {
            _events.Remove(key);
            SaveEvents();
        }

        public static void RemoveEvents(string[] keys)
        {
            foreach (string key in keys)
            {
                _events.Remove(key);
            }
            SaveEvents();
        }

        public static void PushUserProp(string key, UserPropData userProp)
        {
            if (_userProps.ContainsKey(key))
            {
                PushUserProp($"0{key}", userProp);
                return;
            }
            _userProps[key] = userProp;
            SaveUserProps();
        }

        public static void RemoveUserProp(string key)
        {
            _userProps.Remove(key);
            SaveUserProps();
        }
        
        public static void RemoveUserProps(string[] keys)
        {
            foreach (string key in keys)
            {
                _userProps.Remove(key);
            }
            SaveUserProps();
        }
        
        private static void SaveEvents()
        {
            _eventsUpdatesDispatcher.Dispatch(() =>
            {
                Storage.SetString(kEventsDataStore, JsonConvert.SerializeObject(_events));
            });
        }

        private static void SaveUserProps()
        {
            _userPropsUpdatesDispatcher.Dispatch(() =>
            {
                Storage.SetString(kUserPropsStore, JsonConvert.SerializeObject(_userProps));
            });
        }

    }
}