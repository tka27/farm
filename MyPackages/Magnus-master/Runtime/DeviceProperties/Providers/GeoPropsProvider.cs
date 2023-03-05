using System;
using System.Threading.Tasks;
using MagnusSdk.Core.Utils;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace MagnusSdk.Core.DeviceProperties.Providers
{
    public class GeoPropsProvider
    {
        private string _country;
        private string _city;

        public string Country => _country;
        public string City => _city;

        private JsonWebRequestWrapper _requestWrapper;

        public GeoPropsProvider()
        {
            _requestWrapper = new JsonWebRequestWrapper();
            _requestWrapper.SetBaseUrl("http://ip-api.com/json");
        }
        
        public async Task CollectProps()
        {
            try
            {
                JObject response = await _requestWrapper.Get("");
                _country = (string) response.GetValue("countryCode");
                _city = (string) response.GetValue("city");
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }
    }
}