using System.Linq;
using System.Collections.Generic;
using MagnusSdk.Core.DeviceProperties;
using MagnusSdk.Core.UserProperties;
using MagnusSdk.Mutator.Conditions.Checkers;
using Newtonsoft.Json.Linq;

namespace MagnusSdk.Mutator.Conditions
{
    public class ConditionsChecker
    {
        private readonly List<SegmentChecker> _segments = new List<SegmentChecker>();
        private readonly List<PerformedEventChecker> _performedEvents = new List<PerformedEventChecker>();

        public void SetupSegmentsCheckers(JObject[] segmentsData)
        {
            foreach (JObject segmentData in segmentsData)
            {
                _segments.Add(new SegmentChecker(segmentData));
            }
        }

        public void SetupPerformedEventCheckers(JObject[] performedEventsData)
        {
            foreach (JObject performedEventData in performedEventsData)
            {
                _performedEvents.Add(new PerformedEventChecker(performedEventData));
            }
        }

        public bool IsFit(DeviceProps deviceProps, UserProps userProps)
        {
            bool isFitSegments = true;
            if (_segments.Count > 0)
                isFitSegments = _segments.All(s => s.IsFit(deviceProps, userProps));

            bool isFitPerformedEvents = true;
            if (_performedEvents.Count > 0)
                isFitPerformedEvents = _performedEvents.All(pe => pe.IsFit(userProps));

            return isFitSegments && isFitPerformedEvents;
        }
    }
}
