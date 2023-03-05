using UnityEngine;

namespace Game.Scripts.Data
{
    public class StaticData : ScriptableObject
    {
        public static StaticData Instance;

        private Camera _mainCamera;


        public Camera MainCamera
        {
            get
            {
                if (!_mainCamera) _mainCamera = Camera.main;
                return _mainCamera;
            }
        }
    }
}