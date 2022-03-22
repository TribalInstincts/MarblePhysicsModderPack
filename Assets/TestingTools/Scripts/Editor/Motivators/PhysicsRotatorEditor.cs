using UnityEditor;
using UnityEngine;

namespace MarblePhysics.Modding.Test
{
    [CustomEditor(typeof(PhysicsRotator))]
    public class PhysicsRotatorEditor : UnityEditor.Editor
    {
        private SerializedProperty floatGeneratorProp;

        protected virtual void OnEnable()
        {
            floatGeneratorProp = serializedObject.FindProperty("floatGenerator");
        }

        private void OnSceneGUI()
        {
            if (floatGeneratorProp.objectReferenceValue != null)
            {
                Handles.color = Color.red;
                PhysicsRotator generator = (PhysicsRotator) target;
                Handles.DrawWireArc(generator.transform.position,
                    Vector3.forward,
                    Quaternion.Euler(0, 0, generator.GetStartingAngle()) * Vector3.up,
                    generator.GetEndingAngle() - generator.GetStartingAngle(), 2f, 2f);
            }
        }
    }
}