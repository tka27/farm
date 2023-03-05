using UnityEngine;

namespace Game.Scripts
{
    public class WeaponSwitcher : MonoBehaviour
    {
        [SerializeField] private GameObject _weapon;

        public void SwitchWeapon(int value) => _weapon.SetActive(value > 0);
    }
}