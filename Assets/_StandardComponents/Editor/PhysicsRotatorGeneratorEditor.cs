using UnityEditor;
using UnityEngine;

namespace MarblePhysics.Editor
{
    [CustomEditor(typeof(PhysicsRotator))]
    public class PhysicsRotatorGeneratorEditor : UnityEditor.Editor
    {
        private void OnSceneGUI()
        {
            Handles.color = Color.red;
            PhysicsRotator generator = (PhysicsRotator) target;
            Handles.DrawWireArc(generator.transform.position,
                Vector3.forward,
                Quaternion.Euler(0, 0, generator.GetStartingAngle()) * Vector3.up,
                generator.GetEndingAngle() - generator.GetStartingAngle(), 2f, 1f);
        }
    }
}