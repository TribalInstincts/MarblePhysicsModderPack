using UnityEditor;
using UnityEngine;

namespace MarblePhysics.Modding.StandardComponents
{
    public static class VectorHandle
    {
        public static bool DrawHandle(GameObject gameObject, MonoBehaviour behaviour, AngleVector angleVector)
        {
            Vector2 startPoint = gameObject.transform.position;
            Vector2 direction = MathUtils.ConvertDegreesToVector(gameObject.transform, angleVector.Angle, angleVector.Space) * angleVector.Magnitude;
            Vector2 endPoint = startPoint + direction;

            Handles.color = Color.white;
            Handles.DrawDottedLine(startPoint, endPoint, 2f);

            float size = HandleUtility.GetHandleSize(endPoint) * 0.1f;
            Handles.color = Color.red;
            EditorGUI.BeginChangeCheck();
            Vector3 updatedPosition = Handles.FreeMoveHandle(endPoint, size, Vector3.zero, Handles.CircleHandleCap);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects(new Object[] {gameObject, behaviour}, "Modified Trigger Force");

                Vector2 newDirection = (Vector2) updatedPosition - startPoint;
                float angle = Vector2.SignedAngle(Vector2.right, newDirection.normalized);
                if (angleVector.Space == Space.Self)
                {
                    angle -= gameObject.transform.eulerAngles.z;
                }

                angleVector.Angle = angle;
                angleVector.Magnitude = newDirection.magnitude;
                return true;
            }

            return false;
        }
    }
}