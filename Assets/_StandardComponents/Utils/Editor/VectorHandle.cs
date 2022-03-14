using UnityEditor;
using UnityEngine;

namespace MarblePhysics.Modding.StandardComponents
{
    public static class VectorHandle
    {
        public delegate Space GetSpace();
        
        public delegate void SetAngle(float angle);

        public delegate float GetAngle();

        public delegate float GetMagnitude();

        public delegate void SetMagnitude(float magnitude);
        
        public static void DrawHandle(GameObject gameObject, GetAngle getAngle, SetAngle setAngle, GetMagnitude getMagnitude, SetMagnitude setMagnitude, Space space = Space.World)
        {
            Vector2 startPoint = gameObject.transform.position;
            Vector2 direction = MathUtils.ConvertDegreesToVector(gameObject.transform, getAngle(), space) * getMagnitude();
            Vector2 endPoint = startPoint + direction;

            Handles.color = Color.white;
            Handles.DrawDottedLine(startPoint, endPoint, 2f);
            
            float size = HandleUtility.GetHandleSize(endPoint) * 0.1f;
            Handles.color = Color.red;
            EditorGUI.BeginChangeCheck();
            Vector3 updatedPosition = Handles.FreeMoveHandle(endPoint, size, Vector3.zero, Handles.CircleHandleCap);
            
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(gameObject, "Modified Trigger Force");
                
                Vector2 newDirection = (Vector2)updatedPosition - startPoint;
                float angle = Vector2.SignedAngle(Vector2.right, newDirection.normalized);
                if (space == Space.Self)
                {
                    angle -= gameObject.transform.eulerAngles.z;
                }

                setAngle(angle);
                setMagnitude(newDirection.magnitude);
            }
        }
    }
}
