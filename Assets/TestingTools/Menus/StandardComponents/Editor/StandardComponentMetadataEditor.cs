using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MarblePhysics.Modding.Test
{
    [CustomEditor(typeof(StandardComponentMetadata))]
    public class StandardComponentMetadataEditor : Editor
    {
        private StandardComponentMetadata metadata;
        
        private int addTagIndex = 0;
        private HashSet<string> activeTags;
        private HashSet<string> allTags;
        private string[] allTagsArray;

        private void OnEnable()
        {
            metadata = target as StandardComponentMetadata;
            allTagsArray = StandardComponentTagManager.GetTags().Select(t => t.Name).Prepend("Add Tag").ToArray();
            allTags = new HashSet<string>(allTagsArray);
            
            CleanBadTags();

            
            activeTags = new HashSet<string>();
        }

        private void OnDisable()
        {
            CleanBadTags();
        }

        private void CleanBadTags()
        {
            for (int i = metadata.Tags.Count - 1; i >= 0; i--)
            {
                string metadataTag = metadata.Tags[i];
                if (!allTags.Contains(metadataTag))
                {
                    metadata.Tags.RemoveAt(i);
                    EditorUtility.SetDirty(target);
                }
            } 
        }

        public override void OnInspectorGUI()
        {
            activeTags.Clear();
            activeTags.AddRange(metadata.Tags);
            
            Object sourceObject = PrefabUtility.GetCorrespondingObjectFromOriginalSource(target);

            if (sourceObject == target)
            {
                base.OnInspectorGUI();
                
                addTagIndex = EditorGUILayout.Popup(addTagIndex, allTagsArray);
                if (addTagIndex != 0)
                {
                    string newTag = allTagsArray[addTagIndex];
                    if (activeTags.Add(newTag))
                    {
                        Undo.RecordObject(metadata, "Add new tag");
                        metadata.Tags.Add(newTag);
                        EditorUtility.SetDirty(metadata.gameObject);
                    }
                    addTagIndex = 0;
                }
            }
            else
            {
                PrefabAssetType prefabAssetType = PrefabUtility.GetPrefabAssetType(target);
                bool isOutermostRoot = PrefabUtility.IsOutermostPrefabInstanceRoot(metadata.gameObject);

                if (prefabAssetType == PrefabAssetType.Regular && isOutermostRoot)
                {
                    if (GUILayout.Button("Unlink from Prefab"))
                    {
                        PrefabUtility.UnpackPrefabInstance(metadata.gameObject, PrefabUnpackMode.OutermostRoot, InteractionMode.UserAction);
                    }

                    if (GUILayout.Button("Show Asset"))
                    {
                        EditorGUIUtility.PingObject(sourceObject);
                    }
                }
                else
                {
                    GUILayout.Label("This is only editable in the Project Panel");
                }
            }
        }
    }
}