using System;
using Cysharp.Threading.Tasks;
using Game.Scripts.Data;
using UnityEngine;

namespace Game.Scripts
{
    public class Bush : MonoBehaviour
    {
        [SerializeField] private Inventory.Inventory _inventory;


        [SerializeField, Min(0.1f)] private float _spawnTime = 10;
        [SerializeField] private Collider _collider;


        public Inventory.Inventory Inventory => _inventory;

        public void DropItem()
        {
            if (!_inventory.RemoveItem(out var removedItem)) return;
            removedItem.SwitchKinematic(false);
        }

        private void Start()
        {
            _inventory.Add(LevelData.Instance.WheatPool.Get(transform.position, transform.rotation));
            _inventory.OnRemoveItem += SpawnNewItem;
        }

        private void OnDestroy()
        {
            _inventory.OnRemoveItem -= SpawnNewItem;
        }

        private async void SpawnNewItem()
        {
            _collider.enabled = false;
            await UniTask.Delay(TimeSpan.FromSeconds(_spawnTime));
            LevelData.Instance.WheatPool.Get(transform.position, transform.rotation).MoveTo(_inventory).Forget();
            _collider.enabled = true;
        }
    }
}