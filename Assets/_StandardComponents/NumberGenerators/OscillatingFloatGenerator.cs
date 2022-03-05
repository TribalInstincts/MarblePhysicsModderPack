using System;
using UnityEngine;

namespace MarblePhysics
{
    /// <summary>
    /// Returns a value between 0.0 and 1.0 in a cos wave pattern
    /// </summary>
    [Serializable]
    public class OscillatingFloatGenerator : FloatGenerator
    {
        [Serializable]
        private enum OscillationPattern
        {
            PingPong,
            Sin,
            Repeat
        }

        [SerializeField]
        private OscillationPattern oscillationPattern = OscillationPattern.Sin;

        [SerializeField]
        private float minValue = 0f;
        [SerializeField]
        private float maxValue = 1f;

        public override float GetFixedValue()
        {
            float value = oscillationPattern switch
            {
                OscillationPattern.Sin => (1 + Mathf.Sin(GetFixedTime())) * .5f,
                OscillationPattern.PingPong => Mathf.PingPong(GetFixedTime(), 1f),
                OscillationPattern.Repeat => Mathf.Repeat(GetFixedTime(), 1f),
                _ => throw new ArgumentOutOfRangeException("Undefined Wave form: " + oscillationPattern)
            };

            return Mathf.Lerp(minValue, maxValue, value);
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