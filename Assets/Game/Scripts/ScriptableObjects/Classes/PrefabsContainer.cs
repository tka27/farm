using System.Collections.Generic;
using Game.Scripts.Gameplay;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace Game.ScriptableObjects.Classes
{
    [CreateAssetMenu(menuName = "Levels/Prefabs Container")]
    public class PrefabsContainer : GlobalConfig<PrefabsContainer>
    {
        [SerializeField] 
        private bool isDebug;
        [SerializeField, ShowIf("isDebug"), ValueDropdown("levels")]
        private Level debugLevel;
        [SerializeField] 
        private List<Level> levels;

        public List<Level> Levels => levels;
        public bool IsDebug => isDebug;
        public Level DebugLevel => debugLevel;
    }
}