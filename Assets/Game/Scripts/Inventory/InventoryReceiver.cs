using Sirenix.OdinInspector;
using UnityEngine;
using SubLib.Async;
using SubLib.Extensions;

namespace Game.Scripts.Inventory
{
    [RequireComponent(typeof(Inventory), typeof(Collider))]
    public class InventoryReceiver : MonoBehaviour
    {
        public event System.Action OnFull;
        [SerializeField, ReadOnly] protected Inventory SelfInventory;
        [SerializeField, Min(0)] private float _defaultItemsFrequency = 1;
        [SerializeField, ReadOnly] private Inventory _currentProvider;
        private Timer _timer;


        public Inventory CurrentProvider => _currentProvider;
        public Inventory LastProvider { get; private set; }


        private void OnTriggerEnter(Collider other)
        {
            if (_currentProvider != null) return;

            if (!other.TryGetComponent(out _currentProvider)) return;
            LastProvider = _currentProvider;

            _timer?.Destroy();
            _timer = new Timer(_defaultItemsFrequency, ReceiveItem);
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            if (!other.TryGetComponent(out Inventory provider)) return;
            if (_currentProvider != provider) return;

            _timer?.Destroy();
            _currentProvider = null;
        }

        private void OnDestroy()
        {
            _timer?.Destroy();
        }

        private async void ReceiveItem()
        {
            if (_currentProvider == null || this == null) return;

            foreach (var type in SelfInventory.AvailableTypes)
            {
                if (!await _currentProvider.TransferItem(type, SelfInventory)) continue;
                if (!SelfInventory.HasEmptySlot())
                {
                    OnFull?.Invoke();
                    _timer?.Destroy();
                    return;
                }

                if (_currentProvider.IsEmpty)
                {
                    _timer?.Destroy();
                    return;
                }

                _timer.Frequency /= 2;
                return;
            }
        }

        private void OnValidate()
        {
            gameObject.TrySetComponent(ref SelfInventory);
        }
    }
}