using UnityEngine;

namespace MarblePhysics
{
    /// <summary>
    /// Returns 
    /// </summary>
    public class PerlinFloatGenerator : FloatGenerator
    {
        [SerializeField]
        private float minValue = 0f;

        [SerializeField]
        private float maxValue = 1f;

        public override float GetFixedValue()
        {
            return Mathf.Lerp(minValue, maxValue, Perlin.Noise(GetFixedTime()));
        }

        public override float GetMinimum()
        {
            return minValue;
        }

        public override float GetMaximum()
        {
            return maxValue;
        }
    }
}