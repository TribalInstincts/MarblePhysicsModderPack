using System;
using UnityEngine;

namespace MarblePhysics.Modding.StandardComponents
{
    [Serializable]
    public class AngleVector
    {
        public float Angle = default;
        public float Magnitude = default;
        public Space Space = default;

        public float GetAngle()
        {
            return Angle;
        }

        public float GetStrength()
        {
            return Magnitude;
        }

        public Space GetSpace()
        {
            return Space;
        }

        public void SetAngle(float angle)
        {
            Angle = angle;
        }

        public void SetMagnitude(float magnitude)
        {
            Magnitude = magnitude;
        }
        
        public Vector2 GetVector(Transform transform)
        {
            return MathUtils.ConvertDegreesToVector(transform, Angle, Space) * Magnitude;
        }
    }
}