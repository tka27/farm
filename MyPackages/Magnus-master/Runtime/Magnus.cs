using System;
using System.Threading.Tasks;
using MagnusSdk.Core.DeviceProperties;
using MagnusSdk.Core.UserProperties;
using UnityEngine;

namespace MagnusSdk.Core
{
    public static class Magnus
    {
        public static bool IsInitialized => _isInitialized;
        public static string Token => _token;
        public static DeviceProps DeviceProps => _deviceProps;
        public static UserProps UserProps => _userProps;

        private static bool _initializationInProgress = false;
        private static bool _isInitialized = false;
        private static string _token;
        private static Func<string> _appsFlyerIdGetter;

        private static DeviceProps _deviceProps = new DeviceProps();
        private static UserProps _userProps = new UserProps();

        public static void SetAppsFlyerIdGetter(Func<string> getter) => _deviceProps.SetAppsFlyerIdGetter(getter);
        
        public static async Task Initialize(string token)
        {
            if (_isInitialized)
            {
                Debug.LogWarning("SDK already initialized");
                return;
            }
            
            if (_initializationInProgress)
            {
                Debug.LogWarning("Initialization already in progress");
                return;
            }

            _initializationInProgress = true;

            _token = token;
            
            _userProps.Initialize();
            await _deviceProps.Initialize();

            _isInitialized = true;
            _initializationInProgress = false;
        }

    }
}