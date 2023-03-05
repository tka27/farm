using System.Collections.Generic;
using SubLib.Extensions;
using UnityEngine;

namespace Game.Scripts.Inventory
{
    [RequireComponent(typeof(LayoutGroup3D))]
    public class GridInventory : Inventory
    {
        [SerializeField, HideInInspector] private LayoutGroup3D _3dLayout;

        [SerializeField, Sirenix.OdinInspector.ReadOnly]
        protected List<Transform> Grid;

        private const string SlotName = "Slot";

        protected override void SetItemParent(InventoryItem item)
        {
            ItemTarget = Grid[Items.Count - 1];
            item.Transform.parent = ItemTarget;
        }

        public override void AutoInit()
        {
            base.AutoInit();
            if (gameObject.TrySetComponent(ref _3dLayout)) _3dLayout.enabled = false;

            SizeUpdate(DefaultSize);
            ItemTarget = Grid[0];
        }

        public override void SizeUpdate(int desiredSize)
        {
            base.SizeUpdate(desiredSize);
            Grid = transform.GetChildrenWithName(SlotName);

            int missingSlots = desiredSize - Grid.Count;
            if (missingSlots == 0) return;

            for (int i = 0; i < missingSlots; i++)
            {
                NewSlot();
            }

            _3dLayout.RebuildLayout();
        }

        private void NewSlot()
        {
            var newSlot = new GameObject(SlotName).transform;
            newSlot.parent = transform;
            newSlot.localPosition = Vector3.zero;
            newSlot.localRotation = Quaternion.identity;
            newSlot.localScale = Vector3.one;
            Grid.Add(newSlot);

#if UNITY_EDITOR
            var icon = (Texture2D)UnityEditor.EditorGUIUtility.IconContent("sv_label_4").image;
            UnityEditor.EditorGUIUtility.SetIconForObject(newSlot.gameObject, icon);
#endif
        }
    }
}