using UnityEditor;
using UnityEngine;

namespace MarblePhysics.Modding.StandardComponents
{
    [CustomEditor(typeof(StandardComponentMetadata))]
    public class StandardComponentMetadataEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            StandardComponentMetadata metadata = target as StandardComponentMetadata;
            Object sourceObject = PrefabUtility.GetCorrespondingObjectFromOriginalSource(target);

            if (sourceObject == target)
            {
                base.OnInspectorGUI();
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