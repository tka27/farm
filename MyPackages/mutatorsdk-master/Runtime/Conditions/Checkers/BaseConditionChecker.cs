using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace MagnusSdk.Mutator.Conditions.Checkers
{
    public abstract class BaseConditionChecker
    {
        protected bool TestNumeric(float propValue, string condition, float[] conditionValues)
        {
            if (conditionValues.Length == 0) return false;
            
            switch (condition)
            {
                case "<":
                    return propValue < conditionValues[0];
                case "<=":
                    return propValue <= conditionValues[0];
                case ">":
                    return propValue > conditionValues[0];
                case ">=":
                    return propValue >= conditionValues[0];
                case "=":
                    return Array.Exists(conditionValues, v => v == propValue);
                case "!=":
                    return !Array.Exists(conditionValues, v => v == propValue);
            }

            return false;
        }

        protected bool TestString(string propValue, string condition, string[] conditionValues)
        {
            propValue = propValue.ToLower();
            conditionValues = conditionValues.Select(s => s.ToLower()).ToArray();

            bool includes = Array.Exists(conditionValues, cv => cv == propValue);
            bool contains = Array.Exists(conditionValues, cv => propValue.Contains(cv));
            
            switch (condition)
            {
                case "=":
                    return includes;
                case "!=":
                    return !includes;
                case "contains":
                    return contains;
                case "not_contains":
                    return !contains;
            }

            return false;
        }
        
        protected bool TestVersion(string propValue, string condition, string[] conditionValues)
        {
            Version currentVersion = new Version(GetCorrectVersionString(propValue));
            Version[] conditionVersions = conditionValues
                .Select(sv => new Version(GetCorrectVersionString(sv)))
                .ToArray();

            int comparison = currentVersion.CompareTo(conditionVersions[0]);
            bool includes = Array.Exists(conditionVersions, v => v.CompareTo(currentVersion) == 0);
            
            switch (condition)
            {
                case "<":
                    return comparison < 0;
                case "<=":
                    return comparison <= 0;
                case ">":
                    return comparison > 0;
                case ">=":
                    return comparison >= 0;
                case "=":
                    return includes;
                case "!=":
                    return !includes;
            }

            return false;
        }
        
        private string GetCorrectVersionString(string versionString)
        {
            string[] version = {"0", "0", "0"};
            
            Regex semVerRegex = new Regex(@"(\d+(\.\d+(\.\d+)?)?)");
            MatchCollection matches = semVerRegex.Matches(versionString);

            if (matches.Count == 0) return String.Join(".", version);
            
            Match fullMatch = null;
            foreach (Match match in matches)
            {
                if (fullMatch == null || fullMatch.Groups.Count < match.Groups.Count)
                {
                    fullMatch = match;
                }
            }
            
            string[] matchVersion = fullMatch.Value.Split('.');

            for (int i = 0; i < matchVersion.Length && i < version.Length; i++)
            {
                version[i] = matchVersion[i];
            }

            return String.Join(".", version);
        }
    }
}