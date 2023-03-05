using System;
using System.Linq;
using System.Threading.Tasks;
using MagnusSdk.Core.Utils;
using UnityEngine;

namespace MagnusSdk.Core.DeviceProperties.Providers
{
    public class DeviceIdsPropsProvider
    {
        private const string StoragePrefix = "DP_IDS";
        private const string kIdfa = "IDFA";
        private const string kIdfm = "IDFM";

        private string _idfa;
        private string _idfv;
        private string _idfm;

        public string Idfa
        {
            get
            {
                if (string.IsNullOrEmpty(_idfa)) CollectIdfa();
                return _idfa;
            }
        }

        public string Idfv => _idfv;
        public string Idfm => _idfm;

        public async Task CollectProps()
        {
            CollectIdfv();
            CollectIdfm();
            await CollectIdfa();
        }

        private async Task CollectIdfa()
        {
            string storageKey = GetStorageKey(kIdfa);
            _idfa = Storage.GetString(storageKey);

            if (string.IsNullOrEmpty(_idfa))
            {

#if UNITY_ANDROID && !UNITY_EDITOR
                 try
                 {
                     AndroidJavaClass up = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                     AndroidJavaObject currentActivity = up.GetStatic<AndroidJavaObject>("currentActivity");
                     AndroidJavaClass client = new AndroidJavaClass("com.google.android.gms.ads.identifier.AdvertisingIdClient");
                     AndroidJavaObject adInfo = client.CallStatic<AndroidJavaObject>("getAdvertisingIdInfo",currentActivity);

                     _idfa = adInfo.Call<string>("getId");
                 }
                 catch (Exception e)
                 {
                     Debug.Log(e.Message);
                 }

#elif UNITY_IOS && !UNITY_EDITOR
                TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
                Application.RequestAdvertisingIdentifierAsync((idfa, trackingEnabled, error) =>
                {
                    _idfa = idfa;
                    tcs.TrySetResult(true);
                });

                await tcs.Task;
#endif
            }
        }
        
        private void CollectIdfv()
        {
            _idfv = SystemInfo.deviceUniqueIdentifier;
        }
        
        private void CollectIdfm()
        {
            string storageKey = GetStorageKey(kIdfm);
            _idfm = Storage.GetString(storageKey);

            if (string.IsNullOrEmpty(_idfm))
            {
                byte[] randomBytes = (new int[16])
                    .Select((v) => (Byte) UnityEngine.Random.Range(0, 256))
                    .ToArray();

                _idfm = new Guid(randomBytes).ToString();
                Storage.SetString(storageKey, _idfm);
            }
        }

        private string GetStorageKey(string key) => $"{StoragePrefix}/{key}";
    }
}