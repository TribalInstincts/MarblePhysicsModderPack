using System.Collections.Generic;
using System.Linq;
using MarblePhysics.Modding.Shared.Extensions;
using MarblePhysics.Modding.Shared.Level;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MarblePhysics.Modding.StandardComponents
{
    public class StandardComponentsWindow : EditorWindow
    {
        private static readonly string prefabPath = "Assets/_StandardComponents/_Prefabs";
        
        private int currentTab = default;
        private float nextPrefabRefresh = 0;
        private Dictionary<string, List<StandardComponentMetadata>> tagToPrefabs;
        private string[] tabs;

        [MenuItem("Window/Standard Components Window")]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            StandardComponentsWindow window = (StandardComponentsWindow) GetWindow(typeof(StandardComponentsWindow));
            window.titleContent.text = "Standard Components";
        }

        private void OnGUI()
        {
            GetPrefabs();
            if (tabs.Length > 0)
            {   
                currentTab = GUILayout.Toolbar(currentTab, tabs);

                DrawTab(tabs[currentTab]);
            }
        }

        private void DrawTab(string tab)
        {
            foreach (StandardComponentMetadata standardComponentMetadata in tagToPrefabs[tab])
            {
                if (GUILayout.Button(new GUIContent(standardComponentMetadata.Name, standardComponentMetadata.Description)))
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
                    
                    Object instantiatePrefab = null;
                    if (Selection.activeTransform != null)
                    {
                        instantiatePrefab = PrefabUtility.InstantiatePrefab(standardComponentMetadata.gameObject, Selection.activeTransform);
                        instantiatePrefab.GetComponent<Transform>().ClearLocal();
                    }
                    else
                    {
                        PrefabStage currentPrefabStage = PrefabStageUtility.GetCurrentPrefabStage();
                        if (currentPrefabStage != null)
                        {
                            instantiatePrefab = PrefabUtility.InstantiatePrefab(standardComponentMetadata.gameObject, currentPrefabStage.prefabContentsRoot.transform);
                        }
                        else
                        {
                            instantiatePrefab = PrefabUtility.InstantiatePrefab(standardComponentMetadata.gameObject);
                        }
                    }
                    
                    
                    Selection.activeObject = instantiatePrefab;
                }
            }
        }

        private void GetPrefabs()
        {
            if (Time.realtimeSinceStartup > nextPrefabRefresh || tagToPrefabs == null)
            {
                nextPrefabRefresh = Time.realtimeSinceStartup + 5f;
                
                if (tagToPrefabs == null)
                {
                    tagToPrefabs = new Dictionary<string, List<StandardComponentMetadata>>();
                }
                else
                {
                    tagToPrefabs.Clear();
                }

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

                tabs = tagToPrefabs.Keys.ToArray();
            }

            void AddToTag(string tag, StandardComponentMetadata metadata)
            {
                if (!tagToPrefabs.TryGetValue(tag, out List<StandardComponentMetadata> datas))
                {
                    datas = new List<StandardComponentMetadata>();
                    tagToPrefabs[tag] = datas;
                }
                datas.Add(metadata);
            }
        }
    }
}