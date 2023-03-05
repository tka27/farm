using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Extensions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Scripts.Inventory
{
    [RequireComponent(typeof(CapsuleCollider))]
    public class InventoryDistributor : MonoBehaviour, IAutoInit
    {
        [SerializeField] private Inventory[] _providers;
        [SerializeField, ReadOnly] private Inventory _target;
        [SerializeField, Min(0)] private float _delay = 1;

        public void AutoInit()
        {
            GetComponent<CapsuleCollider>().isTrigger = true;
        }

        private void OnTriggerStay(Collider other)
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
            await UniTask.Delay(TimeSpan.FromSeconds(_delay));
            if (_target == null) return;

            var cachedInventoryForAsync = _target;

            while (cachedInventoryForAsync.HasEmptySlot())
            {
                var provider = _providers.GetRandomElement();
                if (!await provider.TransferItem(provider.Items[0].Type, cachedInventoryForAsync)) return;
            }
        }

        [Button]
        private void Config(float radius)
        {
            var inventories = FindObjectsOfType<Inventory>();
            List<Inventory> nearest = new();

            foreach (var inventory in inventories)
            {
                if (inventory.transform.DistanceTo(transform.position) < radius) nearest.Add((inventory));
            }

            _providers = nearest.ToArray();
        }
    }
}