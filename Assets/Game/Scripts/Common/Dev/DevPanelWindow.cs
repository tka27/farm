using Extensions;
using Game.Scripts.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.Common.Dev
{
    [RequireComponent(typeof(CanvasGroup))]
    public class DevPanelWindow : MonoBehaviour
    {
        [SerializeField] private Button _closeButton;

        private CanvasGroup _canvasGroup;
        private DevOption[] _devOptions;

        private void Awake()
        {
            _closeButton.onClick.AddListener(DisableWindow);
            _canvasGroup = GetComponent<CanvasGroup>();
            _devOptions = GetComponentsInChildren<DevOption>();
        }

        private void Start()
        {
            DisableWindow();
        }

        private void OnEnable()
        {
            foreach (var devOption in _devOptions)
            {
                devOption.OnPropertyChanged += CheckOption;
            }
        }

        private void OnDisable()
        {
            foreach (var devOption in _devOptions)
            {
                devOption.OnPropertyChanged -= CheckOption;
            }
        }

        public void EnableWindow()
        {
            _canvasGroup.Show();
        }

        private void DisableWindow()
        {
            _canvasGroup.Hide();
        }

        private void CheckOption(DevOption devOption)
        {
            if (devOption.NeedToHideDevPanel) DisableWindow();
        }
    }
}