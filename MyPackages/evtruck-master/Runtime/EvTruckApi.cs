using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MagnusSdk.Core;
using MagnusSdk.Core.DeviceProperties;
using MagnusSdk.Core.Utils;
using MagnusSdk.EvTruck.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace MagnusSdk.EvTruck
{
    public class EvTruckApi
    {
        private const string ApiBaseUrl = "https://evtruck.magnus.ms";
        
        private readonly string _token;
        private readonly JsonWebRequestWrapper _requestWrapper;

        public EvTruckApi(string token)
        {
            _requestWrapper = new JsonWebRequestWrapper();
            _requestWrapper
                .SetBaseUrl(ApiBaseUrl)
                .AddHeader("Authorization", $"Bearer {token}");
        }

        public async Task TryCollectEvents(EventData[] events)
        {
            Dictionary<string, object> args = GetDefaultArgs();
            
            DeviceProps dp = Magnus.DeviceProps;
            args.Add("app_version", dp.GetAppVersion());
            args.Add("os_version", dp.GetAppVersion());

            object[] eventsData = events.Select(ed => new Dictionary<string, object>
            {
                {"event_name", ed.Name},
                {"event_params", ed.Params},
                {"event_time_ms", ed.Timestamp},
            }).ToArray();
            
            args.Add("events", eventsData);
            
            JObject body = JObject.FromObject(args);

            await _requestWrapper.Post("/collector/event", body);
        }

        public async Task TryCollectUserProps(UserPropData[] userProps)
        {
            Dictionary<string, object> args = GetDefaultArgs();

            Dictionary<string, object> userPropsData = new Dictionary<string, object>();
            foreach (UserPropData up in userProps)
            {
                userPropsData[up.Name] = up.Value;
            }
            
            DeviceProps dp = Magnus.DeviceProps;
            userPropsData.Add("__app_version__", dp.GetAppVersion());
            userPropsData.Add("__device_type__", dp.GetDeviceType());
            userPropsData.Add("__device_vendor__", dp.GetDeviceVendor());
            userPropsData.Add("__device_country__", dp.GetDeviceCountryCode());
            userPropsData.Add("__device_locale__", dp.GetDeviceLocaleCode());
            userPropsData.Add("__os_version__", dp.GetOSVersion());
            userPropsData.Add("__geo_city__", dp.GetCity());
            userPropsData.Add("__geo_country__", dp.GetCountry());

            args["user_properties"] = userPropsData;
            
            JObject body = JObject.FromObject(args);

            await _requestWrapper.Post("/collector/user-property", body);
        }

        private Dictionary<string, object> GetDefaultArgs()
        {
            DeviceProps dp = Magnus.DeviceProps;
            return new Dictionary<string, object>
            {
                {"platform", dp.GetOS()},
                {"idfa", dp.GetIDFA()},
                {"idfv", dp.GetIDFV()},
                {"idfm",dp.GetIDFM()},
                {"appsflyer_uid", dp.GetAFID()}
            };
        }
    }
}