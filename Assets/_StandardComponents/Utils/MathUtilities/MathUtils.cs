using UnityEngine;

namespace MarblePhysics
{
    public static class MathUtils
    {
        public static Vector2 ClosestPointOnLine(Vector2 origin, Vector2 end, Vector2 point)
        {
            Vector2 direction = (end - origin);
            float distance = direction.magnitude;
            direction.Normalize();

            Vector2 lhs = point - origin;
            float dot = Vector2.Dot(lhs, direction);
            dot = Mathf.Clamp(dot, 0f, distance);
            return origin + direction * dot;
        }
        
        public static Vector2 RadianToVector2(float radian)
        {
            return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
        }
  
        public static Vector2 DegreeToVector2(float degree)
        {
            return RadianToVector2(degree * Mathf.Deg2Rad);
        }

        public static Vector2 ConvertDegreesToVector(Transform transform, float angleDegrees, Space space = Space.World)
        {
            float worldAngle = angleDegrees;
            if (space == Space.Self)
            {
                worldAngle += transform.eulerAngles.z;
            }
            return DegreeToVector2(worldAngle);
        }
    }
}
