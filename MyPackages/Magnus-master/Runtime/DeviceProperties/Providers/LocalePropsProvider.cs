using System.Runtime.InteropServices;
using UnityEngine;

namespace MagnusSdk.Core.DeviceProperties.Providers
{
    public class LocalePropsProvider
    {
#if UNITY_IOS && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern string GetIOSLocaleCode();
        
        [DllImport("__Internal")]
        private static extern string GetIOSCountryCode();
#endif

        private string _localeCode;
        private string _countryCode;

        public string LocaleCode => _localeCode;
        public string CountryCode => _countryCode;

        public void CollectProps()
        {
            CollectLocaleCode();
            CollectCountryCode();
        }

        private void CollectLocaleCode()
        {
#if UNITY_IOS && !UNITY_EDITOR
            _localeCode = GetIOSLocaleCode().ToLower();
#elif UNITY_ANDROID && !UNITY_EDITOR
            AndroidJavaObject locale = GetAndroidContextLocale();
            _localeCode = locale.Call<string>("getLanguage").ToLower();
#endif
        }
        
        private void CollectCountryCode()
        {
#if UNITY_IOS && !UNITY_EDITOR
            _countryCode = GetIOSCountryCode();
#elif UNITY_ANDROID && !UNITY_EDITOR
            AndroidJavaObject locale = GetAndroidContextLocale();
            _countryCode = locale.Call<string>("getCountry");
#endif
        }

#if UNITY_ANDROID && !UNITY_EDITOR
        private AndroidJavaObject GetAndroidContextLocale()
        {
            AndroidJavaClass up = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = up.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject context = currentActivity.Call<AndroidJavaObject>("getApplicationContext");
            AndroidJavaObject res = context.Call<AndroidJavaObject>("getResources");
            AndroidJavaObject config = res.Call<AndroidJavaObject>("getConfiguration");
            AndroidJavaObject locale = config.Get<AndroidJavaObject>("locale");
            
            return locale;
        }
#endif

    }
}