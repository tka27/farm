using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.Common.Dev
{
    public class FpsCounterDevOption : DevOption
    {
        [SerializeField] private Button _switchButton;
        [SerializeField] private TextMeshProUGUI _toggleText;

        private FpsCounter _fpsCounter;

        private bool _isVisibleFpsCounter;

        protected override void Init()
        {
            base.Init();
            _fpsCounter = FindObjectOfType<FpsCounter>();

            if (_fpsCounter != null) _isVisibleFpsCounter = _fpsCounter.IsShow;

            RefreshToggleText();
            _switchButton.onClick.AddListener(ToggleVisibility);
        }

        private void RefreshToggleText()
        {
            _toggleText.text = _isVisibleFpsCounter ? "Hide FPS" : "Show FPS";
        }

        private void ToggleVisibility()
        {
            if (_fpsCounter != null)
            {
                _isVisibleFpsCounter = !_isVisibleFpsCounter;
                _fpsCounter.SetActiveFpsCounter(_isVisibleFpsCounter);
                RefreshToggleText();
            }
        }
    }
}