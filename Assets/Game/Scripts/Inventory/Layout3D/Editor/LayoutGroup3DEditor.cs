using UnityEditor;
using UnityEngine;

namespace Game.Scripts.Inventory.Layout3D.Editor
{
    [CustomEditor(typeof(LayoutGroup3D))]
    public class LayoutGroup3DEditor : UnityEditor.Editor
    {
        private LayoutGroup3D _layoutGroup;

        private LayoutStyle _style;
        private float _spacing;
        private Vector3 _elementDimensions;

        private int _gridConstraintCount;
        private int _secondaryConstraintCount;

        private bool _useFullCircle;
        private float _maxArcAngle;
        private float _radius;
        private float _startAngleOffset;
        private bool _alignToRadius;
        private float _spiralFactor;
        private LayoutAxis3D _layoutAxis;
        private LayoutAxis3D _secondaryLayoutAxis;
        private LayoutAxis2D _gridLayoutAxis;
        private Vector3 _startPositionOffset;
        private Alignment _primaryAlignment;
        private Alignment _secondaryAlignment;
        private Alignment _tertiaryAlignment;

        public override void OnInspectorGUI()
        {

            _layoutGroup = target as LayoutGroup3D;

            DrawDefaultInspector();

            bool shouldRebuild = false;

            // Record rotations of all children if not forcing alignment in radial mode
            if (!(_layoutGroup.Style == LayoutStyle.Radial && _layoutGroup.AlignToRadius))
            {
                _layoutGroup.RecordRotations();
            }

            // Element Dimensions
            EditorGUI.BeginChangeCheck();

            _elementDimensions = EditorGUILayout.Vector3Field("Element Dimensions", _layoutGroup.ElementDimensions);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(_layoutGroup, "Change Element Dimensions");
                _layoutGroup.ElementDimensions = _elementDimensions;
                shouldRebuild = true;
            }

            // Start Offset
            EditorGUI.BeginChangeCheck();

            _startPositionOffset = EditorGUILayout.Vector3Field("Start Position Offset", _layoutGroup.StartPositionOffset);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(_layoutGroup, "Change Position Offset");
                _layoutGroup.StartPositionOffset = _startPositionOffset;
                shouldRebuild = true;
            }

            EditorGUI.BeginChangeCheck();

