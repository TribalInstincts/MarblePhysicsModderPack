using UnityEngine;

namespace MarblePhysics.Modding
{
    /// <summary>
    /// Smoothly moves randomly between your min and max value while generally staying near the average of the two 
    /// </summary>
    public class PerlinFloatGenerator : FloatGenerator
    {
        [SerializeField]
        private float minValue = 0f;

        [SerializeField]
        private float maxValue = 1f;

        public override float GetFixedValue()
        {
            return Mathf.Lerp(minValue, maxValue,  (1 + Perlin.Noise(GetFixedTime())) * .5f);
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