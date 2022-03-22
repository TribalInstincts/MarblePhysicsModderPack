using System;
using System.Collections.Generic;
using System.Linq;
using MarblePhysics.Modding.Shared.Extensions;
using MarblePhysics.Modding.Shared.Level;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
using Tag = MarblePhysics.Modding.Test.StandardComponentTagManager.Tag;

namespace MarblePhysics.Modding.Test
{
    public class StandardComponentsWindow : EditorWindow
    {
        private int currentTag = default;
        private int lastTag = 0;
        private Tag[] tags;
        private GUIContent[] tagGuiContent;
        
        private Dictionary<Tag, List<StandardComponentMetadata>> tabToPrefabs;

        private StandardComponentTagManager tagManager;
        private Vector2 scrollPosition = Vector2.zero;

        [MenuItem("Window/Standard Components Window")]
        static void OpenWindow()
        {
            // Get existing open window or if none, make a new one:
            StandardComponentsWindow window = (StandardComponentsWindow) GetWindow(typeof(StandardComponentsWindow));
            window.Refresh();
        }
        
        public void Refresh()
        {
            titleContent.text = "Standard Components";
            tagManager = StandardComponentTagManager.GetInstance();
            tabToPrefabs = new Dictionary<Tag, List<StandardComponentMetadata>>();
            
            tabToPrefabs.Add(tagManager.AllTag, new List<StandardComponentMetadata>());
            foreach (Tag tag in tagManager.Tags)
            {
                tabToPrefabs[tag] = new List<StandardComponentMetadata>();
            }

            GetPrefabs();
            
            foreach (Tag tag in tabToPrefabs.Keys.ToArray())
            {
                if (tabToPrefabs[tag].Count == 0)
                {
                    tabToPrefabs.Remove(tag);
                }
            }

            tags = tabToPrefabs.Keys.ToArray();
            tagGuiContent = tags.Select(t => t.GetGUIContent()).ToArray();
        }

        private void OnEnable()
        {
            Refresh();
        }

        private void OnGUI()
        {
            try
            {
                GUILayout.BeginHorizontal();
                
                for (int i = 0; i < tagGuiContent.Length; i++)
                {
                    if (GUILayout.Toggle(currentTag == i, tagGuiContent[i], "Button", GUILayout.Width(35), GUILayout.Height(35)))
                    {
                        currentTag = i;
                    }
                }

                if (currentTag != lastTag)
                {
                    scrollPosition = Vector2.zero;
                    lastTag = currentTag;
                }
                
                GUILayout.EndHorizontal();

                DrawTab(tags[currentTag]);
            }
            catch
            {
                Refresh();
            }
            
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Refresh"))
            {
                Refresh();
            }
        }

        private void DrawTab(Tag tag)
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            foreach (StandardComponentMetadata metadata in tabToPrefabs[tag])
            {
                if (GUILayout.Button(new GUIContent(metadata.Name, metadata.Description)))
                {
                    LevelRunner levelRunner = FindObjectOfType<LevelRunner>();
                    if (levelRunner != null)
                    {
                        SceneManager.SetActiveScene(levelRunner.gameObject.scene);
                        if (Selection.activeTransform != null && levelRunner.gameObject.scene != Selection.activeTransform.gameObject.scene)
                        {
                            Selection.activeTransform = null;
                        }
                    }

                    Object prefabInstance = null;
                    if (Selection.activeTransform != null)
                    {
                        prefabInstance = PrefabUtility.InstantiatePrefab(metadata.gameObject, Selection.activeTransform);
                        prefabInstance.GetComponent<Transform>().ClearLocal();
                    }
                    else
                    {
                        PrefabStage currentPrefabStage = PrefabStageUtility.GetCurrentPrefabStage();
                        if (currentPrefabStage != null)
                        {
                            prefabInstance = PrefabUtility.InstantiatePrefab(metadata.gameObject, currentPrefabStage.prefabContentsRoot.transform);
                        }
                        else
                        {
                            prefabInstance = PrefabUtility.InstantiatePrefab(metadata.gameObject);
                        }
                    }


                    GameObject gameObject = prefabInstance.GameObject();
                    PrefabUtility.UnpackPrefabInstance(gameObject, PrefabUnpackMode.OutermostRoot, InteractionMode.UserAction);
                    gameObject.name = metadata.Name;
                    if (gameObject.TryGetComponent(out StandardComponentMetadata md))
                    {
                        DestroyImmediate(md);
                    }

                    Selection.activeObject = prefabInstance;
                }
            }
            GUILayout.EndScrollView();
        }

        private void GetPrefabs()
        {
            string[] guids = AssetDatabase.FindAssets("t:Prefab");
            foreach (string guid in guids)
            {
                StandardComponentMetadata metadata = AssetDatabase.LoadAssetAtPath<StandardComponentMetadata>(AssetDatabase.GUIDToAssetPath(guid));
                if (metadata != null)
                {
                    AddToTag("All", metadata);
                    foreach (string prefabDataTag in metadata.Tags)
                    {
                        AddToTag(prefabDataTag, metadata);
                    }
                }
            }

            void AddToTag(string tag, StandardComponentMetadata metadata)
            {
                if (tabToPrefabs.TryGetValue(new Tag(){Name = tag}, out List<StandardComponentMetadata> datas))
                {
                    datas.Add(metadata);
                }

            }
        }
    }
}