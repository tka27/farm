using UnityEngine;

namespace Game.Scripts
{
    public class ToolSwitcher : MonoBehaviour
    {
        [SerializeField] private GameObject _weapon;

        public void SwitchTool(int value) => _weapon.SetActive(value > 0);
    }
}