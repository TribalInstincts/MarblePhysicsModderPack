using System;
using UnityEditor;
using UnityEngine;

namespace MarblePhysics
{
    [CustomEditor(typeof(PhysicsPositionOscillator))]
    [CanEditMultipleObjects]
    public class PhysicsPositionOscillatorEditor : UnityEditor.Editor
    {
        private SerializedProperty targetRigidBodyProp;
        private SerializedProperty floatGeneratorProp;
        private SerializedProperty pointAProp;
        private SerializedProperty pointBProp;

        private float initialLerpValue;
        private static bool play = false;

        protected virtual void OnEnable()
        {
            targetRigidBodyProp = serializedObject.FindProperty("targetRigidBody");
            floatGeneratorProp = serializedObject.FindProperty("floatGenerator");
            pointAProp = serializedObject.FindProperty("pointA");
            pointBProp = serializedObject.FindProperty("pointB");

            if (AreRequiresValuesSet())
            {
                Transform pointA = pointAProp.objectReferenceValue as Transform;
                Transform pointB = pointBProp.objectReferenceValue as Transform;
                Rigidbody2D target = targetRigidBodyProp.objectReferenceValue as Rigidbody2D;
                Vector2 point = MathUtils.ClosestPointOnLine(pointA.position, pointB.position, target.transform.position);
                target.transform.position = point;
                initialLerpValue = (Vector2.Distance(point, pointB.position) / Vector2.Distance(pointA.position, pointB.position));
            }
        }

        private void OnDisable()
        {
            if (AreRequiresValuesSet())
            {
                Transform pointA = pointAProp.objectReferenceValue as Transform;
                Transform pointB = pointBProp.objectReferenceValue as Transform;
                Rigidbody2D target = targetRigidBodyProp.objectReferenceValue as Rigidbody2D;
                target.transform.position = Vector3.Lerp(pointA.position, pointB.position, initialLerpValue);
            }
        }
        

        private bool AreRequiresValuesSet()
        {
            return floatGeneratorProp.objectReferenceValue != null && targetRigidBodyProp.objectReferenceValue != null && pointAProp.objectReferenceValue != null &&
                   pointBProp.objectReferenceValue != null;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            
            if (AreRequiresValuesSet())
            {
                string buttonString = "Play";
                if (play)
                {
                    buttonString = "Stop";
                }
                if (GUILayout.Button(buttonString))
                {
                    play = !play;
                    SceneView.RepaintAll();
                    Debug.Log("playing...");
                }
            }
        }

        private void OnSceneGUI()
        {
            if (AreRequiresValuesSet())
            {
                
                Transform pointA = pointAProp.objectReferenceValue as Transform;
                Transform pointB = pointBProp.objectReferenceValue as Transform;
                Rigidbody2D targetRigidBody = targetRigidBodyProp.objectReferenceValue as Rigidbody2D;

                bool positionChanged = DrawHandle(pointA) || DrawHandle(pointB) || DrawHandle(targetRigidBody.transform);

                Handles.color = Color.red;
                PhysicsPositionOscillator oscillator = (PhysicsPositionOscillator) target;
                Handles.DrawLine(pointA.position, pointB.position, 2f);

                if (positionChanged)
                {
                    Vector2 point = MathUtils.ClosestPointOnLine(pointA.position, pointB.position, targetRigidBody.transform.position);
                    targetRigidBody.transform.position = point;
                    initialLerpValue = (Vector2.Distance(point, pointB.position) / Vector2.Distance(pointA.position, pointB.position));
                }
                
                bool DrawHandle(Transform target)
                {
                    float size = HandleUtility.GetHandleSize(pointA.position) * 0.1f;
                    Vector3 snap = Vector3.one * 0.5f;

                    EditorGUI.BeginChangeCheck();
                    Vector3 newTargetPosition = Handles.FreeMoveHandle(target.position, size, snap, Handles.CircleHandleCap);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(target, "Dragged Position");
                        target.position = newTargetPosition;
                        return true;
                    }

                    return false;
                }


                if (play)
                {
                    Debug.Log("playing...");
                    targetRigidBody.transform.position = oscillator.GetNextPosition();
                    SceneView.RepaintAll();
                }
            }
        }
    }
}
