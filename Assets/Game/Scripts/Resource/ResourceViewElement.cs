using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Scripts.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.Resource
{
    public class ResourceViewElement : MonoBehaviour
    {
        [SerializeField, GroupComponent] private TextMeshProUGUI _resourceCountText;
        [SerializeField, GroupComponent] private Image _resourceIcon;

        [SerializeField, Min(.1f)] private float _refreshTime = .5f;


        private float _currentResourceCount;
        private float _targetResourceCount;
        private ResourceData.ResourceData _resourceData;
        private bool _autoHide;
        private SubLib.Async.ReusableCancellationTokenSource _cts;

        private void Awake()
        {
            _cts = new(this.GetCancellationTokenOnDestroy());
        }

        public ResourceType Type => _resourceData.Type;
        public Vector3 ResourceIconPosition => _resourceIcon.transform.position;

        public void Init(ResourceData.ResourceData resourceData, bool autoHide)
        {
            _resourceData = resourceData;
            _autoHide = autoHide;
            SetIcon();
            UpdateResourceCount();
        }

        private void SetIcon()
        {
            if (_resourceData == null) return;
            _resourceIcon.sprite = _resourceData.ResourceIcon;
        }

        public void SetCanvasShow(bool force = false)
        {
            gameObject.SetActive(!(_autoHide && !(force || _targetResourceCount > 0)));
        }

        private void ScaleIcon()
        {
            _resourceIcon.transform.DOKill();
            _resourceIcon.transform.DOScale(1.15f, 0.1f).SetEase(Ease.OutQuint)
                .OnComplete(() => _resourceIcon.transform.DOScale(1f, 0.3f));
        }

        private void UpdateResourceCount()
        {
            _targetResourceCount = ResourceHandler.GetResourceCount(_resourceData.Type);
        }

        private async UniTaskVoid RefreshCurrentCount()
        {
            _cts.Create();
            float transition = 0;
            var token = _cts.Token;
            while (transition < 1)
            {
                transition += Time.deltaTime / _refreshTime;
                _currentResourceCount = Mathf.Lerp(_currentResourceCount, _targetResourceCount, transition);
                _resourceCountText.text = _currentResourceCount.ToString("0000");
                await UniTask.Yield();
                if (token.IsCancellationRequested) return;
            }
        }

        public void AddResource(int value)
        {
            _targetResourceCount += value;
            ScaleIcon();
            RefreshText();
        }

        public void SubtractResource(int value)
        {
            _targetResourceCount -= value;
            ScaleIcon();
            RefreshText();
        }

        public void SetResourceCount(int value)
        {
            _targetResourceCount = value;
            RefreshText();
        }

        private void RefreshText()
        {
            RefreshCurrentCount().Forget();
            SetCanvasShow();
        }
    }
}