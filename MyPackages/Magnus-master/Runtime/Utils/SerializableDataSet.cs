using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace MagnusSdk.Core.Utils
{
    [Serializable]
    public class SerializableDataSet : ISerializable
    {

        private readonly Dictionary<string, bool> _boolProps = new Dictionary<string, bool>();
        private readonly Dictionary<string, int> _intProps = new Dictionary<string, int>();
        private readonly Dictionary<string, float> _floatProps = new Dictionary<string, float>();
        private readonly Dictionary<string, string> _stringProps = new Dictionary<string, string>();

        public SerializableDataSet() {}
        
        public SerializableDataSet(SerializationInfo info, StreamingContext context)
        {
            foreach (SerializationEntry infoItem in info)
            {
                if (info.ObjectType == typeof(bool)) SetBool(infoItem.Name, (bool)infoItem.Value);
                else if (info.ObjectType == typeof(float)) SetFloat(infoItem.Name, (float)infoItem.Value);
                else if (info.ObjectType == typeof(int)) SetInt(infoItem.Name, (int)infoItem.Value);
                else if (info.ObjectType == typeof(string)) SetString(infoItem.Name, (string)infoItem.Value);
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            foreach (KeyValuePair<string, int> pair in _intProps) info.AddValue(pair.Key, pair.Value);
            foreach (KeyValuePair<string, float> pair in _floatProps) info.AddValue(pair.Key, pair.Value);
            foreach (KeyValuePair<string, bool> pair in _boolProps) info.AddValue(pair.Key, pair.Value);
            foreach (KeyValuePair<string, string> pair in _stringProps) info.AddValue(pair.Key, pair.Value);
        }
        
        public bool GetBool(string name) { return _boolProps[name]; }
        public SerializableDataSet SetBool(string name, bool value)
        {
            _boolProps[name] = value;
            return this;
        }

        
        public int GetInt(string name) { return _intProps[name]; }
        public SerializableDataSet SetInt(string name, int value)
        {
            _intProps[name] = value;
            return this;
        }

        public float GetFloat(string name) { return _floatProps[name]; }
        public SerializableDataSet SetFloat(string name, float value)
        {
            _floatProps[name] = value;
            return this;
        }

        public string GetString(string name) { return _stringProps[name]; }
        public SerializableDataSet SetString(string name, string value)
        {
            _stringProps[name] = value;
            return this;
        }

    }

}
