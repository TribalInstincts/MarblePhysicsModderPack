using System;
using UnityEditor;
using UnityEngine;

namespace MarblePhysics.Modding.Test
{
    
    [CreateAssetMenu(fileName = "StandardComponentTagManager", menuName = "MarblePhysics/Util/StandardComponentTagManager")]
    public class StandardComponentTagManager : ScriptableObject
    {
        [Serializable]
        public struct Tag
        {
            public string Name;
            public Texture Icon;

            public GUIContent GetGUIContent()
            {
                if (Icon == null)
                {
                    return new GUIContent(Name, Name);
                }
                else
                {
                    return new GUIContent(Icon, Name);
                }
            }

            public bool Equals(Tag other)
            {
                return Name == other.Name;
            }

            public override bool Equals(object obj)
            {
                return obj is Tag other && Equals(other);
            }

            public override int GetHashCode()
            {
                return (Name != null ? Name.GetHashCode() : 0);
            }

            public static bool operator ==(Tag left, Tag right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(Tag left, Tag right)
            {
                return !left.Equals(right);
            }
        }

        [SerializeField]
        private Tag allTag = default;
        public Tag AllTag => allTag;
        
        [SerializeField]
        private Tag[] tags = default;

        public Tag[] Tags => tags;

        public static StandardComponentTagManager GetInstance()
        {
            string[] componentTags = AssetDatabase.FindAssets("t:" + nameof(StandardComponentTagManager));
            if (componentTags.Length != 1)
            {
                Debug.LogError("Must have exactly one StandardComponentTags in project!");
                return null;
            }

            return AssetDatabase.LoadAssetAtPath<StandardComponentTagManager>(AssetDatabase.GUIDToAssetPath(componentTags[0]));
        }
        
        public static Tag[] GetTags()
        {
            StandardComponentTagManager instance = GetInstance();
            return instance != null ? instance.Tags : new Tag[0];
        }
    }
}
