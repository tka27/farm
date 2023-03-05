using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game.Scripts.Inventory
{
    public class ItemsCollector : MonoBehaviour
    {
        [SerializeField] private Inventory _selfInventory;

        private void OnTriggerEnter(Collider other)
        {
            if (!_selfInventory.HasEmptySlot()) return;
            if (!other.TryGetComponent<InventoryItem>(out var item)) return;
            item.SwitchPhysicsRequest?.Invoke(false);

            if (!item.AbleToUse) return;
            item.MoveTo(_selfInventory).Forget();
        }
    }
}