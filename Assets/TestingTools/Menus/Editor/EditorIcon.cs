using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace MarblePhysics.Modding
{
    [CustomEditor(typeof(EditorIcon))]
    public class EditorIconsEditor : Editor
    {
        private EditorIcon editorIcon;
        
        private void OnEnable()
        {
            editorIcon = target as EditorIcon;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            if (editorIcon.TryGetValidKeys(out IEnumerable<EditorIcon.IconData> iconKeys))
            {
                if (GUILayout.Button("Update Constants File"))
                {
                    WriteConstFile(iconKeys);
                }
            }
            else
            {
                GUILayout.Label("ERROR! See log!");
            }
        }


        private static void WriteConstFile(IEnumerable<EditorIcon.IconData> values)
        {
            string contents = $@"//This class is auto-generated, do not modify (EditorIcons.cs)
namespace MarblePhysics.Modding
{{
    public partial class EditorIcon
    {{
        --ICONDATAS--

        public class Paths
        {{
            --PATHS--
        }}
    }}
}}"
                .Replace("--ICONDATAS--", string.Join(Environment.NewLine + "\t\t", values.Select(v => "public static IconData KEY => GetIcon(\"KEY\");".Replace("KEY", v.Key))))
                .Replace("--PATHS--", string.Join(Environment.NewLine + "\t\t\t", values.Select(v => $"public const string {v.Key} = \"{(AssetDatabase.GetAssetPath(v.Icon))}\";")))
                .Replace("\t", "    ");

            string relativePath = Path.Combine(Path.GetDirectoryName(EditorIcon.GetAssetPath()), "EditorIconConstants.cs");
            File.WriteAllText(Path.Combine(Application.dataPath, relativePath).Replace(@"Assets\Assets", "Assets"), contents);
            AssetDatabase.ImportAsset(relativePath, ImportAssetOptions.ForceUpdate);
        }
    }
    
    [CreateAssetMenu(fileName = "EditorIcon", menuName = "MarblePhysics/Util/EditorIcon")]
    public partial class EditorIcon : ScriptableObject
    {
        [Serializable]
        public struct IconData
        {
            public string Key;
            public Texture Icon;
            
            public GUIContent GetGUIContent(string tooltip)
            {
                return new GUIContent(Icon, tooltip);
            }

            public bool DrawButton(string tooltip = "", float size = 35)
            {
                return GUILayout.Button(GetGUIContent(tooltip), GUILayout.Width(size), GUILayout.Height(size));
            }

            public bool DrawToggle(bool isEnabled, string tooltip = "", float size = 35)
            {
                return GUILayout.Toggle(isEnabled, GetGUIContent(tooltip), "Button", GUILayout.Width(35), GUILayout.Height(35));
            }
            
            public bool Equals(IconData other)
            {
                return Key == other.Key;
            }

            public override bool Equals(object obj)
            {
                return obj is IconData other && Equals(other);
            }

            public override int GetHashCode()
            {
                return (Key != null ? Key.GetHashCode() : 0);
            }

            public static bool operator ==(IconData left, IconData right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(IconData left, IconData right)
            {
                return !left.Equals(right);
            }
        }

        private static Regex validNameRegex = new Regex(@"^[a-zA-Z_]\w*(\.[a-zA-Z_]\w*)*$");
        private static EditorIcon instance;

        [SerializeField]
        private IconData defaultIcon = default;

        [SerializeField]
        private IconData[] icons = default;

        private HashSet<IconData> iconsSet;

        public static IconData GetIcon(string key)
        {
            return GetInstance().Get(key);
        }

        private IconData Get(string key)
        {
            if (iconsSet == null || iconsSet.Count != icons.Length)
            {
                iconsSet = new HashSet<IconData>(icons.Where(i => !string.IsNullOrEmpty(i.Key)));
            }

            if (!iconsSet.TryGetValue(new IconData() {Key = key}, out IconData iconData))
            {
                iconData = defaultIcon;
            }

            return iconData;
        }

        public bool TryGetValidKeys(out IEnumerable<IconData> iconDatas)
        {
            if (iconsSet == null)
            {
                iconsSet = new HashSet<IconData>();
            }
            else
            {
                iconsSet.Clear();
            }

            bool hasError = false;
            for (int i = 0; i < icons.Length; i++)
            {
                IconData iconData = icons[i];
                if (iconData.Icon != null)
                {
                    if (string.IsNullOrWhiteSpace(iconData.Key))
                    {
                        iconData.Key = iconData.Icon.name;
                        icons[i] = iconData;
                    }

                    if (!validNameRegex.IsMatch(iconData.Key))
                    {
                        Debug.LogError($"Invalid name({iconData.Key})! Name Must: Start with a letter and only contain: letters, numbers, underscore. Index: " + i);
                        hasError = true;
                    }
                    else if (!iconsSet.Add(iconData))
                    {
                        Debug.LogError($"Duplicate Icon Names Found({iconData.Key})! Please rename one of them! Last index: " + i);
                        hasError = true;
                    }
                }
            }

            if (hasError)
            {
                iconDatas = null;
                return false;
            }
            else
            {
                iconDatas = iconsSet;
                return true;
            }
        }
        
        private static EditorIcon GetInstance()
        {
            if (instance == null)
            {
                instance = AssetDatabase.LoadAssetAtPath<EditorIcon>(GetAssetPath());
            }

            return instance;
        }

        public static string GetAssetPath()
        {
            string[] editorIcons = AssetDatabase.FindAssets("t:" + nameof(EditorIcon));
            if (editorIcons.Length != 1)
            {
                Debug.LogError("Must have exactly one EditorIcons in project! Current count: " + editorIcons.Length);
                return null;
            }

            return AssetDatabase.GUIDToAssetPath(editorIcons[0]);
        }
    }
}
