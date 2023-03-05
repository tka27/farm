using Game.Scripts.Data;
using UnityEngine;

namespace SubLib.UI
{
    public class MovableUIObject : MonoBehaviour
    {
        public Transform Target;
        private Transform _cachedTransform;


        [SerializeField, Min(0)] private int dampingSpeed = 0;

        private void Awake()
        {
            _cachedTransform = transform;
        }

        private void LateUpdate()
        {
            _cachedTransform.position = dampingSpeed == 0
                ? StaticData.Instance.MainCamera.WorldToScreenPoint(Target.position)
                : Vector3.Lerp(_cachedTransform.position,
                    StaticData.Instance.MainCamera.WorldToScreenPoint(Target.position), Time.deltaTime * dampingSpeed);
        }
    }
}