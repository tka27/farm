using UnityEngine;

namespace Game.Scripts.Inventory
{
    public class ItemPhysicsHandler : MonoBehaviour
    {
        [SerializeField] private InventoryItem _item;
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private Collider _collider;
        [SerializeField, Min(1)] private float _jumpForce = 400;
        [SerializeField] private float _angularSpeed = 5;


        private void Start()
        {
            _item.SwitchPhysicsRequest += SwitchPhysics;
        }

        private void OnDestroy()
        {
            _item.SwitchPhysicsRequest -= SwitchPhysics;
        }

        private void SwitchPhysics(bool value)
        {
            _collider.enabled = value;
            _rigidbody.isKinematic = !value;
            if (!value) return;
            _rigidbody.AddForce(transform.up * _jumpForce + SubLib.Utils.Vector3.DisplaceXZ());
            _rigidbody.angularVelocity = SubLib.Utils.Vector3.Displace(_angularSpeed);
        }
    }
}