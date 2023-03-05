using System;
using Cysharp.Threading.Tasks;
using Game.Scripts.Data;
using Game.Scripts.Inventory;
using Game.Scripts.Resource;
using UnityEngine;

namespace Game.Scripts
{
    public class RewardHandler : MonoBehaviour
    {
        [SerializeField] private Inventory.Inventory _inventory;
        [SerializeField] private float _delay = 1;


        private void Start()
        {
            _inventory.OnAddItemWithItem += GetReward;
        }

        private void OnDestroy()
        {
            _inventory.OnAddItemWithItem -= GetReward;
        }

        private async void GetReward(InventoryItem inventoryItem)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(_delay));
            ResourceHandler.AddResource(ResourceType.Money, inventoryItem.Price, true,
                StaticData.Instance.MainCamera.WorldToScreenPoint(transform.position));
        }
    }
}