using System;
using System.Collections.Generic;
using System.Reflection;
using Sirenix.OdinInspector;

namespace Game.Scripts.Extensions
{
#if UNITY_EDITOR

    using Sirenix.OdinInspector.Editor;

    public class AttributeProcessor<T> : OdinAttributeProcessor<T>
    {
        public override void ProcessChildMemberAttributes(InspectorProperty parentProperty, MemberInfo member,
            List<Attribute> attributes)
        {
            foreach (var attribute in attributes.ToArray())
            {
                if (attribute is ICustomAttribute metadata)
                {
                    attributes.AddRange(metadata.Attributes());
                }
            }
        }
    }
#endif

    public interface ICustomAttribute
    {
        public Attribute[] Attributes();
    }

    public class Group : Attribute, ICustomAttribute
    {
        private readonly string _name;
        private readonly bool _required;

        public Group(string name, bool required = true)
        {
            _name = name;
            _required = required;
        }

        public Attribute[] Attributes()
        {
            List<Attribute> list = new();
            if (_required) list.Add(new RequiredAttribute());
            list.Add(new FoldoutGroupAttribute(_name));
            return list.ToArray();
        }
    }

    public class GroupComponent : Attribute, ICustomAttribute
    {
        private readonly bool _required;

        public GroupComponent(bool required = true)
        {
            _required = required;
        }

        public Attribute[] Attributes()
        {
            List<Attribute> list = new();
            if (_required) list.Add(new RequiredAttribute());
            list.Add(new FoldoutGroupAttribute("Components", -2));
            return list.ToArray();
        }
    }

    public class GroupSetting : Attribute, ICustomAttribute
    {
        public Attribute[] Attributes()
        {
            var attributes = new Attribute[]
            {
                new FoldoutGroupAttribute("Settings", -1)
            };
            return attributes;
        }
    }

    public class GroupSceneObject : Attribute, ICustomAttribute
    {
        public Attribute[] Attributes()
        {
            var attributes = new Attribute[]
            {
                new SceneObjectsOnlyAttribute(),
                new RequiredAttribute(),
                new FoldoutGroupAttribute("SceneObject", -1)
            };
            return attributes;
        }
    }

    public class GroupAssets : Attribute, ICustomAttribute
    {
        public Attribute[] Attributes()
        {
            var attributes = new Attribute[]
            {
                new AssetsOnlyAttribute(),
                new RequiredAttribute(),
                new FoldoutGroupAttribute("Assets", -1)
            };
            return attributes;
        }
    }

    public class GroupView : Attribute, ICustomAttribute
    {
        public Attribute[] Attributes()
        {
            var attributes = new Attribute[]
            {
                new ShowInInspectorAttribute(),
                new ReadOnlyAttribute(),
                new FoldoutGroupAttribute("View")
            };
            return attributes;
        }
    }
}