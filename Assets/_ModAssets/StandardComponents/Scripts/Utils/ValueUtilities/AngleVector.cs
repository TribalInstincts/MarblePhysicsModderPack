using System;
using UnityEngine;

namespace MarblePhysics.Modding
{
    [Serializable]
    public class AngleVector
    {
        public float Angle = default;
        public float Magnitude = default;
        public Space Space = default;

        public Vector2 GetVector(Transform transform)
        {
            return MathUtils.ConvertDegreesToVector(transform, Angle, Space) * Magnitude;
        }

        public override string ToString()
        {
            return $"(A:{Angle},M:{Magnitude},S:{Space})";
        }
    }
}