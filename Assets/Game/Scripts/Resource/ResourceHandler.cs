using System;
using System.Collections.Generic;
using Game.Scripts.Common.SavableValues;
using Unity.Profiling;
using UnityEngine;

namespace Game.Scripts.Resource
{
    public static class ResourceHandler
    {
        public static event Action<ResourceType, int> OnValueSet;
        public static event Action<ResourceType, int, Vector3> OnValueAdded;
        public static event Action<ResourceType, int> OnValueSubtracted;
        public static event Action<ResourceType, int> OnValueChanged;

        private static readonly Dictionary<ResourceType, IntDataValueSavable> Resources = new();

        public static void AddResource(ResourceType type, int addedValue, bool autoSave = false,
            Vector3 screenPosition = default)
        {
            if (addedValue < 0) throw new ArgumentOutOfRangeException(null, "ArgumentOutOfRange_BadAddedValue");

            if (!Resources.ContainsKey(type)) Resources.Add(type, new IntDataValueSavable(type.ToString()));

            OnValueAdded?.Invoke(type, addedValue, screenPosition);
            var value = Resources[type].Value += addedValue;
            OnValueChanged?.Invoke(type, value);
            if (autoSave) SaveData(type);
            ProfilerRecorder.StartNew(ProfilerCategory.Scripts, "Custom");
        }

        public static bool TrySubtractResource(ResourceType type, int subtractValue, bool autoSave = false)
        {
            if (subtractValue < 0) throw new ArgumentOutOfRangeException(null, "ArgumentOutOfRange_BadSubtractedValue");

            if (!Resources.ContainsKey(type)) Resources.Add(type, new IntDataValueSavable(type.ToString()));
            var value = Resources[type].Value;
            if (value < subtractValue) return false;

            OnValueSubtracted?.Invoke(type, subtractValue);
            Resources[type].Value = value -= subtractValue;
            OnValueChanged?.Invoke(type, value);
            if (autoSave) SaveData(type);
            return true;
        }

        public static void SaveData()
        {
            foreach (var valueSavable in Resources)
            {
                valueSavable.Value.Save();
            }
        }

        public static void SaveData(ResourceType type)
        {
            if (Resources.ContainsKey(type))
            {
                Resources[type].Save();
            }
        }

        public static int GetResourceCount(ResourceType type)
        {
            if (!Resources.ContainsKey(type)) Resources.Add(type, new IntDataValueSavable(type.ToString()));
            return Resources[type].Value;
        }

        public static void ResetAllData()
        {
            foreach (ResourceType resourceType in Enum.GetValues(typeof(ResourceType)))
            {
                TrySubtractResource(resourceType, GetResourceCount(resourceType), true);
            }
        }

        public static void LoadAllData()
        {
            foreach (ResourceType resourceType in Enum.GetValues(typeof(ResourceType)))
            {
                LoadData(resourceType);
            }
        }

        private static void LoadData(ResourceType type)
        {
            var saveData = new IntDataValueSavable(type.ToString());
            if (!Resources.ContainsKey(type))
            {
                Resources.Add(type, saveData);
            }
            else
            {
                Resources[type].Value = saveData.Value;
            }

            OnValueSet?.Invoke(type, Resources[type].Value);
        }
    }

    public enum ResourceType
    {
        Money,
        Resource2,
        Resource3,
    }
}