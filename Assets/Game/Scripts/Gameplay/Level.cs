using UnityEngine;

namespace Game.Scripts.Gameplay
{
    public class Level : MonoBehaviour
    {
        public event System.Action OnLevelLoaded;
        public event System.Action OnLevelCompleted;
        public event System.Action OnLevelLosing;

        public bool LevelStarted { get; set; }

        public void StartGameProcess()
        {
            LevelStarted = true;
        }

        private void Start()
        {
            OnLevelLoaded?.Invoke();
        }


#if UNITY_EDITOR
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space)) OnLevelCompleted?.Invoke();
            if (Input.GetKeyDown(KeyCode.Backspace)) OnLevelLosing?.Invoke();
        }
#endif
    }
}