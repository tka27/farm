using System;
using System.Collections.Generic;

namespace MagnusSdk.EvTruck.Data
{
    public struct EventData
    {
        public string Name;
        public Dictionary<string, object> Params;
        public long Timestamp;
    }
}