using System;
using UnityEngine;

namespace MagnusSdk.Core.Utils
{
    public static class Storage
    {
        private static string StoragePrefix = "@MAGNUS_SDK";

        public static bool GetBool(string key) => Convert.ToBoolean(PlayerPrefs.GetInt(GetKey(key)));
        public static void SetBoot(string key, bool value) => PlayerPrefs.SetInt(GetKey(key), Convert.ToInt32(value));
        
        public static int GetInt(string key) => PlayerPrefs.GetInt(GetKey(key));
        public static void SetInt(string key, int value) => PlayerPrefs.SetInt(GetKey(key), value);
        
        public static float GetFloat(string key) => PlayerPrefs.GetFloat(GetKey(key));
        public static void SetFloat(string key, float value) => PlayerPrefs.SetFloat(GetKey(key), value);
        
        public static string GetString(string key) => PlayerPrefs.GetString(GetKey(key));
        public static void SetString(string key, string value) => PlayerPrefs.SetString(GetKey(key), value);
        
        private static string GetKey(string key) => $"{StoragePrefix}/{key}";
    }
}