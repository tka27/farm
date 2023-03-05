using Cysharp.Threading.Tasks;
using UnityEngine;
using SubLib.Async;
using SubLib.Extensions;

namespace Game.Scripts.Inventory
{
    public class InventoryItem : MonoBehaviour
    {
        [field: SerializeField] public ItemType Type { get; private set; }
        [field: SerializeField, Min(0)] public int Price { get; private set; } = 5;


        public Transform Transform { get; private set; }
        public bool AbleToUse { get; private set; } = true;

        private ReusableCancellationTokenSource _cts;


        private void Awake()
        {
            _cts = new(this.GetCancellationTokenOnDestroy());
            Transform = transform;
        }

        public async UniTask<bool> MoveTo(Inventory inventory)
        {
            AbleToUse = false;
            var token = _cts.Create();

            if (!inventory.Add(this))
            {
                AbleToUse = true;
                return false;
            }

            await transform.CurveMoveAsync(inventory.ItemTarget, inventory.Curves, token);
            AbleToUse = true;

            return true;
        }

        private void OnDestroy()
        {
            _cts?.Cancel();
        }

        public void SwitchActive(bool value)
        {
            _cts?.Cancel();
            gameObject.SetActive(value);
        }
    }

    public enum ItemType
    {
        Item
    }
}