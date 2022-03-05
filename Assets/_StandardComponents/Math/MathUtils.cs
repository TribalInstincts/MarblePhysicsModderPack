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
    }
}
