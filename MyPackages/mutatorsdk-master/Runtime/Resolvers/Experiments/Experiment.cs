using System;
using System.Collections.Generic;
using MagnusSdk.Core.DeviceProperties;
using MagnusSdk.Core.UserProperties;
using MagnusSdk.Mutator.Conditions;
using Newtonsoft.Json.Linq;

namespace MagnusSdk.Mutator.Resolvers.Experiments
{
    public enum ExperimentStatus
    {
        running,
        collecting,
        skipped
    }
    
    public class Experiment
    {
        public readonly int Id;
        public readonly string Name;
        public readonly string Description;
        public readonly ExperimentStatus Status;
        public readonly bool IsNewUsersOnly;
        public readonly float Percentage;
        public readonly List<ExperimentVariant> Variants = new List<ExperimentVariant>();

        private readonly ConditionsChecker _conditionsChecker = new ConditionsChecker();

        public Experiment(JObject data)
        {
            Id = (int) data["id"];
            Name = data["name"].ToString();
            Description = data["description"].ToString();
            Status = (ExperimentStatus) Enum.Parse(typeof(ExperimentStatus), data["status"].ToString());
            IsNewUsersOnly = (bool) data["new_users_only"];
            Percentage = (float) data["percentage"];

            foreach (JObject variantData in data["variants"].ToObject<JObject[]>())
            {
                Variants.Add(new ExperimentVariant(variantData));
            }
            
            _conditionsChecker.SetupSegmentsCheckers(data["segments"].ToObject<JObject[]>());
            _conditionsChecker.SetupPerformedEventCheckers(data["performed_events"].ToObject<JObject[]>());
        }
        
        public bool IsFit(DeviceProps deviceProps, UserProps userProps) => _conditionsChecker.IsFit(deviceProps, userProps);
    }
}