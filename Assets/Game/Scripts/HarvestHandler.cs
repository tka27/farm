using UnityEngine;

namespace Game.Scripts
{
    public class HarvestHandler : MonoBehaviour
    {

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent<Bush>(out var bush)) return;
            if (bush.Inventory.IsEmpty) return;

            bush.DropItem();
        }
    }
}