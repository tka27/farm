using System;
using Cysharp.Threading.Tasks;
using SubLib.Extensions;
using Unity.Collections;
using UnityEngine;

namespace Game.Scripts.Inventory
{
    [RequireComponent(typeof(Collider))]
    public class InventoryProvider : MonoBehaviour
    {
        [SerializeField] private Inventory _selfInventory;
        [SerializeField, Min(0)] private float _delay = 1;

        [SerializeField, ReadOnly] private Inventory _target;

        private void OnTriggerEnter(Collider other)
        {
            if (_target != null) return;

            if (!other.TryGetComponent(out Inventory receiver)) return;

            _target = receiver;
            TransferItems();
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.TryGetComponent<Inventory>(out _)) return;

            _target = null;
        }

        private async void TransferItems()
        {
            if (_target == null) return;
            if (_selfInventory.IsEmpty) return;
            await UniTask.Delay(TimeSpan.FromSeconds(_delay));
            if (_target == null) return;

            var cachedInventoryForAsync = _target;
            for (int i = _selfInventory.Items.Count - 1; i >= 0; i--)
            {
                if (!cachedInventoryForAsync.HasEmptySlot()) return;
                InventoryItem item = _selfInventory.Items[i];

                _ = _selfInventory.TransferItem(item.Type, cachedInventoryForAsync);
                await UniTask.Yield();
            }
        }

        private void OnValidate()
        {
            gameObject.TrySetComponent(ref _selfInventory);
        }
    }
}