            _style = (LayoutStyle)EditorGUILayout.EnumPopup("Layout Style", _layoutGroup.Style);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(_layoutGroup, "Change Layout Style");
                _layoutGroup.Style = _style;
                shouldRebuild = true;
            }

            EditorGUI.BeginChangeCheck();

            if (_style == LayoutStyle.Linear)
            {
                _layoutAxis = (LayoutAxis3D)EditorGUILayout.EnumPopup("Layout Axis", _layoutGroup.LayoutAxis);
                _primaryAlignment = (Alignment)EditorGUILayout.EnumPopup("Alignment", _layoutGroup.PrimaryAlignment);

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(_layoutGroup, "Change Layout Axis");
                    _layoutGroup.LayoutAxis = _layoutAxis;
                    _layoutGroup.PrimaryAlignment = _primaryAlignment;
                    shouldRebuild = true;
                }
            }
            else if (_style == LayoutStyle.Grid)
            {
                _gridLayoutAxis = (LayoutAxis2D)EditorGUILayout.EnumPopup("Primary Layout Axis", _layoutGroup.GridLayoutAxis);
                _gridConstraintCount = EditorGUILayout.IntField("Constraint Count", _layoutGroup.GridConstraintCount);

                string pAlignStr = _gridLayoutAxis == LayoutAxis2D.X ? "X Alignment" : "Y Alignment";
                string sAlignStr = _gridLayoutAxis == LayoutAxis2D.X ? "Y Alignment" : "X Alignment";

                _primaryAlignment = (Alignment)EditorGUILayout.EnumPopup(pAlignStr, _layoutGroup.PrimaryAlignment);
                _secondaryAlignment = (Alignment)EditorGUILayout.EnumPopup(sAlignStr, _layoutGroup.SecondaryAlignment);

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(_layoutGroup, "Change Grid Layout Options");
                    _layoutGroup.GridConstraintCount = _gridConstraintCount;
                    _layoutGroup.GridLayoutAxis = _gridLayoutAxis;
                    _layoutGroup.PrimaryAlignment = _primaryAlignment;
                    _layoutGroup.SecondaryAlignment = _secondaryAlignment;
                    shouldRebuild = true;
                }
            }
            else if (_style == LayoutStyle.Euclidean)
            {
                _layoutAxis = (LayoutAxis3D)EditorGUILayout.EnumPopup("Primary Layout Axis", _layoutGroup.LayoutAxis);
                _secondaryLayoutAxis = (LayoutAxis3D)EditorGUILayout.EnumPopup("Secondary Layout Axis", _layoutGroup.SecondaryLayoutAxis);

                _gridConstraintCount = EditorGUILayout.IntField("Primary Constraint Count", _layoutGroup.GridConstraintCount);
                _secondaryConstraintCount = EditorGUILayout.IntField("Secondary Constraint Count", _layoutGroup.SecondaryConstraintCount);

                _primaryAlignment = (Alignment)EditorGUILayout.EnumPopup("Primary Alignment", _layoutGroup.PrimaryAlignment);
                _secondaryAlignment = (Alignment)EditorGUILayout.EnumPopup("Secondary Alignment", _layoutGroup.SecondaryAlignment);
                _tertiaryAlignment = (Alignment)EditorGUILayout.EnumPopup("Tertiary Alignment", _layoutGroup.TertiaryAlignment);

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(_layoutGroup, "Change Euclidean Layout Options");
                    _layoutGroup.GridConstraintCount = _gridConstraintCount;
                    _layoutGroup.SecondaryConstraintCount = _secondaryConstraintCount;
                    _layoutGroup.LayoutAxis = _layoutAxis;
                    _layoutGroup.SecondaryLayoutAxis = _secondaryLayoutAxis;
                    _layoutGroup.PrimaryAlignment = _primaryAlignment;
                    _layoutGroup.SecondaryAlignment = _secondaryAlignment;
                    _layoutGroup.TertiaryAlignment = _tertiaryAlignment;
                    shouldRebuild = true;
                }
            }
            else if (_style == LayoutStyle.Radial)
            {
                _useFullCircle = EditorGUILayout.Toggle("Use Full Circle", _layoutGroup.UseFullCircle);
                if(!_useFullCircle)
                {
                    _maxArcAngle = EditorGUILayout.FloatField("Max Arc Angle", _layoutGroup.MaxArcAngle);
                }
                else
                {
                    int childCount = _layoutGroup.transform.childCount;
                    _maxArcAngle = 360f - 360f / childCount;
                }
                _radius = EditorGUILayout.FloatField("Radius", _layoutGroup.Radius);
                _startAngleOffset = EditorGUILayout.FloatField("Start Angle Offset", _layoutGroup.StartAngleOffset);
                _spiralFactor = EditorGUILayout.FloatField("Spiral Factor", _layoutGroup.SpiralFactor);
                _alignToRadius = EditorGUILayout.Toggle("Align To Radius", _layoutGroup.AlignToRadius);

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(_layoutGroup, "Change Radial Layout Options");
                    _layoutGroup.UseFullCircle = _useFullCircle;
                    _layoutGroup.MaxArcAngle = _maxArcAngle;
                    _layoutGroup.Radius = _radius;
                    _layoutGroup.StartAngleOffset = _startAngleOffset;
                    _layoutGroup.SpiralFactor = _spiralFactor;
                    _layoutGroup.AlignToRadius = _alignToRadius;
                    shouldRebuild = true;
                }
            }

            if (_layoutGroup.Style != LayoutStyle.Radial)
            {
                EditorGUI.BeginChangeCheck();
                _spacing = EditorGUILayout.FloatField("Spacing", _layoutGroup.Spacing);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(_layoutGroup, "Change spacing");
                    _layoutGroup.Spacing = _spacing;
                    shouldRebuild = true;
                }
            }

            if (!(_layoutGroup.Style == LayoutStyle.Radial && _layoutGroup.AlignToRadius))
            {
                _layoutGroup.RestoreRotations();
            }

            if (shouldRebuild || _layoutGroup.NeedsRebuild || EditorUtility.IsDirty(_layoutGroup.transform))
            {
                _layoutGroup.RebuildLayout();
            }


        }

        private void OnEnable()
        {
            Undo.undoRedoPerformed += ForceRebuild;
        }

        private void OnDisable()
        {
            Undo.undoRedoPerformed -= ForceRebuild;
        }

        void ForceRebuild()
        {
            if(_layoutGroup)
            {
                _layoutGroup.RebuildLayout();
            }
        }

    }
}
