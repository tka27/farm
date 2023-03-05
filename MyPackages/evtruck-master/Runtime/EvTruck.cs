using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MagnusSdk.EvTruck.Data;
using UnityEngine;
using MagnusSdk.Core;

namespace MagnusSdk.EvTruck
{
    public static class EvTruck
    {

        private const int CollectingInterval = 4000;
        
        private static int _collectingErrorsCounter = 0;
        private static bool _collectingRunning = false;
        
        private static EvTruckApi _api;
        
        public static void Initialize()
        {
            if (!Magnus.IsInitialized)
            {
                Debug.LogError("EvTruck require MagnusSdk initialization");
                return;
            }

            EvTruckStorage.Initialize();
            _api = new EvTruckApi(Magnus.Token);
            
            SetUserProperty("__last_session__", GetTimestamp());

            RunDataCollecting();
        }

        public static void TrackEvent(string eventName, Dictionary<string, object> eventParams = null)
        {
            long timestamp = GetTimestamp();
            EvTruckStorage.PushEvent(
                timestamp.ToString(),
                new EventData() { Name = eventName, Params = eventParams, Timestamp = timestamp }
                );
            Magnus.UserProps.TrackEvent(eventName);   
        }

        public static void SetUserProperty(string propName, object propValue)
        {
            long timestamp = GetTimestamp();
            EvTruckStorage.PushUserProp(
                timestamp.ToString(),
                new UserPropData() { Name = propName, Value = propValue}
            );
            Magnus.UserProps.SetUserProperty(propName, propValue);
        }

        private static async Task CollectData()
        {
            string[] eventsKeys = EvTruckStorage.Events.Select(kvp => kvp.Key).ToArray();
            string[] upKeys = EvTruckStorage.UserProps.Select(kvp => kvp.Key).ToArray();

            List<Func<Task<bool>>> collectors = new List<Func<Task<bool>>>();
            
            if (eventsKeys.Length > 0)
            {
                EventData[] ed = EvTruckStorage.Events.Select(kvp => kvp.Value).ToArray();
                collectors.Add(async () =>
                {
                    try
                    {
                        await _api.TryCollectEvents(ed);
                        EvTruckStorage.RemoveEvents(eventsKeys);
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e.Message);
                        return false;
                    }
                    
                    return true;
                });
            }

            if (upKeys.Length > 0)
            {
                UserPropData[] upd = EvTruckStorage.UserProps.Select(kvp => kvp.Value).ToArray();
                collectors.Add(async () =>
                {
                    try
                    {
                        await _api.TryCollectUserProps(upd);
                        EvTruckStorage.RemoveUserProps(upKeys);
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e.Message);
                        return false;
                    }
                    
                    return true;
                });
            }


            Task<bool>[] tasks = collectors.Select(c => c.Invoke()).ToArray();
            
            await Task.WhenAll(tasks);

            bool isSuccess = true;
            foreach (Task<bool> task in tasks)
            {
                isSuccess &= task.Result;
            }

            if (!isSuccess) _collectingErrorsCounter++;
            else _collectingErrorsCounter = 0;
        }

        private static async void RunDataCollecting()
        {
            _collectingRunning = true;
            while (_collectingRunning)
            {
                await CollectData();

                int errorsCounterMult = (Mathf.Clamp(_collectingErrorsCounter, 0, 5) + 1);
                await Task.Delay(CollectingInterval * errorsCounterMult);
            }
        }

        private static void StopDataCollecting() => _collectingRunning = false;

        private static long GetTimestamp() => DateTimeOffset.Now.ToUnixTimeMilliseconds();

    }
}