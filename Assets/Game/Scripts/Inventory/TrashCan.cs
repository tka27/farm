using Cysharp.Threading.Tasks;

namespace Game.Scripts.Inventory
{
    public class TrashCan : InventoryReceiver
    {
        private float _clearDelayTime;

        private void Start()
        {
            _clearDelayTime = (int)(SelfInventory.Curves.Duration * 1000);
            SelfInventory.OnAddItem += ClearReceiver;
        }

        private void OnDestroy()
        {
            SelfInventory.OnAddItem -= ClearReceiver;
        }

        private async void ClearReceiver()
        {
            var token = UniTaskCancellationExtensions.GetCancellationTokenOnDestroy(this);
            if (!await SubLib.Utils.Async.Delay(_clearDelayTime, token)) return;
            SelfInventory.Clear();
        }
    }
}