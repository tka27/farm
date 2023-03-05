using Game.Scripts.Data;
using Unity.Collections;
using UnityEngine;

namespace Game.Scripts
{
    [RequireComponent(typeof(Joystick))]
    public class MainJoystick : MonoBehaviour, IAutoInit
    {
        public static MainJoystick Instance { get; private set; }
        [SerializeField, ReadOnly] private Joystick _joystick;

        public bool IsActive()
        {
            const float joystickTrashold = 0.15f;
            return _joystick.Direction.magnitude > joystickTrashold;
        }

        public Vector3 Direction => GetNormalizedWorldDirection();

        public void Awake()
        {
            Instance = this;
        }

        private Vector3 GetNormalizedWorldDirection()
        {
            float angle = -StaticData.Instance.MainCamera.transform.rotation.eulerAngles.y;
            var cos = Mathf.Cos(angle * Mathf.Deg2Rad);
            var sin = Mathf.Sin(angle * Mathf.Deg2Rad);

            Vector2 direction = _joystick.Direction;
            float x = direction.x * cos - direction.y * sin;
            float z = direction.x * sin + direction.y * cos;

            return new Vector3(x, 0, z).normalized;
        }

        public void AutoInit()
        {
            _joystick = GetComponent<Joystick>();
        }
    }
}