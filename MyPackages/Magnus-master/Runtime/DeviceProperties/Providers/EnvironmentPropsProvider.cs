using System.Runtime.InteropServices;
using UnityEngine;

namespace MagnusSdk.Core.DeviceProperties.Providers
{
    public class EnvironmentPropsProvider
    {
#if UNITY_IOS && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern bool GetIOSDeviceIsTablet();
#endif

        private const string DeviceTypePhone = "phone";
        private const string DeviceTypeTablet = "tablet";
        
        private string _appVersion;
        private string _deviceType;
        private string _deviceVendor;
        private string _os;
        private string _osVersion;

        public string AppVersion => _appVersion;
        public string DeviceType => _deviceType;
        public string DeviceVendor => _deviceVendor;
        public string OS => _os;
        public string OSVersion => _osVersion;

        public void CollectProps()
        {
            _appVersion = Application.version;
            CollectDeviceType();
            CollectDeviceVendor();
            CollectOS();
            CollectOSVersion();
        }

        private void CollectDeviceType()
        {
#if UNITY_IOS && !UNITY_EDITOR
            bool isTablet = GetIOSDeviceIsTablet();
            _deviceType = isTablet
                ? DeviceTypeTablet
                : DeviceTypePhone;
#else
            float ssw = Screen.width > Screen.height ? Screen.width : Screen.height;

            if (ssw > 800)
            {
                _deviceType = DeviceTypePhone;
                return;
            }
       
            if(ssw >= 800){
                float screenWidth = Screen.width / Screen.dpi;
                float screenHeight = Screen.height / Screen.dpi;
                float size = Mathf.Sqrt(Mathf.Pow(screenWidth, 2) + Mathf.Pow(screenHeight, 2));
                if (size >= 6.5f)
                {
                    _deviceType = DeviceTypeTablet;
                    return;
                }
            }
            
            _deviceType = DeviceTypePhone;
#endif
        }

        private void CollectDeviceVendor()
        {
#if UNITY_IOS && !UNITY_EDITOR
            _deviceVendor = "Apple";
#elif UNITY_ANDROID && !UNITY_EDITOR
            AndroidJavaClass build = new AndroidJavaClass("android.os.Build");
            _deviceVendor = build.GetStatic<string>("MANUFACTURER");
#endif
        }

        private void CollectOS()
        {
#if UNITY_IOS && !UNITY_EDITOR
            _os = "ios";
#elif UNITY_ANDROID && !UNITY_EDITOR
            _os = "android";
#endif
        }

        private void CollectOSVersion()
        {
#if UNITY_IOS && !UNITY_EDITOR
            _osVersion = UnityEngine.iOS.Device.systemVersion;
#elif UNITY_ANDROID && !UNITY_EDITOR
            AndroidJavaClass version = new AndroidJavaClass("android.os.Build$VERSION");
            _osVersion = version.GetStatic<string>("RELEASE");
#endif
        }
        
    }
}