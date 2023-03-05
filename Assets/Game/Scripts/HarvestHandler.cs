using UnityEngine;

namespace Game.Scripts
{
    public class HarvestHandler : MonoBehaviour
    {
        private static readonly int AttackHash = Animator.StringToHash("Attack");

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent<Bush>(out var bush)) return;
            if (bush.Inventory.IsEmpty) return;

            Player.Instance.Animator.SetTrigger(AttackHash);
            bush.DropItem();
        }
    }
}