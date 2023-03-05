using System.Collections.Generic;
using Game.Scripts.UtilsSubmodule.Inventory;
using UnityEngine;
using static SubLib.Utils.Editor;

namespace Game.Scripts.Inventory
{
    public class DynamicInventory : GridInventory
    {
        [SerializeField] private MagnetSettings _magnetSettings;
        private readonly List<Vector3> _prevPositions = new();
        private readonly List<Quaternion> _prevRotations = new();

        private float _height;

        public override void SizeUpdate(int desiredSize)
        {
            base.SizeUpdate(desiredSize);
            _height = Grid[Grid.Count - 1].localPosition.y - Grid[0].localPosition.y;
            TransformsUpdate(desiredSize);
        }

        private void TransformsUpdate(int desiredSize)
        {
            int missingSlots = desiredSize - _prevPositions.Count;
            _prevPositions.AddRange(new Vector3[missingSlots]);
            _prevRotations.AddRange(new Quaternion[missingSlots]);
        }

        public override void AutoInit()
        {
            base.AutoInit();
            _magnetSettings ??= GetAllInstances<MagnetSettings>()[0];
        }

        private void FixedUpdate()
        {
            for (int i = 0; i < Items.Count; i++)
            {
                if (!Items[i].AbleToUse) continue;

                var itemForce = GetForce(i);

                Items[i].Transform.position = Vector3.Lerp(_prevPositions[i],
                    Grid[i].position, Time.fixedDeltaTime * itemForce);

                Items[i].Transform.rotation = Quaternion.Lerp(_prevRotations[i],
                    Grid[i].rotation, Time.fixedDeltaTime * itemForce * 2);

                _prevPositions[i] = Items[i].Transform.position;
            }

            RecordTransforms();
        }

        private void RecordTransforms()
        {
            for (int i = 0; i < Items.Count; i++)
            {
                _prevPositions[i] = Items[i].Transform.position;
                _prevRotations[i] = Items[i].Transform.rotation;
            }
        }

        private float GetForce(int index)
        {
            var lerpValue = Grid[index].localPosition.y / _height;
            return _magnetSettings.ForceCurve.Evaluate(lerpValue) * _magnetSettings.ForceMultiplier;
        }
    }
}