using System;
using UnityEditor;
using UnityEngine;

namespace MarblePhysics.Modding
{
    [CustomEditor(typeof(EnvironmentShaper))]
    public class EnvironmentShaperEditor : Editor
    {
        private EnvironmentShaper environmentShaper;

        private void OnEnable()
        {
            environmentShaper = target as EnvironmentShaper;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }

        private void OnSceneGUI()
        {
            foreach (PolygonCollider2D collider in environmentShaper.CutColliders)
            {
                DrawHandle(collider.transform);
            }
            
            void DrawHandle(Transform target)
            {
                float size = HandleUtility.GetHandleSize(target.position) * 0.1f;
                
                EditorGUI.BeginChangeCheck();
                Vector3 newTargetPosition = Handles.FreeMoveHandle(target.position, size, Vector2.zero, Handles.CircleHandleCap);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(target, "Dragged Target");
                    target.position = newTargetPosition;
                }
            }
        }
    }
}