using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Inventory
{
    public enum LayoutStyle
    {
        Linear,
        Grid,
        Euclidean,
        Radial
    }

    public enum LayoutAxis3D
    {
        X,
        Y,
        Z
    }

    public enum LayoutAxis2D
    {
        X,
        Y
    }

    public enum Alignment
    {
        Min,
        Center,
        Max
    }

    [ExecuteInEditMode]
    public class LayoutGroup3D : MonoBehaviour
    {
        [HideInInspector] public List<Transform> LayoutElements;
        [HideInInspector] public Vector3 ElementDimensions = Vector3.one;

        [HideInInspector] public float Spacing;
        [HideInInspector] public LayoutStyle Style;

        [HideInInspector] public int GridConstraintCount;
        [HideInInspector] public int SecondaryConstraintCount;

        [HideInInspector] public bool UseFullCircle;
        [HideInInspector] public float MaxArcAngle;
        [HideInInspector] public float Radius;
        [HideInInspector] public float StartAngleOffset;
        [HideInInspector] public bool AlignToRadius;
        [HideInInspector] public LayoutAxis3D LayoutAxis;
        [HideInInspector] public LayoutAxis3D SecondaryLayoutAxis;
        [HideInInspector] public LayoutAxis2D GridLayoutAxis;
        [HideInInspector] public List<Quaternion> ElementRotations;
        [HideInInspector] public Vector3 StartPositionOffset;
        [HideInInspector] public float SpiralFactor;
        [HideInInspector] public Alignment PrimaryAlignment;
        [HideInInspector] public Alignment SecondaryAlignment;
        [HideInInspector] public Alignment TertiaryAlignment;

        [HideInInspector] public bool NeedsRebuild;

        [Tooltip(
            "Forces the layout group to rebuild every frame while in Play mode.  WARNING: This may have performance implications for very large / complex layouts, use with caution")]
        public bool ForceContinuousRebuild;

        [Tooltip(
            "If true, child GameObjects that are not activeSelf will be ignored for the layout.  Otherwise, inactive child GameObjects will be included in the layout.")]
        public bool IgnoreDeactivatedElements = true;

        public void RebuildLayout()
        {
            PopulateElementsFromChildren();

            switch (Style)
            {
                case LayoutStyle.Linear:
                    LinearLayout();
                    break;

                case LayoutStyle.Grid:
                    GridLayout();
                    break;

                case LayoutStyle.Euclidean:
                    EuclideanLayout();
                    break;

                case LayoutStyle.Radial:
                    RadialLayout();
                    break;
            }

            NeedsRebuild = false;
        }

        private void LinearLayout()
        {
            Vector3 pos = Vector3.zero;
            Vector3 alignmentOffset = GetVectorForAxis(LayoutAxis);

            // Calculate alignment offset amount
            switch (PrimaryAlignment)
            {
                case Alignment.Min:
                    alignmentOffset = Vector3.zero;
                    break;
                case Alignment.Center:
                    alignmentOffset *= -(LayoutElements.Count - 1) * (ElementDimensions.x + Spacing) / 2f;
                    break;
                case Alignment.Max:
                    alignmentOffset *= -(LayoutElements.Count - 1) * (ElementDimensions.x + Spacing);
                    break;
            }

            for (int i = 0; i < LayoutElements.Count; i++)
            {
                Vector3 dimensions = ElementDimensions;

                switch (LayoutAxis)
                {
                    case LayoutAxis3D.X:
                        pos.x = i * (dimensions.x + Spacing);
                        break;
                    case LayoutAxis3D.Y:
                        pos.y = i * (dimensions.y + Spacing);
                        break;
                    case LayoutAxis3D.Z:
                        pos.z = i * (dimensions.z + Spacing);
                        break;
                }

                LayoutElements[i].localPosition = pos + StartPositionOffset + alignmentOffset;
            }
        }

        private void GridLayout()
        {
            Vector3 pos = Vector3.zero;

            int primaryCount = GridConstraintCount;
            int secondaryCount = Mathf.CeilToInt((float)LayoutElements.Count / primaryCount);

            float pDim = GridLayoutAxis == LayoutAxis2D.X ? ElementDimensions.x : ElementDimensions.y;
            float sDim = GridLayoutAxis == LayoutAxis2D.X ? ElementDimensions.y : ElementDimensions.x;

            LayoutAxis2D secondaryAxis = GridLayoutAxis == LayoutAxis2D.X ? LayoutAxis2D.Y : LayoutAxis2D.X;

            // Calculate primary alignment offset
            Vector3 alignmentOffset = Vector3.zero;
            switch (PrimaryAlignment)
            {
                case Alignment.Min:
                    break;
                case Alignment.Center:
                    alignmentOffset -= GetVectorForAxis(GridLayoutAxis) *
                                       ((GridConstraintCount - 1) * (pDim + Spacing)) /
                                       2f;
                    break;
                case Alignment.Max:
                    alignmentOffset -= GetVectorForAxis(GridLayoutAxis) *
                                       ((GridConstraintCount - 1) * (pDim + Spacing));
                    break;
            }

            // Calculate secondary alignment offset
            switch (SecondaryAlignment)
            {
                case Alignment.Min:
                    break;
                case Alignment.Center:
                    alignmentOffset -= GetVectorForAxis(secondaryAxis) * ((secondaryCount - 1) * (sDim + Spacing)) / 2f;
                    break;
                case Alignment.Max:
                    alignmentOffset -= GetVectorForAxis(secondaryAxis) * ((secondaryCount - 1) * (sDim + Spacing));
                    break;
            }

            int i = 0;

            for (int s = 0; s < secondaryCount; s++)
            {
                for (int p = 0; p < primaryCount; p++)
                {
                    if (i >= LayoutElements.Count) continue;
                    float pOffset = p * (pDim + Spacing);
                    float sOffset = s * (sDim + Spacing);

                    pos.x = GridLayoutAxis == LayoutAxis2D.X ? pOffset : sOffset;
                    pos.y = GridLayoutAxis == LayoutAxis2D.X ? sOffset : pOffset;

                    LayoutElements[i++].localPosition = pos + StartPositionOffset + alignmentOffset;
                }
            }
        }

        private void EuclideanLayout()
        {
            Vector3 pos = Vector3.zero;

            int i = 0;

            int primaryCount = GridConstraintCount;
            int secondaryCount = SecondaryConstraintCount;
            int tertiaryCount = Mathf.CeilToInt((float)LayoutElements.Count / (primaryCount * secondaryCount));

            // Bit mask to determine final driven axis (001 = X, 010 = Y, 100 = Z)
            int tertiaryAxisMask = 7;
            LayoutAxis3D tertiaryAxis = LayoutAxis3D.X;

            #region Determine Element Dimensions in Each Axis

            float pDim = 0f, sDim = 0f, tDim = 0f;

            switch (LayoutAxis)
            {
                case LayoutAxis3D.X:
                    pDim = ElementDimensions.x;
                    tertiaryAxisMask ^= 1;
                    break;
                case LayoutAxis3D.Y:
                    pDim = ElementDimensions.y;
                    tertiaryAxisMask ^= 2;
                    break;
                case LayoutAxis3D.Z:
                    pDim = ElementDimensions.z;
                    tertiaryAxisMask ^= 4;
                    break;
            }

            switch (SecondaryLayoutAxis)
            {
                case LayoutAxis3D.X:
                    sDim = ElementDimensions.x;
                    tertiaryAxisMask ^= 1;
                    break;
                case LayoutAxis3D.Y:
                    sDim = ElementDimensions.y;
                    tertiaryAxisMask ^= 2;
                    break;
                case LayoutAxis3D.Z:
                    sDim = ElementDimensions.z;
                    tertiaryAxisMask ^= 4;
                    break;
            }

            switch (tertiaryAxisMask)
            {
                case 1:
                    tDim = ElementDimensions.x;
                    break;
                case 2:
                    tDim = ElementDimensions.y;
                    break;
                case 4:
                    tDim = ElementDimensions.z;
                    break;
            }

            switch (tertiaryAxisMask)
            {
                case 1:
                    tertiaryAxis = LayoutAxis3D.X;
                    break;
                case 2:
                    tertiaryAxis = LayoutAxis3D.Y;
                    break;
                case 4:
                    tertiaryAxis = LayoutAxis3D.Z;
                    break;
            }

            #endregion


            Vector3 alignmentOffset = Vector3.zero;

            #region Calculate alignment offset vectors

            // Calculate primary alignment offset
            switch (PrimaryAlignment)
            {
                case Alignment.Min:
                    break;
                case Alignment.Center:
                    alignmentOffset -= GetVectorForAxis(LayoutAxis) * ((primaryCount - 1) * (pDim + Spacing)) / 2f;
                    break;
                case Alignment.Max:
                    alignmentOffset -= GetVectorForAxis(LayoutAxis) * ((primaryCount - 1) * (pDim + Spacing));
                    break;
            }

            // Calculate secondary alignment offset
            switch (SecondaryAlignment)
            {
                case Alignment.Min:
                    break;
                case Alignment.Center:
                    alignmentOffset -= GetVectorForAxis(SecondaryLayoutAxis) *
                                       ((secondaryCount - 1) * (sDim + Spacing)) /
                                       2f;
                    break;
                case Alignment.Max:
                    alignmentOffset -= GetVectorForAxis(SecondaryLayoutAxis) *
                                       ((secondaryCount - 1) * (sDim + Spacing));
                    break;
            }

            // Calculate tertiary alignment offset
            switch (TertiaryAlignment)
            {
                case Alignment.Min:
                    break;
                case Alignment.Center:
                    alignmentOffset -= GetVectorForAxis(tertiaryAxis) * ((tertiaryCount - 1) * (tDim + Spacing)) / 2f;
                    break;
                case Alignment.Max:
                    alignmentOffset -= GetVectorForAxis(tertiaryAxis) * ((tertiaryCount - 1) * (tDim + Spacing));
                    break;
            }

            #endregion

            for (int t = 0; t < tertiaryCount; t++)
            {
                for (int s = 0; s < secondaryCount; s++)
                {
                    for (int p = 0; p < primaryCount; p++)
                    {
                        if (i >= LayoutElements.Count) continue;
                        float pOffset = p * (pDim + Spacing);
                        float sOffset = s * (sDim + Spacing);
                        float tOffset = t * (tDim + Spacing);

                        switch (LayoutAxis)
                        {
                            case LayoutAxis3D.X:
                                pos.x = pOffset;
                                break;
                            case LayoutAxis3D.Y:
                                pos.y = pOffset;
                                break;
                            case LayoutAxis3D.Z:
                                pos.z = pOffset;
                                break;
                        }

                        switch (SecondaryLayoutAxis)
                        {
                            case LayoutAxis3D.X:
                                pos.x = sOffset;
                                break;
                            case LayoutAxis3D.Y:
                                pos.y = sOffset;
                                break;
                            case LayoutAxis3D.Z:
                                pos.z = sOffset;
                                break;
                        }

                        switch (tertiaryAxisMask)
                        {
                            case 1:
                                pos.x = tOffset;
                                break;
                            case 2:
                                pos.y = tOffset;
                                break;
                            case 4:
                                pos.z = tOffset;
                                break;
                        }

                        LayoutElements[i++].localPosition = pos + StartPositionOffset + alignmentOffset;
                    }
                }
            }
        }

        private void RadialLayout()
        {
            Vector3 pos = Vector3.zero;
            float spiralSum = 0f;
            float spiralIncrement = SpiralFactor / LayoutElements.Count;

            if (UseFullCircle)
            {
                MaxArcAngle = 360f - 360f / LayoutElements.Count;
            }

            for (int i = 0; i < LayoutElements.Count; i++)
            {
                float angle = (float)i / (LayoutElements.Count - 1) * MaxArcAngle * Mathf.Deg2Rad;
                pos.x = Mathf.Cos(angle + Mathf.Deg2Rad * StartAngleOffset) * (Radius + spiralSum);
                pos.y = Mathf.Sin(angle + Mathf.Deg2Rad * StartAngleOffset) * (Radius + spiralSum);

                if (AlignToRadius)
                {
                    Vector3 dir = transform.TransformDirection(pos);
                    LayoutElements[i].up = dir;
                }

                LayoutElements[i].localPosition = pos + StartPositionOffset;
                spiralSum += spiralIncrement;
            }
        }

        private void PopulateElementsFromChildren()
        {
            LayoutElements ??= new List<Transform>();

            ElementRotations ??= new List<Quaternion>();

            LayoutElements.Clear();
            foreach (Transform child in transform)
            {
                if (child.gameObject.activeSelf || !IgnoreDeactivatedElements)
                {
                    LayoutElements.Add(child);
                }
            }
        }

        public void RecordRotations()
        {
            if (LayoutElements == null)
            {
                return;
            }

            ElementRotations ??= new List<Quaternion>();

            if (HasChildCountChanged())
            {
                PopulateElementsFromChildren();
            }

            ElementRotations.Clear();

            for (int i = 0; i < LayoutElements.Count; i++)
            {
                ElementRotations.Add(LayoutElements[i].localRotation);
            }
        }

        public void RestoreRotations()
        {
            if (LayoutElements == null || ElementRotations == null || LayoutElements.Count != ElementRotations.Count)
            {
                return;
            }

            for (int i = 0; i < LayoutElements.Count; i++)
            {
                LayoutElements[i].localRotation = ElementRotations[i];
            }
        }

        private bool HasChildCountChanged()
        {
            if (LayoutElements == null) return false;
            if (IgnoreDeactivatedElements) return GetNumActiveChildren() != LayoutElements.Count;
            return transform.childCount != LayoutElements.Count;
        }

        private int GetNumActiveChildren()
        {
            int num = 0;
            foreach (Transform child in transform)
            {
                if (child.gameObject.activeSelf)
                {
                    num++;
                }
            }

            return num;
        }

        private static Vector3 GetVectorForAxis(LayoutAxis3D axis3D)
        {
            Vector3 axis = Vector3.zero;
            // Calculate alignment direction
            switch (axis3D)
            {
                case LayoutAxis3D.X:
                    axis = Vector3.right;
                    break;
                case LayoutAxis3D.Y:
                    axis = Vector3.up;
                    break;
                case LayoutAxis3D.Z:
                    axis = Vector3.forward;
                    break;
            }

            return axis;
        }

        private static Vector3 GetVectorForAxis(LayoutAxis2D axis2D)
        {
            Vector3 axis = axis2D switch
            {
                LayoutAxis2D.X => Vector3.right,
                LayoutAxis2D.Y => Vector3.up,
                _ => Vector3.zero
            };

            return axis;
        }

        private void Update()
        {
            if (NeedsRebuild || HasChildCountChanged())
            {
                RebuildLayout();
            }
            else if (Application.IsPlaying(gameObject) && ForceContinuousRebuild)
            {
                RebuildLayout();
            }
        }

        public void OnTransformChildrenChanged()
        {
            NeedsRebuild = true;
            Debug.Log("Transform children changed");
        }
    }
}