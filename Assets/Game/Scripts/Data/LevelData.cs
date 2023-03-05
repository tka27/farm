using Game.Scripts.Inventory;
using SubLib.ObjectPool;
using UnityEngine;

namespace Game.Scripts.Data
{
    public class LevelData : MonoBehaviour
    {
        public static LevelData Instance;
        [field: SerializeField] public Canvas MainCanvas { get; private set; }

        [field: SerializeField] public ObjectPool<InventoryItem> WheatPool { get; private set; }
    }
}