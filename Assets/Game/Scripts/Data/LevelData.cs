using UnityEngine;

namespace Game.Scripts.Data
{
    public class LevelData : MonoBehaviour
    {
        public static LevelData Instance;
        [field: SerializeField] public Canvas MainCanvas { get; private set; }
    }
}