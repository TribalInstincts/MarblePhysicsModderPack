using UnityEditor;
using UnityEngine;

namespace MarblePhysics.Modding
{
    [CustomEditor(typeof(EnvironmentCanvas))]
    public class EnvironmentCanvasEditor : Editor
    {
        private EnvironmentCanvas environmentCanvas;
        private CutVolume[] cutVolumes;

        private void OnEnable()
        {
             environmentCanvas = target as EnvironmentCanvas;
             environmentCanvas.Init();
             cutVolumes = FindObjectsOfType<CutVolume>();
        }

        private void OnSceneGUI()
        {
            foreach (CutVolume activeCutVolume in cutVolumes)
            {
                DrawHandle(activeCutVolume.transform);
            }
            
            environmentCanvas.CutHoles();
            
            void DrawHandle(Transform cutTransform)
            {
                float size = HandleUtility.GetHandleSize(cutTransform.position) * 0.1f;
                
                EditorGUI.BeginChangeCheck();
                Vector3 newTargetPosition = Handles.FreeMoveHandle(cutTransform.position, size, Vector2.zero, Handles.CircleHandleCap);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(target, "Dragged Target");
                    cutTransform.position = newTargetPosition;
                }
            }
        }
    }
}
