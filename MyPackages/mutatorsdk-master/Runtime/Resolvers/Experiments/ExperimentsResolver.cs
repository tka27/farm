using System.Collections.Generic;
using System.Linq;
using MagnusSdk.Core;
using MagnusSdk.Mutator.Parameters;
using MagnusSdk.Mutator.Utils;
using Newtonsoft.Json.Linq;
using Random = UnityEngine.Random;

namespace MagnusSdk.Mutator.Resolvers.Experiments
{
    public class ExperimentsResolver
    {
        private List<Experiment> _experiments = new List<Experiment>();

        public ExperimentsResolver(JObject[] experimentsData)
        {
            foreach (JObject experimentData in experimentsData)
            {
                _experiments.Add(new Experiment(experimentData));
            }
        }

        public Dictionary<int, int> ResolveExperiments(ref List<Parameter> parameters)
        {
            Dictionary<string, bool> testingParams = new Dictionary<string, bool>();
            Dictionary<int, int> selectedVariantsMap = new Dictionary<int, int>();
            
            foreach (Experiment experiment in _experiments)
            {
                ExperimentVariant variant = null;

                if (MutatorStorage.IsExperimentSkipped(experiment.Id)) continue;
                
                int variantId = MutatorStorage.GetSavedVariantId(experiment.Id);
                if (variantId != 0)
                {
                    if (experiment.Status == ExperimentStatus.running ||
                        experiment.Status == ExperimentStatus.collecting)
                    {
                        variant = experiment.Variants.Find(ev => ev.Id == variantId);
                    }
                }
                else if (experiment.Status == ExperimentStatus.running)
                {
                    if (MutatorStorage.IsFirstLaunch || !experiment.IsNewUsersOnly)
                    {
                        if (experiment.IsFit(Magnus.DeviceProps, Magnus.UserProps))
                        {
                            ExperimentVariant testVariant = ChooseVariant(experiment);
                            if (testVariant.Parameters.All(vp => !testingParams.ContainsKey(vp.Key)))
                            {
                                if (Random.Range(0f, 1f) < experiment.Percentage / 100f)
                                {
                                    variant = testVariant;
                                    selectedVariantsMap.Add(experiment.Id, variant.Id);
                                }
                                else
                                {
                                    MutatorStorage.SetSkippedExperiment(experiment.Id, true);
                                }
                            }
                        }
                    }
                }

                if (variant != null)
                {
                    variant.Parameters.ForEach(vp => testingParams[vp.Key] = true);
                    variant.Apply(ref parameters);
                }
            }

            return selectedVariantsMap;
        }

        private ExperimentVariant ChooseVariant(Experiment experiment)
        {
            List<ExperimentVariant> specifiedVariants = experiment.Variants
                .FindAll(v => v.Percentage != 0);

            float specifiedPercentage = 0;
            foreach (ExperimentVariant variant in specifiedVariants)
                specifiedPercentage += variant.Percentage;

            int notSpecifiedVariants = experiment.Variants.Count - specifiedVariants.Count;
            float notSpecifiedPercentage = notSpecifiedVariants > 0
                ? (100f - specifiedPercentage) / (experiment.Variants.Count - specifiedVariants.Count)
                : 0f;

            float rand = Random.Range(0f, 100f);

            float percentageTotal = 0f;
            foreach (ExperimentVariant variant in experiment.Variants)
            {
                percentageTotal += variant.Percentage != 0
                    ? variant.Percentage
                    : notSpecifiedPercentage;

                if (percentageTotal >= rand)
                    return variant;
            }

            return experiment.Variants[experiment.Variants.Count];
        }
        
    }
}