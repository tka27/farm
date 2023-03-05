using Cysharp.Threading.Tasks;
using Game.Scripts.Data;
using UnityEngine;

namespace Game.Scripts
{
    public class Bush : MonoBehaviour
    {
        [SerializeField] private Inventory.Inventory _inventory;

        private void Start()
        {
            SpawnNewItem();
            _inventory.OnRemoveItem += SpawnNewItem;
        }

        private void OnDestroy()
        {
            _inventory.OnRemoveItem -= SpawnNewItem;
        }

        private void SpawnNewItem()
        {
            LevelData.Instance.WheatPool.Get(transform.position, transform.rotation).MoveTo(_inventory).Forget();
        }
    }
}