using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using static SubLib.Utils.Editor;

namespace Game.Scripts.Inventory
{
    public class Inventory : MonoBehaviour, IAutoInit
    {
        public event System.Action OnAddItem;
        public event System.Action OnRemoveItem;
        public event System.Action<InventoryItem> OnRemoveItemWithItem;

        [field: SerializeField] public TransitionCurves Curves { get; private set; }
        [field: SerializeField] public bool LimitTypeSpace { get; private set; }
        [field: SerializeField, Min(1)] public int DefaultSize { get; private set; } = 1;

        [field: SerializeField, Sirenix.OdinInspector.ReadOnly]
        public int CurrentSize { get; private set; }

        [field: SerializeField] public Transform ItemTarget { get; protected set; }


        [SerializeField] private List<InventoryItem> _items;
        public List<ItemType> AvailableTypes;


        public List<InventoryItem> Items => _items;
        private InventoryItem LastItem => _items.Count > 0 ? _items[_items.Count - 1] : null;

        public void Clear()
        {
            for (int i = 0; i < _items.Count; i++)
            {
                _items[i].SwitchActive(false);
                _items[i].Transform.parent = null;
            }

            _items.Clear();
            OnRemoveItem?.Invoke();
        }

        public bool Add(InventoryItem item)
        {
            if (!AvailableTypes.Contains(item.Type)) return false;
            if (LimitTypeSpace && !HasTypeSpace(item.Type)) return false;
            if (!HasEmptySlot()) return false;

            _items.Add(item);
            SetItemParent(item);
            OnAddItem?.Invoke();
            return true;
        }

        protected virtual void SetItemParent(InventoryItem item)
        {
            item.Transform.parent = ItemTarget;
        }

        public async UniTask<bool> TransferItem(ItemType itemType, Inventory targetInventory)
        {
            if (!targetInventory.AvailableTypes.Contains(itemType)) return false;
            if (targetInventory.LimitTypeSpace && !targetInventory.HasTypeSpace(itemType)) return false;
            if (!targetInventory.HasEmptySlot()) return false;
            if (!RemoveItem(itemType, out var removedItem)) return false;

            return await removedItem.MoveTo(targetInventory);
        }

        public bool IsEmpty => _items.Count == 0;

        public bool HasEmptySlot()
        {
            if (_items.Count < CurrentSize) return true;

            Debug.Log("Inventory is full");
            return false;
        }

        public bool HasItem(ItemType type, out int index)
        {
            for (int i = _items.Count - 1; i >= 0; i--)
            {
                if (_items[i].Type != type) continue;
                if (!_items[i].AbleToUse) continue;

                index = i;
                return true;
            }

            index = -1;
            Debug.Log("Item not found");
            return false;
        }

        private bool RemoveItem(ItemType type, out InventoryItem removedItem)
        {
            if (!HasItem(type, out int itemIndex))
            {
                removedItem = null;
                return false;
            }

            removedItem = _items[itemIndex];
            _items.RemoveAt(itemIndex);
            OnRemoveItem?.Invoke();
            OnRemoveItemWithItem?.Invoke(removedItem);
            return true;
        }

        public bool RemoveItem(out InventoryItem removedItem)
        {
            removedItem = LastItem;
            if (removedItem == null) return false;

            _items.Remove(removedItem);
            OnRemoveItem?.Invoke();
            OnRemoveItemWithItem?.Invoke(removedItem);
            return true;
        }

        public int GetItemsCount(ItemType type)
        {
            int count = 0;
            foreach (var item in _items)
            {
                if (item.Type == type) count++;
            }

            return count;
        }


        /* public bool RemoveItems(in ItemType[] request, out List<InventoryItem> removedItems)
         {
             var indices = new List<int>();
 
             for (int i = 0; i < request.Length; i++)
             {
                 for (int j = _items.Count - 1; j >= 0; j--)
                 {
                     if (indices.Contains(j)) continue;
 
                     if (_items[j]?.Type == request[i])
                     {
                         indices.Add(j);
                         break;
                     }
                 }
             }
 
             removedItems = new List<InventoryItem>();
             if (indices.Count == request.Length)
             {
                 foreach (var index in indices)
                 {
                     removedItems.Add(_items[index]);
                     _items[index] = null;
                 }
 
                 OnRemoveItem?.Invoke();
                 TrySort();
                 return true;
             }
 
             return false;
         }
 
 
         public void SetAwailableTypes(ItemType[] types)
         {
             for (int i = 0; i < types.Length; i++)
             {
                 if (AwailableTypes.Contains(types[i])) continue;
                 AwailableTypes.Add(types[i]);
             }
         }*/

        [Button]
        public virtual void SizeUpdate(int desiredSize)
        {
            CurrentSize = desiredSize;
        }


        private bool HasTypeSpace(ItemType type)
        {
            return GetItemsCount(type) < _items.Count / AvailableTypes.Count;
        }

        /* private ItemType[] CreateTypeArray(ItemType type, int count)
         {
             ItemType[] array = new ItemType[count];
             for (int i = 0; i < array.Length; i++)
             {
                 array[i] = type;
             }
 
             return array;
         }*/

        protected virtual void Awake()
        {
            SizeUpdate(DefaultSize);
        }

        [Button("Reinit")]
        public virtual void AutoInit()
        {
            ItemTarget = transform;
            if (AvailableTypes == null || AvailableTypes.Count == 0)
                AvailableTypes = new List<ItemType> { ItemType.Item };

            _items ??= new List<InventoryItem>();
        }

        private void OnValidate()
        {
            if (Curves == null) Curves = GetAllInstances<TransitionCurves>()[0];
            if (_items == null || _items.Count <= DefaultSize) return;
            DefaultSize = _items.Count;
            CurrentSize = DefaultSize;
        }
    }
}