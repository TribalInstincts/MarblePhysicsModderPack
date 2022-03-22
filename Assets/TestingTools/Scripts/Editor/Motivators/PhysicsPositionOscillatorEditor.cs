using UnityEditor;
using UnityEngine;

namespace MarblePhysics.Modding.Test
{
    [CustomEditor(typeof(PhysicsPositionOscillator))]
    [CanEditMultipleObjects]
    public class PhysicsPositionOscillatorEditor : UnityEditor.Editor
    {
        private SerializedProperty targetRigidBodyProp;
        private SerializedProperty floatGeneratorProp;
        private SerializedProperty pointAProp;
        private SerializedProperty pointBProp;

        private static bool play = false;
        private float sliderValue = .5f;

        private PhysicsPositionOscillator oscillator;

        protected virtual void OnEnable()
        {
            oscillator = target as PhysicsPositionOscillator;
            sliderValue = .5f;
            
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
            }
        }

        private void OnDisable()
        {
            if (AreRequiresValuesSet())
            {
                oscillator.TargetRigidBody.transform.position = Vector3.Lerp(oscillator.PointA.position, oscillator.PointB.position, .5f);
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

                if (!play)
                {
                    sliderValue = EditorGUILayout.Slider("Preview Position", sliderValue, 0f, 1f);
                    
                    oscillator.TargetRigidBody.transform.position = Vector3.Lerp(oscillator.PointA.position, oscillator.PointB.position, sliderValue);
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
                }
                
                bool DrawHandle(Transform target)
                {
                    float size = HandleUtility.GetHandleSize(target.position) * 0.1f;
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
