using System.Collections.Generic;
using Lofelt.NiceVibrations;
using UnityEngine;

namespace Game.Scripts.Common.Vibration
{
    public class VibrationHandler : MonoBehaviour
    {
        #region Singleton

        private static VibrationHandler _instance;

        public static VibrationHandler Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<VibrationHandler>();

                    if (_instance == null)
                    {
                        var go = new GameObject("[Vibration Handler]");
                        DontDestroyOnLoad(go);
                        _instance = go.AddComponent<VibrationHandler>();
                    }
                }

                return _instance;
            }
        }

        #endregion
        
        private readonly List<MyHapticTypes> _addedVibrationPerFrame = new();

        private readonly Dictionary<int, HapticPatterns.PresetType> _overrideHapticsType =
            new()
            {
                { (int)MyHapticTypes.LightImpact, HapticPatterns.PresetType.LightImpact },
                { (int)MyHapticTypes.MediumImpact, HapticPatterns.PresetType.MediumImpact },
                { (int)MyHapticTypes.HeavyImpact, HapticPatterns.PresetType.HeavyImpact },
                { (int)MyHapticTypes.RigidImpact, HapticPatterns.PresetType.RigidImpact },
                { (int)MyHapticTypes.SoftImpact, HapticPatterns.PresetType.SoftImpact },
                { (int)MyHapticTypes.Selection, HapticPatterns.PresetType.Selection },
                { (int)MyHapticTypes.Failure, HapticPatterns.PresetType.Failure },
                { (int)MyHapticTypes.Success, HapticPatterns.PresetType.Success },
                { (int)MyHapticTypes.Warning, HapticPatterns.PresetType.Warning },
            };

        private bool _isCanPlayVibro;

        private readonly MyHapticTypes[] _orderHaptic =
        {
            MyHapticTypes.LightImpact,
            MyHapticTypes.MediumImpact,
            MyHapticTypes.HeavyImpact,
            MyHapticTypes.RigidImpact,
            MyHapticTypes.SoftImpact,
            MyHapticTypes.Selection,
            MyHapticTypes.Failure,
            MyHapticTypes.Success,
            MyHapticTypes.Warning,
        };

        public void AddVibration(MyHapticTypes hapticTypes)
        {
            _addedVibrationPerFrame.Add(hapticTypes);
            _isCanPlayVibro = true;
        }

        private void TryPlayVibration()
        {
            if (!_isCanPlayVibro) return;
            _isCanPlayVibro = false;

            PlayVibration(CalculateVibrationTypeByOrder());
        }

        private void PlayVibration(MyHapticTypes hapticType)
        {
            if (hapticType == MyHapticTypes.Selection)
            {
                HapticPatterns.PlayPreset(HapticPatterns.PresetType.Selection);
            }
            else
            {
                if (_overrideHapticsType.TryGetValue((int)hapticType, out var value))
                {
                    HapticPatterns.PlayPreset(value);
                }
            }

            _addedVibrationPerFrame.Clear();
        }

        private MyHapticTypes CalculateVibrationTypeByOrder()
        {
            var maxOrder = int.MinValue;

            for (int i = 0; i < _addedVibrationPerFrame.Count; i++)
            {
                for (int j = 0; j < _orderHaptic.Length; j++)
                {
                    if (_addedVibrationPerFrame[i] == _orderHaptic[j] && maxOrder < j)
                    {
                        maxOrder = j;
                    }
                }
            }

            return _orderHaptic[maxOrder];
        }

        private void LateUpdate() => TryPlayVibration();
    }
}