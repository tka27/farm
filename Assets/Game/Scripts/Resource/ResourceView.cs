using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Extensions;
using Game.Scripts.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using static Game.Scripts.Resource.ResourceHandler;

namespace Game.Scripts.Resource
{
    public class ResourceView : MonoBehaviour
    {
        [SerializeField, GroupComponent] private Transform _resourceViewElementParent;
        [SerializeField, GroupComponent] private Transform _resourceParent;

        [SerializeField, GroupAssets] private ResourceViewElement _resourceViewElementPrefab;

        [SerializeField, GroupSetting] private bool _resourceFly;
        [SerializeField, GroupSetting] private bool _autoHide;
        [SerializeField, GroupSetting] private int _maxSpawnCount = 15;
        [SerializeField, GroupSetting] private int _minAddedValueAtOneTime = 10;
        [SerializeField, GroupSetting] private float _randomRadius = 100;

        [SerializeField, GroupAssets] private Image _resourceViewPrefab;

        [SerializeField, GroupSetting] protected List<ResourceData.ResourceData> _showResourceData;

        private readonly Dictionary<ResourceData.ResourceData, ResourceViewElement> _resourceViewElements = new();

        private readonly Stack<Image> _flyResources = new();

        private Sequence _flySequence;

        private void Awake() => ClearResourceViewElements();
        private void Start() => LoadAllData();

        private void OnEnable()
        {
            OnValueAdded += AddResourceCount;
            OnValueSet += SetResourceCount;
            OnValueSubtracted += SubtractResource;
        }

        private void OnDisable()
        {
            OnValueAdded -= AddResourceCount;
            OnValueSet -= SetResourceCount;
            OnValueSubtracted -= SubtractResource;
        }

#if UNITY_EDITOR
        [Button, GUIColor(0, 1, 0)]
        private void UpdateViews()
        {
            if (Application.isPlaying)
            {
                ClearResourceViewElements();
                LoadAllData();
            }
            else
            {
                try
                {
                    ClearResourceViewElements(true);
                    foreach (var resourceData in _showResourceData)
                    {
                        SetResourceCount(resourceData.Type, 100);
                    }

                    LoadAllData();
                }
                catch
                {
                    Debug.LogWarning("Not enough permissions, try editing prefab");
                }
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                AddResource(ResourceType.Money, 100, false, Input.mousePosition);
            }

            if (Input.GetKeyDown(KeyCode.M))
            {
                ResourceHandler.TrySubtractResource(ResourceType.Money, 100);
            }
        }

#endif

        private void ClearResourceViewElements(bool immediate = false)
        {
            _resourceViewElementParent.DestroyChildren(immediate);
            _resourceViewElements.Clear();
        }

        private void AddResourceCount(ResourceType type, int value, Vector3 screenPosition)
        {
            if (TryGetElement(type, out var element))
            {
                AddResourceCount(element, value, screenPosition);
            }
        }

        private void SubtractResource(ResourceType type, int value)
        {
            if (TryGetElement(type, out var element))
            {
                element.SubtractResource(value);
            }
        }

        private void SetResourceCount(ResourceType type, int value)
        {
            if (TryGetElement(type, out var element))
            {
                element.SetResourceCount(value);
            }
        }

        private bool TryGetElement(ResourceType type, out ResourceViewElement element)
        {
            var resourceData = GetResourceData(type);

            if (resourceData != null)
            {
                if (_resourceViewElements.ContainsKey(resourceData))
                {
                    element = _resourceViewElements[resourceData];
                }
                else
                {
                    element = Instantiate(_resourceViewElementPrefab, _resourceViewElementParent);
                    element.Init(resourceData, _autoHide);
                    element.name = $"{type.ToString()} View Element";
                    _resourceViewElements.Add(resourceData, element);
                }

                return true;
            }

            element = null;
            return false;
        }

        private ResourceData.ResourceData GetResourceData(ResourceType type)
        {
            return _showResourceData.Find(x => x.Type == type);
        }

        private void AddResourceCount(ResourceViewElement viewElement, int value, Vector3 screenPosition)
        {
            StartCoroutine(AddResourceCountCor(viewElement, value, screenPosition));
        }

        private IEnumerator AddResourceCountCor(ResourceViewElement viewElement, int value, Vector3 screenPosition)
        {
            viewElement.SetCanvasShow(true);
            yield return null;
            if (!_resourceFly)
            {
                viewElement.AddResource(value);
                yield break;
            }

            var resourceCount = Mathf.CeilToInt((float)value / _minAddedValueAtOneTime);

            if (resourceCount > _maxSpawnCount) resourceCount = _maxSpawnCount;

            var defaultIncreaseValue = Mathf.FloorToInt((float)value / resourceCount);
            var lastIncreaseValue = value - (defaultIncreaseValue * resourceCount) + defaultIncreaseValue;

            var globalFlySequence = DOTween.Sequence();

            var flyResourceTemp = new List<Image>();

            for (var i = 0; i < resourceCount; i++)
            {
                var index = i;
                var flyResource = GetFlyResource(GetResourceData(viewElement.Type));

                var flyTransform = flyResource.transform;
                flyTransform.DOKill();

                flyResourceTemp.Add(flyResource);

                flyTransform.localScale = Vector3.zero;
                if (screenPosition == default)
                    flyTransform.localPosition = Random.insideUnitCircle * _randomRadius;
                else
                    flyTransform.position = screenPosition + (Vector3)(Random.insideUnitCircle * _randomRadius);
                flyResource.gameObject.SetActive(true);
                _flySequence?.Kill();
                _flySequence = DOTween.Sequence();

                var delay = i == 0 ? 0 : Random.Range(0.1f, 0.25f);
                _flySequence.Append(
                    flyTransform.DOScale(1f, 0.2f).SetEase(Ease.OutCirc).SetDelay(delay));
                _flySequence.Join(flyTransform.DOMove(viewElement.ResourceIconPosition, 0.6f).SetEase(Ease.InBack));
                _flySequence.OnComplete(delegate
                {
                    flyTransform.DOScale(0f, 0.2f).SetEase(Ease.InCirc)
                        .OnComplete(delegate
                        {
                            flyResource.gameObject.SetActive(false);
                            flyTransform.SetParent(_resourceParent);
                        });

                    viewElement.AddResource(index == resourceCount - 1 ? lastIncreaseValue : defaultIncreaseValue);
                });

                globalFlySequence.Join(_flySequence);
                globalFlySequence.OnComplete(delegate
                {
                    foreach (var resource in flyResourceTemp)
                    {
                        resource.gameObject.SetActive(false);
                        resource.transform.localPosition = Vector3.zero;
                        _flyResources.Push(resource);
                    }
                });
            }
        }

        private void SpawnResource()
        {
            var resource = Instantiate(_resourceViewPrefab, _resourceParent);
            resource.gameObject.SetActive(false);
            resource.transform.localPosition = Vector3.zero;
            _flyResources.Push(resource);
        }

        private Image GetFlyResource(ResourceData.ResourceData resourceData)
        {
            if (_flyResources.Count == 0) SpawnResource();
            var resource = _flyResources.Pop();
            resource.sprite = resourceData.ResourceIcon;
            return resource;
        }
    }
}