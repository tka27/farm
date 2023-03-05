using UnityEngine;

namespace Game.Scripts.Inventory
{
    public class HandsInventory : Inventory
    {
        [SerializeField] private Animator _animator;
        private static readonly int HandsUp = Animator.StringToHash("HandsUp");

        protected override void Awake()
        {
            base.Awake();
            OnAddItem += SwitchHands;
            OnRemoveItem += SwitchHands;
        }

        private void OnDestroy()
        {
            OnAddItem -= SwitchHands;
            OnRemoveItem -= SwitchHands;
        }

        private void SwitchHands() => _animator.SetBool(HandsUp, Items.Count > 0);
    }
}