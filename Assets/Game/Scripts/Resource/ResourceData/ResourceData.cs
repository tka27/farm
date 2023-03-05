using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Game.Scripts.Resource.ResourceData
{
    [CreateAssetMenu(fileName = "New ResourceData", menuName = "Resource Data")]
    public class ResourceData : ScriptableObject
    {
        [SerializeField] private ResourceType _type;

        [PreviewField(200, ObjectFieldAlignment.Left), SerializeField]
        private Sprite _resourceIcon;

        public ResourceType Type => _type;
        public Sprite ResourceIcon => _resourceIcon;

#if UNITY_EDITOR
        [Button(ButtonSizes.Large), GUIColor(0, 1, 0)]
        private void ApplyName()
        {
            var assetPath = AssetDatabase.GetAssetPath(this);
            AssetDatabase.RenameAsset(assetPath, _type.ToString());
            AssetDatabase.Refresh();
        }
#endif
    }
}