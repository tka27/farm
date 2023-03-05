using UnityEngine;

namespace Game.Scripts.Inventory
{
    public class TrashCan : InventoryReceiver
    {
        protected override void OnTriggerExit(Collider other)
        {
            base.OnTriggerExit(other);
            SelfInventory.Clear();
        }
    }
}