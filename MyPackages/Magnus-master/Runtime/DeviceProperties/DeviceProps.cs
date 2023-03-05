using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MagnusSdk.Core.DeviceProperties.Providers;

namespace MagnusSdk.Core.DeviceProperties
{
    public class DeviceProps
    {
        private Func<string> _appsFlyerIdGetter;
        
        private EnvironmentPropsProvider _environment = new EnvironmentPropsProvider();
        private DeviceIdsPropsProvider _device = new DeviceIdsPropsProvider();
        private GeoPropsProvider _geo = new GeoPropsProvider();
        private LocalePropsProvider _locale = new LocalePropsProvider();
        
        public async Task Initialize()
        {
            _environment.CollectProps();
            _locale.CollectProps();

            List<Task> asyncCollectActions = new List<Task>
            {
                _device.CollectProps(),
                _geo.CollectProps()
            };

            await Task.WhenAll(asyncCollectActions);
        }

        public void SetAppsFlyerIdGetter(Func<string> getter) => _appsFlyerIdGetter = getter;
        
        // Environment props
        public string GetAppVersion() => _environment.AppVersion;
        public string GetDeviceType() => _environment.DeviceType;
        public string GetDeviceVendor() => _environment.DeviceVendor;
        public string GetOS() => _environment.OS;
        public string GetOSVersion() => _environment.OSVersion;
        
        // Device Ids
        public string GetIDFA() => _device.Idfa;
        public string GetIDFV() => _device.Idfv;
        public string GetIDFM() => _device.Idfm;

        public string GetAFID() => _appsFlyerIdGetter?.Invoke() ?? string.Empty;
        
        // Geo props
        public string GetCountry() => _geo.Country;
        public string GetCity() => _geo.City;
        
        // Locale porps
        public string GetDeviceCountryCode() => _locale.CountryCode;
        public string GetDeviceLocaleCode() => _locale.LocaleCode;

        public Dictionary<string, object> GetPropsDictionary() => new Dictionary<string, object>
        {
            {"app_version", GetAppVersion()},
            {"device_type", GetDeviceType()},
            {"device_vendor", GetDeviceVendor()},
            {"os", GetOS()},
            {"os_version", GetOSVersion()},
            {"idfa", GetIDFA()},
            {"idfv", GetIDFV()},
            {"idfm", GetIDFM()},
            {"afid", GetAFID()},
            {"device_country", GetDeviceCountryCode()},
            {"device_locale", GetDeviceLocaleCode()},
            {"geo_city", GetCity()},
            {"geo_country", GetCountry()},
        };
    }
    
}
